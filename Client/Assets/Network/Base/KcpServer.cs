/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/30 22:31:49
/// 功能描述:  Kcp 服务端
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



    /// <summary>
    /// Kcp 服务器  后续有时间重构
    /// </summary>
    public class KcpServer : KcpCSBase
    {



        //Dictionary<string, uint> m_uuidMap = new Dictionary<string, uint>();

        /// <summary>
        /// 只跟ip地址绑定  
        /// 后面检查一下 是否会出现 切换两次变回原来的 ip地址与端口号的情况
        /// </summary>
        Dictionary<uint, KcpSession> m_Sessions = new Dictionary<uint, KcpSession>();


        public override bool IsClient => false;
        public override bool IsServer => true;

        /// <summary>
        /// 绑定端口号  kcp刷新间隔 帧同步33s每次
        /// int port  用于局域网监听广播  -1表示不建立upd 监听  网上服务器不需要监听广播
        /// kcpPort  表示正常服务地址  -1表示随机地址  
        /// </summary>
        /// <param name="port"></param>
        public void Bing(int port = -1,int kcpPort = -1, uint interval = 10)  //间隔时间
        {
            if (port == kcpPort && kcpPort == -1)
            {
                sLog.Error("非法操作");
                return;
            }
            KcpServerRecvHelper recvHelper = new KcpServerRecvHelper();
            base.Bing(recvHelper, port, kcpPort, interval);
            if (m_disposed) return; //启动失败
    
            this.Debug("服务端已经开启");
            recvHelper.kcpServer = this;

            //OnBing();

            //
            StartHeartBeat();   //开始心跳
        }


        void StartHeartBeat()
        {
            //sLog.Debug("------StartHeartBeat---11111---");
            LStream tmpStream = new LStream(255);
            S2C_HeartBeat heartBeat = new S2C_HeartBeat();
            heartBeat.NSerialize(tmpStream);
            List<KcpSession> sendList = null;
            List<uint> closeList = new List<uint>();
            bool isSend = true;
            int CheckHeartBeatPackTime = NetConst.CheckHeartBeatPackTime;
            Task.Run(() =>
            {
                while (!m_disposed)
                {
                    Thread.Sleep(6010);
                    if (isSend)
                    {
                        isSend = false;
                        sendList = ListPool<KcpSession>.Get();
                    }
                  
                    //sLog.Debug("------StartHeartBeat---start---");
                    if (m_disposed) break; //启动失败
                    long nowTicks = DateTime.UtcNow.Ticks;

                    lock (m_Sessions) {
                        foreach (var session in m_Sessions) {
                            if (session.Value.checkTime <= nowTicks) {
                                if (m_disposed) break; //启动失败
                                if (session.Value.heartbeatClose) continue;

                                if (session.Value.checkCount >= 3) {
                                    sLog.Debug($">>>>>> 心跳关闭 guid: <{session.Key}> endPoint: <{session.Value.kcpAgent.endPoint.ToString()}>  useid:{session.Value.useid} ");
                                    // 心跳只表示 连接状态
                                    //session.Close();
                                    if (session.Value.useid == 0) {
                                        closeList.Add(session.Key); //未登录游戏  只是临时窗口 断开连接后直接关闭
                                    }
                                    else {
                                        session.Value.heartbeatClose = true;    //关闭心跳  已经登录的玩家 在 游戏中控制关闭逻辑
                                    }
                                   
                                    netMsgMgr.BroadCastEventByMainThreadt(GameEventType.ServerEvent_Disconnect, session.Value);   //断开连接 如果是玩家的话 断开连接
                                }
                                else {
                                    session.Value.checkCount++;
                                    session.Value.UpdateNextCheckTime();
                                    sendList.Add(session.Value);
                                }
                            }
                        }
                    }

                    if (closeList.Count > 0) {
                        for (int i = 0; i < closeList.Count; i++) {
                            CloseSessions(closeList[i]);
                        }
                        closeList.Clear();
                    }

                    //不在该线程执行  session.Send 没锁，如果在不同线程调用  会造成未知错误   所以放到主线程send
                    if (sendList.Count > 0)
                    {
                        isSend = true;
                        netMsgMgr.QueueOnMainThreadt((_sendList, _tmpStream) =>
                        {
                            foreach (var item in _sendList)
                            {
                                if (item.close) //存在延迟  以及关闭  不执行
                                    continue;
                                item.Send(_tmpStream);
                                //sLog.Debug($"{item.name} : send HeartBeat");
                            }
                            //bool f = _sendList.Equals(sendList);
                            //sLog.Debug("------StartHeartBeat--QueueOnMainThreadt----" + f);
                          
                            _sendList.Release();

                        }, sendList, tmpStream);
                    }

                }
            });


        }

        //public virtual void OnBing()
        //{
        //    tmpData.n_S2C_GetAllServersInfo.port = LocalPort;

        //}


        public class KcpServerRecvHelper : RecvHelper
        {
            public KcpServer kcpServer;

            public KcpServerRecvHelper()
            {
                sLog.Warning("  初始化成功  ");
            }

            protected override void OnRecv(KcpSocket.KcpAgent kcp, int count)
            {
                //sLog.Debug("------OnRecv-------");
                //base.OnRecv(kcp, count);
                if (count < 4)
                {
                    sLog.Error("接收到一个未知的信息 count: " + count);
                    return;
                }

                //sLog.Debug("  kcp.uid： " + kcp.uid);
         
                //执行代码
                var NetMsgtype = this.stream.GetNetMsgType();
                kcpServer.NetMsgHandling.OnServerMsg(kcpServer, NetMsgtype, kcpServer.GetOrAddSessions(kcp), stream);

            }

            protected override void OnConnectEvent(ConnectEvent evt, KcpSocket.KcpAgent kcp)
            {
                if (evt == ConnectEvent.Server_ClentReConnectDone) {
                    //重登
                    kcpServer.ReConnect(kcp);
                }
                else {
                    this.DebugError($" 未知回调 : <{evt}>");
                }
            }

        }



        

        void OnConnect(KcpSocket.KcpAgent kcp)
        {
            //var session = GetSessions(kcp.uid);     //用ip地址绑定Sessions
            //if (session == null)
            //{
            //    //session = AddSessions(kcp);

            //    //session.UpdateCheckTime();

            //    sLog.Debug("OnConnect  tmpData.S2C_Connect msgType {0} account: {1}", tmpData.S2C_Connect.msgType, session.account);
               
            //}
            //else
            //{
            //    //表示已经存在了 需要去处理下 比如断网重连  我这里不做处理 
            //    sLog.Warning(" guid 在登录中 当前暂时先退出原来的"); //表示重新登录  那么顶替之前的
               
            //}
        }



        /// <summary>
        /// 退出重进 再连接
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="newkcp"></param>
        void ReConnect(KcpSocket.KcpAgent kcp)
        {
            ////暂时不使用
            //KcpSession session = GetSessions(guid);
            ////ReConnect 关闭原来的
            //session.kcpAgent.Close();   //
            //session.kcpAgent = newkcp;  //重新连接
            ////tmpData.S2C_Connect.uid = guid;   //连接成功
            //session.account = tmpData.C2S_Connect.account;
            //session.UpdateCheckTime();
            //session.Send(tmpData.S2C_Connect, false);    //发送数据 线程不安全  后面有时间 哭放s_mouble 里面处理  这样线程安全

            KcpSession session = GetSessions(kcp.uid);
            session.UpdateCheckTime();
            netMsgMgr.BroadCastEventByMainThreadt(GameEventType.ServerEvent_ReConnect, session);    //修改游戏里面的数据数据
        }


        KcpSession GetOrAddSessions(KcpSocket.KcpAgent kcp)
        {
            KcpSession session = GetSessions(kcp.uid);
            if (session == null) {
                session = AddSessions(kcp);
            }
            return session;
        }


        public KcpSession GetSessions(uint id)
        {
            KcpSession session = null;
            lock (m_Sessions)   //保证线程安全
                m_Sessions.TryGetValue(id, out session);
            return session;
        }

        /// <summary>
        /// 外卖调用注意线程安全
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        KcpSession AddSessions(KcpSocket.KcpAgent kcp)
        {
            KcpSession session = new KcpSession();
            lock (m_Sessions)   //保证线程安全
                m_Sessions.Add(kcp.uid, session);

            session.kcpAgent = kcp;
            session.useid = 0;
            session.server = this;
            return session;
        }

        protected void CloseSessions(uint guid)
        {
            KcpSession session;
            lock (m_Sessions)   //保证线程安全
            {
                session = m_Sessions[guid];
                m_Sessions.Remove(guid);
            }
            session.Close();
            //session.kcpAgent.Close();   //
        }



        ///// <summary>
        ///// 广播给所有玩家
        ///// </summary>
        //public void Broadcast<T>(T data, bool IsRecycle = true) where T : S2C_BASE<T>, new()
        //{
        //    if (m_Sessions.Count == 0)
        //    {
        //        return;
        //    }

        //    if (sLog.OpenMsgInfo)
        //        sLog.Debug(">>>>>Broadcast Send msgType : {0}", data.msgType);

        //    LStream stream = null;

        //    foreach (var item in m_Sessions)
        //    {
        //        KcpSession session = item.Value;
        //        if (stream == null)
        //        {
        //            stream = session.GetStream();
        //            data.Serialize(stream);
        //        }

        //        session.Send(stream);
        //    }

        //    if (IsRecycle)
        //        data.Release();
        //}

        /// <summary>
        /// 广播给所有玩家
        /// </summary>
        public void Broadcast<T>(T data, List<KcpSession> sessions, bool IsRecycle = true) where T : S2C_BASE<T>, new()
        {

            if (sessions == null || sessions.Count ==  0)
            {
                return;
            }

            LStream stream = null;

            if (sLog.OpenMsgInfo)
                sLog.Debug(">>>>>Broadcast Send msgType : {0}", data.msgType);

            foreach (var item in sessions)
            {
                KcpSession session = item;
                if (stream == null)
                {
                    stream = session.GetStream();
                    data.Serialize(stream);
                }

                session.Send(stream);
            }

            if (IsRecycle)
                data.Release();
        }


        public class KcpSession //: ISession
        {
            public uint sessionID => kcpAgent.uid;
            public bool close = false;
            LStream m_SendStream = new LStream(NetConst.Socket_SendBufferSize);
            //byte[] m_SendBuffer = new byte[NetConst.Socket_SendBufferSize]; //发送最大大小
            public uint useid = 0;
            public KcpSocket.KcpAgent kcpAgent;
            public string account;
            public KcpServer server;
            public long checkTime;     //时间
            public float checkCount;    //心跳
            public bool heartbeatClose; //心跳关闭

            public LStream GetStream() => m_SendStream;

            public void SendNotRecycle<T>(T data) where T : S2C_BASE<T>, new()
            {
                if (close) {
                    return;
                }

                if (sLog.OpenMsgInfo)
                    sLog.Debug(">>>>> Send msgType : {0}", data.msgType);

                data.Serialize(m_SendStream);
                kcpAgent.Send(m_SendStream.GetBuffer(), m_SendStream.Lenght);
            }

            /// <summary>
            /// 发送数据 线程不安全  注意使用的场合  后面看情况加不加锁
            /// 注意是否在同一个线程中
            /// </summary>
            /// <typeparam name="T"></typeparam>
            public void Send<T>(T data, bool IsRecycle = true) where T : S2C_BASE<T>, new()
            {
                if (close) {
                    if (IsRecycle)
                        data.Release();
                    return;
                }

                if (sLog.OpenMsgInfo)
                    sLog.Debug(">>>>> Send msgType : {0}", data.msgType);

                data.Serialize(m_SendStream); 
                //后面可以将序列化单独一个线程中 队列中方list就行了 如果全在一个list的话 那么全部默认回收
                kcpAgent.Send(m_SendStream.GetBuffer(), m_SendStream.Lenght);
                if (IsRecycle)
                    data.Release();
            }


            /// <summary>
            /// 自定义流
            /// </summary>
            /// <param name="_stream"></param>
            public void Send(LStream _stream)
            {
                if (close) {
                    return;
                }
                if (sLog.OpenMsgInfo)
                    sLog.Debug(">>>>> Send _stream.Lenght : {0}", _stream.Lenght);

                kcpAgent.Send(_stream.GetBuffer(), _stream.Lenght);
            }


            internal void UpdateNextCheckTime()
            {
                checkTime = DateTime.UtcNow.Ticks + 5000;     //5s检查一次
            }
            /// <summary>
            /// 更新 心跳时间
            /// </summary>
            internal void UpdateCheckTime()
            {
              
                //更新
                UpdateNextCheckTime();
                checkCount = 0;
                heartbeatClose = false;
                //sLog.Debug(">>>> checkTime : {0}  checkCount： {1}", checkTime, checkCount);
            }


            public void ServerCloseSession()
            {
                if (close) return;  
                server.CloseSessions(sessionID);
            }

            internal void Close()
            {
                if (close) return;

                close = true;
                //server.CloseSessions(playerID); //注释掉了 不然无线循环了
                m_SendStream = null;
                server = null;
                this.Debug(" KcpSession close {0}", kcpAgent.endPoint);
                kcpAgent.Close();
            }


          
        }




        public override void Dispose()
        {
            base.Dispose();
        }

    }

    

}


