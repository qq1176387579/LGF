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

/// <summary>
/// 后面自己写关闭函数
/// </summary>
public class S_ModuleMgr : SingletonBase<S_ModuleMgr>
{
    Dictionary<Type, S_ModuleBase> m_Module = new Dictionary<Type, S_ModuleBase>();

    public KcpServer Server { get; private set; }


    public override void Init()
    {
        CreationModule<S_ChatModule>();

        InitServer();
    }

    void CreationModule<T>() where T : S_ModuleBase, new()
    {
        T tmp = new T();
        m_Module.Add(typeof(T), tmp);
        tmp.Init();
    }

    public T GetModule<T>() where T : S_ModuleBase
    {
        if (m_Module.TryGetValue(typeof(T), out S_ModuleBase module))
        {
            return module as T;
        }
        return null;
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
        Server.Bing(NetConst.ServerPort, interval);    //30间隔
    }


    public void Close()
    {
        //_Instance = null;
    }
}
