/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/11 2:00:25
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;

namespace LGF.Server.Hotfix
{
    public class S_RoomModule : S_ModuleBase
    {
        S2C_CreateRoom tmp_CreateRoom = new S2C_CreateRoom();
        S2C_InformRoomChange tmp_informRoomChange = new S2C_InformRoomChange();
        S2C_GetAllTheRooms tmp_GetAllTheRooms = new S2C_GetAllTheRooms();
        
        protected override void OnInit()
        {
            base.OnInit();

            RegisterServerMsg<C2S_CreateRoom>(NetMsgDefine.C2S_CreateRoom, OnCreateRoom);
            RegisterServerMsg<C2S_InformRoomChange>(NetMsgDefine.C2S_InformRoomChange, OnRoomChange);
            RegisterServerMsg<C2S_GetAllTheRooms>(NetMsgDefine.C2S_GetAllTheRooms, OnGetAllTheRooms);
            RegisterServerMsg<C2S_RoomProgress>(NetMsgDefine.C2S_RoomProgress, OnRoomProgress, false);
        }


        /// <summary>
        /// 获得全部房间信息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="c2S_CreateRoom"></param>
        void OnGetAllTheRooms(KcpServer.KcpSession session, C2S_GetAllTheRooms c2S_CreateRoom)
        {
            var rooms = roomMgr.GetAllRoom();
            var info = tmp_GetAllTheRooms;
            if (info.roomList == null)
                info.roomList = new List<CMD_SimpleRoomInfo>();

            int count = 0;
            foreach (var item in rooms)
            {
                if (info.roomList.Count <= count) //2 <= 2
                {
                    info.roomList.Add(CMD_SimpleRoomInfo.Get());
                }
                CMD_SimpleRoomInfo roomInfo = info.roomList[count];
                roomInfo.roomID = item.Value.id;
                roomInfo.roomName = item.Value.roomName;
                count++;
            }

            for (int i = info.roomList.Count - 1; i >= count; i--)
            {
                info.roomList[i].Release();
                info.roomList.RemoveAt(i);
            }
       

            session.SendNotRecycle(info);


        }






        void OnCreateRoom(KcpServer.KcpSession session, C2S_CreateRoom c2S_CreateRoom)
        {
            var player = GetPlayer(session);

            if (player.InRoom)
            {
                tmp_CreateRoom.ErrorCode = ErrCode.Fail;
                session.SendNotRecycle(tmp_CreateRoom);
                return;
            }

            S_Room room = roomMgr.CreateRoom(player);
            tmp_CreateRoom.roomID = room.id;
            tmp_CreateRoom.roomName = room.roomName;

            tmp_CreateRoom.ErrorCode = ErrCode.Succeed;
            session.SendNotRecycle(tmp_CreateRoom);
        }





        void OnRoomChange(KcpServer.KcpSession session, C2S_InformRoomChange msg)
        {
            //sLog.Debug("-------OnRoomChange--t {0}", t);

            var player = GetPlayer(session);

            if (player == null)
            {
                sLog.Debug(" session.playerID  si {0}", session.playerID);
                return;
            }
            int optType = msg.opt;
            var info = tmp_informRoomChange;
            info.playerID = player.uid;
            info.opt = optType;
            info.ErrorCode = ErrCode.Succeed;

         

            //可以用策略模式
            switch (optType)
            {
                case 1: //请求加入房间
                    {
                        var room = roomMgr.GetRoom(msg.roomID);
                        if (room == null)
                        {
                            info.ErrorCode = ErrCode.RoomNotExist;  //房间不存在
                            break;
                        }
                        if (room.curState != RoomStateEnum.Create)
                        {
                            sLog.Debug("非法操作  房间已经在游戏中");
                            break;
                        }

                        //sLog.Debug("--test--OnRoomChange--");
                        //return;

                        room.JoinPlayer(player);
                        room.SyncRoomInfoToPlayer(player);  //
                        return;
                    }
                case 2: //2表示离开房间
                    {
                        if (player.room.curState != RoomStateEnum.Create)
                        {
                            sLog.Error("非法操作  房间已经在游戏中");
                            break;
                        }

                        player.room.LeaveRoom(player);  //离开房间
                        break;
                    }
                case 3: //3表示玩家准备
                    {
                        if (player.room.curState != RoomStateEnum.Create)
                        {
                            sLog.Error("非法操作  房间已经在游戏中");
                            break;
                        }

                        if (player.roomReally)
                        {
                            sLog.Debug("---------player.roomReally{0}", player.roomReally);
                            info.ErrorCode = ErrCode.INVALID_OPT;   //非法操作
                            break;
                        }

                        player.SetPrepared(true);
                        SendNotRecycle(player.room.GetAllPlayer(), info);
                        CheakStartLoading(player.room); //检查房间
                        return;
                    }
                case 4: // 4表示玩家取消准备
                    {
                        if (player.room.curState != RoomStateEnum.Create)
                        {
                            sLog.Error("非法操作  房间已经在游戏中");
                            break;
                        }

                        if (!player.roomReally)
                        {
                            sLog.Debug("--------player.roomReally{0}-", player.roomReally);
                            info.ErrorCode = ErrCode.INVALID_OPT;   //非法操作
                            break;
                        }
                        sLog.Debug("-----------ff--");
                        player.SetPrepared(false);
                        SendNotRecycle(player.room.GetAllPlayer(), info);
                        CheakStartLoading(player.room); //检查房间
                        return;
                    }

                default:
                    {
                        sLog.Debug("玩家非法操作 {0} ：ID: {1}  opt{2}", player.name, player.uid, optType);
                        //非法操作
                        info.ErrorCode = ErrCode.Fail;

                        break;
                    }
            }

            session.SendNotRecycle(info);
        }


        /// <summary>
        /// 检查开始加载
        /// </summary>
        void CheakStartLoading(S_Room room)
        {
            sLog.Debug("-----------CheakStartLoading count--" + room.reallyCount);
            sLog.Debug("-----------room.GetAllPlayer().Count--" + room.GetAllPlayer().Count);
            //var players = room.GetAllPlayer();
            if (room.GetAllPlayer().Count != room.reallyCount)
            {
                sLog.Debug("--------->   {0} {1}",room.reallyCount,room.GetAllPlayer().Count);
                sLog.Debug("--------->   {0}", room.reallyCount == room.GetAllPlayer().Count);
                //没有准备
                return;
            }
            sLog.Debug("全部玩家 准备完成 开始进入加载状态");

            if (!room.ChangState(RoomStateEnum.Loading))
            {
                sLog.Debug("状态改变失败");
            }

            
            //SendNotRecycle(room.GetAllPlayer(),);
        }


        /// <summary>
        /// 获得进度值
        /// </summary>
        void OnRoomProgress(KcpServer.KcpSession session, C2S_RoomProgress msg)
        {
            var player = GetPlayer(session);

            if (player == null)
            {
                sLog.Error("玩家不存在 playerID: {0}", session.playerID);
                return;
            }

            if (player.room == null)
            {
                sLog.Error(" 房间不存在 playerID : {0}", session.playerID);
                return;
            }
            
            player.room.AddLoadingMsg(msg);
        }

    }
}





