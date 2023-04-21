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



        Dictionary<string, uint> m_uuidMap = new Dictionary<string, uint>();

        /// <summary>
        /// 所有的Session号  和玩家ID绑定
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

            OnBing();

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
            bool isSend = true;
            Task.Run(() =>
            {
                while (!m_disposed)
                {
                    Thread.Sleep(2510);
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

                                if (session.Value.checkCount >= 3) {
                                    sLog.Debug($"------心跳关闭 guid: {session.Key} playerID: {session.Value.playerID}");
                                    //主线程去关闭连接
                                    netMsgMgr.BroadCastEventByMainThreadt(GameEventType.ServerEvent_Disconnect, session.Value);   
                                }
                                else {
                                    session.Value.checkCount++;
                                    session.Value.UpdateNextCheckTime();
                                    sendList.Add(session.Value);
                                }
                            }
                        }
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

        public virtual void OnBing()
        {
            tmpData.n_S2C_GetAllServersInfo.port = LocalPort;

        }


        public class KcpServerRecvHelper : RecvHelper
        {
            public KcpServer kcpServer;

            protected override void OnRecv(KcpSocket.KcpAgent kcp, int count)
            {
                //sLog.Debug("------OnRecv-------");
                //base.OnRecv(kcp, count);
                if (count < 8)
                {

                    sLog.Error("接收到一个未知的信息 count: " + count);
                    return;
                }

                //执行代码
                var NetMsgtype = this.stream.GetNetMsgType();
                uint uid = this.stream.GetUid();   //获得协议id  也是玩家的唯一ID

                if (NetMsgtype == NetMsgDefine.C2S_Connect)
                {
                    kcpServer.OnConnect(stream, kcp);
                }
                else
                {
                    //记得处理下换网路的问题 这里没处理了
                    kcpServer.NetMsgHandling.OnServerMsg(kcpServer, NetMsgtype, uid, stream);
                }

            }

        }

        void OnConnect(LStream stream, KcpSocket.KcpAgent kcp)
        {
            //处理服务器
            //特殊处理
            //创建数据
            tmpData.C2S_Connect.Deserialize(stream);

            if (m_uuidMap.TryGetValue(tmpData.C2S_Connect.uuid, out uint guid)) 
            {

                sLog.Warning(" uuid : {0} 登录过", tmpData.C2S_Connect.uuid); //表示重新登录  那么顶替之前的
            }
            else
            {
                guid = GenSessionUniqueID();
                m_uuidMap.Add(tmpData.C2S_Connect.uuid, guid);  //绑定
                sLog.Warning(" SessionID : {0} --- uuid{1} 登录", guid, tmpData.C2S_Connect.uuid);
            }

            var session = GetSessions(guid);
            if (session == null)
            {
                InitConnect(kcp, guid);
            }
            else
            {
                //表示已经存在了 需要去处理下 比如断网重连  我这里不做处理 
                sLog.Warning(" guid 在登录中 当前暂时先退出原来的"); //表示重新登录  那么顶替之前的
                ReConnect(guid, kcp);    //退出重进的情况
            }
          
        }


        void InitConnect(KcpSocket.KcpAgent kcp, uint guid)
        {
            var session = AddSessions(guid, kcp);
            tmpData.S2C_Connect.uid = guid;   //连接成功
            session.name = tmpData.C2S_Connect.name;
            session.UpdateCheckTime();

            sLog.Debug("OnConnect  tmpData.S2C_Connect msgType {0} name: {1}", tmpData.S2C_Connect.msgType, session.name);
            //这里其实线程不安全 但是登录模块 session 是新的 所以直接用
            session.Send(tmpData.S2C_Connect, false);    //发送数据
   

            //通知玩家登录了
            netMsgMgr.BroadCastEventByMainThreadt(GameEventType.ServerEvent_PlayerConnect, session);
        }


        /// <summary>
        /// 退出重进 再连接
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="newkcp"></param>
        void ReConnect(uint guid, KcpSocket.KcpAgent newkcp)
        {
            KcpSession session = GetSessions(guid);

            session.kcpAgent.Close();   //
            session.kcpAgent = newkcp;  //重新连接
            tmpData.S2C_Connect.uid = guid;   //连接成功
            session.name = tmpData.C2S_Connect.name;

            session.UpdateCheckTime();

            session.Send(tmpData.S2C_Connect, false);    //发送数据 线程不安全  后面有时间 哭放s_mouble 里面处理  这样线程安全
            netMsgMgr.BroadCastEventByMainThreadt(GameEventType.ServerEvent_ReConnect, session);    //修改游戏里面的数据数据
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
        KcpSession AddSessions(uint guid, KcpSocket.KcpAgent kcp)
        {
            KcpSession session = new KcpSession();
            lock (m_Sessions)   //保证线程安全
                m_Sessions.Add(guid, session);

            session.kcpAgent = kcp;
            session.playerID = guid;
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


        //暂时先用uint表示guid
        //后面可以写个回收 回收KcpSession
        uint allSessionGuid = 100000;   //uid以 100000 开始
        uint GenSessionUniqueID() => allSessionGuid++;


        public class KcpSession //: ISession
        {
            public bool close = false;
            LStream m_SendStream = new LStream(NetConst.Socket_SendBufferSize);
            //byte[] m_SendBuffer = new byte[NetConst.Socket_SendBufferSize]; //发送最大大小
            public uint playerID;
            public KcpSocket.KcpAgent kcpAgent;
            public string name;
            public KcpServer server;
            public long checkTime;     //时间
            public float checkCount;    //心跳

            public LStream GetStream() => m_SendStream;

            public void SendNotRecycle<T>(T data) where T : S2C_BASE<T>, new()
            {
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
                if (sLog.OpenMsgInfo)
                    sLog.Debug(">>>>> Send msgType : {0}", data.msgType);

                data.Serialize(m_SendStream);
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
                //sLog.Debug(">>>> checkTime : {0}  checkCount： {1}", checkTime, checkCount);
            }


            public void ServerCloseSession()
            {
                server.CloseSessions(playerID);
            }

            internal void Close()
            {
                if (close) return;

                close = true;
                //server.CloseSessions(playerID); //注释掉了 不然无线循环了
                m_SendStream = null;
                server = null;
                kcpAgent.Close();
            }


          
        }




        public override void Dispose()
        {
            m_uuidMap.Clear();
            m_uuidMap = null;
            base.Dispose();
        }

    }

    

}


