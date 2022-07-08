using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigSystemInfo.SocketServer
{
    public class NetServer
    {
        /// <summary>
        /// 核心监听方法
        /// </summary>
        TcpListener listener;
        /// <summary>
        /// 服务端监听的端口  作为服务端口
        /// </summary>
        public int ListenPort;
        /// <summary>
        /// 监听的端口
        /// </summary>
        /// <param name="port"></param>
        public NetServer(int port)
        {
            this.ListenPort = port;
        }
        /// <summary>
        /// socket 事件
        /// </summary>
        public delegate void SocketHandler(Socket socket, string data);
        /// <summary>
        /// socket 事件
        /// </summary>
        public delegate void SocketExceptionHandler(Socket socket, Exception ex);
        /// <summary>
        /// 新用户的事件
        /// </summary>
        public event SocketHandler OnOpen;
        /// <summary>
        /// 新用户的事件
        /// </summary>
        public event SocketHandler OnClose;
        /// <summary>
        /// 新用户的事件
        /// </summary>
        public event SocketHandler OnMessage;
        /// <summary>
        /// 异常事件
        /// </summary>
        public event SocketExceptionHandler OnException;
        private bool IsRuning = false;
        /// <summary>
        /// 开始监听
        /// </summary>
        /// <returns></returns>
        public NetServer Listen()
        {
            IsRuning = true;
            listener = new TcpListener(IPAddress.Any, this.ListenPort);
            listener.Start();
            ServerStart();
            Task.Run(() =>
            {
                while (IsRuning)
                {
                    TcpClient s = listener.AcceptTcpClient();
                    //来一个新的链接
                    ThreadPool.QueueUserWorkItem(r =>
                    {
                        try
                        {
                            Accept(s);
                        }
                        catch (Exception)
                        {
                        }
                    });
                }
            });
            return this;
        }
        public void Stop()
        {
            IsRuning = false;
            listener.Stop();
        }
        /// <summary>
        /// 一个新的连接
        /// </summary>
        /// <param name="s"></param>
        private void Accept(TcpClient s)
        {
            BinaryReader rs = new BinaryReader(s.GetStream());
            var ReceiveBuffer = new byte[1024];
            List<byte> ReceiveList = new List<byte>();
            var ConnectSocket = s.Client;

            try
            {
                newAcceptHandler(ConnectSocket);
                while (s.Connected)
                {
                    int length = 0;
                    try
                    {
                        length = rs.Read(ReceiveBuffer, 0, ReceiveBuffer.Length);
                        //如果没有读完，就一直读
                        for (int i = 0; i < length; i++)
                        {
                            ReceiveList.Add(ReceiveBuffer[i]);
                        }
                        if (s.Client.Available == 0)
                        {
                            var data = ReceiveList.ToArray();
                            //接收完毕
                            Task.Run(() => ReceiveHandler(ConnectSocket, data));
                            ReceiveList.Clear();
                        }
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    if (length == 0)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex, ConnectSocket);
            }
            finally
            {
                s.Close();//客户端连接关闭
                newQuitHandler(ConnectSocket);
            }
        }
        /// <summary>
        /// 接收信息的处理
        /// </summary>
        /// <param name="UserToken"></param>
        public void ReceiveHandler(Socket ConnectSocket, byte[] Receivedata)
        {
            string info = Encoding.UTF8.GetString(Receivedata);
            if (info != null)
            {
                OnMessage(ConnectSocket, info);
                //接管数据处理
                Console.WriteLine("收到数据");
            }
            else
            {
                //接管数据处理
                Console.WriteLine("收到空数据");
            }
        }
        /// <summary>
        /// 新的链接
        /// </summary>
        public void newAcceptHandler(Socket ConnectSocket)
        {
            if (OnOpen != null)
            {
                OnOpen(ConnectSocket, null);
            }
            Console.WriteLine("一个新的用户:" + ConnectSocket?.RemoteEndPoint.ToString());
        }
        /// <summary>
        /// 服务开始
        /// </summary>
        public void ServerStart()
        {
            Console.WriteLine("服务开启:local:" + this.ListenPort);
        }
        /// <summary>
        /// 用户退出
        /// </summary>
        public void newQuitHandler(Socket ConnectSocket)
        {
            if (OnClose != null)
            {
                OnClose(ConnectSocket, null);
            }
            Console.WriteLine("用户退出:" + ConnectSocket?.RemoteEndPoint.ToString());
        }
        /// <summary>
        /// 用户异常
        /// </summary>
        public void ExceptionHandler(Exception ex, Socket ConnectSocket)
        {
            if (OnException != null)
            {
                OnException(ConnectSocket, ex);
            }
            Console.WriteLine("用户异常:" + ConnectSocket?.RemoteEndPoint.ToString());
        }
        /// <summary>
        /// 对客户发送数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int SendMessage(Socket socket, string data)
        {
            int length = -1;
            try
            {
                var bytes = Encoding.UTF8.GetBytes(data);
                if (socket != null && socket.Connected)
                {
                    length = socket.Send(bytes);
                }
            }
            catch (Exception)
            { }
            return length;
        }
    }
}
