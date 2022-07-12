/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/1 23:29:06
/// 功能描述:  kcp客户端
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
using LGF.Util;
using LGF.Serializable;
using UnityEngine;


namespace LGF.Net
{


    public class KcpClient : KcpCSBase
    {
        public uint guid => tmpData.S2C_Connect.uid;
        LStream m_SendStream = new LStream(NetConst.Socket_SendBufferSize);
        private KcpSocket.KcpAgent m_ServerKcpAgent;    //服务器的kcp代理
        KcpClientRecvHelper recvHelper;
        public void Bing(int port = 0, uint interval = 10)
        {
            recvHelper = new KcpClientRecvHelper();
            base.Bing(recvHelper, port, interval);
            if (m_disposed) return; //启动失败

            this.Debug("客户端已经开启");
            m_ConnectSock.SetBroadcast();
            recvHelper.kcpClient = this;
        }

        bool isGetAllServerInfo = false;    //后面可以用二进制标签 如果处理的多的话

        /// <summary>
        /// 获得全部服务器 外部调用
        /// </summary>
        public void GetAllServerInfo()
        {
            isGetAllServerInfo = true;
        }

        void GetAllServerInfoEx()
        {
            if (!isGetAllServerInfo) return;
            isGetAllServerInfo = false;
            sLog.Debug(">>>> GetAllServerInfo");

            EndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, NetConst.ServerPort);
            m_UdpStream.Clear();
            tmpData.n_C2S_GetAllServersInfo.Serialize(m_UdpStream);
            SendTo(m_UdpStream, m_UdpStream.Lenght, endPoint);
        }

        protected override void OnListener()
        {
            GetAllServerInfoEx();
        }




        /// <summary>
        /// 尝试连接服务器
        /// </summary>
        public void TryToConnect(string localName,in EndPoint endPoint)
        {
            lock (recvHelper)   //防止重复请求
            {
                if (m_ServerKcpAgent != null)
                {
                    sLog.Error("非法操作 你有正在连接的服务器!!!");
                    return;
                }

                if (MacAddressUtils.Instance.HasUUID())
                {
                    //PC端不做限制 后面测试时候可以测试
                    //安卓端检查断网情况
#if NOT_UNITY || UNITY_ANDROID
                    tmpData.C2S_Connect.uuid = MacAddressUtils.Instance.GetUUID() + Common.Random(100000);
#else
                    tmpData.C2S_Connect.uuid = MacAddressUtils.Instance.GetUUID();
#endif

                }
                else
                {
                    sLog.Error("未初始化 MacAddressUtils UUID");
                    return;
                }

                m_ServerKcpAgent = m_kcpSocket.GetKcpAgent(endPoint);
            }

            tmpData.C2S_Connect.name = localName;
            Send2(tmpData.C2S_Connect, false);
        }



        public class KcpClientRecvHelper : RecvHelper
        {
            public KcpClient kcpClient;

            protected override void OnRecv(KcpSocket.KcpAgent kcp, int count)
            {
                //base.OnRecv(kcp, count);
                if (count < 8)
                {
                    sLog.Error("接收到一个未知的信息 count: " + count);
                    return;
                }

                var NetMsgtype = this.stream.GetNetMsgType();
                if (NetMsgtype == NetMsgDefine.S2C_Connect)
                {
                    kcpClient.tmpData.S2C_Connect.Deserialize(stream);
                }

                kcpClient.NetMsgHandling.OnClientMsg(kcpClient, NetMsgtype, stream);


            }

        }




        /// <summary>
        /// 发送数据 线程不安全  注意使用的场合  后面看情况加不加锁
        /// 需要检查是否是同一个线程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Send<T>(T data, bool IsRecycle = true) where T : C2S_BASE<T>, new()
        {
            data.uid = guid;
            data.Serialize(m_SendStream);
            m_ServerKcpAgent.Send(m_SendStream.GetBuffer(), m_SendStream.Lenght);
            if (IsRecycle)
                data.Release();
        }





        /// <summary>
        /// 发送数据 线程不安全  注意使用的场合  后面看情况加不加锁
        /// 需要检查是否是同一个线程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Send2<T>(T data, bool IsRecycle = true) where T : ISerializer
        {
            data.Serialize(m_SendStream);
            m_ServerKcpAgent.Send(m_SendStream.GetBuffer(), m_SendStream.Lenght);
            if (IsRecycle)
                data.Release();
        }


    }



}

