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
using System.Reflection;

namespace LGF.Server
{
    class AppEntry
    {
        static KcpSocket.KcpAgent kcpAgent1;
        static void Main(string[] args)
        {
            LGFEntry.Startup();
            ModuleMgr.Instance.Init();
            Task.Run(sendMsg);
            
            while (true)
            {
                try
                {
                    HotfixHelper.CheckHotfix();

                    LGFEntry.FixedUpdate();
                    LGFEntry.Update();
                    LGFEntry.LateUpdate();
                }
                catch (Exception e)
                {
                    e.DebugError();
                }
               

                Thread.Sleep(1);
            }
        }


      

        /// <summary>
        /// 向特定ip的主机的端口发送数据报
        /// </summary>
        static void sendMsg()
        {
            Console.InputEncoding = System.Text.Encoding.Unicode;
            sLog.isSave = true;
            while (true)
            {
                string msg = Console.ReadLine();

                if (msg == "reload")
                {
                    HotfixHelper.ReLoadHotfix();
                    //HotfixHelper.LoadHotfixAssembly();
                }

                if (msg == "openlog") {
                    KcpSocket.OpenLog = true;
                }

                if (msg == "closelog") {
                    KcpSocket.OpenLog = false;
                }

                if (kcpAgent1 == null)
                {
                    continue;
                }

                //kcpAgent1.Send(Encoding.UTF8.GetBytes(msg));
            }
        }


        //public static class DllHelper
        //{
           
        //}

    }
}
