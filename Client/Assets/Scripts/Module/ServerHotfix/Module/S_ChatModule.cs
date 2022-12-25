/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/12 21:12:06
/// 功能描述:  服务端聊天管理
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;
using UnityEngine;

namespace LGF.Server.Hotfix
{
    /// <summary>
    /// 服务器 chat管理
    /// </summary>
    public class S_ChatModule : S_ModuleBase
    {

        S2C_ChatMsg textMsg = new S2C_ChatMsg();

        protected override void OnInit()
        {
            base.OnInit();
            RegisterServerMsg<C2S_ChatMsg>(NetMsgDefine.C2S_ChatMsg, OnTextMsg);
        }



        private void OnTextMsg(KcpServer.KcpSession arg1, C2S_ChatMsg msg)
        {

            var player = GetPlayer(arg1);
            if (player == null)
            {
                sLog.Error("玩家为空 出错 id: {0}", arg1.playerID);
                return;
            }
            InitTmpChatMsg();
            textMsg.type = msg.type;
            if (textMsg.type == ChatType.Room)
            {
                if (!player.InRoom)
                {
                    sLog.Debug("非法操作 不在房间内");
                    textMsg.ErrorCode = ErrCode.INVALID_OPT;
                    SendNotRecycle(player, textMsg);
                    return;
                }
                textMsg.name = player.name;
                textMsg.msg = msg.msg;
                SendNotRecycle(player.room.GetAllPlayer(), textMsg);
            }

            sLog.Debug("{0} 广播消息  player > {1} : {2}", textMsg.type, textMsg.name, textMsg.msg);


            //服务器获得信息
            //Server.Broadcast(textMsg,false);  //广播信息
        }

        void InitTmpChatMsg()
        {
            textMsg.msg = null;
            textMsg.ErrorCode = ErrCode.Succeed;
            textMsg.name = null;
        }




    }

}

