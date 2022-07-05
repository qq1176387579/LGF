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
    public partial class Program
    {
        public static KcpSocket.KcpAgent kcpAgent1;
        public static KcpServer kcpServer;
        public static void Main(string[] args)
        {
            kcpServer = new KcpServer();
            kcpServer.Bing(OnRecv, NetConst.ServerPort, 30);    //30间隔

            //sendMsg();
        }


        static void OnRecv(KcpSocket.KcpAgent kcpAgent, LGF.Serializable.LGFStream stream, int count)
        {
            //string message = Encoding.UTF8.GetString(bytes, 0, count);
            //kcpAgent.Debug(kcpAgent.endPoint.ToString() + "  " + message);
            //kcpAgent.Send(Encoding.UTF8.GetBytes(" 服务器 接收完成 f"));
            //kcpAgent1 = kcpAgent;
        }

        ///// <summary>
        ///// 向特定ip的主机的端口发送数据报
        ///// </summary>
        //static void sendMsg()
        //{
        //    Console.InputEncoding = System.Text.Encoding.Unicode;
        //    while (kcpServer)
        //    {
        //        string msg = Console.ReadLine();
        //        if (kcpAgent1 == null)
        //        {
        //            continue;
        //        }
        //        kcpAgent1.Send(Encoding.UTF8.GetBytes(msg));
        //    }
        //}

    }
}
