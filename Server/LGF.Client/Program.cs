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
        static Dictionary<string, System.Action<string[]>> func;
        static KcpSocket.KcpAgent kcpAgent1;
        static KcpClient kcpClient;
        static List<EndPoint> serverList = new List<EndPoint>();    //服务器列表
        static void Main(string[] args)
        {
            LGFEntry.Startup();
            kcpClient = new KcpClient();
            kcpClient.Bing(NetConst.RandomPort, 15);    //15间隔
            Task.Run(sendMsg);

            Init();

            while (true)
            {
                try
                {
                    LGFEntry.Update();
                    LGFEntry.LateUpdate();
                    LGFEntry.FixedUpdate();
                }
                catch (Exception e)
                {
                    
                    e.DebugError();
                }

                Thread.Sleep(10);
            }
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

                if (CheckString(msg))
                {
                    continue;
                }

                sLog.Debug($"非法命令!! --- {msg}");

            
            }
        }

        public static bool CheckString(string msg)
        {
            string[] tmp = msg.Split(' ');
            if (func.ContainsKey(tmp[0]))
            {
                func[tmp[0]].Invoke(tmp);
                return true;
            }
         
            return false;
        }


        static void Init()
        {
            func = new Dictionary<string, Action<string[]>>();
            //在其他线程里面 非主线程
            EventManager.Instance.AddListener<EndPoint>(GameEventType.Net_GetServersInfo, (point) =>
            {
                Program.serverList.Add(point);
                sLog.Debug("Program 获得服务器信息 {0}", point);

                //自动连接 连接第一个服务器
                //if (kcpAgent1 == null)
                //{
                //    kcpAgent1 = kcpClient.GetKcpAgent(point);   //获得成功
                //}
            });


            NetMsgHandlingMgr.Instance.RegisterClientMsg(NetMsgDefine.S2C_Connect, (S2C_Connect data) =>
            {
                data.Debug($" 连接服务器端成功 当前id: {data.uid}");
            });

            NetMsgHandlingMgr.Instance.RegisterClientMsg(NetMsgDefine.S2C_TextMsg, (S2C_TextMsg data) =>
            {
                data.Debug($" call 消息 {data.name} : {data.msg}");
            });


            func.Add("get_s", (param) =>
            {
                sLog.Debug("get_s 获得服务器信息");
                kcpClient.GetAllServerInfo();
            });


            func.Add("connect_s", (param) =>
            {
                if (serverList.Count == 0)
                {
                    sLog.Error("非法操作  没有该服务器");
                    return;
                }
                sLog.Debug("connect_s 连接服务器 " + serverList[0].ToString() + "  客户单名称 :" + param.GetByID(1));
                kcpClient.TryToConnect(param.GetByID(1), serverList[0]);
            });


            func.Add("call_s", (param) =>
            {
                var tmp = C2S_TextMsg.Get();
                tmp.msg = param.GetByID(1);
                kcpClient.Send(tmp);
            });
        }


    }
}