/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/12 22:00:29
/// 功能描述:  
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;

public class C_ModuleMgr : SingletonBase<C_ModuleMgr>
{
    public C_MainPlayerManager mainPlayerMgr { get; private set; }
    public C_MainPlayerManager Player => mainPlayerMgr;
    public C_RoomManager roomMgr { get; private set; }

    Dictionary<Type, C_ModuleBase> m_Module = new Dictionary<Type, C_ModuleBase>();
    //NetMsgHandlingMgr netMsgHandlingMgr;

    public KcpClient Client { get; private set; }


    public override void Init()
    {
        InitClient();

        InitAllManager();
        InitAllModule();

    }

    void InitAllManager()
    {
        mainPlayerMgr = C_MainPlayerManager.Instance.Init(this);
        roomMgr = C_RoomManager.Instance.Init(this);
    }


    void InitAllModule()
    {
        CreationModule<C_ChatModule>();
        CreationModule<C_LoginModule>();
        CreationModule<C_RoomModuble>();
        CreationModule<C_FrameSyncModule>();
    }

    void CreationModule<T>() where T : C_ModuleBase, new()
    {
        T tmp = new T();
        m_Module.Add(typeof(T), tmp);
        tmp.Init(this);
    }

    T GetModuleEX<T>() where T : C_ModuleBase
    {
        if (m_Module.TryGetValue(typeof(T), out C_ModuleBase module))
        {
            return module as T;
        }
        sLog.Warning("没有 {0} 模块", typeof(T).Name);
        return null;
    }

    public static T GetModule<T>() where T : C_ModuleBase
    {
        return _Instance.GetModuleEX<T>();
    }



    public void InitClient()
    {
        if (Client != null)
        {
            sLog.Error("已经初始服务器了 请检查当前服务器");
            return;
        }
        Client = new KcpClient();
        Client.Bing(NetConst.RandomPort); 
    }



    public void SendNotRecycle<T>(T data) where T : C2S_BASE<T>, new()
    {
        Client.SendNotRecycle(data);
    }


    public void Send<T>(T data, bool IsRecycle = true) where T : C2S_BASE<T>, new()
    {
        Client.Send(data, IsRecycle);
    }



    public static C_MainPlayerManager GetPlayer()
    {
        return _Instance.mainPlayerMgr;
    }

    public static KcpClient GetSession()
    {
        return _Instance.Client;
    }



    public void Close()
    {
        //_Instance = null;
        Client.Dispose();
    }

}
