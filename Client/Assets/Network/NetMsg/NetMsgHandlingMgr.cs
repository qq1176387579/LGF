/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/4 23:27:00
/// 功能描述:  消息处理管理器
****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Net;
using LGF;
using LGF.Log;
using LGF.Serializable;
using LGF.DataStruct;
using UnityEngine;

namespace LGF.Net
{
    public abstract class NetMsgHandlingMgrBase<T> : SingletonBase<T>  where T : NetMsgHandlingMgrBase<T>, new ()
    {

        protected virtual void InvokeServerMsgEx(NetMsgDefine type, KcpServer.KcpSession session, LStream _stream)
        {
            //switch (type)
            //{
            //    case NetMsgDefine.C2S_ChatMsg: InvokeServerMsg<C2S_ChatMsg>(type, session, _stream); break;
            //    default:
            //        //sLog.Error(<DoubleQuotationMarks> Server 未注册该事件 或者 流程出错 请检查!! < DoubleQuotationMarks > +type);
            //        sLog.Error("Server 未注册该事件 或者 流程出错 请检查!!   " + type);
            //        break;
            //}

            sLog.Error("请先实现化注册该方法 或者 使用unity菜单栏 tools/SteamSerializable/Generated  生成");
        }


        protected virtual void InvokeClientMsgEx(NetMsgDefine type, LStream _stream)
        {

            //switch (type)
            //{
            //    case NetMsgDefine.S2C_Connect:  //连接成功
            //        InvokeClientMsg<S2C_Connect>(type, _stream);    //这里封装下
            //        break;
            //    case NetMsgDefine.S2C_ChatMsg: InvokeClientMsg<S2C_ChatMsg>(type, _stream); break;
            //    default:
            //        sLog.Error("Client 未注册该事件 或者 流程出错 请检查!!   " + type);
            //        //sLog.Error(<DoubleQuotationMarks> Client 未注册该事件 或者 流程出错 请检查!! < DoubleQuotationMarks > +type);
            //        break;
            //}

            sLog.Error("请先实现化注册该方法 或者 使用unity菜单栏 tools/SteamSerializable/Generated  生成");
        }

    }



    /// <summary>
    /// 消息处理管理器 
    /// 需要在先注册完 所有的数据事件
    /// </summary>
    public partial class NetMsgHandlingMgr : NetMsgHandlingMgrBase<NetMsgHandlingMgr> , INetMsgHandling
    {
        class NetMsgDelegateInfo
        {
            public System.Delegate evt;
            public bool autoRecycle;
        }

        NetMsgMgr netMsgMgr;
        Dictionary<NetMsgDefine, NetMsgDelegateInfo> m_NetEvent = new Dictionary<NetMsgDefine, NetMsgDelegateInfo>();
        //Dictionary<NetMsgDefine, bool> m_AutoRecycleData = new Dictionary<NetMsgDefine, bool>();    //是否自动回收
        protected override void OnNew()
        {
            base.OnNew();
            netMsgMgr = NetMsgMgr.Instance;
        }


        public NetMsgMgr GetNetMsgMgr() => netMsgMgr;

        //System.Action<NetMsgDefine, KcpServer.KcpSession, LStream> InvokeServerMsgEvent;
        //可以写成这种方法 进行注册


        #region 客户端的处理


        /// <summary>
        /// 
        /// </summary>
        /// <param name="client">我觉得用不上</param>
        /// <param name="type"></param>
        /// <param name="_stream"></param>
        public void OnClientMsg(KcpClient client, NetMsgDefine type, LStream _stream)
        {
            InvokeClientMsgEx(type, _stream);
        }

        /// <summary>
        /// 注册客户端消息
        /// 回调完成后会自动回收 数据
        /// </summary>
        public void RegisterClientMsg<T>(NetMsgDefine type, System.Action<T> action) where T : S2C_BASE<T>, new()
        {
            if (m_NetEvent.ContainsKey(type))
            {
                sLog.Warning("客户端 重复注册 请检查一下 {0}", type);
                return;
            }
            m_NetEvent.Add(type, new NetMsgDelegateInfo() { evt = action, autoRecycle = true });
        }


        public void InvokeClientMsg<T>(NetMsgDefine type, LStream _stream) where T : S2C_BASE<T>, new()
        {

            m_NetEvent.TryGetValue(type, out var delega);
            if (delega == null)
            {
                sLog.Warning("未注册该事件 !! {0}", type);
                return;
            }

            System.Action<T> action = delega.evt as System.Action<T>;
            if (action == null)
            {
                sLog.Error("事件出错 传输数据不对 {0}", typeof(T));
                return;
            }

            //主线程处理  客户端放这里反序列化 能减少判定
            //如果_stream 没有序列化 也不影响 后续接收的数据  接收数据是以_stream.GetBuffer() 来接收的
            T data = S2C_BASE<T>.Get(_stream);

            netMsgMgr.QueueOnMainThreadt((_action, _data, _autoRecycle) =>
            {
                if (sLog.OpenMsgInfo)
                {
                    if (_data.msgType != NetMsgDefine.S2C_HeartBeat)
                        sLog.Debug(">>>> Accept MsgType:{0}", _data.msgType);
                }
                 

                if (_data.ErrorCode != ErrCode.Succeed)
                {
                    sLog.Debug("------错误码---" + _data.ErrorCode); //正常应该弹一个tps
                }

                _action.Invoke(_data);

                if (_autoRecycle)   //自动回收
                    _data.Release();

            }, action, data, delega.autoRecycle);

        }


        #endregion



        #region kcpServerEvent  服务器处理



        /// <summary>
        /// 注册服务器消息
        /// 注意 泛型回调完成的时候 会自动回收
        /// 请自己做好处理
        /// </summary>
        public void RegisterServerMsg<T>(NetMsgDefine type, System.Action<KcpServer.KcpSession,T> action, bool isAutoRecycleData = true) where T : C2S_BASE<T>, new()
        {
            if (m_NetEvent.TryGetValue(type,out var info))
            {
                //如果是热更新的话 
                sLog.Warning("服务器 重复注册 请检查一下 {0}  热更新加载请忽略该提示", type);
                //服务端热更注册  
                //后面加个判断 热更修复
                info.autoRecycle = isAutoRecycleData;
                info.evt = action;
                return; 
            }
            m_NetEvent.Add(type, new NetMsgDelegateInfo() { evt = action, autoRecycle = isAutoRecycleData });
        
        }


        public void InvokeServerMsg<T>(NetMsgDefine type, KcpServer.KcpSession session, LStream _stream) where T : C2S_BASE<T>, new()
        {
         
            T data = C2S_BASE<T>.Get(_stream);

            //因为存在热更新的原因 服务器的放到QueueOnMainThreadt里面执行 防止调用热更新前的程序
            //所有逻辑跑主线程 所有NetEvent 赋值与更改都在线程执行。  保证下面的程序调用正确
            netMsgMgr.QueueOnMainThreadt((_NetEvent, _session, _data) =>
            {
                _NetEvent.TryGetValue(type, out var delega);
                if (delega == null)
                {
                    sLog.Warning("未注册该事件 !! {0}", type);
                    _data.Release();
                    return;
                }

                System.Action<KcpServer.KcpSession, T> action = delega.evt as System.Action<KcpServer.KcpSession, T>;
                if (action == null)
                {
                    sLog.Error("事件出错 传输数据不对 {0}", typeof(T));
                    _data.Release();
                    return;
                }

                if (sLog.OpenMsgInfo)
                    sLog.Debug(">>>> Accept MsgType:{0}", _data.msgType);

                action.Invoke(_session, _data);
                if (delega.autoRecycle)
                    _data.Release();    //自动回收
             
            }, m_NetEvent, session, data);


        }



        /// <summary>
        /// 服务器 消息
        /// </summary>
        public void OnServerMsg(KcpServer server, NetMsgDefine type, uint sessionID, LStream _stream)
        {
            var session = server.GetSessions(sessionID);
            //这里可以优化 原来的没办法判断  是否登录  并且kcp还在代理还在
            if (session == null)
            {
                sLog.Error("非法请求  请先登录");   
                return;
            }
            session.UpdateCheckTime();
            if (type == NetMsgDefine.C2S_HeartBeat) return; //心跳不要需要处理


            InvokeServerMsgEx(type, session, _stream);
        }




        #endregion




        #region cs udp 处理

        /// <summary>
        /// 消息处理cs处理  接收数据
        /// c2s 服务器处理
        /// s2c 客户端处理
        /// 非kcp流程  udp处理
        /// </summary>
        public void OnCSNetMsg(KcpCSBase csBase, in EndPoint point, LStream stream)
        {
            NetMsgDefine type = stream.GetNetMsgType();//
            //消息处理
            switch (type)
            {

                case NetMsgDefine.N_C2S_GetAllServersInfo:
                    {
                        this.Debug(StringPool.Concat(point.ToString(), $"  请求当前服务器信息 cur port: {csBase.tmpData.n_S2C_GetAllServersInfo.port} ")); 
                        csBase.SendTo(csBase.tmpData.n_S2C_GetAllServersInfo, point);
                    }
                    break;
                case NetMsgDefine.N_S2C_GetAllServersInfo:
                    {
                        csBase.tmpData.n_S2C_GetAllServersInfo.Deserialize(stream);
                        int port = csBase.tmpData.n_S2C_GetAllServersInfo.port;
                        EndPoint endPoint = new IPEndPoint((point as IPEndPoint).Address, port);

                        sLog.Debug("获得服务器信息  " + endPoint.ToString());

                        //主线程执行
                        netMsgMgr.QueueOnMainThreadt((_endPoint) =>
                        {
                            EventManager.Instance.BroadCastEvent(GameEventType.ServerEvent_GetServersInfo, _endPoint);
                        }, in endPoint);
                    }
                    break;
                default:
                    this.Debug("type {0} 非法消息" , type);
                    stream.Clear(); ;
                    break;
            }
        }

      
        #endregion
    }





}

