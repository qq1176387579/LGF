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

            if (m_uuidMap.TryGetValue(tmpData.C2S_Connect.uuid,out uint guid))
            {
                //表示已经存在了 需要去处理下 比如断网重连  我这里不做处理 
                
                sLog.Warning(" uuid 已经存在 当前暂时先退出原来的"); //表示重新登录  那么顶替之前的

                ReConnect(guid, kcp);    //退出重进的情况
                
            }
            else
            {
                guid = GenSessionUniqueID();
                m_uuidMap.Add(tmpData.C2S_Connect.uuid, guid);
                var session = AddSessions(guid, kcp);
                tmpData.S2C_Connect.uid = guid;   //连接成功
                session.name = tmpData.C2S_Connect.name;

                sLog.Debug("OnConnect  tmpData.S2C_Connect msgType {0} name: {1}" , tmpData.S2C_Connect.msgType, session.name);
                //这里其实线程不安全 但是登录模块 session 是新的 所以直接用
                session.Send(tmpData.S2C_Connect, false);    //发送数据
                
                //通知玩家登录了
                netMsgMgr.BroadCastEventByMainThreadt(GameEventType.ServerEvent_PlayerConnect, session);
              
            }
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
            session.kcpAgent.Close();   //
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

            session.Send(tmpData.S2C_Connect, false);    //发送数据 线程不安全  后面有时间 哭放s_mouble 里面处理  这样线程安全
            netMsgMgr.BroadCastEventByMainThreadt(GameEventType.ServerEvent_ReConnect, session);    //修改游戏里面的数据数据
        }



        /// <summary>
        /// 广播给所有玩家
        /// </summary>
        public void Broadcast<T>(T data, bool IsRecycle = true) where T : S2C_BASE<T>, new()
        {
            if (m_Sessions.Count == 0)
            {
                return;
            }

            if (sLog.OpenMsgInfo)
                sLog.Debug(">>>>>Broadcast Send msgType : {0}", data.msgType);

            LStream stream = null;

            foreach (var item in m_Sessions)
            {
                KcpSession session = item.Value;
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
        uint allSessionGuid = 1;
        uint GenSessionUniqueID() => allSessionGuid++;


        public class KcpSession //: ISession
        {
            LStream m_SendStream = new LStream(NetConst.Socket_SendBufferSize);
            //byte[] m_SendBuffer = new byte[NetConst.Socket_SendBufferSize]; //发送最大大小
            public uint playerID;
            public KcpSocket.KcpAgent kcpAgent;
            public string name;
            public KcpServer server;

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

            public void Close()
            {
                server.CloseSessions(playerID); //关闭
                m_SendStream = null;
                server = null;
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


