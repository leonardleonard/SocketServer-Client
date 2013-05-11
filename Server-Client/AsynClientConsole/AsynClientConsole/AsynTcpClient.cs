using System;
using System.Collections.Generic;
using System.Text;
#region 命名空间
using System.Net;
using System.Net.Sockets;
using System.Threading;
#endregion

namespace AsynClientConsole
{
    /// <summary>
    /// Tcp协议异步通讯类(客户端)
    /// </summary>
    public class AsynTcpClient
    {
        #region 异步连接
        /// <summary>
        /// Tcp协议异步连接服务器
        /// </summary>
        public void AsynConnect()
        {
            //主机IP
            IPEndPoint serverIp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8686);
            Socket tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpClient.BeginConnect(serverIp, asyncResult =>
            {
                tcpClient.EndConnect(asyncResult);
                Console.WriteLine("client-->-->{0}", serverIp.ToString());
                AsynSend(tcpClient, "我上线了...");
                AsynSend(tcpClient, "第一次发送消息...");
                AsynSend(tcpClient, "第二次发送消息...");
                AsynRecive(tcpClient);
            }, null);
        }
        #endregion

        #region 异步接受消息
        /// <summary>
        /// 异步连接客户端回调函数
        /// </summary>
        /// <param name="tcpClient"></param>
        public void AsynRecive(Socket tcpClient)
        {
            byte[] data = new byte[1024];
            tcpClient.BeginReceive(data, 0, data.Length, SocketFlags.None, asyncResult =>
            {
                int length = tcpClient.EndReceive(asyncResult);
                Console.WriteLine("client<--<--server:{0}", Encoding.UTF8.GetString(data));
                AsynRecive(tcpClient);
            }, null);
        }
        #endregion

        #region 异步发送消息
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="tcpClient">客户端套接字</param>
        /// <param name="message">发送消息</param>
        public void AsynSend(Socket tcpClient, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            tcpClient.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
            {
                //完成发送消息
                int length = tcpClient.EndSend(asyncResult);
                Console.WriteLine("client-->-->server:{0}", message);
            }, null);
        }
        #endregion
    }
}
