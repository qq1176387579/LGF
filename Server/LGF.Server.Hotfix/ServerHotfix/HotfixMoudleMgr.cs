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

    public class HotfixMoudleMgr : SingletonBase<HotfixMoudleMgr>
    {
        List<IHotfixSingletonBase> m_mgrs = new List<IHotfixSingletonBase>();

        Dictionary<Type, ModuleBase> m_Module = new Dictionary<Type, ModuleBase>();
      
        public PlayerManager playerMgr;
        public RoomManager roomMgr;
        public ModuleMgr moduleMgr;

        public override void Init()
        {
            moduleMgr   = ModuleMgr.Instance;
            //sLog.Debug("S_HotfixMoudleMgr init moduleMgr is {0}", moduleMgr != null);
            InitMgr();
            CreationAllModule();

            //sLog.Debug("---------uid ->{0}", GetModule<S_RoomModule>().t);
        }


        void InitMgr()
        {
            playerMgr = PlayerManager.Instance.Init(this);
            roomMgr = RoomManager.Instance.Init(this);
        }


        void CreationAllModule()
        {
            CreationModule<ChatModule>();
            CreationModule<RoomModule>();
            CreationModule<LoginModule>();
            CreationModule<FrameSyncMoudule>();
            
        }



        public void AddManager(IHotfixSingletonBase mgr)
        {
            m_mgrs.Add(mgr);
        }

        void CreationModule<T>() where T : ModuleBase, new()
        {
            T tmp = new T();
            m_Module.Add(typeof(T), tmp);
            tmp.Init(this);
        }


        public T GetModuleEX<T>() where T : ModuleBase
        {
            if (m_Module.TryGetValue(typeof(T), out ModuleBase module))
            {
                return module as T;
            }
            sLog.Warning("没有 {0} 模块", typeof(T).Name);
            return null;
        }

        public static T GetModule<T>() where T : ModuleBase
        {
            return Instance.GetModuleEX<T>();
        }


        #region Send


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
