/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/13 0:27:44
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

public class C_RoomManager : C_SingletonBase<C_RoomManager>
{
    List<CMD_SimpleRoomInfo> roomInfos; //临时的数据 后面回收  不要拿去用。

    Dictionary<uint, CMD_UserRoomInfo> curRoomUserlist;

    public uint houseOwnerID;
    public string roomName;


    public uint GetHouseOwnerID()
    {
        return houseOwnerID;
    }

    public override void Init()
    {
        base.Init();
        roomInfos = new List<CMD_SimpleRoomInfo>();
        curRoomUserlist = new Dictionary<uint, CMD_UserRoomInfo>();
    }


    public void LoadRoomInfo(S2C_SyncRoomInfo msg)
    {
        if (curRoomUserlist.Count > 0)
            curRoomUserlist.ClearReleaseMember();
        foreach (var item in msg.infoList)
        {
            curRoomUserlist.Add(item.useinfo.uid, item);
        }

        houseOwnerID = msg.houseOwnerID;
        roomName     = msg.roomName;            //房间名字

        msg.infoList.Clear();
    }

    public void LoadAllRoom(S2C_GetAllTheRooms msg)
    {
        LGF.Util.Common.Swap(ref msg.roomList, ref roomInfos);  //交换数据

    }

    public List<CMD_SimpleRoomInfo> GetAllRoomInfo()
    {
        return roomInfos;
    }

    public void ChangeHouseOwner(uint _houseOwnerID)
    {
        houseOwnerID = _houseOwnerID;
    }

    public void ChangeReadyState(uint playerID, bool ready)
    {
        if (!curRoomUserlist.TryGetValue(playerID, out var info))
        {
            sLog.Error("非法操作  没有该玩家------");
            return;
        }

        info.ready = ready;
    }


    public void JoinRoom(S2C_InformRoomChange msg)
    {
        curRoomUserlist.Add(msg.newUser.useinfo.uid, msg.newUser);    //加入
        msg.newUser = null;
    }

    public void LeaveRoom(uint playerID)
    {
        if (!curRoomUserlist.TryGetValue(playerID, out var info))
        {
            sLog.Error("非法操作  没有该玩家------");
            return;
        }
        curRoomUserlist.Remove(playerID);   //玩家离开

        info.Release();
    }

    public void OnCreateRoom(S2C_CreateRoom msg)
    {
        player.RoomID = msg.roomID;
        houseOwnerID = player.uid;

        roomName = msg.roomName;
        var info = CMD_UserRoomInfo.Get();
        info.roomjoinRank = 1;
        info.ready = false;
        info.useinfo = player.CopyUserInfo();

        curRoomUserlist.ClearReleaseMember();
        curRoomUserlist.Add(player.uid, info);
    }



    public Dictionary<uint, CMD_UserRoomInfo> GetRoomUsersInfo()
    {
        return curRoomUserlist;
    }

}