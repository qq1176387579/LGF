/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/11 2:00:25
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;

public class S_RoomModule : S_ModuleBase
{
    S2C_CreateRoom tmp_CreateRoom = new S2C_CreateRoom();
    S2C_InformRoomChange tmp_informRoomChange = new S2C_InformRoomChange();
    S2C_GetAllTheRooms tmp_GetAllTheRooms = new S2C_GetAllTheRooms();
    
    protected override void OnInit()
    {
        base.OnInit();

        RegisterServerMsg<C2S_CreateRoom>(NetMsgDefine.C2S_CreateRoom, OnCreateRoom);
        RegisterServerMsg<C2S_InformRoomChange>(NetMsgDefine.C2S_InformRoomChange, OnRoomChange);
        RegisterServerMsg<C2S_GetAllTheRooms>(NetMsgDefine.C2S_GetAllTheRooms, OnGetAllTheRooms);
    }


    /// <summary>
    /// 获得全部房间信息
    /// </summary>
    /// <param name="session"></param>
    /// <param name="c2S_CreateRoom"></param>
    void OnGetAllTheRooms(KcpServer.KcpSession session, C2S_GetAllTheRooms c2S_CreateRoom)
    {
        var rooms = roomMgr.GetAllRoom();
        var info = tmp_GetAllTheRooms;
        if (info.roomList == null)
            info.roomList = new List<CMD_SimpleRoomInfo>();

        int count = 0;
        foreach (var item in rooms)
        {
            if (info.roomList.Count <= count) //2 <= 2
            {
                info.roomList.Add(CMD_SimpleRoomInfo.Get());
            }
            CMD_SimpleRoomInfo roomInfo = info.roomList[count];
            roomInfo.roomID = item.Value.id;
            roomInfo.roomName = item.Value.roomName;
            count++;
        }

        for (int i = info.roomList.Count -1 ; i >= count; i--)
        {
            info.roomList[i].Release();
            info.roomList.RemoveAt(i);
        }

        session.SendNotRecycle(info);
    }
    





    void OnCreateRoom(KcpServer.KcpSession session, C2S_CreateRoom c2S_CreateRoom)
    {
        var player = GetPlayer(session);

        if (player.InRoom)
        {
            tmp_CreateRoom.ErrorCode = ErrCode.Fail;
            session.SendNotRecycle(tmp_CreateRoom);
            return;
        }

        S_Room room = roomMgr.CreateRoom(player);
        tmp_CreateRoom.roomID = room.id;
        tmp_CreateRoom.roomName = room.roomName;

        tmp_CreateRoom.ErrorCode = ErrCode.Succeed;
        session.SendNotRecycle(tmp_CreateRoom);
    }





    void OnRoomChange(KcpServer.KcpSession session, C2S_InformRoomChange msg)
    {
        var player = GetPlayer(session);

        if (player == null)
        {
            sLog.Debug(" session.playerID  si {0}", session.playerID);
            return;
        }
        int optType = msg.opt;
        var info = tmp_informRoomChange;
        info.playerID = player.uid;
        info.opt = optType;
        info.ErrorCode = ErrCode.Succeed;
        switch (optType)
        {
            case 1: //请求加入房间
                {
                    var room = roomMgr.GetRoom(msg.roomID);
                    if (room == null)
                    {
                        info.ErrorCode = ErrCode.RoomNotExist;  //房间不存在
                        break;
                    }
                    room.JoinPlayer(player);
                    room.SyncRoomInfoToPlayer(player);  //
                    return;
                }
            case 2: //2表示离开房间
                {
                    player.room.LeaveRoom(player);  //离开房间
                    break;
                }
            case 3: //3表示玩家准备
                {
                    if (player.roomReally)
                    {
                        sLog.Debug("---------player.roomReally{0}", player.roomReally);
                        info.ErrorCode = ErrCode.INVALID_OPT;   //非法操作
                        break;
                    }

                    player.roomReally = true;
                    SendNotRecycle(player.room.GetAllPlayer(), info);
                    return;
                }
            case 4: // 4表示玩家取消准备
                {
                    if (!player.roomReally)
                    {
                        sLog.Debug("--------player.roomReally{0}-" , player.roomReally);
                        info.ErrorCode = ErrCode.INVALID_OPT;   //非法操作
                        break;
                    }

                    player.roomReally = false;
                    SendNotRecycle(player.room.GetAllPlayer(), info);
                    return;
                }

            default:
                {
                    sLog.Debug("玩家非法操作 {0} ：ID: {1}  opt{2}", player.name, player.uid, optType);
                    //非法操作
                    info.ErrorCode = ErrCode.Fail;

                    break;
                }
        }

        session.SendNotRecycle(info);
    }

}



