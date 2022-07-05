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

namespace LGF.Client
{
    internal class Program
    {

        static KcpSocket.KcpAgent kcpAgent1;
        static KcpClient kcpClient;
        static List<EndPoint> serverList = new List<EndPoint>();    //服务器列表
        static void Main(string[] args)
        {
            AppEntry.Startup();
            kcpClient = new KcpClient();
            kcpClient.Bing(OnRecv, NetConst.RandomPort, 15);    //15间隔
            Task.Run(sendMsg);

            Init();

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

        static void Init()
        {
            //在其他线程里面 非主线程
            EventManager.Instance.AddListener<EndPoint>(GameEventType.Net_S2C_Connect, (point) =>
            {
                //主线程处理
                NetMsgMgr.QueueOnMainThread((_point) =>
                { 
                    Program.serverList.Add(_point);
                    Debug.Log("获得服务器信息 {0}", point);
                    //自动连接 连接第一个服务器
                    if (kcpAgent1 == null)
                    {
                        kcpAgent1 = kcpClient.GetKcpAgent(point);   //获得成功
                    }
                    //kcpAgent1.Send();
                }, point);
            });
        }


        static void OnRecv(KcpSocket.KcpAgent kcpAgent, LGF.Serializable.LGFStream stream , int count)
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


                if (msg == "connect")   //尝试连接服务器
                {
                    if (kcpAgent1 == null)
                    {
                        kcpClient.TryToConnect();   //尝试连接服务器 获得服务器信息
                    }
                    else
                    {
                        Debug.LogError(" 无法连接多个服务器");
                    }
                 
                    continue;
                }

                if (kcpAgent1 == null)
                {
                    
                    Debug.Log("未连接服务器! ");
                    continue;
                } 
             


                //kcpAgent1.Send(Encoding.UTF8.GetBytes(msg));
            }
        }


    }
}