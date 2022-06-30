using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LGF;
using LGF.Net;
using System.Threading;
using System.Net.Sockets;
using LGF.Log;

namespace LHTestClient
{
    public class Program
    {
        public static LGF.Net.KcpSocket kcpSock;

        static KcpSocket.KcpAgent kcpAgent;
        public class OnRecvHelper : KcpSocketOnRecvHelper
        {
            protected override void OnRecv(KcpSocket.KcpAgent kcp, int count)
            {
                string message = Encoding.UTF8.GetString(bytebuffer, 0, count);
                kcpSock.Debug(kcp.endPoint.ToString() + "  " + message);
                kcpAgent = kcp;
            }
        }


        public static void Main(string[] args)
        {
            OnRecvHelper ff = new OnRecvHelper();
            kcpSock = new LGF.Net.KcpSocket();

            if (!kcpSock.Bing(ff, LGF.Net.NetConst.ClientPort))
            {
                kcpSock.Debug("------4444---");
            }

            Task.Run(async () =>
            {
                while (true)
                {
                    kcpSock.OnUpdate(DateTime.UtcNow);
                    await Task.Delay(10);
                }
            });

            Task.Run(() =>
            {
                sendMsg();
            });


            byte[] buffer = new byte[512];

            kcpSock.Debug("客户端已经开启");
            //EndPoint point = new IPEndPoint(IPAddress.Broadcast, NetConst.ServerPort);  //广播
            Socket broadcastSock = SocketHelper.UdpBind(IPAddress.Any, 0);
            kcpSock.Debug("-----4444---");
            broadcastSock.SetBroadcast();

            EndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, NetConst.ServerPort);
            broadcastSock.SendTo(Encoding.UTF8.GetBytes(" asd  "), endPoint);
            broadcastSock.ReceiveFrom(buffer, ref endPoint);

            EndPoint point = new IPEndPoint((endPoint as IPEndPoint).Address, buffer.bytesToInt(0));  //广播
            kcpSock.Debug($"连接服务器成功  {point}");

            kcpAgent = kcpSock.GetKcpAgent(point);

            kcpAgent.Send(Encoding.UTF8.GetBytes(" 测试 一下效果 "));
#if NOT_UNITY
            while (true)
            {
                Thread.Sleep(10);
            }
#endif
        }

        /// <summary>
        /// 向特定ip的主机的端口发送数据报
        /// </summary>
        static void sendMsg()
        {
            //IPAddress.Broadcast

            //广播消息
            //EndPoint point = new IPEndPoint(IPAddress.Broadcast, NetConst.ServerPort);  //广播
       
            //kcpSock.Sock.SetBroadcast();    //设置广播

            while (true)
            {
                string msg = Console.ReadLine();
                //msg.Debug("send " + msg);
                kcpAgent.Send(Encoding.UTF8.GetBytes(msg));
            }
        }


    }
}
