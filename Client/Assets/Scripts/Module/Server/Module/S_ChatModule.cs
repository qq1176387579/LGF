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

    S2C_ChatMsg textMsg = new S2C_ChatMsg();

    protected override void OnInit()
    {
        base.OnInit();
        RegisterServerMsg<C2S_ChatMsg>(NetMsgDefine.C2S_ChatMsg, OnTextMsg);
    }



    private void OnTextMsg(KcpServer.KcpSession arg1, C2S_ChatMsg arg2)
    {
        //Debug.Log("");
        //EventManager.Instance.BroadCastEvent(GameEventType.OnUpdate, arg2); //显示到面板中
        //arg2.uid
        textMsg.name = arg1.name; 
        textMsg.msg = arg2.msg;
        

        sLog.Debug("广播消息：" + textMsg.name + " : " + textMsg.msg);
        //服务器获得信息
        //Server.Broadcast(textMsg,false);  //广播信息
    }




    

}
