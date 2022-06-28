using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.Sockets.Kcp;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using LGF.Log;


namespace LGF.Net
{


    public class KcpSocketOnRecvHelper : IKcpSocketOnRecv
    {
        protected byte[] bytebuffer = new byte[NetConst.Socket_RecvBufferSize];
        byte[] IKcpSocketOnRecv.bytebuffer => bytebuffer;

        void IKcpSocketOnRecv.OnRecv(KcpSocket.KcpAgent kcp, int count)
        {
            if (count >= -3 && count <= 0)
                return;     //不需要处理

            if (count <= -3)
            {
                count = -1 * count - 3;
                bytebuffer = (byte[])System.Array.CreateInstance(typeof(byte), SocketHelper.ceilpow2(count));
                count = kcp.Recv(bytebuffer);   //重新接收数据
            }

            //看需要需不需要新弄个线程用来处理new 的数据 如果需要个到的话
            //弄一个需要弄一个大空间的队列然后在线程里面处理   我觉得这里不需要弄了

            OnRecv(kcp, count);
        }

        protected virtual void OnRecv(KcpSocket.KcpAgent kcp, int count)
        {
            //反序列化处理
        }
    }



    /// <summary>
    /// 绑定接收空间
    /// </summary>
    public interface IKcpSocketOnRecv
    {
        byte[] bytebuffer { get;}

        /// <summary>
        /// count < -3 的时候 表示空间不够 (-*count) -3 获得总共的大小空间 
        /// </summary>
        /// <param name=""></param>
        /// <param name="count"></param>
        void OnRecv(KcpSocket.KcpAgent kcp, int count);   
 
    }

    /// <summary>
    /// KcpSocket  线程安全  
    /// </summary>
    public class KcpSocket 
    {
        Socket m_Socket;
        IPEndPoint m_ipEndPoint;
        byte[] m_SendBuffer = new byte[NetConst.Socket_SendBufferSize];
        byte[] m_RecvBuffer = new byte[NetConst.Socket_RecvBufferSize];
        Thread thread;

        Dictionary<ulong, KcpAgent> m_KcpAgents = new Dictionary<ulong, KcpAgent>();
        List<KcpAgent> m_addKcpAgent = new List<KcpAgent>();     //缓冲代理

        public bool HasBing => m_Socket != null;
        public Socket Sock => m_Socket;
        IKcpSocketOnRecv objRecv;

        public bool Bing(IKcpSocketOnRecv onRecv, int port)
        {
            objRecv = onRecv;
            try
            {
                m_ipEndPoint = new IPEndPoint(IPAddress.Any, port);
                m_Socket = SocketHelper.UdpBind(m_ipEndPoint);
            }
            catch (Exception e)
            {
                this.Debug(e.ToString());
                return false;
                //throw;
            }

            //后续有外部线程 这个可以放外部去 线程过多的话 没必要单独给他一个线程
            thread = new Thread(Thread_Recv) { IsBackground = true };
            thread.Start(); //
            this.Debug(" Bing " + m_Socket.LocalEndPoint.ToString());
            return true;
        }


     


        /// <summary>
        /// 更新和发送线程
        /// 后面单独一个线程去处理他  我这里打算用 LGF.Timer 调用他  LGF.Timer 自带线程
        /// </summary>
        /// <param name="dateTime"></param>
        public void OnUpdate(in DateTime dateTime)   
        {
            //this.Debug(" OnUpdate ");
            if (m_addKcpAgent.Count > 0)
            {
                lock(m_addKcpAgent)
                {
                    lock (m_KcpAgents)  //m_KcpAgents锁
                    {
                        for (int i = 0; i < m_addKcpAgent.Count; i++)   //添加
                        {
                            var kcpAgent = m_addKcpAgent[i];

                            m_KcpAgents.Add(kcpAgent.uid, kcpAgent);
                        }
                    }
                    m_addKcpAgent.Clear();
                }
            }


            foreach (var item in m_KcpAgents)
            {
                objRecv.OnRecv(item.Value, item.Value.Recv(objRecv.bytebuffer));
                item.Value.OnUpdate(in dateTime);
                //this.Debug(" OnUpdate >>" + item.Value.ToString());
            }
        }


        /// <summary>
        /// 接收线程
        /// </summary>
        void Thread_Recv()
        {
            while(true)
            {
                try
                {
                    OnRecv();
                }
                catch (Exception e)
                {
                    this.Debug(e.ToString());
                    Thread.Sleep(10);
                }
            }
        }


        void OnRecv()
        {
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            int length = m_Socket.ReceiveFrom(m_RecvBuffer, m_RecvBuffer.Length, SocketFlags.None, ref endPoint);
            GetKcpAgent(endPoint).Input(length);
        }


        /// <summary>
        /// 获得自动添加KcpSocket管理类里面 线程安全
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lockKcpAgents"></param>
        /// <returns></returns>
        public KcpAgent GetKcpAgent(EndPoint point,bool lockKcpAgents = false)
        {
            //endPoint 如果不是 IPEndPoint 另外处理  当前不处理这种情况  后续处理
            IPEndPoint tmpPoint = point as IPEndPoint;
            KcpAgent kcpAgent = null;
            ulong uid = SocketHelper.CastIP(tmpPoint.Address);
            uid = uid << 16 | (ushort)tmpPoint.Port;
            //this.Debug(key.ToString());
            if (lockKcpAgents)
            {
                lock (m_KcpAgents)
                    m_KcpAgents.TryGetValue(uid, out kcpAgent);
            }
            else
            {
                m_KcpAgents.TryGetValue(uid, out kcpAgent);
            }



            if (kcpAgent == null)
            {
                lock (m_addKcpAgent)
                {
                    for (int i = 0; i < m_addKcpAgent.Count; i++)   //在添加队列里面
                    {
                        if (m_addKcpAgent[i].endPoint.Equals(point)) //测试过 成立EndPoint 等于判定成立
                        {
                            kcpAgent = m_addKcpAgent[i];
                            break;
                        }
                    }

                    if (kcpAgent == null)   //添加新的
                    {
                        this.ThreadDebugInfo("sadasd");
                        this.Debug("添加新的 " + tmpPoint.ToString());
                        kcpAgent = new KcpAgent().Bing(this, uid, point);
                        m_addKcpAgent.Add(kcpAgent);
                    }
                }
            }
            return kcpAgent;
        }



        /// <summary>
        /// 外部接口 线程安全 外部提供缓冲区
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <param name="endPoint"></param>
        public void Send(byte[] buffer, int length, IPEndPoint endPoint)
        {
            GetKcpAgent(endPoint).Send(buffer, length);
        }




        ///// 后面封装下 换成id
        ///// </summary>
        ///// <param name="buffer"></param>
        ///// <param name="endPoint"></param>
        ///// <returns></returns>
        //public int Recv(byte[] buffer, IPEndPoint endPoint)
        //{
        //    return GetKcpAgent(endPoint).Recv(buffer);
        //}



        /// <summary>
        /// socket 直接发送
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <param name="endPoint"></param>
        void SockSend(byte[] buffer,int length, EndPoint endPoint)
        {
             m_Socket.SendTo(buffer, length, SocketFlags.None, endPoint);
        }



        void CheckBufferSize(int size)
        {
            if (m_SendBuffer.Length > size) return;
            int tmpLength = m_SendBuffer.Length;

            do
            {
                tmpLength *= 2;
            } while (tmpLength <= size);

            m_SendBuffer = new byte[tmpLength];
        }


        /// <summary>
        /// KCP 代理  一个socket接收 可能有多个代理
        /// </summary>
        public class KcpAgent : IKcpCallback
        {
            public ulong uid;
            public EndPoint endPoint; //后面要接收其他的再改EndPoint  现在只接收IPEndPoint
            KcpSocket m_Socket;
            UnSafeSegManager.Kcp kcp;

          

            internal KcpAgent Bing(KcpSocket socket, ulong uid_, EndPoint endPoint_)
            {
                uid = uid_;
                m_Socket = socket;
                kcp = new UnSafeSegManager.Kcp(NetConst.KcpConv, this);
                kcp.NoDelay(1, 15, 2, 1);
                kcp.WndSize(256, 256);
                endPoint = endPoint_;
                return this;
            }

            /// <summary>
            /// 外部接收
            /// </summary>
            /// <returns></returns>
            public int Recv(byte[] buffer)  //外部可以直接调用
            {
                return kcp.Recv(buffer);
            }

            /// <summary>
            /// 外部发送
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="length"></param>
            public void Send(byte[] buffer, int length) //外部可以直接调用
            {
                Span<byte> buffBytes = new Span<byte>(buffer, 0, length);
                kcp.Send(buffBytes);
            }

            public void Send(byte[] buffer)
            {
                kcp.Send(buffer);
            }


            internal void Input(int length)
            {
                Span<byte> buffBytes = new Span<byte>(m_Socket.m_RecvBuffer, 0, length);
                kcp.Input(buffBytes);
            }

            internal void OnUpdate(in DateTime dateTime)
            {
                kcp.Update(in dateTime);
            }


            void IKcpCallback.Output(IMemoryOwner<byte> buffer, int avalidLength)
            {
                m_Socket.CheckBufferSize(avalidLength);
                Span<byte> buffBytes = new Span<byte>(m_Socket.m_SendBuffer);
                buffer.Memory.Span.Slice(0, avalidLength).CopyTo(buffBytes);
                m_Socket.SockSend(m_Socket.m_SendBuffer, avalidLength, endPoint);
                this.Debug("  Output ");
                buffer.Dispose();
            }

        }

    }




 


}
