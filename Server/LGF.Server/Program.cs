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
using LGF;

namespace LHTestServer
{
    class Program
    {
        static KcpSocket.KcpAgent kcpAgent1;
        static void Main(string[] args)
        {
            AppEntry.Startup();
            KcpServer kcpServer = new KcpServer();
            kcpServer.Bing(OnRecv, NetConst.ServerPort, 30);    //30间隔
            Task.Run(sendMsg);
            
            while (true)
            {
                try
                {
                    AppEntry.Update();
                    AppEntry.LateUpdate();
                    AppEntry.FixedUpdate();
                }
                catch (Exception e)
                {
                    e.DebugError();
                }
               

                Thread.Sleep(10);
            }
        }


        static void OnRecv(KcpSocket.KcpAgent kcpAgent, LGF.Serializable.LGFStream stream, int count)
        {
            //string message = Encoding.UTF8.GetString(bytes, 0, count);
            //Console.WriteLine(kcpAgent.endPoint.ToString() + "  " + message);
            //kcpAgent.Send(Encoding.UTF8.GetBytes(" 服务器 接收完成 f"));
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
