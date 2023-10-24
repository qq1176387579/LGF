/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/2 0:00:39
/// 功能描述:  kcp 客户端 服务端基类
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LGF;
using LGF.Log;
using LGF.Serializable;
using UnityEngine;

namespace LGF.Net
{

    public class RecvHelper : KcpSocketOnRecvHelper
    {
        //protected override void OnRecv(in KcpSocket.KcpAgent kcp,in int count)
        //{
        //    string message = Encoding.UTF8.GetString(bytebuffer, 0, count);
        //    Console.WriteLine(kcp.endPoint.ToString() + "  " + message);
        //    kcp.Send(Encoding.UTF8.GetBytes(" 服务器 接收完成 f"));
        //}
    }

    /// <summary>
    /// 客户端服务端 通用数据
    /// 客户端 S2C 用来读取    服务端 S2C 用来发送
    /// </summary>
    public class KcpCSBaseTmpData
    {
        //public N_C2S_GetAllServersInfo n_C2S_GetAllServersInfo = new N_C2S_GetAllServersInfo();
        //public N_S2C_GetAllServersInfo n_S2C_GetAllServersInfo = new N_S2C_GetAllServersInfo();

        //public S2C_Connect S2C_Connect = new S2C_Connect();  //临时数据
        //public C2S_Connect C2S_Connect = new C2S_Connect(); //临时数据

        public C2S_HeartBeat C2S_HeartBeat = new C2S_HeartBeat(); //临时数据
    }


    public class KcpCSBase
    {
        public virtual bool IsClient { get => true; }
        public virtual bool IsServer { get => false; }
        //public EndPoint LocalEndPoint { get => m_kcpSocket.Sock.LocalEndPoint; } 私有地址信息是0.0.0.0:port 导致该方法无法使用

        protected KcpSocket m_kcpSocket;
        protected RecvHelper m_recvHelper;
        protected bool m_disposed = false;
        public bool IsDisposed => m_disposed;

        public int LocalPort { get; set; }
        public INetMsgHandling NetMsgHandling { get=> m_NetMsgHandlingMgr; }

        INetMsgHandling m_NetMsgHandlingMgr = NetMsgHandlingMgr.Instance;
        protected NetMsgMgr netMsgMgr = NetMsgMgr.GetSingleton();

        /// <summary>
        /// 专门用于连接的Socket kcp 接收广播协议会出问题  发送广播的kcp会接收不到三次握手的回来的数据
        /// </summary>
        protected Socket m_ConnectSock;
        /// <summary>
        /// 处理udp的流
        /// </summary>
        protected LStream m_UdpStream = new LStream(512);  //流 推送和接收都是同一个流

        public KcpCSBaseTmpData tmpData = new KcpCSBaseTmpData();

        /// <summary>
        /// 正常间隔时间 走配置 客户端是33fps 可以通过服务器通知配置 但是没必要
        /// </summary>
        /// <param name="OnRecv"></param>
        /// <param name="port"> 0 客户端随机绑定就行   </param>
        /// <param name="interval"></param>
        public virtual void Bing(RecvHelper recvHelper, int port = -1, int kcpPort = -1, uint interval = 10)  //间隔时间
        {
            if (m_kcpSocket != null)
            {
                m_disposed = true;
                this.Debug("该协议号已经绑定了!!");
                return;
            }

            try
            {
                if (port != -1)
                {
                    m_ConnectSock = SocketHelper.UdpBind(IPAddress.Any, port);
                }
            }
            catch (Exception e)
            {
                m_disposed = true;
                e.DebugError(); //绑定失败
                return;
            }

            m_disposed = false;
            m_recvHelper = recvHelper;
            //m_recvHelper = new RecvHelper();
            //m_recvHelper.Bing(OnRecv);
            m_kcpSocket = new KcpSocket();


            if (!m_kcpSocket.Bing(m_recvHelper, kcpPort != -1 ? kcpPort : LGF.Net.NetConst.RandomPort, interval, IsServer)) 
            {
                m_kcpSocket.Debug("初始化失败!!");
                m_disposed = true;
                return;
            }

            LocalPort = (m_kcpSocket.Sock.LocalEndPoint as IPEndPoint).Port;    //当前端口

            //Listener();
        }

        #region 
        ///// <summary>
        ///// 监听 连接端口
        ///// </summary>
        //public void Listener()
        //{
        //    EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
        //    Task.Run(() =>
        //    {
        //        while (!m_disposed)
        //        {
        //            try
        //            {
        //                if (m_ConnectSock == null) return;
        //                OnListener();   //用来处理外部发送信息的

        //                if (m_ConnectSock.Available <= 0)
        //                {
        //                    Thread.Sleep(1000);    //没事情做休息一下
        //                    continue;
        //                }


        //                int length = 0;
        //                length = m_ConnectSock.ReceiveFrom(m_UdpStream.GetBuffer(), ref endPoint);   //监听端口

        //                if (length < NetMsgHelper.INT32_SIZE)
        //                {
        //                    sLog.Error("非法消息!");
        //                    Thread.Sleep(100);
        //                    continue;
        //                }

        //                if (m_disposed) return;
        //                m_NetMsgHandlingMgr.OnCSNetMsg(this, endPoint, m_UdpStream);
        //            }
        //            catch (Exception e)
        //            {
        //                this.DebugError(e);
        //                Thread.Sleep(100);
        //                //Task.Delay(10);   任务延迟有GC
        //            }
        //        }
        //    });
        //}
        //protected virtual void OnListener()
        //{

        //}
        #endregion









        /// <summary>
        /// 线程不安全   需要确保是同一个线程
        /// 在 OnListener 处理是线程安全的
        /// 发送udp数据  
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="point"></param>
        public virtual void SendTo(ISerializer serializer, in EndPoint point)
        {
            serializer.Serialize(m_UdpStream);
            m_ConnectSock.SendTo(m_UdpStream.GetBuffer(), m_UdpStream.Lenght, SocketFlags.None, point);    //继续返回给刚刚发送过来的
        }

        /// <summary>
        /// 线程不安全   需要确保是同一个线程
        /// </summary>
        protected virtual void SendTo(LStream stream, int lenght, in EndPoint point)
        {
            m_ConnectSock.SendTo(stream.GetBuffer(), lenght, SocketFlags.None, point);    //继续返回给刚刚发送过来的
        }





        //public KcpSocket.KcpAgent GetKcpAgent(in EndPoint point)
        //{
        //    return m_kcpSocket.GetKcpAgent(point); 
        //}


        public virtual void Dispose()
        {
            m_disposed = true;
            //严格按照执行顺序才能保证期
            m_kcpSocket.Dispose();
            m_ConnectSock.Dispose();
            m_ConnectSock = null;
            m_kcpSocket = null;
            m_recvHelper = null;
            tmpData = null;
        }

    }

}
