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
using LGF.Serializable;

namespace LGF.Net
{
    public delegate void KcpOnRecv(KcpSocket.KcpAgent kcpAgent, LStream stream, int count);

    public class KcpSocketOnRecvHelper : IKcpSocketOnRecv
    {
        public KcpOnRecv kcpOnRecv;
        public LStream stream = new LStream(NetConst.Socket_RecvBufferSize);
        protected byte[] bytebuffer => stream.GetBuffer();
        byte[] IKcpSocketOnRecv.bytebuffer => bytebuffer;

        void IKcpSocketOnRecv.OnRecv(KcpSocket.KcpAgent kcp, int count)
        {
            if (count >= -3 && count <= 0)
                return;     //不需要处理

            if (count <= -3)
            {
                count = -1 * count - 3;
                //bytebuffer = (byte[])System.Array.CreateInstance(typeof(byte), SocketHelper.ceilpow2(count));
                stream = new LStream(count);  //空间不够
                count = kcp.Recv(bytebuffer);   //重新接收数据
            }

            //看需要需不需要新弄个线程用来处理new 的数据 如果需要个到的话
            //弄一个需要弄一个大空间的队列然后在线程里面处理   我觉得这里不需要弄了
            OnRecv(kcp, count);
        }

        protected virtual void OnRecv(KcpSocket.KcpAgent kcp,int count)
        {
            
        }

        protected virtual void OnConnectEvent(ConnectEvent evt, KcpSocket.KcpAgent kcp)
        {
            throw new NotImplementedException();
        }

        void IKcpSocketOnRecv.OnConnectEvent(ConnectEvent evt, KcpSocket.KcpAgent kcp)
        {
            OnConnectEvent(evt, kcp);
        }
    }

    /// <summary>
    /// 连接事件
    /// </summary>
    public enum ConnectEvent
    {
        Client_ConnectDone,  //登录完成
        Client_ReConnectDone, //客户端重新连接
        Client_Close,

        Server_ClentReConnectDone, //服务器的 客户重新连接 
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

        void OnConnectEvent(ConnectEvent evt, KcpSocket.KcpAgent kcp);

    }

    /// <summary>
    /// KcpSocket  线程安全    后面看你用条件变量写Dispose  阻塞其他线程 现在写得感觉有段乱
    /// 没做删除检查
    /// </summary>
    public class KcpSocket 
    {
        System.Net.Sockets.Socket m_Socket;
        IPEndPoint m_ipEndPoint;
        byte[] m_SendBuffer = new byte[NetConst.Socket_SendBufferSize];
        byte[] m_RecvBuffer = new byte[NetConst.Socket_RecvBufferSize];
        Thread thread;
        bool m_disposed = false;
        Dictionary<ulong, KcpAgent> m_KcpAgents = new Dictionary<ulong, KcpAgent>();   
        List<KcpAgent> m_addKcpAgent = new List<KcpAgent>();     //缓冲代理
        List<KcpAgent> m_delKcpAgent = new List<KcpAgent>();
        Timers.SimpleAsynTimer m_timer; //简单定时器
        public bool IsServer;   //是否是服务器
        int m_SendSpinLock = 0; //发送的自旋锁
      
        public bool IsDisposed => m_disposed;


        public bool HasBing => m_Socket != null;
        public Socket Sock => m_Socket;
        IKcpSocketOnRecv objRecv;
        uint CurUid = 0;
        /// <summary>
        /// 服务器代理  客户端使用
        /// </summary>
        public KcpAgent ServerAgent;    

        public bool Bing(IKcpSocketOnRecv onRecv, int port, uint interval = 10, bool isServer = true)
        {
            IsServer = isServer;
            objRecv = onRecv;
            try
            {
                m_ipEndPoint = new IPEndPoint(IPAddress.Any, port);
                m_Socket = SocketHelper.UdpBind(m_ipEndPoint);
            }
            catch (Exception e)
            {
                this.DebugError(e.ToString());
                return false;
                //throw;
            }

            this.Debug("Bing " + m_Socket.LocalEndPoint.ToString());
            m_disposed = false;

            //后续有外部线程 这个可以放外部去 线程过多的话 没必要单独给他一个线程
            thread = new Thread(Thread_Recv) { IsBackground = true };
            thread.Start(); //


            m_timer = new Timers.SimpleAsynTimer(() =>
            {
                OnUpdate(DateTime.UtcNow);
            }, interval);

            return true;
        }


       


        /// <summary>
        /// 线程安全移除
        /// </summary>
        public void Dispose()
        {
            m_disposed = true;
            //thread.Abort();
            thread = null;
            lock (m_addKcpAgent)
            {
                for (int i = 0; i < m_addKcpAgent.Count; i++)
                    m_addKcpAgent[i].Dispose();

                m_addKcpAgent.Clear();
                m_addKcpAgent = null;
            }
            m_ipEndPoint = null;
            m_Socket.Close();
            this.DebugError("m_Socket Close");
            m_Socket.Dispose();
            m_Socket = null;

            m_timer?.Disposed(); //阻塞线程
            m_timer = null;
            //清理下m_KcpAgents数据   m_timer未阻塞线程话
            OnUpdate(DateTime.Now);
        }


        /// <summary>
        /// 更新和发送线程
        /// 后面单独一个线程去处理他  我这里打算用 LGF.Timer 调用他  LGF.Timer 自带线程
        /// 后面在优化下  
        /// 搭配LGF.SimpleAsynTimer 应该线程安全
        /// </summary>
        /// <param name="dateTime"></param>
        public void OnUpdate(in DateTime dateTime)   
        {
            //this.Debug(" OnUpdate11  ");
            //在这里自己处理 在处理安全点不然要加锁 如果要加锁 就没必要addKcpAgent的方法了
            if (m_disposed)
            {
                if (m_KcpAgents == null) return;

                foreach (var item in m_KcpAgents)
                    item.Value.Dispose();
                m_KcpAgents.Clear();
                m_KcpAgents = null;
                return;
            }

            //this.Debug(" OnUpdate ");
            if (m_addKcpAgent?.Count > 0)
            {
                lock(m_addKcpAgent)
                {
                    if (m_disposed) return;
                    lock (m_KcpAgents)  //m_KcpAgents锁
                    {
                        if (m_disposed) return;
                        for (int i = 0; i < m_addKcpAgent.Count; i++)   //添加
                        {
                            var kcpAgent = m_addKcpAgent[i];
                            this.Debug($" OnUpdate add kcpAgent.uid {kcpAgent.uid} ");
                            m_KcpAgents.Add(kcpAgent.uid, kcpAgent);
                        }
                    }
                    m_addKcpAgent.Clear();
                }
            }

        

            foreach (var item in m_KcpAgents)
            {
                if (item.Value.close)
                {
                    m_delKcpAgent.Add(item.Value);
                    continue;
                }
                   
                objRecv.OnRecv(item.Value, item.Value.Recv(objRecv.bytebuffer));
                item.Value.OnUpdate(in dateTime);
                //this.Debug(" OnUpdate >>" + item.Value.ToString());
            }

            //--------------删除--------------------
            if (m_delKcpAgent.Count > 0)
            {
                lock(m_KcpAgents)
                {
                    for (int i = 0; i < m_delKcpAgent.Count; i++)
                    {
                        this.Debug("KcpAgent Remove >>" + m_delKcpAgent[i].uid);
                        m_KcpAgents.Remove(m_delKcpAgent[i].uid);
                        m_delKcpAgent[i].Dispose();
                    }
                   
                }
                m_delKcpAgent.Clear();
            }
           
            

        }


        /// <summary>
        /// 接收线程
        /// </summary>
        void Thread_Recv()
        {
            while(!m_disposed)
            {
                //OnRecv();
                try
                {
                    OnRecv();
                }
                catch (Exception e)
                {
                    //unity可以无视第一次线程报错 
                    //我关掉异常捕获就没有 报错了  不知道为啥  开启有时候报错  有时候又有
                    this.DebugError(e.ToString());
                    Thread.Sleep(10);
                }
            }
        }


        void OnRecv()
        {
            if (m_Socket == null) return;
            if (m_Socket.Available <= 0)
            {
                Thread.Sleep(10);    //没事情做休息一下
                return;
            }

            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            int length = m_Socket.ReceiveFrom(m_RecvBuffer, m_RecvBuffer.Length, SocketFlags.None, ref endPoint);
            //this.Debug($"OnRecv:  {endPoint} {length}");
            if (m_disposed) return;
            //TODO:未完成
            //Check 需要加个检查条件 如果时服务端的话。  ---只接收 对应服务端的 KcpAgent 代理  可以用回调实现
            //这里需要处理一下未知的 EndPoint
            //GetKcpAgent(endPoint, false)?.Input(length);
            if (!IsServer) {
                ClientOnRecv(endPoint, length);
            }
            else {
                ServerOnRecv(endPoint, length);
            }
        }

        #region Client


        /// <summary>
        /// 重新绑定 ip 地址
        /// 用来实现 客户端重连机制
        /// </summary>
        public bool ReBing()
        {
            var tmpSocket = m_Socket;
            try {
                m_Socket = SocketHelper.UdpBind(m_ipEndPoint);
            }
            catch (Exception e) {
                this.DebugError(e.ToString());
                return false;
                //throw;
            }

            tmpSocket.Close();   //关闭原来的socket
            //this.DebugError("Bing ReBing");
            this.Debug("Bing ReBing" + m_Socket.LocalEndPoint.ToString());
            return true;
        }

        void ClientOnRecv(in EndPoint endPoint, int length)
        {
            uint uid = BitConverter.ToUInt32(m_RecvBuffer, 0);  //id
            if (uid == 0) { //连接成功
                uid = BitConverter.ToUInt32(m_RecvBuffer, 4);
                if (uid >= NetConst.KcpConvInitialValue) {
                    //连接成功
                    CurUid = uid;
                    IPEndPoint tmpPoint = endPoint as IPEndPoint;
                    ulong AddressUid = ((ulong)tmpPoint.Address.GetHashCode() << 16 | (ushort)tmpPoint.Port);  //唯一key值
                    ServerAgent?.Close();   //关闭原来的代理开启新的   存在重连
                    ServerAgent = null;
                    ServerAgent = GetOrAddKcpAgent(uid, endPoint, AddressUid, false); //添加代理
                    //且需要发送
                    //OnConnectServerEvent.Invoke(ConnectServerEvent.LoginDone);
                    objRecv.OnConnectEvent(ConnectEvent.Client_ConnectDone, ServerAgent); //客户端连接完毕
                }
                else {
                    
                    ////连接失败
                    //NetConst.KcpAbnormal_1
                    if (NetConst.KcpAbnormal_1 == uid) {
                        //重连失败 需要 重新登录  服务端已经关闭代理了
                        //重新请求
                        sLog.Debug("掉线了 需要重启游戏");
                        objRecv.OnConnectEvent(ConnectEvent.Client_Close, ServerAgent);
                    }
                    else if (NetConst.KcpAbnormal_2 == uid) {
                        sLog.Debug("重连成功");
                        objRecv.OnConnectEvent(ConnectEvent.Client_ReConnectDone, ServerAgent);
                    }
                    else {
                        sLog.Error(">>>> 出错.....");
                    }
                }
            }
            else {
                //正常流程
                if (CurUid == uid) {
                    ServerAgent.Input(length);
                }
                else {
                    //和服务端异常1一样
                    //objRecv.OnConnectEvent(ConnectEvent.Client_Close, ServerAgent);//让他重启游戏
                    ReBing();//重连试下？
                    sLog.Error(" 出错.... ");
                }

            }
        }




        ///// <summary>
        ///// 连接服务器事件
        ///// 1表示成功
        ///// 2表示需要重连
        ///// </summary>
        //System.Action<ConnectServerEvent> OnConnectServerEvent;

        public void ClientTryConnectServer(EndPoint point)
        {
            SockSend(new byte[4], 4, point);
        }


        public KcpAgent ClientGetServer()
        {
            if (0 == CurUid) {
                return null;
            }
            return GetKcpAgent(CurUid, true);
        }

        #endregion

        #region server

        
        //处理 重复发送问题  一个地址绑定一个uid  同时检测掉线重登的问题
        Dictionary<ulong, uint> Address2Uid = new Dictionary<ulong, uint>();    //晚点加自旋锁

        void ServerOnRecv(in EndPoint endPoint, int length)
        {
            uint uid = BitConverter.ToUInt32(m_RecvBuffer, 0);  //id
            IPEndPoint tmpPoint = endPoint as IPEndPoint;
            ulong AddressUid = ((ulong)tmpPoint.Address.GetHashCode() << 16 | (ushort)tmpPoint.Port);  //唯一key值
            if (0 == uid) {
                //连接 
                lock (Address2Uid) {    
                    //同一个ip地址可能发送多个可能发送多个过来    过滤掉多余的生成的uid
                    if (!Address2Uid.TryGetValue(AddressUid, out uid)) {
                        uid = GenerateUniqueUID();
                        Address2Uid.Add(AddressUid, uid);
                    }
                    else {
                        //有时候出现重复连接请求的情况  在网络不稳定的情况下

                        //sLog.Error("  出现非法情况， 请检查一下代码 当前IP地址 已经在服务器中 出现重复连接的情况");
                        //return;
                    }

                }

                ServerSendUID(uid, endPoint);
                //GetOrAddKcpAgent(uid,endPoint, false);  //
            }
            else {
                bool hasAddress2Uid = false;
                uint uid2 = 0;
                lock (Address2Uid) {
                    hasAddress2Uid = Address2Uid.TryGetValue(AddressUid, out uid2);
                }
               
                if (hasAddress2Uid) {
                    //没换ip地址
                    if (uid2 != uid) {
                        //异常  这种情况是 ip地址与端口号没有换掉 且原来的ip地址没回收  这时候用导致同一个端口号 不同的uid 出问题。
                        this.DebugError("---------->> 异常1");
                    }
                    else {
                        //正常流程  
                        GetOrAddKcpAgent(uid, endPoint, AddressUid, false).Input(length);
                    }

                  
                }
                else {
                    //新的ip地址  但是客户端有老的uid的数据  表示那么该状态表示断线重连
                    this.Debug($">>> 断线重连 <{uid}>");
                    var kcp = GetKcpAgent(uid);
                    if (kcp == null) {
                        this.DebugError($"---------->> 客户端重新在服务器中已掉线  但是客户端还能继续请求。 发送客户端错误代码 uid<{uid}>");
                        //换ip地址(换wifi 换流量)  或者断网  需要重连
                        //id相同 没有下线 表示值断了一下 卡了一下 或者换了网络环境
                        //卡了的话 服务器会删除当前这个KcpAgent 和  Addressuid
                        //让客户端自己判断走流程 比如重新登录 目前没做这块  晚点查看一下

                        ServerSendUID(NetConst.KcpAbnormal_1, endPoint);
                    }
                    else {
                        kcp.endPoint = endPoint;    //更换新的ip地址

                        lock (Address2Uid) {
                            Address2Uid.Remove(kcp.AddressUid); //删除原来的
                            Address2Uid.Add(AddressUid, uid);   //改成新的IP地址
                            kcp.AddressUid = AddressUid;
                        }

                        ServerSendUID(NetConst.KcpAbnormal_2, endPoint);
                        objRecv.OnConnectEvent(ConnectEvent.Server_ClentReConnectDone, kcp); //客户端连接完毕
                        kcp.Input(length);  //继续处理数据
                    }

                }
              
            }
        }

        void ServerSendUID(uint uid, in EndPoint endPoint)
        {
            StartSpinLock_Send();   //开启发送锁
            m_SendBuffer[0] = 0;
            m_SendBuffer[1] = 0;
            m_SendBuffer[2] = 0;
            m_SendBuffer[3] = 0;
            Array.Copy(BitConverter.GetBytes(uid), 0, m_SendBuffer, 4, 4);
            SockSend(m_SendBuffer, 8, endPoint);
            EndSpinLock_Send();     //关闭发送锁
        }

        #endregion
        //获得或者添加代理
        public KcpAgent GetOrAddKcpAgent(uint uid, in EndPoint endPoint,ulong AddressUid, bool lockKcpAgents = true)
        {
            KcpAgent kcpAgent = GetKcpAgent(uid, lockKcpAgents);
        
            if (kcpAgent == null) {
                lock (m_addKcpAgent) {
                    if (m_disposed) return null;
                    //在添加队列里面
                    for (int i = 0; i < m_addKcpAgent.Count; i++) {
                        //测试过 成立EndPoint 等于判定成立
                        if (m_addKcpAgent[i].uid == uid) {  
                            kcpAgent = m_addKcpAgent[i];
                            break;
                        }
                    }
                    //添加新的
                    if (kcpAgent == null)  {
                        this.Debug("添加新的 kcpAgent : " + endPoint.ToString());
                        kcpAgent = new KcpAgent().Bing(this, uid, AddressUid, endPoint);
                        m_addKcpAgent.Add(kcpAgent);
                    }
                }
            }
            else {

                //if (!kcpAgent.endPoint.Equals(point))  
                //{
                //    this.DebugError($"{point} {kcpAgent.endPoint} 的哈希值相同  {uid}");
                //}
            }
            if (m_disposed) return null;
            return kcpAgent;
        }

        public KcpAgent GetKcpAgent(uint uid, bool lockKcpAgents = true)
        {
            KcpAgent kcpAgent = null;
            if (lockKcpAgents) {
                //不知道为啥之前报错 现在又不报错了先不管了
                lock (m_KcpAgents) {
                    //m_KcpAgents Dispose注销时 m_KcpAgents 为null
                    m_KcpAgents?.TryGetValue(uid, out kcpAgent);
                }
            }
            else {
                m_KcpAgents?.TryGetValue(uid, out kcpAgent);
            }
            return kcpAgent;
        }



        ///// <summary>
        ///// 获得自动添加KcpSocket管理类里面 线程安全
        ///// </summary>
        ///// <param name="point"></param>
        ///// <param name="lockKcpAgents"></param>
        ///// <returns></returns>
        //public KcpAgent GetKcpAgent(in EndPoint point, bool lockKcpAgents = true)
        //{
        //    //endPoint 如果不是 IPEndPoint 另外处理  当前不处理这种情况  后续处理
        //    IPEndPoint tmpPoint = point as IPEndPoint;
        //    KcpAgent kcpAgent = null;
        //    ulong uid = (ulong)((ushort)tmpPoint.Address.GetHashCode() << 16 | tmpPoint.Port);  //唯一key值

        //    if (lockKcpAgents)
        //    {
        //        //不知道为啥之前报错 现在又不报错了先不管了
        //        lock (m_KcpAgents)  
        //        {
        //            //m_KcpAgents Dispose注销时 m_KcpAgents 为null
        //            m_KcpAgents?.TryGetValue(uid, out kcpAgent);    
        //        }

        //    }
        //    else
        //    {
        //        m_KcpAgents?.TryGetValue(uid, out kcpAgent);
        //    }

        //    if (kcpAgent == null)
        //    {
        //        lock (m_addKcpAgent)
        //        {
        //            if (m_disposed) return null;

        //            for (int i = 0; i < m_addKcpAgent.Count; i++)   //在添加队列里面
        //            {
        //                if (m_addKcpAgent[i].endPoint.Equals(point)) //测试过 成立EndPoint 等于判定成立
        //                {
        //                    kcpAgent = m_addKcpAgent[i];
        //                    break;
        //                }
        //            }

        //            if (kcpAgent == null)   //添加新的
        //            {
        //                this.Debug("添加新的 kcpAgent : " + tmpPoint.ToString());
        //                kcpAgent = new KcpAgent().Bing(this, uid, point);
        //                m_addKcpAgent.Add(kcpAgent);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //验证代码 我觉得不可能出现 关掉了
        //        //if (!kcpAgent.endPoint.Equals(point))  
        //        //{
        //        //    this.DebugError($"{point} {kcpAgent.endPoint} 的哈希值相同  {uid}");
        //        //}
        //    }
        //    if (m_disposed) return null;
        //    return kcpAgent;
        //}

        /// <summary>
        /// 开始自旋锁 发送的
        /// </summary>
        private void StartSpinLock_Send()
        {
            while (Interlocked.Exchange(ref m_SendSpinLock, 1) != 0) {
                Thread.SpinWait(1);//自旋锁等待
            }
        }

        /// <summary>
        /// 关闭自旋锁 发送的
        /// </summary>
        private void EndSpinLock_Send()
        {
            //释放锁：将_SpinLock重置会0；
            Interlocked.Exchange(ref m_SendSpinLock, 0);
        }




        /// <summary>
        /// socket 直接发送
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <param name="endPoint"></param>
        void SockSend(byte[] buffer,int length, EndPoint endPoint)
        {
            //
#if !UNITY_ANDROID
            m_Socket.SendTo(buffer, length, SocketFlags.None, endPoint);
#else
            //安卓端的时候  断网的时候 会异常  子线程无法捕获异常导致报错
            //报错也无任何信息
            try {
                m_Socket.SendTo(buffer, length, SocketFlags.None, endPoint);
            }
            catch (Exception e) {
                this.Debug("网络不可达...");
            }
#endif

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


     
        public uint GenerateUniqueUID()
        {
          
            lock (m_KcpAgents) {
                while (true) {
                    ++CurUid;
                    if (CurUid == uint.MaxValue || CurUid < NetConst.KcpConvInitialValue) {
                        CurUid = NetConst.KcpConvInitialValue;
                    }
                    if (!m_KcpAgents.ContainsKey(CurUid)) {
                        break;
                    }
                }
            }
            return CurUid;
        }



        /// <summary>
        /// KCP 代理  一个socket接收 可能有多个代理
        /// TODO:未完成
        /// 后面需要写个心跳检查连接时间
        /// </summary>
        public class KcpAgent : IKcpCallback
        {
            public bool close = false;
            public uint uid;
            public ulong AddressUid;
            /// <summary>
            /// 如果断线重连 直接修改 endPoint地址就行了  
            /// 服务器和客户端同时换 不修改其他部分
            /// </summary>
            public EndPoint endPoint;
            KcpSocket m_Socket; 
            UnSafeSegManager.Kcp kcp;
            //内部提供一个缓冲流 LGF

            internal KcpAgent Bing(KcpSocket socket, uint conv_, ulong AddressUid_, EndPoint endPoint_)
            {
                AddressUid = AddressUid_;
                uid = conv_;
                m_Socket = socket;
                kcp = new UnSafeSegManager.Kcp(conv_, this);  //conv 与 uid 共用一个
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
                if (kcp == null) {
                    this.DebugError("-----44444--");
                }
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

                //m_Socket.testReBing
                m_Socket.StartSpinLock_Send();  //开启自旋锁
                m_Socket.CheckBufferSize(avalidLength);
                Span<byte> buffBytes = new Span<byte>(m_Socket.m_SendBuffer);
                buffer.Memory.Span.Slice(0, avalidLength).CopyTo(buffBytes);
                m_Socket.SockSend(m_Socket.m_SendBuffer, avalidLength, endPoint);
                //this.Debug(">>  Output avalidLength{0}", avalidLength);
                buffer.Dispose();
                m_Socket.EndSpinLock_Send();    //关闭自旋锁

            }


            public void Dispose()
            {
                //this.DebugError("------Dispose");
                kcp.Dispose();
                m_Socket = null;
                endPoint = null;
                kcp = null;
            }

            public void Close()
            {
                close = true;
                lock (m_Socket.Address2Uid) {
                    m_Socket.Address2Uid.Remove(AddressUid);
                }
            }


        }

    }




 


}
