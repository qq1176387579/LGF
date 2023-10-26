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

    /// <summary>
    /// kcp客户端 
    /// 后续有时间重构  当前结构 需要tmpData.C2S_Connect.uuid 等外部结构  
    /// 好结构不应该依赖太多这种外部结构
    /// 这里绑定了一些业务层代码 无法换个项目通用后续考虑优化结构
    /// </summary>
    public class KcpClient : KcpCSBase
    {
        //public uint guid => tmpData.S2C_Connect.uid;
        LStream m_SendStream = new LStream(NetConst.Socket_SendBufferSize);
        private KcpSocket.KcpAgent m_ServerKcpAgent;    //服务器的kcp代理
        KcpClientRecvHelper recvHelper;
        public bool IsTryConnecting;  //连接中
        int connectCount;
        public long checkTime;     //时间 用来检测心跳



        public void Bing(int port = 0, uint interval = 10)
        {
            recvHelper = new KcpClientRecvHelper();
            base.Bing(recvHelper, port, -1, interval);
            if (m_disposed) return; //启动失败

            this.Debug("客户端已经开启");
            //m_ConnectSock.SetBroadcast();
            recvHelper.kcpClient = this;
        }

        bool isGetAllServerInfo = false;    //后面可以用二进制标签 如果处理的多的话

        #region 弃用
        ///// <summary>
        ///// 获得全部服务器 外部调用
        ///// </summary>
        //public void GetAllServerInfo()
        //{
        //    isGetAllServerInfo = true;
        //}

        //void GetAllServerInfoEx()
        //{
        //    if (!isGetAllServerInfo) return;
        //    isGetAllServerInfo = false;
        //    sLog.Debug(">>>> GetAllServerInfo");

        //    EndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, NetConst.ServerPort);
        //    sLog.Debug(">> GetAllServerInfo endPoint : " + endPoint.ToString());
        //    m_UdpStream.Clear();
        //    tmpData.n_C2S_GetAllServersInfo.Serialize(m_UdpStream);
        //    SendTo(m_UdpStream, m_UdpStream.Lenght, endPoint);
        //}

        //protected override void OnListener()
        //{
        //    GetAllServerInfoEx();
        //}
        #endregion


        System.Action<bool> OnConnectCallback;


        /// <summary>
        /// 尝试连接服务器
        /// 后面需要写 失败的情况
        /// </summary>
        public void TryToConnect(EndPoint endPoint, System.Action<bool> _OnConnectCallback)   
        {
            OnConnectCallback = _OnConnectCallback;
            lock (recvHelper)   //防止重复请求
            {
                if (m_ServerKcpAgent != null){
                    sLog.Error("非法操作 你有正在连接的服务器!!!");
                    return;
                }
               
            }


            IsTryConnecting = true;
            connectCount = 0;
            Task.Run(() =>
            {
                while (IsTryConnecting)
                {
                    connectCount++;
                    m_kcpSocket.ClientTryConnectServer(endPoint);

                    Thread.Sleep(3000);
                    if (connectCount >= 3) {
                        IsTryConnecting = false;

                        ConnectStateCallback(false);
                        sLog.Error("连接失败");
                        ////暂时先这样
                        //this.Delay(() => {
                        //    EventManager.Instance.BroadCastEvent(GameEventType.ConnectServerFail);
                        //});
                        m_ServerKcpAgent.Close();
                        m_ServerKcpAgent = null;
                        break;
                    }
                }
            });
        }



        public class KcpClientRecvHelper : RecvHelper
        {
            public KcpClient kcpClient;

            protected override void OnRecv(KcpSocket.KcpAgent kcp, int count)
            {
                //base.OnRecv(kcp, count);
                if (count < 4)
                {
                    sLog.Error("接收到一个未知的信息 count: " + count);
                    return;
                }
                if (kcpClient.m_ServerKcpAgent != kcp)  //不是同一个服务器
                {
                    sLog.Error("不是同一个服务器 请检查 ");
                    //kcp.Dispose(); 需要写关闭  我这里没实现 现在这样   以后重构
                    return; 
                }

                var NetMsgtype = this.stream.GetNetMsgType();
                kcpClient.checkTime = DateTime.UtcNow.AddMilliseconds(NetConst.Client_CheckHeartBeatPackTime).Ticks; //跟新时间
                //sLog.Debug("NetMsgtype----" + NetMsgtype);
                if (NetMsgtype == NetMsgDefine.HeartBeat)
                {
                    kcpClient.SendHeartBeat();
                    return;
                }
                //else if (NetMsgtype == NetMsgDefine.Connect)
                //{
                //    //if (kcpClient.guid != 0)
                //    //{
                //    //    sLog.Error("重复登录了  请检查一下");
                //    //}
                //    //kcpClient.tmpData.S2C_Connect.Deserialize(stream);
                //    //kcpClient.IsTryConnecting = false;
                //}


                kcpClient.NetMsgHandling.OnClientMsg(kcpClient, NetMsgtype, stream);
              

            }

            protected override void OnConnectEvent(ConnectEvent evt, KcpSocket.KcpAgent kcp)
            {
                if (evt == ConnectEvent.Client_ConnectDone) {
                    kcpClient.SetToServerKcpAgent(kcp); //连接成功
                    //连接服务器成功  需要实现调用登录回调
                    kcpClient.checkTime = DateTime.UtcNow.AddMilliseconds(NetConst.Client_CheckHeartBeatPackTime).Ticks; //跟新时间
                }
                else if(evt == ConnectEvent.Client_ReConnectDone) {
                    //重登
                    kcpClient.OnReConnectDone();
                }
                else if(evt == ConnectEvent.Client_Close) {
                    this.DebugError($" 关闭连接 需要重新登录");
                    kcpClient.Dispose();    //客户端关闭
                }
                else {
      
                    this.DebugError($" 未知回调 : <{evt}>");
                }
            }

        }

        /// <summary>
        /// 是否断开连接
        /// </summary>
        /// <returns></returns>
        public bool IsDisconnection()
        {
            if (m_ServerKcpAgent == null) {
                return true;
            }
            //this.DebugError($" Millisecond : <{DateTime.UtcNow.Ticks}>  {checkTime + NetConst.Client_CheckHeartBeatPackTime}");
            return DateTime.UtcNow.Ticks >= checkTime;
        }

        /// <summary>
        /// 尝试重新连接
        /// </summary>
        public void TryReConnect()
        {
            if (IsDisposed) {
                return;
            }
            m_kcpSocket.ReBing();
            SendHeartBeat();
        }


        /// <summary>
        /// 重新登录完成
        /// </summary>
        internal void OnReConnectDone()
        {
            //通知外面
            //重登成功
        }

        internal void ConnectStateCallback(bool f)
        {
            netMsgMgr.QueueOnMainThreadt((_this, _f) => {
                //_this.SendNotRecycle(_this.tmpData.C2S_HeartBeat);
                _this.OnConnectCallback(_f);
            }, this, f);
        }

        internal void SetToServerKcpAgent(KcpSocket.KcpAgent kcp)
        {
            IsTryConnecting = false;
            m_ServerKcpAgent = kcp;
            ConnectStateCallback(true);
            //客戶端初始建立连接的时候 需要发送一个心跳来和服务端 创建会话 
            //不然服务器没有会话的话 服务器端也没有KcpAgent进行管理  不是持续发送心跳 
            SendHeartBeat();   
        }

        ////登录
        //internal  void SendConnect()
        //{
        //    sLog.Debug($"{tmpData.C2S_Connect.name} 请求登录 ");
        //    netMsgMgr.QueueOnMainThreadt((_this) => {
        //        _this.Send2<C2S_Connect>(_this.tmpData.C2S_Connect, false);
        //        sLog.Debug($"{tmpData.C2S_Connect.name} 请求登录 发送完成 ");
        //    }, this);   
           
        //}

        /// <summary>
        /// 主线程发 心跳
        /// </summary>
        internal void SendHeartBeat()
        {
            netMsgMgr.QueueOnMainThreadt((_this) =>
            {
                _this.SendNotRecycle(_this.tmpData.C2S_HeartBeat);
            }, this);
        }

        public void SendNotRecycle<T>(T data) where T : C2S_BASE<T>, new()
        {
            //data.uid = guid;
            data.Serialize(m_SendStream);
            if (sLog.OpenMsgInfo)
            {
                if (data.msgType == NetMsgDefine.FrameOpKey || data.msgType == NetMsgDefine.HeartBeat) { }
                else  sLog.Debug(">>>>> Send msgType : {0}", data.msgType);
            }
 
            m_ServerKcpAgent.Send(m_SendStream.GetBuffer(), m_SendStream.Lenght);
        }


        /// <summary>
        /// 发送数据 线程不安全  注意使用的场合  后面看情况加不加锁
        /// 需要检查是否是同一个线程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Send<T>(T data, bool IsRecycle = true) where T : C2S_BASE<T>, new()
        {
            if (sLog.OpenMsgInfo)
                sLog.Debug(">>>>> Send msgType : {0}", data.msgType);
            //data.uid = guid;
            data.Serialize(m_SendStream);
            m_ServerKcpAgent.Send(m_SendStream.GetBuffer(), m_SendStream.Lenght);
            //if (data.uid == 0) {
            //    this.Debug("--data.uid == 0--非法操作--");
            //}
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
            if (sLog.OpenMsgInfo)
                sLog.Debug(">>>>> Send msgType : {0}", typeof(T).Name);

            data.Serialize(m_SendStream);
            m_ServerKcpAgent.Send(m_SendStream.GetBuffer(), m_SendStream.Lenght);
            if (IsRecycle)
                data.Release();
        }


    }



}

