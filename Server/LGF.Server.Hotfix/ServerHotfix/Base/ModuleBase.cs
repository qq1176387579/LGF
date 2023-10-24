/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/12 23:31:26
/// 功能描述:  对应模块协议的处理
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;
using LGF.Serializable;

namespace LGF.Server.Hotfix
{
    public class ModuleBase
    {
        protected ModuleMgr moduleMgr;
        protected HotfixMoudleMgr hotfixMgr;
        protected KcpServer Server;
        NetMsgHandlingMgr netMsgHandlingMgr;

        protected PlayerManager playerMgr => hotfixMgr.playerMgr;
        protected RoomManager roomMgr => hotfixMgr.roomMgr;

        protected TimeManager timerMgr => moduleMgr.timerMgr;
        protected DataMaganer dataMgr => moduleMgr.dataMgr;

        /// <summary>
        /// 注意 泛型回调完成的时候 会自动回收
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="action"></param>
        public void RegisterServerMsg<T>(System.Action<KcpServer.KcpSession, T> action,bool isAutoRecycleData = true) where T : C2S_BASE<T>, new()
        {
            T t = new T();
            netMsgHandlingMgr.RegisterServerMsg<T>(t.msgType, action, isAutoRecycleData);
        }


        public void Init(HotfixMoudleMgr mgr)
        {
            hotfixMgr = mgr;
            netMsgHandlingMgr = NetMsgHandlingMgr.Instance;
            moduleMgr = hotfixMgr.moduleMgr;
            Server = moduleMgr.Server;

            OnInit();
        }



        protected virtual void OnInit()
        {

        }


        protected Player GetPlayer(KcpServer.KcpSession session)
        {
            return playerMgr.GetPlayerByID(session.useid);
        }

        protected Player GetPlayer(uint id)
        {
            return playerMgr.GetPlayerByID(id);
        }


        protected T GetModule<T>() where T : ModuleBase
        {
            return hotfixMgr.GetModuleEX<T>();
        }


        #region send

        public void Send<T>(Dictionary<uint, Player> players, T data, bool IsRecycle = true) where T : S2C_BASE<T>, new()
        {
            moduleMgr.Send(players, data, IsRecycle);
        }


        public void Send<T>(List<Player> players, T data, bool IsRecycle = true) where T : S2C_BASE<T>, new()
        {
            moduleMgr.Send(players, data, IsRecycle);
        }


        public void Send<T>(Player player, T data, bool IsRecycle = true) where T : S2C_BASE<T>, new()
        {
            moduleMgr.Send(player, data, IsRecycle);
        }



        public void SendNotRecycle<T>(Dictionary<uint, Player> players, T data) where T : S2C_BASE<T>, new()
        {
            moduleMgr.SendNotRecycle(players, data);
        }

        //Do not recycle
        public void SendNotRecycle<T>(List<Player> players, T data) where T : S2C_BASE<T>, new()
        {
            moduleMgr.SendNotRecycle(players, data);
        }


        public void SendNotRecycle<T>(Player players, T data) where T : S2C_BASE<T>, new()
        {
            moduleMgr.SendNotRecycle(players, data);
        }

        #endregion


        public virtual void Close()
        {
            //用于注销一些事件 如 EventManager.Instance.RemoveListerner
            //因为是单线程 所以调用没有问题 EventManager.Instance.BroadCastEvent 也是在主线程调用的  这样不会缺少事件
        }
    }

}

