/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/12 0:35:49
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;


namespace LGF.Server.Hotfix
{
    public class RoomManager : HotfixSingletonBase<RoomManager, RoomManagerData>
    {

        uint generateUID => _data.generateUID;

        Dictionary<uint, Room> roomList => _data.roomList;
        Dictionary<uint,Room> playingRoomList => _data.playingRoomList;   //没用上 没写功能先弃用

        public override void Init()
        {
            base.Init();
        }


        public uint GenerateUID()
        {
            do
            {
                _data.generateUID++;
            } while (roomList.ContainsKey(generateUID));
            return generateUID;
        }

        public Room CreateRoom(Player player, string name = null)
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

            Room room = Room.Get();
            room.Init(GenerateUID(), name, player);
            roomList.Add(room.id, room);
            return room;
        }

        public void DelRoom(Room room)
        {
            sLog.Debug("删除房间 {0}", room.id);
            roomList.Remove(room.id);   //删除房间
            room.Release();
            HotfixEventMgr.Instance.BroadCastEvent(HotfixEventType.DelRoom, room.id);
        }

        public Dictionary<uint, Room> GetAllRoom()
        {
            return roomList;
        }

        public Dictionary<uint, Room> GetPlayingRoom()
        {
            //游戏中房间
            return roomList; 
        }


        public Room GetRoom(uint uid)
        {
            roomList.TryGetValue(uid, out Room room);
            return room;
        }



    }

}
