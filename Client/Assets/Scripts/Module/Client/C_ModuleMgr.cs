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
    Dictionary<Type, C_ModuleBase> m_Module = new Dictionary<Type, C_ModuleBase>();

    public KcpClient Client { get; private set; }


    public override void Init()
    {
        CreationModule<C_ChatModule>();

        InitClient();
    }

    void CreationModule<T>() where T : C_ModuleBase, new()
    {
        T tmp = new T();
        m_Module.Add(typeof(T), tmp);
        tmp.Init();
    }

    public T GetModule<T>() where T : C_ModuleBase
    {
        if (m_Module.TryGetValue(typeof(T), out C_ModuleBase module))
        {
            return module as T;
        }
        sLog.Warning("没有 {0} 模块", typeof(T).Name);
        return null;
    }


    /// <summary>
    /// 帧同步 服务器帧率为30帧
    /// </summary>
    /// <param name="interval"></param>
    public void InitClient(uint interval = 15)
    {
        if (Client != null)
        {
            sLog.Error("已经初始服务器了 请检查当前服务器");
            return;
        }
        Client = new KcpClient();
        Client.Bing(NetConst.RandomPort, interval);    //30间隔
    }


    public void Close()
    {
        //_Instance = null;
        Client.Dispose();
    }

}
