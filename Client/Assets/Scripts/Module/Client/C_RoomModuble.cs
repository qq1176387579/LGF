/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/11 21:54:53
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

public class C_RoomModuble : C_ModuleBase
{
    C2S_CreateRoom tmp_CreateRoom = new C2S_CreateRoom();
    C2S_GetAllTheRooms tmp_GetAllTheRooms = new C2S_GetAllTheRooms();
    C2S_InformRoomChange tmp_InformRoomChange = new C2S_InformRoomChange();




    protected override void OnInit()
    {
        base.OnInit();
        RegisterClientMsg<S2C_CreateRoom>(NetMsgDefine.S2C_CreateRoom, OnCreateRoom);
        RegisterClientMsg<S2C_GetAllTheRooms>(NetMsgDefine.S2C_GetAllTheRooms, OnGetAllTheRooms);
        RegisterClientMsg<S2C_InformRoomChange>(NetMsgDefine.S2C_InformRoomChange, OnInformRoomChange);

        RegisterClientMsg<S2C_SyncRoomInfo>(NetMsgDefine.S2C_SyncRoomInfo, OnSucceededJoining);
        
    }

    public void GetAllTheRoomsInfo()
    {
        SendNotRecycle(tmp_GetAllTheRooms);
    }

    void OnGetAllTheRooms(S2C_GetAllTheRooms msg)
    {
        roomMgr.LoadAllRoom(msg);
        EventManager.Instance.BroadCastEvent(GameEventType.ClientEvent_GetAllRooms, roomMgr.GetAllRoomInfo());
    }

    /// <summary>
    /// 成功加入 同步房间信息
    /// </summary>
    /// <param name="msg"></param>
    void OnSucceededJoining(S2C_SyncRoomInfo msg)
    {
        roomMgr.LoadRoomInfo(msg);
        EventManager.Instance.BroadCastEvent(GameEventType.ClientEvent_JionRoom);
    }

    public void CreateRoom()
    {
        SendNotRecycle(tmp_CreateRoom);
    }


    public void JionRoom(uint roomID)
    {
        var info = tmp_InformRoomChange;
        info.uid = player.uid;
        info.opt = 1;
        info.roomID = roomID;
        Send(info);
    }

    //public void bool


    void OnCreateRoom(S2C_CreateRoom msg)
    {
        if (msg.ErrorCode != ErrCode.Succeed)
        {
            sLog.Error("非法操作" + msg.ErrorCode);
            return;
        }

        roomMgr.OnCreateRoom(msg);

        EventManager.Instance.BroadCastEvent(GameEventType.ClientEvent_CreateRoomSucceed);  //成功创建
    }


    bool requestReadying;

    public void SetReady(bool isReady)
    {
        if (requestReadying)
            return;

        requestReadying = true;

        var info = tmp_InformRoomChange;
        info.uid = player.uid;
        //3当前未准备 想要准备 和 4当前准备
        info.opt = isReady ? 3 : 4;   
        SendNotRecycle(info);
    }




    /// <summary>
    /// 房间的状态改变 通知
    /// </summary>
    /// <param name="msg"></param>
    void OnInformRoomChange(S2C_InformRoomChange msg)
    {
        int optType = msg.opt;
        if (optType == 3 || optType == 4)
        {
            if (msg.playerID == player.uid)
            {
                requestReadying = false;
            }
        }

        if (msg.ErrorCode != ErrCode.Succeed)
        {
            sLog.Error(" 请求失败 错误信息 : {0}", msg.ErrorCode.ToString());
            return;
        }

        switch (optType)   
        {
            case 1:  //玩家加入新玩家加入
                roomMgr.JoinRoom(msg);
                break;
            case 2: //玩家加入新玩家加入离开
                roomMgr.LeaveRoom(msg.playerID);
                break;
            case 3: //玩家准备
                roomMgr.ChangeReadyState(msg.playerID, true);
                break;
            case 4: //4表示玩家取消准备
                roomMgr.ChangeReadyState(msg.playerID, false);
                break;
            case 5: // 5表示更换房主
                roomMgr.ChangeHouseOwner(msg.playerID);
                break;
            default: 
                break;
        }
        EventManager.Instance.BroadCastEvent(GameEventType.ClientEvent_RoomOptSync, msg.playerID, optType);  //有玩家加入房间

    }


}
