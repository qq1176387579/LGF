/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/11 23:43:02
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using LGF;
using LGF.Log;
using static LGF.Net.KcpServer;


namespace LGF.Server.Hotfix
{

    public class S_LoginModule : S_ModuleBase
    {
        protected override void OnInit()
        {
            base.OnInit();

            EventManager.Instance.AddListener<KcpSession>(GameEventType.ServerEvent_PlayerConnect, OnPlayerConnect);
            EventManager.Instance.AddListener<KcpSession>(GameEventType.ServerEvent_ReConnect, OnReConnect);
            EventManager.Instance.AddListener<KcpSession>(GameEventType.ServerEvent_Disconnect, OnDisconnect);

        }


        void OnPlayerConnect(KcpSession session)
        {
            if (playerMgr.GetPlayerByID(session.playerID) != null)
            {
                sLog.Error("非法请求");
                return;
            }
            sLog.Debug("添加新玩家 地址： {0}  名字{1}", session.kcpAgent.endPoint, session.name);
            playerMgr.AddNewPlayer(session);
        }

        void OnReConnect(KcpSession session)
        {
            sLog.Debug(" >>>>>>>> OnReConnect " + session.playerID);
          
            var player = GetPlayer(session.playerID);
            if (player == null)
            {
                sLog.Debug("重进出错  玩家为空 出错 ： {0}  名字{1}", session.kcpAgent.endPoint, session.name);
                return;
            }

            //重连
            player.ReConnect();

            //player.room?.LeaveRoom(player);   //离开房间
        }



        void OnDisconnect(KcpSession session)
        {
            sLog.Debug(" >>>>>>>> OnDisconnect " + session.playerID);
            var player = GetPlayer(session.playerID);
            if (player == null)
            {
                sLog.Debug("重进出错  玩家为空 出错 ： {0}  名字{1}", session.kcpAgent.endPoint, session.name);
                return;
            }

            player.OnOffline();
        }

        public override void Close()
        {
            EventManager.Instance.RemoveListerner<KcpSession>(GameEventType.ServerEvent_PlayerConnect, OnPlayerConnect);
            EventManager.Instance.RemoveListerner<KcpSession>(GameEventType.ServerEvent_ReConnect, OnReConnect);
            EventManager.Instance.RemoveListerner<KcpSession>(GameEventType.ServerEvent_Disconnect, OnDisconnect);
        }

    }


    

}

