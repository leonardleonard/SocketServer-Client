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
    /// Udp协议异步通讯类(客户端)
    /// </summary>
    public class AsynUdpClient
    {
        #region 容器对象
        /// <summary>
        /// 容器对象
        /// </summary>
        public class StateObject
        {
            //客户端套接字
            public Socket udpClient = null;
            //接收信息缓冲区
            public byte[] buffer = new byte[1024];
            //服务器端终节点
            public IPEndPoint serverIp;
            //远程终端节点
            public EndPoint remoteEP;
        }

        public StateObject state;
        #endregion

        #region 客户端初始化
        public void InitClient()
        {
            state = new StateObject();
            state.udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            state.serverIp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8686);
            state.remoteEP = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
            //此处注意：
            //  由于当前是客户端，所以没有绑定终节点
            //  不可直接接收消息，必须先向其他终端发送信息告知本机终节点
            AsynSend("第1次发送消息");
            AsynSend("第2次发送消息");
            AsynRecive();
        }
        #endregion

        #region 异步接收来自其他终端发送的消息
        public void AsynRecive()
        {
            state.udpClient.BeginReceiveFrom(state.buffer, 0, state.buffer.Length, SocketFlags.None, ref state.remoteEP,
                new AsyncCallback(ReciveCallback), null);
        }
        #endregion

        #region 异步接收来自其他终端发送的消息回调函数
        public void ReciveCallback(IAsyncResult asyncResult)
        {
            //信息接收完成
            if (asyncResult.IsCompleted)
            {
                state.udpClient.EndReceiveFrom(asyncResult, ref state.remoteEP);
                Console.WriteLine("client<--<--{0}:{1}", state.remoteEP.ToString(), Encoding.UTF8.GetString(state.buffer));
                AsynRecive();
            }
        }
        #endregion

        #region 异步发送消息
        public void AsynSend(string message)
        {
            Console.WriteLine("client-->-->{0}:{1}", state.serverIp.ToString(), message);
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            state.udpClient.BeginSendTo(buffer, 0, buffer.Length, SocketFlags.None, state.serverIp,
                new AsyncCallback(SendCallback), null);
        }
        #endregion

        #region 异步发送消息回调函数
        public void SendCallback(IAsyncResult asyncResult)
        {
            //消息发送完成
            if (asyncResult.IsCompleted)
            {
                state.udpClient.EndSendTo(asyncResult);
            }
        }
        #endregion
    }
}
