using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigSystemInfo
{

    /// <summary>
    /// 本地NetworkServer信息
    /// </summary>
    public class NetworkServer
    {
        /// <summary>
        /// 服务的端口
        /// </summary>
        public int ServerPort { get; set; } = 1235;
        /// <summary>
        /// 网卡名，如果没有的话，会自己获取,建议指定
        /// </summary>
        public string NetworkCard { get; set; }
        ///// <summary>
        ///// 网卡名，如果没有的话，会自己获取,建议指定 // NetworkManager or  Systemd-networkd
        ///// </summary>  
        //public string NetworkRenderer { get; set; } = "NetworkManager";
    }
}
