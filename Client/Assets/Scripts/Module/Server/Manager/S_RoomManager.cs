/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/12 0:35:49
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

public class S_RoomManager: S_SingletonBase<S_RoomManager>
{
    uint generateUID;

    Dictionary<uint, S_Room> roomList;

    public override void Init()
    {
        base.Init();
        roomList = new Dictionary<uint, S_Room>();
        generateUID = 0;
    }



    public uint GenerateUID()
    {
        do
        {
            generateUID++;
        } while (roomList.ContainsKey(generateUID));
        return generateUID;
    }

    public S_Room CreateRoom(S_Player player,string name = null)
    {
        if (player == null)
        {
            sLog.Error("非法操作 请检查一下 player == null");
            return null;
        }

        if (player.InRoom)
        {
            sLog.Error("非法操作 请检查一下");
            return null;
        }

        S_Room room = S_Room.Get();
        room.Init(GenerateUID(), name, player);
        roomList.Add(room.id, room);    
        return room; 
    }

    public void DelRoom(S_Room room)
    {
        roomList.Remove(room.id);   //删除房间
        room.Release();
    }

    public Dictionary<uint, S_Room> GetAllRoom()
    {
        return roomList;
    }

    public S_Room GetRoom(uint uid)
    {
        roomList.TryGetValue(uid, out S_Room room);
        return room;
    }



}
