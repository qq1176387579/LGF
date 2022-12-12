/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/11 23:43:02
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using static LGF.Net.KcpServer;

public class S_LoginModule : S_ModuleBase
{
    protected override void OnInit()
    {
        base.OnInit();

        EventManager.Instance.AddListener<KcpSession>(GameEventType.ServerEvent_PlayerConnect, OnPlayerConnect);
        EventManager.Instance.AddListener<KcpSession>(GameEventType.ServerEvent_ReConnect, OnReConnect);
        
    }


    public void OnPlayerConnect(KcpSession session)
    {
        sLog.Debug("添加新玩家 地址： {0}  名字{1}", session.kcpAgent.endPoint, session.name);
        playerMgr.AddNewPlayer(session);
    }

    public void OnReConnect(KcpSession session)
    {
        var player = GetPlayer(session.playerID);
        if (player == null)
        {
            sLog.Debug("重进出错  玩家为空 出错 ： {0}  名字{1}", session.kcpAgent.endPoint, session.name);
            return;
        }

        //重连
        player.ReConnect();

    }






}
