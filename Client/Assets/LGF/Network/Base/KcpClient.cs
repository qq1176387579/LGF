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
using LGF.Serializable;
using UnityEngine;


namespace LGF.Net
{
    
    public class KcpClient : KcpCSBase
    {

        public override void Bing(KcpOnRecv OnRecv, int port = 0, uint interval = 10)
        {
            base.Bing(OnRecv, port, interval);
            if (m_disposed) return; //启动失败

            this.Debug("客户端已经开启");
            m_ConnectSock.SetBroadcast();
        }


        /// <summary>
        /// 尝试连接服务器  获得服务器信息
        /// </summary>
        public void TryToConnect()
        {
            C2S_Connect.Get().NSerialize(m_Stream).Release();
            EndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, NetConst.ServerPort);
            lock (m_ConnectSock)
                m_ConnectSock.SendTo(m_Stream.GetBuffer(), m_Stream.Lenght, SocketFlags.None, endPoint);
        }


    }



}

