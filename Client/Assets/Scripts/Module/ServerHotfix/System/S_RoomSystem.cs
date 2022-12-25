/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/26 1:53:02
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

namespace LGF.Server.Hotfix
{
    public static class S_RoomSystem
    {

        public static void Init(this S_Room self, uint _id, string _name, S_Player _houseOwner)
        {

            self.playerList.Clear();

            self.id = _id;
            self.roomName = _name != null ? _name : StringPool.Concat(_houseOwner.name, "的房间");


            self.houseOwner = _houseOwner;

            self.curRoomjoinRank = 0;


            self.JoinPlayer(_houseOwner);

        }


        /// <summary>
        /// 加入玩家
        /// </summary>
        public static void JoinPlayer(this S_Room self, S_Player player)
        {

            player.room = self;
            player.roomid = self.id;
            player.roomjoinRank = ++self.curRoomjoinRank;
            player.roomReally = false;


            //通知
            var info = S2C_InformRoomChange.Get();
            info.opt = 1;   //玩家加入
            info.playerID = player.uid;
            info.newUser = CopyUserRoomInfo(player);
            //sLog.Debug("------ffff---");
            //sLog.Debug("------ffff---count : " + self.playerList.Count);
            //return;
            S_HotfixMoudleMgr.Instance.Send(self.playerList, info);

            self.playerList.Add(player.uid, player); //添加

        }

        /// <summary>
        /// 同步 房间信息 给玩家
        /// </summary>
        public static void SyncRoomInfoToPlayer(this S_Room self, S_Player player)
        {
            var info = S2C_SyncRoomInfo.Get();  //
            if (info.infoList == null)
                info.infoList = new List<CMD_UserRoomInfo>();

            info.houseOwnerID = self.houseOwner.uid;
            info.roomName = self.roomName;

            foreach (var item in self.playerList)
            {
                var userRoominfo = CopyUserRoomInfo(item.Value);
                info.infoList.Add(userRoominfo);
            }

            player.session.Send(info);
        }

        static CMD_UserRoomInfo CopyUserRoomInfo(S_Player player)
        {
            var userRoominfo = CMD_UserRoomInfo.Get();
            userRoominfo.useinfo = CMD_UserInfo.Get();
            userRoominfo.useinfo.uid = player.uid;
            userRoominfo.useinfo.name = player.name;
            userRoominfo.ready = player.roomReally;
            userRoominfo.roomjoinRank = player.roomjoinRank;
            return userRoominfo;
        }


     



        public static void LeaveRoom(this S_Room self, S_Player player)
        {
            self.playerList.Remove(player.uid);
            player.room = null;
            player.roomid = 0;
            player.roomjoinRank = 0;

            if (self.count == 0)
            {
                S_HotfixMoudleMgr.Instance.roomMgr.DelRoom(self);   //删除房间
            }
            else
            {
                if (player == self.houseOwner && player != null)
                {
                    self.ChangeOwner(self.GetMinRoomjoinRank());      //更换房主
                }

                var info = S2C_InformRoomChange.Get();
                info.opt = 2;   //玩家离开
                info.playerID = player.uid;
                S_HotfixMoudleMgr.Instance.Send(self.playerList, info);    //跟换房主
            }

        }

        public static S_Player GetMinRoomjoinRank(this S_Room self)
        {
            uint tmp = uint.MaxValue;
            S_Player player = null;
            foreach (var item in self.playerList)
            {
                if (tmp > item.Value.roomjoinRank)
                {
                    player = item.Value;
                    tmp = item.Value.roomjoinRank;
                }
            }

            return player;
        }

        public static void ChangeOwner(this S_Room self, S_Player player)
        {
            self.houseOwner = player;

            var info = S2C_InformRoomChange.Get();
            info.opt = 5;   //更换房主
            info.playerID = player.uid;
            S_HotfixMoudleMgr.Instance.Send(self.playerList, info);    //跟换房主
        }




        public static Dictionary<uint, S_Player> GetAllPlayer(this S_Room self)
        {
            return self.playerList;
        }




    }

}

