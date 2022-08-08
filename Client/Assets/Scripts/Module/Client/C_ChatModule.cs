/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/12 21:11:52
/// 功能描述:  客户端聊天管理
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;

public class C_ChatModule : C_ModuleBase
{
    //S2C_TextMsg textMsg = new S2C_TextMsg();
    C2S_TextMsg c2s_TextMsg = new C2S_TextMsg();
    public override void Init()
    {
        NetMsgHandlingMgr.Instance.RegisterClientMsg<S2C_TextMsg>(NetMsgDefine.S2C_TextMsg, OnTextMsg);
    }

    private void OnTextMsg(S2C_TextMsg data)
    {
        sLog.Debug($" call 消息 {data.name} : {data.msg}");
        EventManager.Instance.BroadCastEvent(GameEventType.c_TextMsg, data.name, data.msg);
    }

    public void SendMsg(string msg)
    {
        c2s_TextMsg.msg = msg;
        Client.Send(c2s_TextMsg, false);
    }

}
