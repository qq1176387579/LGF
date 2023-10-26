/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/13 0:27:44
/// 功能描述:  房间管理器  管理数据
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

public class RoomManager : ModuleSingletonBase<RoomManager>
{
    List<CMD_SimpleRoomInfo> roomInfos; //临时的数据 后面回收  不要拿去用。

    Dictionary<uint, CMD_UserRoomInfo> userDic;
    List<CMD_UserRoomInfo> userList;
    bool hasNewRoomUserData;
    public RoomStateEnum curState { get; protected set; }

    public uint houseOwnerID;
    public string roomName;


    public uint GetHouseOwnerID()
    {
        return houseOwnerID;
    }

    public override void Init()
    {
        base.Init();
        hasNewRoomUserData = true;
        roomInfos = new List<CMD_SimpleRoomInfo>();
        userDic = new Dictionary<uint, CMD_UserRoomInfo>();
        userList = new List<CMD_UserRoomInfo>();
        curState = RoomStateEnum.Create;
    }


    public void LoadRoomInfo(S2C_SyncRoomInfo msg)
    {
        if (userDic.Count > 0)
            userDic.ClearReleaseMember();
        foreach (var item in msg.infoList)
        {
            userDic.Add(item.useinfo.uid, item);
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
        if (!userDic.TryGetValue(playerID, out var info)) {
            sLog.Error("非法操作  没有该玩家------ playerID :" + playerID);
            return;
        }

        info.ready = ready;
    }


    public void JoinRoom(S2C_InformRoomChange msg)
    {
        hasNewRoomUserData = true;
        userDic.Add(msg.newUser.useinfo.uid, msg.newUser);    //加入
        msg.newUser = null;
        curState = RoomStateEnum.Create;
    }

    public void LeaveRoom(uint playerID)
    {
        hasNewRoomUserData = true;
        if (!userDic.TryGetValue(playerID, out var info))
        {
            sLog.Error("非法操作  没有该玩家------");
            return;
        }
        userDic.Remove(playerID);   //玩家离开

        info.Release();
        curState = RoomStateEnum.Create;
    }

    public void OnCreateRoom(S2C_CreateRoom msg)
    {
        hasNewRoomUserData = true;
        houseOwnerID = player.uid;
        //this.DebugError("player.uid" + player.uid);
        roomName = msg.roomName;
        var info = CMD_UserRoomInfo.Get();
        info.roomjoinRank = 1;
        info.ready = false;
        info.useinfo = player.CopyUserInfo();
        player.RoomID = msg.roomID;

        //sLog.Debug(" player.RoomID : {0}", player.RoomID);
        userDic.ClearReleaseMember();
        userDic.Add(player.uid, info);
        curState = RoomStateEnum.Create;
    }



    public Dictionary<uint, CMD_UserRoomInfo> GetRoomUsersInfo()
    {
        return userDic;
    }

    public CMD_UserRoomInfo GetUserInfo(uint id)
    {
        userDic.TryGetValue(id, out var player);
        return player;
    }

    /// <summary>
    /// 帧同步流程的时候需要使用 list  不能使用dic
    /// </summary>
    /// <returns></returns>
    public List<CMD_UserRoomInfo> GetAllUserInfoByList()
    {
        if (hasNewRoomUserData) {
            hasNewRoomUserData = false;
            userList.Clear();
            foreach (var item in userDic) {
                userList.Add(item.Value);
            }
            //排序
            userList.Sort((a, b) => {
                return a.useinfo.uid.CompareTo(b.useinfo.uid);
            });
        }
        return userList;
    }



    public Dictionary<uint, CMD_UserRoomInfo> GetAllUserInfo()
    {
        return userDic;
    }


    public void ChangeState(RoomStateEnum type)
    {
        curState = type;
    }

}
