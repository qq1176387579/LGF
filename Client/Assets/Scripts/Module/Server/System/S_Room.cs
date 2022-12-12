/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/12 0:43:04
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LGF;

using LGF.Log;


public class S_Room : Poolable<S_Room>
{
    public uint id;                     //房间id
    public string roomName;             //房间名字
    public S_Player houseOwner;         //房主
    public Dictionary<uint, S_Player> playerList = new Dictionary<uint, S_Player>();    //房间玩家
    public int count => playerList.Count;
    public uint curRoomjoinRank;   //当前房间加入排名
    S_ModuleMgr moduleMgr;
    public void Init(uint _id, string _name ,S_Player _houseOwner)
    {
       
        playerList.Clear();

        id = _id;
        roomName = _name != null ? _name : StringPool.Concat(_houseOwner.name, "的房间");

      
        houseOwner = _houseOwner;

        curRoomjoinRank = 0;
        moduleMgr = S_ModuleMgr.Instance;


        JoinPlayer(_houseOwner);

    }


    /// <summary>
    /// 加入玩家
    /// </summary>
    public void JoinPlayer(S_Player player)
    {
    
        player.room = this;
        player.roomid = id;
        player.roomjoinRank = ++curRoomjoinRank;
        player.roomReally = false;


        //通知
        var info = S2C_InformRoomChange.Get();  
        info.opt = 1;   //玩家加入
        info.playerID = player.uid;
        info.newUser = CopyUserRoomInfo(player);
        moduleMgr.Send(playerList, info);

        playerList.Add(player.uid, player); //添加
 
    }

    /// <summary>
    /// 同步 房间信息 给玩家
    /// </summary>
    public void SyncRoomInfoToPlayer(S_Player player)
    {
        var info = S2C_SyncRoomInfo.Get();  //
        if (info.infoList == null)
            info.infoList = new List<CMD_UserRoomInfo>();

        info.houseOwnerID = houseOwner.uid;
        info.roomName = roomName;

        foreach (var item in playerList)
        {
            var userRoominfo = CopyUserRoomInfo(item.Value);
            info.infoList.Add(userRoominfo);
        }

        player.session.Send(info);
    }

    CMD_UserRoomInfo CopyUserRoomInfo(S_Player player)
    {
        var userRoominfo            = CMD_UserRoomInfo.Get();
        userRoominfo.useinfo        = CMD_UserInfo.Get();
        userRoominfo.useinfo.uid    = player.uid;
        userRoominfo.useinfo.name   = player.name;
        userRoominfo.ready          = player.roomReally;
        userRoominfo.roomjoinRank   = player.roomjoinRank;
        return userRoominfo;
    }


    /// <summary>
    /// 玩家准备
    /// </summary>
    /// <param name="player"></param>
    public void PlayerPrepared(S_Player player)
    {

    }



    public void LeaveRoom(S_Player player)
    {
        playerList.Remove(player.uid);
        player.room = null;
        player.roomid = 0;
        player.roomjoinRank = 0;

        if (count == 0)
        {
            moduleMgr.roomMgr.DelRoom(this);   //删除房间
        }
        else
        {
            if (player == houseOwner && player != null) 
            {
                ChangeOwner(GetMinRoomjoinRank());      //更换房主
            }

            var info = S2C_InformRoomChange.Get();
            info.opt = 2;   //玩家离开
            info.playerID = player.uid;
            moduleMgr.Send(playerList, info);    //跟换房主
        }

    }

    public S_Player GetMinRoomjoinRank()
    {
        uint tmp = uint.MaxValue;
        S_Player player = null;
        foreach (var item in playerList)
        {
            if (tmp > item.Value.roomjoinRank)
            {
                player = item.Value;
                tmp = item.Value.roomjoinRank;
            }
        }

        return player;
    }

    public void ChangeOwner(S_Player player)
    {
        houseOwner = player;

        var info = S2C_InformRoomChange.Get();
        info.opt = 5;   //更换房主
        info.playerID = player.uid;
        moduleMgr.Send(playerList, info);    //跟换房主
    }




    public Dictionary<uint, S_Player> GetAllPlayer()
    {
        return playerList;
    }





    protected override void OnRelease()
    {
        base.OnRelease();
        playerList.Clear();
        houseOwner = null;
    }


    


}
