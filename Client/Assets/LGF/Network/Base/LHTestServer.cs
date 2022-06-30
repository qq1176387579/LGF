using LGF.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using LGF.Log;

namespace LHTestServer
{
    class Program
    {
        static LGF.Net.KcpSocket serverkcpSock;
        static KcpSocket.KcpAgent kcpAgent;
        public class OnRecvHelper : KcpSocketOnRecvHelper
        {
            protected override void OnRecv(KcpSocket.KcpAgent kcp, int count)
            {
                kcpAgent = kcp;
                string message = Encoding.UTF8.GetString(bytebuffer, 0, count);
                Console.WriteLine(kcp.endPoint.ToString() + "  " + message);

                kcp.Send(Encoding.UTF8.GetBytes(" 服务器 接收完成 f"));
            }
        }

        static void Main(string[] args)
        {
            serverkcpSock = new LGF.Net.KcpSocket();
            OnRecvHelper ff = new OnRecvHelper();
            if (!serverkcpSock.Bing(ff, 0 /*LGF.Net.NetConst.ServerPort*/)) 
            {
                serverkcpSock.Debug("------4444 服务端绑定失败---");
            }

            byte[] buffer = new byte[512];

            //serverkcpSock.ipEndPoint.Port;
            //IPEndPoint broadcastListenerPoint = new IPEndPoint();
            Socket broadcastSock = SocketHelper.UdpBind(IPAddress.Any, LGF.Net.NetConst.ServerPort);
            Console.WriteLine("   " + serverkcpSock.Sock.LocalEndPoint.ToString());

            Task.Run(() =>
            {
                while(true)
                {
                    EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                    int length = broadcastSock.ReceiveFrom(buffer, ref endPoint);
                    string message = Encoding.UTF8.GetString(buffer, 0, length);
                    Console.WriteLine(endPoint.ToString() + "  " + message);
                    var point = serverkcpSock.Sock.LocalEndPoint;

                    (point as IPEndPoint).Port.intToBytes(buffer, 0);
                    Console.WriteLine("   " + (int)buffer[0]);
                    broadcastSock.SendTo(buffer, 8, SocketFlags.None, endPoint);
                }
            });

            //IPAddress.Any 才能监听 IPAddress.Broadcast
            Console.WriteLine("服务端已经开启");
            Task.Run(async () =>
            {
                while (true)
                {
                    serverkcpSock.OnUpdate(DateTime.UtcNow);
                    await Task.Delay(10);
                }
            });

            Task.Run(() =>
            {
                sendMsg();
            });

            while (true)
            {
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 向特定ip的主机的端口发送数据报
        /// </summary>
        static void sendMsg()
        {
            while (true)
            {
                string msg = Console.ReadLine();
                if (kcpAgent == null)
                {
                    continue;
                }
                kcpAgent.Send(Encoding.UTF8.GetBytes(msg));
            }
        }


    }
}
