using ConfigSystemInfo.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ConfigSystemInfo.Config
{
    /// <summary>
    /// 网络配置
    /// </summary>
    public class NetWorkConfig
    {
        private static string NetPlanFilePath = "/etc/netplan/02-network-config.yaml";
        public string NetworkCard { get; set; } = SystemConfig.GetSocketServerConfig().NetworkCard;
        public string IpAddress { get; set; }
        public string NetMark { get; set; }
        public string Gateway { get; set; }
        public List<string> Dns { get; set; }

        public Dictionary<string, string> GetAllValue()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("networkcard", NetworkCard);
            value.Add("ipaddress", IpAddress);
            value.Add("netmark", GetNetMark(NetMark).ToString());
            value.Add("gateway", Gateway == null ? IpAddress : Gateway);
            if (Dns?.Any() == true)
            {
                value.Add("dns", string.Join(",", Dns));
            }
            else
            {
                value.Add("dns", "");
            }
            return value;
        }
        /// <summary>
        /// 获取网络配置文件信息
        /// </summary>
        /// <returns></returns>
        public string GetNewNetConfigInfo()
        {
            var yaml = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Config", "Network.yaml"));
            foreach (var item in GetAllValue())
            {
                var value = string.IsNullOrEmpty(item.Value) ? "" : item.Value;
                yaml = yaml.Replace($"{{{item.Key}}}", value);
            }
            return yaml;
        }
        public static NetWorkConfig ReadConfigFile(string netPlanFilePath = null)
        {
            if (netPlanFilePath == null)
            {
                netPlanFilePath = NetPlanFilePath;
            }
            if (File.Exists(netPlanFilePath))
            {
                var data = File.ReadAllText(netPlanFilePath);
                var deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
                var b = deserializer.Deserialize<Dictionary<string, object>>(data);
                NetWorkConfig netWorkConfig = new NetWorkConfig();
                if (b?.ContainsKey("network") == true)
                {
                    var network = (Dictionary<object, object>)b["network"];
                    if (network?.ContainsKey("ethernets") == true)
                    {
                        var ethernets = (Dictionary<object, object>)network["ethernets"];
                        if (ethernets?.Count < 1)
                        {
                            return null;
                        }
                        var firstKey = ethernets.Keys.FirstOrDefault();
                        netWorkConfig.NetworkCard = firstKey.ToString();
                        var NetworkCard = (Dictionary<object, object>)ethernets[firstKey];

                        //获取网卡信息
                        if (NetworkCard?.ContainsKey("addresses") == true)
                        {
                            var NetworkCardaddresses = (List<object>)NetworkCard["addresses"];
                            var address = NetworkCardaddresses.FirstOrDefault().ToString();
                            var list = address.Split("/");
                            netWorkConfig.IpAddress = list[0];
                            netWorkConfig.NetMark = GetNetMarkForConfig(int.Parse(list[1])).ToString();
                        }
                        if (NetworkCard?.ContainsKey("gateway4") == true)
                        {
                            var gateway = (string)NetworkCard["gateway4"];
                            netWorkConfig.Gateway = gateway;
                        }
                        if (NetworkCard?.ContainsKey("nameservers") == true)
                        {
                            var nameservers = (Dictionary<object, object>)NetworkCard["nameservers"];
                            if (nameservers?.ContainsKey("addresses") == true)
                            {
                                var Nameserversaddresses = (List<object>)nameservers["addresses"];
                                netWorkConfig.Dns = Nameserversaddresses.Select(t => t.ToString()).ToList();
                            }
                        }
                        return netWorkConfig;
                    }
                }
            }
            return null;
        }
        public bool WriteNetPlanFile()
        {
            try
            {
                var path = "/etc/netplan";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string networkFileContent = GetNewNetConfigInfo();
                File.WriteAllText(NetPlanFilePath, networkFileContent);
                var result = LinuxCmdHelper.RunCommand("netplan", "apply");
                if (result.Contains("permitted"))
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static int GetNetMark(string NetMark)
        {
            if (string.IsNullOrEmpty(NetMark))
            {
                return 24;
            }
            IPAddress iPAddress = IPAddress.Parse(NetMark);
            var bytes = iPAddress.GetAddressBytes();
            int count = 0;
            foreach (var item in bytes)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (GetBit(item, i) == 1)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        /// <summary>
        /// 获取字节中的指定Bit的值
        /// </summary>
        /// <param name="this">字节</param>
        /// <param name="index">Bit的索引值(0-7)</param>
        /// <returns></returns>
        private static int GetBit(byte data, int index)
        {
            byte x = 1;
            switch (index)
            {
                case 0: { x = 0x01; } break;
                case 1: { x = 0x02; } break;
                case 2: { x = 0x04; } break;
                case 3: { x = 0x08; } break;
                case 4: { x = 0x10; } break;
                case 5: { x = 0x20; } break;
                case 6: { x = 0x40; } break;
                case 7: { x = 0x80; } break;
                default: { return 0; }
            }
            return (data & x) == x ? 1 : 0;
        }
        public static IPAddress GetNetMarkForConfig(int mark)
        {
            var list = new List<bool>();
            var d = mark;
            var areas = new List<int>();

            for (int i = 0; i < 4; i++)
            {
                if (d >= (i + 1) * 8)
                {
                    areas.Add(8);
                }
                else
                {
                    int eeee = 0;
                    for (int z = (i) * 8 + 1; z < (i + 1) * 8; z++)
                    {
                        if (d >= z)
                        {
                            eeee++;
                        }
                    }
                    areas.Add(eeee);
                }
            }
            foreach (var item in areas)
            {
                var temp = new List<bool>();
                for (int i = 0; i < 8; i++)
                {
                    if ((i + 1) <= item)
                    {
                        temp.Add(true);
                    }
                    else
                    {
                        temp.Add(false);
                    }
                }
                temp.Reverse();
                list.AddRange(temp);
            }
            var ee = ToBytes(new BitArray(list.ToArray()));
            return new IPAddress(ee.ToArray());
        }
        public static IEnumerable<byte> ToBytes(BitArray bits, bool MSB = false)
        {
            int bitCount = 7;
            int outByte = 0;

            foreach (bool bitValue in bits)
            {
                if (bitValue)
                    outByte |= MSB ? 1 << bitCount : 1 << (7 - bitCount);
                if (bitCount == 0)
                {
                    yield return (byte)outByte;
                    bitCount = 8;
                    outByte = 0;
                }
                bitCount--;
            }
            if (bitCount < 7)
                yield return (byte)outByte;
        }
        public override string ToString()
        {
            var data = GetAllValue();
            return SerializeUtil.JsonSerialize(data);
        }
    }
}
