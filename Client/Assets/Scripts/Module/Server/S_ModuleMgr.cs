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

/// <summary>
/// 后面自己写关闭函数
/// </summary>
public class S_ModuleMgr : SingletonBase<S_ModuleMgr>
{
    Dictionary<Type, S_ModuleBase> m_Module = new Dictionary<Type, S_ModuleBase>();

    public S_PlayerManager playerMgr { get; protected set; }
    public S_RoomManager roomMgr { get; protected set; }


    public KcpServer Server { get; private set; }


    public override void Init()
    {
        InitServer();       //初始化socket

        InitAllManager();       //初始化管理器

        InitAllModule();       //初始化模块
    }

    void InitAllManager()
    {
        sLog.Debug("------InitSystem--");
        playerMgr = S_PlayerManager.Instance.Init(this);
        roomMgr = S_RoomManager.Instance.Init(this);
    }

    


    void InitAllModule()
    {
        CreationModule<S_ChatModule>();
        CreationModule<S_RoomModule>();
        CreationModule<S_LoginModule>();
        
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



    /// <summary>
    /// 帧同步 服务器帧率为30帧
    /// </summary>
    /// <param name="interval"></param>
    public void InitServer(uint interval = 30)
    {
        if (Server != null)
        {
            sLog.Error("已经初始服务器了 请检查当前服务器");
            return;
        }
        Server = new KcpServer();

#if NOT_UNITY
        Server.Bing(-1, NetConst.ServerPort, interval);    //30间隔
#else
        Server.Bing(NetConst.ServerPort, -1, interval);    //30间隔
#endif
    }


    #region Send


    public void Send<T>(Dictionary<uint,S_Player> players, T data, bool IsRecycle = true) where T : S2C_BASE<T>, new()
    {
        if (players == null)
        {
            return;
        }
        LStream stream = null;

        foreach (var item in players)
        {
            KcpSession session = item.Value.session;
            if (stream == null)
            {
                stream = session.GetStream();
                data.Serialize(stream);
            }
            session.Send(stream);
        }

        if (IsRecycle)
            data.Release();
    }


    public void Send<T>(List<S_Player> players, T data, bool IsRecycle = true) where T : S2C_BASE<T>, new()
    {
        if (players == null || players.Count == 0) return;

        LStream stream = null;

        foreach (var item in players)
        {
            KcpSession session = item.session;
            if (stream == null)
            {
                stream = session.GetStream();
                data.Serialize(stream);
            }

            session.Send(stream);
        }

        if (IsRecycle)
            data.Release();
    }


    public void Send<T>(S_Player player, T data, bool IsRecycle = true) where T : S2C_BASE<T>, new()
    {
        KcpSession session = player.session;
        var stream = session.GetStream();
        data.Serialize(stream);

        session.Send(stream);
        if (IsRecycle)
            data.Release();
    }


    public void SendNotRecycle<T>(Dictionary<uint, S_Player> players, T data) where T : S2C_BASE<T>, new()
    {
        Send(players, data, false);
    }

    //Do not recycle
    public void SendNotRecycle<T>(List<S_Player> players, T data) where T : S2C_BASE<T>, new()
    {
        Send(players, data, false);
    }


    public void SendNotRecycle<T>(S_Player players, T data) where T : S2C_BASE<T>, new()
    {
        Send(players, data, false);
    }


    #endregion


    public void Close()
    {
        //_Instance = null;
        Server.Dispose();
    }


   
}
