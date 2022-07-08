using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigSystemInfo
{
    public class SystemConfig
    {
        /// <summary>
        /// 获取服务端口
        /// </summary>
        /// <returns></returns>
        public static NetworkServer GetSocketServerConfig()
        {
            //获取所有配置
            var jsonConfiguration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            var NetworkServer = new NetworkServer();
            jsonConfiguration.Bind("NetworkServer", NetworkServer);
            return NetworkServer;
        }
    }
}
