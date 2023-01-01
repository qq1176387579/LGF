/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/26 14:51:06
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;

namespace LGF.Server.Hotfix
{
    public class S_FrameSyncMoudule : S_ModuleBase
    {
        protected override void OnInit()
        {
            moduleMgr.GetMgr<S_FrameSyncManager>().RegisterLogicFrame(OnLogicFrame);    //逻辑帧

            RegisterServerMsg<C2S_FrameOpKey>(NetMsgDefine.C2S_FrameOpKey, OnFrameOpKey, false);
            
        }



        void OnLogicFrame()
        {
            //sLog.Debug("--OnLogicFrame--");
            var allroom = roomMgr.GetPlayingRoom();
            foreach (var room in allroom)
            {
                room.Value.OnLogicFrame();
            }

        }

        void OnFrameOpKey(KcpServer.KcpSession session, C2S_FrameOpKey msg)
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

            player.room.AddFrameOpKey(msg);
        }


        public override void Close()
        {
            moduleMgr.GetMgr<S_FrameSyncManager>().UnRegisterLogicFrame(OnLogicFrame);
            base.Close();
        }
    }

}
