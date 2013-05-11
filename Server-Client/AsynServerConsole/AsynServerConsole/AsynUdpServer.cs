using System;
using System.Collections.Generic;
using System.Text;
#region 命名空间
using System.Net;
using System.Net.Sockets;
using System.Threading;
#endregion

namespace AsynServerConsole
{
    /// <summary>
    /// Udp协议异步通讯类(服务器端)
    /// </summary>
    public class AsynUdpServer
    {
        #region 容器对象
        /// <summary>
        /// 容器对象
        /// </summary>
        public class StateObject
        {
            //服务器端
            public Socket udpServer = null;
            //接受数据缓冲区
            public byte[] buffer = new byte[1024];
            //远程终端
            public EndPoint remoteEP;
        }

        public StateObject state;
        #endregion

        #region 服务器绑定终端节点
        public void ServerBind()
        {
            //主机IP
            IPEndPoint serverIp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8686);
            Socket udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpServer.Bind(serverIp);
            Console.WriteLine("server ready...");
            IPEndPoint clientIp = new IPEndPoint(IPAddress.Any, 0);
            state = new StateObject();
            state.udpServer = udpServer;
            state.remoteEP = (EndPoint)clientIp;
            AsynRecive();
        }
        #endregion

        #region 异步接受消息
        public void AsynRecive()
        {
            state.udpServer.BeginReceiveFrom(state.buffer, 0, state.buffer.Length, SocketFlags.None, ref state.remoteEP,
                new AsyncCallback(ReciveCallback), null);
        }
        #endregion

        #region 异步接受消息回调函数
        public void ReciveCallback(IAsyncResult asyncResult)
        {
            if (asyncResult.IsCompleted)
            {
                //获取发送端的终节点
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 0);
                EndPoint remoteEP = (EndPoint)ipep;
                state.udpServer.EndReceiveFrom(asyncResult, ref remoteEP);
                Console.WriteLine("server<--<--client:{0}", Encoding.UTF8.GetString(state.buffer));
                //向发送端通知：收到消息
                state.remoteEP = remoteEP;
                AsynSend("收到消息");
                //继续接受消息
                AsynRecive();
            }
        }
        #endregion

        #region 异步发送消息
        public void AsynSend(string message)
        {
            Console.WriteLine("server-->-->client:{0}", message);
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            state.udpServer.BeginSendTo(buffer, 0, buffer.Length, SocketFlags.None, state.remoteEP,
                new AsyncCallback(SendCallback), null);
        }
        #endregion

        #region 异步发送消息回调函数
        public void SendCallback(IAsyncResult asyncResult)
        {
            //消息发送完毕
            if (asyncResult.IsCompleted)
            {
                state.udpServer.EndSendTo(asyncResult);
            }
        }
        #endregion
    }
}
