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
    /// Kcp 服务器
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
        /// </summary>
        /// <param name="port"></param>
        public void Bing(int port = 0, uint interval = 10)  //间隔时间
        {
            KcpServerRecvHelper recvHelper = new KcpServerRecvHelper();
            base.Bing(recvHelper, port, interval);
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

            if (m_uuidMap.ContainsKey(tmpData.C2S_Connect.uuid))
            {
                //表示已经存在了 需要去处理下 比如断网重连  我这里不做处理 
                
                sLog.Error(" uuid 已经存在 当前不处理 同一个guid 登录的情况");
            }
            else
            {
                uint guid = KcpSession.GenUniqueID();
                m_uuidMap.Add(tmpData.C2S_Connect.uuid, guid);
                var session = AddSessions(guid, kcp);
                tmpData.S2C_Connect.uid = guid;   //连接成功
                sLog.Debug("OnConnect  tmpData.S2C_Connect msgType {0}" , tmpData.S2C_Connect.msgType);
                session.Send(tmpData.S2C_Connect, false);    //发送数据
            }
        }


        public KcpSession GetSessions(uint id,bool islock = true)
        {
            KcpSession session = null;
            if (islock)
            {
                lock (m_Sessions)   //保证线程安全
                    m_Sessions.TryGetValue(id, out session);
            }
            else
            {
                m_Sessions.TryGetValue(id, out session);
            }
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

            return session;
        }



        public class KcpSession //: ISession
        {
            LStream m_SendStream = new LStream(NetConst.Socket_SendBufferSize);
            //byte[] m_SendBuffer = new byte[NetConst.Socket_SendBufferSize]; //发送最大大小
            public uint playerID;
            public KcpSocket.KcpAgent kcpAgent;


            /// <summary>
            /// 发送数据 线程不安全  注意使用的场合  后面看情况加不加锁
            /// 注意是否在同一个线程中
            /// </summary>
            /// <typeparam name="T"></typeparam>
            public void Send<T>(T data, bool IsRecycle = true) where T : ISerializer
            {
                data.Serialize(m_SendStream);
                kcpAgent.Send(m_SendStream.GetBuffer(), m_SendStream.Lenght);
                if (IsRecycle)
                    data.Release();
            }


            //暂时先用uint表示guid
            //后面可以写个回收 回收KcpSession
            static uint allGuid = 1;
            public static uint GenUniqueID() => allGuid++;

        }

    }



}


