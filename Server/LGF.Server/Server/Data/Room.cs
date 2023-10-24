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

namespace LGF.Server
{
    public class Room : Poolable<Room>
    {
        public uint id;                     //房间id
        public string roomName;             //房间名字
        public Player houseOwner;         //房主
        public Dictionary<uint, Player> playerList = new Dictionary<uint, Player>();    //房间玩家
        public int count => playerList.Count;
        public uint curRoomjoinRank;   //当前房间加入排名
        public int reallyCount;
        public int loadFinishCount;

        public ulong curFrame;  //当前帧
        public RoomStateEnum curState;  //当前状态
                                        //public 
                                        //public int 

        public S2C_RoomProgress tmpRoomProgress;
        public S2C_FrameOpKey tmpFrameOpKey;


        protected override void OnGet()
        {
            base.OnGet();
            tmpRoomProgress = S2C_RoomProgress.Get();
            tmpRoomProgress.list = new List<C2S_RoomProgress>();

            tmpFrameOpKey = S2C_FrameOpKey.Get();
            tmpFrameOpKey.allOpkey = new List<C2S_FrameOpKey>();

            loadFinishCount = 0;
            reallyCount = 0;
        }



        protected override void OnRelease()
        {
            base.OnRelease();
            playerList.Clear();
            houseOwner = null;
            tmpRoomProgress.Release();
            tmpRoomProgress = null;

            tmpFrameOpKey.Release();
            tmpFrameOpKey = null;

        }


    }

}
