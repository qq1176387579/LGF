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

            self.curState = RoomStateEnum.Create;   //进入当前房间
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
            if (player.roomReally)  //执行这个 不然报错
            {
                player.SetPrepared(false);
            }


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


        
            if (self.reallyCount < 0)
            {
                sLog.Error("data error  self.reallyCount : {0}", self.reallyCount);
            }

            if (player.loadFinish)
            {
                player.loadFinish = false;
                self.loadFinishCount--;
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


        public static S_Player GetPlayer(this S_Room self ,uint playeID)
        {
            self.playerList.TryGetValue(playeID, out var player);
            return player;
        }



        public static Dictionary<uint, S_Player> GetAllPlayer(this S_Room self)
        {
            return self.playerList;
        }


        /// <summary>
        /// 改变状态
        /// 感觉用状态机不太合适  因为需要热更新   会将原来的额舍弃掉， 然后又重新new 造成加载时间过长
        /// 但是状态机效果是最好的  不然代码看着臃肿  后面看情况处理吧
        /// </summary>
        /// <param name="self"></param>
        /// <param name="stateEnum"></param>
        /// <returns></returns>
        public static bool ChangState(this S_Room self, RoomStateEnum stateEnum)
        {
            RoomStateEnum curState = self.curState;
            switch (stateEnum)
            {
                case RoomStateEnum.Create:
                    return false;
                case RoomStateEnum.Loading:
                    if (curState == RoomStateEnum.Create)
                    {
                        self.curState = stateEnum;
                        self.loadFinishCount = 0;
                        foreach (var item in self.playerList)
                        {
                            item.Value.loadFinish = false;
                        }
                        StartLoadingScene(self);
                        return true;
                    }
                    break;
                case RoomStateEnum.Playing:
                    if (curState == RoomStateEnum.Loading)
                    {
                        self.curState = stateEnum;
                        self.StartPlayingGame();
                        return true;
                    }
                    break;
                default:
                    break;
            }


            sLog.Error("非法操作 curState ： {0} ->stateEnum  {1}", curState, stateEnum);
            return false;
        }

        /// <summary>
        /// 逻辑帧 
        /// </summary>
        public static void OnLogicFrame(this S_Room self)
        {
            if (self.curState == RoomStateEnum.Create)
                return;

            //可以用状态机 
            //但是状态机怎么维护热更新呢
            if (self.curState == RoomStateEnum.Loading)
            {
                self.SendLoadingMsg();
                return;
            }
            else if (self.curState == RoomStateEnum.Playing)
            {
                return;
                self.curFrame++;    //帧递增
                self.SendPlayingLogicFrame();
                return;
            }


            sLog.Error("OnLogicFrame  -> {0} ", self.curState);
            return;
          
        }


        /// <summary>
        /// 发送加载消息
        /// </summary>
        /// <param name="self"></param>
        public static void SendLoadingMsg(this S_Room self)
        {
            if (self.tmpRoomProgress.list.Count == 0)
                return;
    
            S_HotfixMoudleMgr.Instance.SendNotRecycle(self.playerList, self.tmpRoomProgress); 
            self.tmpRoomProgress.list.ClearReleaseMember(); // 清除数据
            
            //发送进入游戏命令
            if (self.loadFinishCount >= self.playerList.Count)
            {
                self.ChangState(RoomStateEnum.Playing);
            }

        }


        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="self"></param>
        public static void AddLoadingMsg(this S_Room self,C2S_RoomProgress msg)
        {
            //if (self.tmpRoomProgress.list == null)
            //{
            //    self.tmpRoomProgress.list = ListPool<C2S_RoomProgress>.Get();
            //}
            self.tmpRoomProgress.list.Add(msg);
            if (msg.progress == -1)
            {

                var player = self.GetPlayer(msg.uid);
                if (!player.loadFinish) //防止重复发送
                {
                    player.loadFinish = true;
                    self.loadFinishCount++;
                }
             
            }
        }

        /// <summary>
        /// 发送游戏中的逻辑帧
        /// </summary>
        /// <param name="self"></param>

        public static void SendPlayingLogicFrame(this S_Room self)
        {
            self.tmpFrameOpKey.curFrame = self.curFrame;
            S_HotfixMoudleMgr.Instance.SendNotRecycle(self.playerList, self.tmpFrameOpKey);  
            self.tmpFrameOpKey.allOpkey.ClearReleaseMember(); // 清除数据
        }


        /// <summary>
        /// 开始加载场景
        /// </summary>
        /// <param name="self"></param>
        public static void StartLoadingScene(this S_Room self)
        {
            var tmp = S2C_RoomtFinishType.Get();
            tmp.type = 1;
            S_HotfixMoudleMgr.Instance.Send(self.playerList, tmp); 
        }


        /// <summary>
        /// 开始进入副本 全部加载完成
        /// </summary>
        /// <param name="self"></param>
        public static void StartPlayingGame(this S_Room self)
        {
            var tmp = S2C_RoomtFinishType.Get();
            tmp.type = 2;
            S_HotfixMoudleMgr.Instance.Send(self.playerList, tmp);
        }


    }

}

