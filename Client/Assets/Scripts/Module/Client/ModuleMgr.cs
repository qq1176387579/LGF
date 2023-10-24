/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/12 22:00:29
/// 功能描述:  
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LGF;
using LGF.Log;
using LGF.Net;

public class ModuleMgr : SingletonBase<ModuleMgr>
{
    public MainPlayerManager mainPlayerMgr { get; private set; }
    public MainPlayerManager Player => mainPlayerMgr;
    public RoomManager roomMgr { get; private set; }

    Dictionary<Type, ModuleBase> m_Module = new Dictionary<Type, ModuleBase>();
    //NetMsgHandlingMgr netMsgHandlingMgr;

    public KcpClient Client { get; private set; }

    //public string serverAddress = "192.168.31.241:51503";
    public string serverAddress = "124.221.91.172:51503";
    public bool ConnectSucceed = false; //连接成功

    public override void Init()
    {
        LGFEntry.RegisterOnUpdate(OnUpdate);
        InitClient();

        InitAllManager();
        InitAllModule();

    }

    void InitAllManager()
    {
        mainPlayerMgr = MainPlayerManager.Instance.Init(this);
        roomMgr = RoomManager.Instance.Init(this);
    }


    void InitAllModule()
    {
        CreationModule<ChatModule>();
        CreationModule<LoginModule>();
        CreationModule<RoomModuble>();
        CreationModule<FrameSyncModule>();
    }

    void CreationModule<T>() where T : ModuleBase, new()
    {
        T tmp = new T();
        m_Module.Add(typeof(T), tmp);
        tmp.Init(this);
    }

    T GetModuleEX<T>() where T : ModuleBase
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
        StartConnectServer();
    }



    public void SendNotRecycle<T>(T data) where T : C2S_BASE<T>, new()
    {
        Client.SendNotRecycle(data);
    }


    public void Send<T>(T data, bool IsRecycle = true) where T : C2S_BASE<T>, new()
    {
        Client.Send(data, IsRecycle);
    }



    public static MainPlayerManager GetPlayer()
    {
        return _Instance.mainPlayerMgr;
    }

    public static KcpClient GetSession()
    {
        return _Instance.Client;
    }


    #region Client connect


    void OnUpdate()
    {
        if (Client.IsDisposed) {
            return;
        }
        if (ConnectSucceed) {

            if (Client.IsDisconnection() && ReConnectState != 1) {
                //Client.TryReConnect();  //尝试重新连接
                ReConnectState = 1;
                StartReConnect();
            }
        }
    }

    /// <summary>
    /// 0 表示空
    /// 1表示连接中
    /// 2 表示连接完成
    /// </summary>
    int ReConnectState = 0;

    void StartReConnect()
    {
        Task.Run(() => {
            while (Client.IsDisposed || Client.IsDisconnection()) {
                Client.TryReConnect();
                Thread.Sleep(5000); //5s一次重连检查一次
            }
            ReConnectState = 2;
        });
    }



    IPEndPoint serverEndPoint = null;

    IPEndPoint GetEndPoint()
    {
        if (serverEndPoint != null) {
            return serverEndPoint;
        }
        string[] path2 = serverAddress.Split(':');
        serverEndPoint = new IPEndPoint(IPAddress.Parse(path2[0]), int.Parse(path2[1]));
        return serverEndPoint;
    }


    /// <summary>
    /// 开始连接服务器
    /// </summary>
    public void StartConnectServer()
    {
        var ss = GetEndPoint();
        this.DebugError("连接服务器 >> " + ss.ToString());
        Client.TryToConnect(ss, OnConnect);
    }


    void OnConnect(bool IsSucceed)
    {
        ConnectSucceed = IsSucceed;
        if (IsSucceed) {
            //连接成功
            this.Debug("连接 服务器成功");
        }
        else {
            //连接失败
            this.DebugError("连接 服务器失败");
            StartConnectServer();
        }

    }



    #endregion


    public void Close()
    {
        this.Debug("关闭中 Close");
        //_Instance = null;
        Client.Dispose();
        this.Debug("关闭Close");
    }

}
