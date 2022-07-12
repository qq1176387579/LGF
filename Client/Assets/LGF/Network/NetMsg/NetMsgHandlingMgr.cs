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



    /// <summary>
    /// 消息处理管理器 
    /// 需要在先注册完 所有的数据事件
    /// </summary>
    public partial class NetMsgHandlingMgr : SingletonBase<NetMsgHandlingMgr> , INetMsgHandling
    {
        NetMsgMgr netMsgMgr;
        Dictionary<NetMsgDefine, System.Delegate> m_NetEvent = new Dictionary<NetMsgDefine, System.Delegate>();
        protected override void OnNew()
        {
            base.OnNew();
            netMsgMgr = NetMsgMgr.Instance;
        }


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
        /// 注册服务器消息
        /// </summary>
        public void RegisterClientMsg<T>(NetMsgDefine type, System.Action<T> action) where T : S2C_BASE<T>, new()
        {
            if (m_NetEvent.ContainsKey(type))
            {
                Debug.LogWarningFormat("重复注册 请检查一下 {0}", type);
                return;
            }
            m_NetEvent.Add(type, action);
        }


        public void InvokeClientMsg<T>(NetMsgDefine type, LStream _stream) where T : S2C_BASE<T>, new()
        {
            //主线程处理
            T data = S2C_BASE<T>.Get(_stream);

            m_NetEvent.TryGetValue(type, out System.Delegate delega);
            if (delega == null)
            {
                Debug.LogWarningFormat("未注册该事件 !! {0}", type);
                return;
            }

            System.Action<T> action = delega as System.Action<T>;
            if (action == null)
            {
                Debug.LogErrorFormat("事件出错 传输数据不对 {0}", typeof(T));
                return;
            }

            netMsgMgr.QueueOnMainThreadt((_action, _data) =>
            {
                _action.Invoke(data);
            }, action, data);
        }


        #endregion



        #region kcpServerEvent  服务器处理



        /// <summary>
        /// 注册服务器消息
        /// </summary>
        public void RegisterServerMsg<T>(NetMsgDefine type, System.Action<KcpServer.KcpSession,T> action) where T : C2S_BASE<T>, new()
        {
            if (m_NetEvent.ContainsKey(type))
            {
                Debug.LogWarningFormat("重复注册 请检查一下 {0}", type);
                return; 
            }
            m_NetEvent.Add(type, action);
        }


        public void InvokeServerMsg<T>(NetMsgDefine type, KcpServer.KcpSession session, LStream _stream) where T : C2S_BASE<T>, new()
        {
            //主线程处理
            T data = C2S_BASE<T>.Get(_stream);

            m_NetEvent.TryGetValue( type,out System.Delegate delega);
            if (delega == null)
            {
                Debug.LogWarningFormat("未注册该事件 !! {0}", type);
                return;
            }

            System.Action<KcpServer.KcpSession, T> action = delega as System.Action<KcpServer.KcpSession, T>;
            if (action == null)
            {
                Debug.LogErrorFormat("事件出错 传输数据不对 {0}", typeof(T));
                return;
            }

            netMsgMgr.QueueOnMainThreadt((_action, _session, _data) => {
                _action.Invoke(session, data);
            }, action, session,data);
        }



        /// <summary>
        /// 服务器 消息
        /// </summary>
        public void OnServerMsg(KcpServer server, NetMsgDefine type, uint sessionID, LStream _stream)
        {
            var session = server.GetSessions(sessionID);
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

                        Debug.Log("获得服务器信息  " + endPoint.ToString());

                        //主线程执行
                        netMsgMgr.QueueOnMainThreadt((_endPoint) =>
                        {
                            EventManager.Instance.BroadCastEvent(GameEventType.Net_GetServersInfo, _endPoint);
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

