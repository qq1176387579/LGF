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

    public class LoginModule : ModuleBase
    {
        protected override void OnInit()
        {
            base.OnInit();

            //EventManager.Instance.AddListener<KcpSession>(GameEventType.ServerEvent_PlayerConnect, OnPlayerConnect);
            EventManager.Instance.AddListener<KcpSession>(GameEventType.ServerEvent_ReConnect, OnReConnect);
            EventManager.Instance.AddListener<KcpSession>(GameEventType.ServerEvent_Disconnect, OnDisconnect);

            RegisterServerMsg<C2S_Login>(OnLogin);

        }

        private void OnLogin(KcpSession session, C2S_Login data)
        {
            var tmp = S2C_Login.Get();
            tmp.ErrorCode = ErrCode.Fail;
            if (data.name.IsNullOrEmpty()) {
                session.Send(tmp);  //非法输出名称
                return;
            }

            if (session.useid != 0) {
                sLog.Debug($" >>>>>>>> OnLogin 玩家登录过 uid:{session.useid}");
                session.Send(tmp);
                return;
            }
            var player = GetPlayer(session.useid);
            if (player != null) {
                sLog.Debug($" >>>>>>>> OnLogin 无法重复登录 uid:{session.useid}");
                session.Send(tmp);
                return;
            }
            player = playerMgr.AddNewPlayer(session, data.name);
            tmp.uid = player.uid;
            sLog.Debug($"OnLogin 添加新的玩家 useid:<{session.useid}>  name:<{player.name}>");
            tmp.ErrorCode = ErrCode.Succeed;
            session.Send(tmp);
        }


        void OnReConnect(KcpSession session)
        {
            sLog.Debug(" >>>>>>>> OnReConnect " + session.useid);
            if (session.useid == 0) {
                return; //未登录 重连不做处理
            }

            var player = GetPlayer(session.useid);
            if (player == null)
            {
                sLog.Debug("重进出错  玩家为空 出错 ： {0}  名字{1}", session.kcpAgent.endPoint, session.useid);
                return;
            }
            else {
                sLog.Debug("暂时不实现重登操作  需要退出通知前端 退出操作");
            }

            //重连
            //player.ReConnect();

            //player.room?.LeaveRoom(player);   //离开房间
        }



        void OnDisconnect(KcpSession session)
        {
            sLog.Debug(" >>>>>>>> OnDisconnect 断开连接 " + session.useid);
            var player = GetPlayer(session.useid);
            if (player == null)
            {
                sLog.Debug("重进出错  玩家为空 出错 ： {0}  名字{1}", session.kcpAgent.endPoint, session.useid);
                return;
            }

            player.OnOffline();
        }

        public override void Close()
        {
            //EventManager.Instance.RemoveListerner<KcpSession>(GameEventType.ServerEvent_PlayerConnect, OnPlayerConnect);
            EventManager.Instance.RemoveListerner<KcpSession>(GameEventType.ServerEvent_ReConnect, OnReConnect);
            EventManager.Instance.RemoveListerner<KcpSession>(GameEventType.ServerEvent_Disconnect, OnDisconnect);
        }

    }


    

}

