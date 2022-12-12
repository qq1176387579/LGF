/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/12 23:31:26
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;
using LGF.Serializable;

public class S_ModuleBase
{
    protected S_ModuleMgr moduleMgr;
    protected KcpServer Server;
    NetMsgHandlingMgr netMsgHandlingMgr;

    protected S_PlayerManager playerMgr => moduleMgr.playerMgr;
    protected S_RoomManager roomMgr => moduleMgr.roomMgr; 


    /// <summary>
    /// 注意 泛型回调完成的时候 会自动回收
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="action"></param>
    public void RegisterServerMsg<T>(NetMsgDefine type, System.Action<KcpServer.KcpSession, T> action) where T : C2S_BASE<T>, new()
    {
        netMsgHandlingMgr.RegisterServerMsg<T>(type, action);
    }


    public void Init(S_ModuleMgr mgr)
    {
        netMsgHandlingMgr   = NetMsgHandlingMgr.Instance; ;
        moduleMgr = mgr;
        Server              = moduleMgr.Server;

        OnInit();
    }



    protected virtual void OnInit()
    {

    }


    protected S_Player GetPlayer(KcpServer.KcpSession session)
    {
        return playerMgr.GetPlayerByID(session.playerID);
    }

    protected S_Player GetPlayer(uint id)
    {
        return playerMgr.GetPlayerByID(id);
    }


    protected T GetModule<T>() where T : S_ModuleBase
    {
        return moduleMgr.GetModuleEX<T>();
    }


    #region send

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

}
