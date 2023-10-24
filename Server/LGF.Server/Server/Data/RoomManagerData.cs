///***************************************************
///// 作者:      liuhuan
///// 创建日期:  2022/12/12 0:35:49
///// 功能描述:  
//****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

namespace LGF.Server
{
    public class RoomManagerData : ManagerDataBase
    {
        public uint generateUID;

        public Dictionary<uint, Room> roomList;
        public Dictionary<uint, Room> playingRoomList;

        public override void Init()
        {

            roomList = new Dictionary<uint, Room>();
            playingRoomList = new Dictionary<uint, Room>();   //游戏中的房间
            generateUID = 0;
        }
    }
}

