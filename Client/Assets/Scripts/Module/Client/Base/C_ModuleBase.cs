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

public class C_ModuleBase 
{
    protected C_ModuleMgr moduleMgr;
    protected KcpClient Client;
    protected C_MainPlayerManager mainPlayer;
    protected C_MainPlayerManager player => mainPlayer;
    protected C_RoomManager roomMgr;
    NetMsgHandlingMgr netMsgHandlingMgr;


    /// <summary>
    /// 回调完成后会自动回收 数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="action"></param>
    protected void RegisterClientMsg<T>(NetMsgDefine type, System.Action<T> action) where T : S2C_BASE<T>, new()
    {
        netMsgHandlingMgr.RegisterClientMsg(type, action);
    }

    public void Init(C_ModuleMgr _c_ModuleMgr)
    {
        moduleMgr               = _c_ModuleMgr;
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
