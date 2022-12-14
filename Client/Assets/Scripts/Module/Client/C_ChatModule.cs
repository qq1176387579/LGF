/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/12 21:11:52
/// 功能描述:  客户端聊天管理
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;




public class C_ChatModule : C_ModuleBase
{
    //S2C_ChatMsg textMsg = new S2C_ChatMsg();
    C2S_ChatMsg C2S_ChatMsg = new C2S_ChatMsg();
    protected override void OnInit()
    {
        RegisterClientMsg<S2C_ChatMsg>(NetMsgDefine.S2C_ChatMsg, OnTextMsg);
    }

    private void OnTextMsg(S2C_ChatMsg data)
    {
        sLog.Debug($" call 消息 {data.name} : {data.msg}");
        EventManager.Instance.BroadCastEvent(GameEventType.ClientEvent_RoomChatMsg, data.name, data.msg);
    }

    public void SendMsgToRoom(string msg)
    {
        if (!player.InRoom)
        {
            sLog.Error("非法操作  不在房间内");
            return;
        }

        C2S_ChatMsg.msg = msg;
        C2S_ChatMsg.type = ChatType.Room;
        SendNotRecycle(C2S_ChatMsg);
    }

}
