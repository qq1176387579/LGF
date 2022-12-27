/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/26 14:51:06
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

namespace LGF.Server.Hotfix
{
    public class S_FrameSyncMoudule : S_ModuleBase
    {
        protected override void OnInit()
        {
            moduleMgr.GetMgr<S_FrameSyncManager>().RegisterLogicFrame(OnLogicFrame);    //逻辑帧
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




        public override void Close()
        {
            moduleMgr.GetMgr<S_FrameSyncManager>().UnRegisterLogicFrame(OnLogicFrame);
            base.Close();
        }
    }

}
