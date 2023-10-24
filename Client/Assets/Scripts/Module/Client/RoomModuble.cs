/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/11 21:54:53
/// 功能描述:  房间模块  用于监听net 
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

public class RoomModuble : ModuleBase
{
    C2S_CreateRoom          tmp_CreateRoom          = new C2S_CreateRoom();
    C2S_GetAllTheRooms      tmp_GetAllTheRooms      = new C2S_GetAllTheRooms();
    C2S_InformRoomChange    tmp_InformRoomChange    = new C2S_InformRoomChange();
    C2S_RoomProgress        tmpRoomProgress         = new C2S_RoomProgress();



    protected override void OnInit()
    {
        base.OnInit();
        RegisterClientMsg<S2C_CreateRoom>(OnCreateRoom);
        RegisterClientMsg<S2C_GetAllTheRooms>(OnGetAllTheRooms);
        RegisterClientMsg<S2C_InformRoomChange>(OnInformRoomChange);

        RegisterClientMsg<S2C_SyncRoomInfo>(OnSucceededJoining);

        RegisterClientMsg<S2C_RoomtFinishType>(OnRoomtFinishType);
        RegisterClientMsg<S2C_RoomProgress>(OnRoomProgress);

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
        //info.uid = player.uid;
        info.opt = 1;
        info.roomID = roomID;
        player.RoomID = roomID; //加入房间
        SendNotRecycle(info);
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
        {
            sLog.Debug("---requestReadying---请求中-");
            return;
        }

        if (roomMgr.curState != RoomStateEnum.Create)
        {
            sLog.Debug("--房间状态在游戏中 转移-");
            return;
        }
            

        requestReadying = true;

        var info = tmp_InformRoomChange;
       // info.uid = player.uid;
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
            sLog.Error(" 请求失败 错误信息 : {0} optType {1}", msg.ErrorCode.ToString(), optType);
            if (msg.ErrorCode == ErrCode.RoomNotExist && optType == 1)  //如果房间不存在 且是加入新房间
            {
                GetAllTheRoomsInfo();
            }
            return;
        }

        string name = "";

        switch (optType)   
        {
            case 1:  //玩家加入新玩家加入
                roomMgr.JoinRoom(msg);
                name = roomMgr.GetUserInfo(msg.playerID).useinfo.name;
                break;
            case 2: //玩家加入新玩家加入离开
                name = roomMgr.GetUserInfo(msg.playerID).useinfo.name;
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
        EventManager.Instance.BroadCastEvent<uint, int, string>(GameEventType.ClientEvent_RoomOptSync, msg.playerID, optType, name);  //有玩家加入房间

    }



    public void OnRoomtFinishType(S2C_RoomtFinishType msg)
    {
        sLog.Debug("--OnRoomtFinishType-"+ msg.type);
        if (!player.InRoom)
        {
            sLog.Error("非法 请求操作");
            return;
        }



        switch (msg.type)
        {
            case 1:
                if ( roomMgr.curState != RoomStateEnum.Create)
                {
                    sLog.Error("非法 请求操作");
                    return;
                }
                StartLoadingScene();
                break;
            case 2:
                if (roomMgr.curState != RoomStateEnum.Loading)
                {
                    sLog.Error("非法 请求操作");
                    return;
                }
                StartPlayingGame();
                break;
            default:
                break;
        }


        //开始加载
    }


    void StartLoadingScene()
    {
        sLog.Debug("--StartLoadingScene-");

        roomMgr.ChangeState(RoomStateEnum.Loading);

        SceneManager.Instance.AsyncLoadScene("map_102", (prg) =>
        {
            //EventManager.Instance.BroadCastEvent(GameEventType.);

            tmpRoomProgress.progress = (int)(prg * 100);
            if (tmpRoomProgress.progress >= 100)
            {
                tmpRoomProgress.progress = 100;
            }
            SendNotRecycle(tmpRoomProgress);

        }, () =>
        {
            tmpRoomProgress.progress = -1;
            SendNotRecycle(tmpRoomProgress);
        });

        roomMgr.ChangeState(RoomStateEnum.Loading);
        EventManager.Instance.BroadCastEvent(GameEventType.ClientEvent_StartLoadingScene);  //开始加载场景
    }


    void StartPlayingGame()
    {
        sLog.Debug("--StartPlayingGame-2");
        roomMgr.ChangeState(RoomStateEnum.Playing);
        EventManager.Instance.BroadCastEvent(GameEventType.ClientEvent_StartPlay); //用于显示
    }

    public void OnRoomProgress(S2C_RoomProgress msg)
    {
        sLog.Debug("--OnRoomProgress-");
        EventManager.Instance.BroadCastEvent(GameEventType.ClientEvent_RoomProgress, msg); //用于显示

    }





}
