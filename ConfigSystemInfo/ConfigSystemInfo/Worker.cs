using ConfigSystemInfo.Common;
using ConfigSystemInfo.Core;
using ConfigSystemInfo.SocketServer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigSystemInfo
{
    public class Worker : BackgroundService
    {
        private readonly NetServer netServer;
        public Worker()
        {
            Log.Info("后台服务初始化");
            var portInfo = SystemConfig.GetSocketServerConfig();
            Log.Info($"服务监听:{portInfo.ServerPort} 端口，监听  :{portInfo.NetworkCard} 网卡!");
            netServer = new NetServer(portInfo.ServerPort);
            netServer.OnOpen += NetServer_OnOpen;
            netServer.OnClose += NetServer_OnClose;
            netServer.OnMessage += NetServer_OnMessage;
            netServer.OnException += NetServer_OnException;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Info("服务开始监听");
            netServer.Listen();
            await Task.CompletedTask;
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Info("服务停止监听");
            Log.Info("停止服务");
            netServer.Stop();
            return Task.CompletedTask;
        }
        private void NetServer_OnMessage(System.Net.Sockets.Socket socket, string data)
        {
            Log.Info($"收到消息!{data}");
            var result = MessageProcess.Process(data);
            if (result != null)
            {
                netServer.SendMessage(socket, result);
            }
            Log.Info($"响应消息!{result}");
        }

        private void NetServer_OnClose(System.Net.Sockets.Socket socket, string data)
        {
            Log.Info($"连接关闭!{socket?.RemoteEndPoint}");
        }

        private void NetServer_OnOpen(System.Net.Sockets.Socket socket, string data)
        {
            Log.Info($"连接开启!{socket?.RemoteEndPoint}");
        }
        private void NetServer_OnException(System.Net.Sockets.Socket socket, Exception ex)
        {
            Log.Error(ex, $"连接异常!{socket?.RemoteEndPoint}");
        }
    }
}
