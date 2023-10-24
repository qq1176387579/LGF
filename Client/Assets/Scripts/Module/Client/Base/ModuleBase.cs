/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/13 0:21:00
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;

/// <summary>
/// 模块不做存储数据  处理一些逻辑 与监听消息
/// 客户端可以不用这块的。  我尝试下多
/// </summary>
public class ModuleBase
{
    protected ModuleMgr moduleMgr;
    protected KcpClient Client;
    protected MainPlayerManager mainPlayer;
    protected MainPlayerManager player => mainPlayer;
    protected RoomManager roomMgr;
    NetMsgHandlingMgr netMsgHandlingMgr;


    /// <summary>
    /// 回调完成后会自动回收 数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="action"></param>
    protected void RegisterClientMsg<T>(System.Action<T> action) where T : S2C_BASE<T>, new()
    {
        T tmp = new T();    //临时生成
        netMsgHandlingMgr.RegisterClientMsg(tmp.msgType, action);
    }

    public void Init(ModuleMgr _ModuleMgr)
    {
        moduleMgr               = _ModuleMgr;
        netMsgHandlingMgr       = NetMsgHandlingMgr.Instance;
        Client                  = moduleMgr.Client;


        mainPlayer              = moduleMgr.mainPlayerMgr;
        roomMgr                 = moduleMgr.roomMgr;

        OnInit();
    }



    protected virtual void OnInit()
    {
    }


    public void SendNotRecycle<T>(T data) where T : C2S_BASE<T>, new()
    {
        moduleMgr.SendNotRecycle(data);
    }


    public void Send<T>(T data, bool IsRecycle = true) where T : C2S_BASE<T>, new()
    {
        moduleMgr.Send(data, IsRecycle);
    }
}
