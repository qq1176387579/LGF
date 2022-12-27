///***************************************************
///// 作者:      liuhuan
///// 创建日期:  2022/12/12 0:35:49
///// 功能描述:  
//****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

public class S_RoomManagerData : S_ManagerDataBase
{
    public uint generateUID;

    public Dictionary<uint, S_Room> roomList;
    public Dictionary<uint, S_Room> playingRoomList;

    public override void Init()
    {
        
        roomList = new Dictionary<uint, S_Room>();
        playingRoomList = new Dictionary<uint, S_Room>();   //游戏中的房间
        generateUID = 0;
    }
}
