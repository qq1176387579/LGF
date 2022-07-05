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
    public class KcpCSBase
    {
        public virtual bool IsClient { get=> true; }
        public virtual bool IsServer { get => false; }

        protected KcpSocket m_kcpSocket;
        protected RecvHelper m_recvHelper;
        protected bool m_disposed = false;
        public bool IsDisposed => m_disposed;

        public int LocalPort { get; set; }


        /// <summary>
        /// 专门用于连接的Socket kcp 接收广播协议会出问题  发送广播的kcp会接收不到三次握手的回来的数据
        /// </summary>
        protected Socket m_ConnectSock;
        protected LGFStream m_Stream = new LGFStream(512);  //流

        /// <summary>
        /// 正常间隔时间 走配置 客户端是33fps 可以通过服务器通知配置 但是没必要
        /// </summary>
        /// <param name="OnRecv"></param>
        /// <param name="port"> 0 客户端随机绑定就行   </param>
        /// <param name="interval"></param>
        public virtual void Bing(KcpOnRecv OnRecv, int port = 0, uint interval = 10)  //间隔时间
        {
            if (m_kcpSocket != null)
            {
                m_disposed = true;
                m_kcpSocket.Debug("该协议号已经绑定了!!");
                return;
            }

            try
            {
                m_ConnectSock = SocketHelper.UdpBind(IPAddress.Any, port);
            }
            catch (Exception e)
            {
                m_disposed = true;
                e.DebugError(); //绑定失败
                return;
            }

            m_disposed = false;
            m_recvHelper = new RecvHelper();
            m_recvHelper.Bing(OnRecv);
            m_kcpSocket = new KcpSocket();
            if (!m_kcpSocket.Bing(m_recvHelper, LGF.Net.NetConst.RandomPort, interval))
            {
                m_kcpSocket.Debug("初始化失败!!");
                m_disposed = true;
                return;
            }

            LocalPort = (m_kcpSocket.Sock.LocalEndPoint as IPEndPoint).Port;    //当前端口

            Listener();
        }





        /// <summary>
        /// 监听 连接端口
        /// </summary>
        public virtual void Listener()
        {
            Task.Run(() =>
            {
                while (!m_disposed)
                {
                    try
                    {
                        if (m_ConnectSock == null) return;
                        if (m_ConnectSock.Available <= 0)
                        {
                            Thread.Sleep(1000);    //没事情做休息一下
                            continue;
                        }

                        EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

                        int length = 0;
                        lock (m_ConnectSock)
                            length = m_ConnectSock.ReceiveFrom(m_Stream.GetBuffer(), ref endPoint);   //监听端口

                        if (length < NetMsgHelper.INT32_SIZE)
                        {
                            Debug.LogError("非法消息!");
                            Thread.Sleep(100);
                            continue;
                        }

                        if (m_disposed) return;
                        NetMsgMgr.Instance.OnCSNetMsg(this, endPoint, m_Stream);
                    }
                    catch (Exception e)
                    {
                        this.DebugError(e);
                        Thread.Sleep(100);
                        //Task.Delay(10);   任务延迟有GC
                    }
                }
            });
        }



        public virtual void Send(LGFStream stream, in EndPoint point)
        {
            lock (m_ConnectSock)
                m_ConnectSock.SendTo(stream.GetBuffer(), stream.Lenght, SocketFlags.None, point);    //继续返回给刚刚发送过来的
        }


        public virtual void Send(ISerializer serializer, in EndPoint point)
        {
            serializer.Serialize(m_Stream);
            lock(m_ConnectSock)
                m_ConnectSock.SendTo(m_Stream.GetBuffer(), m_Stream.Lenght, SocketFlags.None, point);    //继续返回给刚刚发送过来的
        }





        public class RecvHelper : KcpSocketOnRecvHelper
        {
            //protected override void OnRecv(in KcpSocket.KcpAgent kcp,in int count)
            //{
            //    string message = Encoding.UTF8.GetString(bytebuffer, 0, count);
            //    Console.WriteLine(kcp.endPoint.ToString() + "  " + message);

            //    kcp.Send(Encoding.UTF8.GetBytes(" 服务器 接收完成 f"));
            //}
        }


        public KcpSocket.KcpAgent GetKcpAgent(in EndPoint point)
        {
            return m_kcpSocket.GetKcpAgent(point); 
        }


        public virtual void Dispose()
        {
            m_disposed = true;
            //严格按照执行顺序才能保证期
            m_kcpSocket.Dispose();
            m_ConnectSock.Dispose();
            m_ConnectSock = null;
            m_kcpSocket = null;
            m_recvHelper = null;
        }

    }

}
