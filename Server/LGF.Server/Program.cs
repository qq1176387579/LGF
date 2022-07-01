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
        static KcpSocket.KcpAgent kcpAgent1;
        static void Main(string[] args)
        {
            KcpServer kcpServer = new KcpServer();
            kcpServer.Bing(OnRecv, NetConst.ServerPort, 30);    //30间隔

            sendMsg();
        }


        static void OnRecv(KcpSocket.KcpAgent kcpAgent, byte[] bytes, int count)
        {
            string message = Encoding.UTF8.GetString(bytes, 0, count);
            Console.WriteLine(kcpAgent.endPoint.ToString() + "  " + message);
            kcpAgent.Send(Encoding.UTF8.GetBytes(" 服务器 接收完成 f"));
            kcpAgent1 = kcpAgent;
        }

        /// <summary>
        /// 向特定ip的主机的端口发送数据报
        /// </summary>
        static void sendMsg()
        {
            Console.InputEncoding = System.Text.Encoding.Unicode;
            while (true)
            {
                string msg = Console.ReadLine();
                if (kcpAgent1 == null)
                {
                    continue;
                }
                kcpAgent1.Send(Encoding.UTF8.GetBytes(msg));
            }
        }

    }
}
