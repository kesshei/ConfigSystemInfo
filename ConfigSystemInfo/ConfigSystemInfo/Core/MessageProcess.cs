using ConfigSystemInfo.Common;
using ConfigSystemInfo.Config;
using ConfigSystemInfo.Network;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigSystemInfo.Core
{
    internal class MessageProcess
    {
        public static string Process(string data)
        {
            string result = null;
            try
            {
                var msg = SerializeUtil.JsonDeserialize<Dictionary<string, object>>(data);
                if (msg.TryGetValue("type", out object key))
                {
                    Log.Info($"获取到的key:{key}");
                    msg.TryGetValue("value", out object value);
                    Log.Info($"获取到的value:{value}");
                    switch (key)
                    {
                        case "getIP":
                            {
                                result = getIP();
                            }
                            break;
                        case "configIP":
                            {
                                var tempNetWorkConfig = new NetWorkConfig();
                                foreach (var item in (JArray)value)
                                {
                                    var keypair = item.ToObject<Dictionary<string, string>>();
                                    keypair.TryGetValue("type", out string Key);
                                    keypair.TryGetValue("value", out string Value);
                                    switch (Key)
                                    {
                                        case "setIP":
                                            {
                                                tempNetWorkConfig.IpAddress = Value;
                                            }
                                            break;
                                        case "setZWYM":
                                            {
                                                tempNetWorkConfig.NetMark = Value;
                                            }
                                            break;
                                        case "setMRWG":
                                            {
                                                tempNetWorkConfig.Gateway = Value;
                                            }
                                            break;
                                        case "setDNS":
                                            {
                                                tempNetWorkConfig.Dns = new List<string>() { Value };
                                            }
                                            break;
                                    }
                                }
                                Log.Info($"获取到的配置信息:{tempNetWorkConfig}");
                                result = SetNetInfo(tempNetWorkConfig);
                            }
                            break;
                        case "setSysTime":
                            {
                                result = setSysTime(value.ToString());
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            if (string.IsNullOrEmpty(result))
            {
                result = ResponseSet(false);
            }
            return result;
        }
        public static string getIP()
        {
            Log.Info("开始获取IP");
            var config = NetWorkConfig.ReadConfigFile();
            if (config == null)
            {
                Log.Info("开始直接获取IP");
                var info = NetworkInfo.GetRealNetworkInfos().Where(t => t != null).FirstOrDefault();
                return ResponseGet(info.AddressIpv4.ToString(), info.UnicastAddresses, info.Gayway.ToString(), info.DNSAddresses?.FirstOrDefault()?.ToString());
            }
            return ResponseGet(config.IpAddress, config.NetMark, config.Gateway, config.Dns.FirstOrDefault());
        }
        public static string SetNetInfo(NetWorkConfig netWorkConfig)
        {
            var config = NetWorkConfig.ReadConfigFile();
            if (config == null)
            {
                config = netWorkConfig;
            }
            else
            {
                config.IpAddress = netWorkConfig.IpAddress;
                config.NetMark = netWorkConfig.NetMark;
                config.Gateway = netWorkConfig.Gateway;
                config.Dns = netWorkConfig.Dns;
            }
            var result = config.WriteNetPlanFile();
            return ResponseSet(result);
        }
        public static string setSysTime(string time)
        {
            Log.Info($"获取到的时间:{time}");
            string result = LinuxCmdHelper.RunCommand("timedatectl", $" set-ntp false ");
            result += LinuxCmdHelper.RunCommand("date", $"-s \"{time}\"");
            result += LinuxCmdHelper.RunCommand("hwclock", "--systohc");
            Log.Info($"执行的结果:{result}");
            return ResponseSet(!result.Contains("permitted"));
        }
        public static string ResponseSet(bool State = false)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("type", "set");
            data.Add("value", State ? "OK" : "");
            return SerializeUtil.JsonSerialize(data);
        }
        public static string ResponseGet(string ip, string netmark, string gatway, string dns)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("IP", ip == null ? "" : ip);
            data.Add("ZWYM", netmark == null ? "" : netmark);
            data.Add("MRWG", gatway == null ? "" : gatway);
            data.Add("DNS", dns == null ? "" : dns);
            return SerializeUtil.JsonSerialize(data.ToList());
        }
    }
}
