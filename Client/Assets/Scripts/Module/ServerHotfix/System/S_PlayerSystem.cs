/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/26 1:46:21
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;


namespace LGF.Server.Hotfix
{
    public static class S_PlayerSystem
    {
        public static void ReConnect(this S_Player self)
        {
            self.room?.LeaveRoom(self);   //离开房间
            self.name = self.session.name;  //修改成现在名称
        }


        /// <summary>
        /// 玩家准备
        /// </summary>
        /// <param name="player"></param>
        public static void SetPrepared(this S_Player self, bool val)
        {
            if (self.roomReally == val)
            {
                sLog.Error(" 禁止 非法请求 已经准备了 或者 取消准备了 ");
                return;
            }

            if (!self.roomReally)
            {
                self.roomReally = val;
                self.room.reallyCount++;
            }
            else
            {
                self.roomReally = val;
                self.room.reallyCount--;
            }
        }

    }



}

