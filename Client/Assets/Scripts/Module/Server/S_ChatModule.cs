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


/// <summary>
/// 服务器 chat管理
/// </summary>
public class S_ChatModule : S_ModuleBase
{
    S2C_TextMsg textMsg = new S2C_TextMsg();


    public override void Init()
    {
        NetMsgHandlingMgr.Instance.RegisterServerMsg<C2S_TextMsg>(NetMsgDefine.C2S_TextMsg, OnTextMsg);
    }

    private void OnTextMsg(KcpServer.KcpSession arg1, C2S_TextMsg arg2)
    {
        //Debug.Log("");
        //EventManager.Instance.BroadCastEvent(GameEventType.OnUpdate, arg2); //显示到面板中

        textMsg.name = arg1.name; 
        textMsg.msg = arg2.msg;

        //服务器获得信息
        Server.Broadcast(textMsg);  //广播信息
    }




    

}
