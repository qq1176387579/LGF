using System.Collections;
using System.Collections.Generic;
using UnityEngine;








public class GameRequestModule : ModuleBase
{
    public Dictionary<OperationCode, IBaseRequest> RequestDict;
    public Dictionary<EventCode, IBaseEvent> EventDict;

    protected override void OnInit()
    {
        base.OnInit();
        RequestDict = GameRequestMgr.Instance.RequestDict;  
        EventDict = GameRequestMgr.Instance.EventDict;  

        RegisterClientMsg<S2C_GameMsg>(OnMsg);
        RegisterClientMsg<S2C_GameEvent>(OnEvent);

    }


    void OnMsg(S2C_GameMsg msg)
    {
        if (RequestDict.TryGetValue((OperationCode)msg.type, out var val)) {
            Debug.Log("----响应堆栈测试-- opCode :" + (OperationCode)msg.type);
            val.OnOperationResponse(msg);
        }
        else {
            Debug.LogError($"没找到对应的响应处理对象 <{(OperationCode)msg.type}>");
        }

    }


    void OnEvent(S2C_GameEvent evt)
    {
        if (EventDict.TryGetValue((EventCode)evt.type, out var val)) {
            Debug.Log("OnEvent code :" + (EventCode)evt.type);
            val.OnEvent(evt);
        }
        else {
            Debug.LogError($"OnEvent code没找到对应的响应处理对象 <{(EventCode)evt.type}>");
        }

    }


}
