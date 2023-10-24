/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/12 22:01:51
/// 功能描述:  
****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;
using LGF.Serializable;
using static LGF.Net.KcpServer;

namespace LGF.Server
{
    /// <summary>
    /// 后面自己写关闭函数
    /// </summary>
    public class ModuleMgr : SingletonBase<ModuleMgr>
    {
        Dictionary<Type, IServerSingletonBase> m_mgrs = new Dictionary<Type, IServerSingletonBase>();

        //List<IServerSingletonBase> m_mgrs = new List<IServerSingletonBase>(); //管理器
        public ulong curTime => timerMgr.curTime;
        public DataMaganer dataMgr { get; protected set; }
        public TimeManager timerMgr { get; protected set; }

        public KcpServer Server { get; private set; }


        public override void Init()
        {
            InitServer();       //初始化socket

            InitAllManager();       //初始化管理器

            InitAllModule();       //初始化模块
        }

        void InitAllManager()
        {
            sLog.Debug("------InitAllManager--");

            timerMgr = TimeManager.Instance.Init(this);
            dataMgr = DataMaganer.Instance.Init(this);

            FrameSyncManager.Instance.Init(this);
        }

        //AddManager<T>(out T mgr) 发现索引器无法添加
        public void AddManager(Type type, IServerSingletonBase mgr)
        {
            m_mgrs.Add(type, mgr);
        }


        public T GetMgr<T>() where T : class, IServerSingletonBase
        {
            m_mgrs.TryGetValue(typeof(T), out var mgr);
            return mgr as T;
        }


        void InitAllModule()
        {
#if NOT_UNITY
            LGF.Server.HotfixHelper.LoadHotfixAssembly();
#else
        LGF.Server.Hotfix.S_HotfixEntry.Init();
#endif
        }



        public void InitServer()
        {
            if (Server != null) {
                sLog.Error("已经初始服务器了 请检查当前服务器");
                return;
            }
            Server = new KcpServer();

#if NOT_UNITY
            Server.Bing(-1, NetConst.ServerPort);
#else
        Server.Bing(NetConst.ServerPort, -1);    
#endif
        }


        #region Send


        public void Send<T>(Dictionary<uint, Player> players, T data, bool IsRecycle = true) where T : S2C_BASE<T>, new()
        {
            if (players == null) {
                return;
            }
            LStream stream = null;

            if (sLog.OpenMsgInfo)
                sLog.Debug(">>>>>Broadcast Send msgType : {0}", data.msgType);

            foreach (var item in players) {
                KcpSession session = item.Value.session;
                if (stream == null) {
                    stream = session.GetStream();
                    data.Serialize(stream);
                }
                session.Send(stream);
            }

            if (IsRecycle)
                data.Release();
        }


        public void Send<T>(List<Player> players, T data, bool IsRecycle = true) where T : S2C_BASE<T>, new()
        {
            if (players == null || players.Count == 0) return;

            LStream stream = null;
            if (sLog.OpenMsgInfo)
                sLog.Debug(">>>>>Broadcast Send msgType : {0}", data.msgType);

            foreach (var item in players) {
                KcpSession session = item.session;
                if (stream == null) {
                    stream = session.GetStream();
                    data.Serialize(stream);
                }

                session.Send(stream);
            }

            if (IsRecycle)
                data.Release();
        }


        public void Send<T>(Player player, T data, bool IsRecycle = true) where T : S2C_BASE<T>, new()
        {
            if (sLog.OpenMsgInfo)
                sLog.Debug(">>>>> Send msgType : {0}", data.msgType);

            KcpSession session = player.session;
            var stream = session.GetStream();
            data.Serialize(stream);

            session.Send(stream);
            if (IsRecycle)
                data.Release();
        }


        public void SendNotRecycle<T>(Dictionary<uint, Player> players, T data) where T : S2C_BASE<T>, new()
        {
            Send(players, data, false);
        }

        //Do not recycle
        public void SendNotRecycle<T>(List<Player> players, T data) where T : S2C_BASE<T>, new()
        {
            Send(players, data, false);
        }


        public void SendNotRecycle<T>(Player players, T data) where T : S2C_BASE<T>, new()
        {
            Send(players, data, false);
        }


        #endregion


        public void Close()
        {
            //_Instance = null;
            Server.Dispose();

            //客户端
            foreach (var item in m_mgrs) {
                item.Value.Close();
            };
        }



    }


}


