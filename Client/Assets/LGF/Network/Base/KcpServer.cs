/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/30 22:31:49
/// 功能描述:  
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


namespace LGF.Net
{
    /// <summary>
    /// Kcp 服务器
    /// </summary>
    public class KcpServer
    {
        KcpSocket m_kcpSocket;
        RecvHelper m_recvHelper;
        bool m_disposed = false;
        public bool IsDisposed => m_disposed;

        /// <summary>
        /// 专门用于连接的Socket kcp 接收广播协议会出问题  发送广播的kcp会接收不到三次握手的回来的数据
        /// </summary>
        Socket m_ConnectSock;


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
        /// 绑定端口号  kcp刷新间隔 帧同步33s每次
        /// </summary>
        /// <param name="port"></param>
        public void Bing(KcpOnRecv OnRecv, int port, uint interval = 10)  //间隔时间
        {
            if (m_kcpSocket != null)
            {
                m_kcpSocket.Debug("该服务器已经绑定了!!");
                return;
            }

            try
            {
                m_ConnectSock = SocketHelper.UdpBind(IPAddress.Any, port);
            }
            catch (Exception e)
            {
                e.DebugError(); //绑定失败
                return;
            }
      

            m_disposed = false;
            m_recvHelper = new RecvHelper();
            m_recvHelper.Bing(OnRecv);
            m_kcpSocket = new KcpSocket();
            if (!m_kcpSocket.Bing(m_recvHelper, LGF.Net.NetConst.RandomPort, interval))
            {
                m_kcpSocket.Debug("初始化服务器失败!!");
                m_disposed = true;
                return;
            }

            Listener();     //监听

            this.Debug("服务端已经开启");
        }


        /// <summary>
        /// 监听 连接端口
        /// </summary>
        void Listener()
        {

            Task.Run(() =>
            {
                byte[] buffer = new byte[512];  //缓冲数据
                int localport = (m_kcpSocket.Sock.LocalEndPoint as IPEndPoint).Port;
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
                        int length = m_ConnectSock.ReceiveFrom(buffer, ref endPoint);   //监听端口

                        string message = Encoding.UTF8.GetString(buffer, 0, length);
                        this.Debug(StringPool.Concat(endPoint.ToString(), " : ", message)); //后面可能乱码 记得加线程
                        if (m_disposed) return;
                        localport.intToBytes(buffer, 0);
                        m_ConnectSock.SendTo(buffer, 8, SocketFlags.None, endPoint);    //继续返回给刚刚发送过来的
                    }
                    catch (Exception e)
                    {
                        this.DebugError(e);
                        Thread.Sleep(10);
                        //Task.Delay(10);   任务延迟有GC
                    }
                 
                }
            });
        }


        public void Dispose()
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


