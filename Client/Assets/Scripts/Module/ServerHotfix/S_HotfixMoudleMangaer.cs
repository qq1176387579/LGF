/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/25 21:36:47
/// 功能描述:  热修复模块 管理
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;
using LGF.Serializable;

namespace LGF.Server.Hotfix
{

    public class S_HotfixMoudleMgr : SingletonBase<S_HotfixMoudleMgr>
    {
        List<IS_HotfixSingletonBase> m_mgrs = new List<IS_HotfixSingletonBase>();

        Dictionary<Type, S_ModuleBase> m_Module = new Dictionary<Type, S_ModuleBase>();
      
        public S_PlayerManager playerMgr;
        public S_RoomManager roomMgr;
        public S_ModuleMgr moduleMgr;

        public override void Init()
        {
            moduleMgr   = S_ModuleMgr.Instance;
            //sLog.Debug("S_HotfixMoudleMgr init moduleMgr is {0}", moduleMgr != null);
            InitMgr();
            CreationAllModule();

            //sLog.Debug("---------uid ->{0}", GetModule<S_RoomModule>().t);
        }


        void InitMgr()
        {
            playerMgr = S_PlayerManager.Instance.Init(this);
            roomMgr = S_RoomManager.Instance.Init(this);
        }


        void CreationAllModule()
        {
            CreationModule<S_ChatModule>();
            CreationModule<S_RoomModule>();
            CreationModule<S_LoginModule>();
            CreationModule<S_FrameSyncMoudule>();
            
        }



        public void AddManager(IS_HotfixSingletonBase mgr)
        {
            m_mgrs.Add(mgr);
        }

        void CreationModule<T>() where T : S_ModuleBase, new()
        {
            T tmp = new T();
            m_Module.Add(typeof(T), tmp);
            tmp.Init(this);
        }


        public T GetModuleEX<T>() where T : S_ModuleBase
        {
            if (m_Module.TryGetValue(typeof(T), out S_ModuleBase module))
            {
                return module as T;
            }
            sLog.Warning("没有 {0} 模块", typeof(T).Name);
            return null;
        }

        public static T GetModule<T>() where T : S_ModuleBase
        {
            return Instance.GetModuleEX<T>();
        }


        #region Send


        public void Send<T>(Dictionary<uint, S_Player> players, T data, bool IsRecycle = true) where T : S2C_BASE<T>, new()
        {
            moduleMgr.Send(players, data, IsRecycle);
        }


        public void Send<T>(List<S_Player> players, T data, bool IsRecycle = true) where T : S2C_BASE<T>, new()
        {
            moduleMgr.Send(players, data, IsRecycle);
        }


        public void Send<T>(S_Player player, T data, bool IsRecycle = true) where T : S2C_BASE<T>, new()
        {
            moduleMgr.Send(player, data, IsRecycle);
        }



        public void SendNotRecycle<T>(Dictionary<uint, S_Player> players, T data) where T : S2C_BASE<T>, new()
        {
            moduleMgr.SendNotRecycle(players, data);
        }

        //Do not recycle
        public void SendNotRecycle<T>(List<S_Player> players, T data) where T : S2C_BASE<T>, new()
        {
            moduleMgr.SendNotRecycle(players, data);
        }


        public void SendNotRecycle<T>(S_Player players, T data) where T : S2C_BASE<T>, new()
        {
            moduleMgr.SendNotRecycle(players, data);
        }

        #endregion


        public void Close()
        {
            foreach (var item in m_Module)
            {
                item.Value.Close();
            }

            foreach (var item in m_mgrs)
            {
                item.Close();
            }

            SingletonClear();
        }

    }





}
