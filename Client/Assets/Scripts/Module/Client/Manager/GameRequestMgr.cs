/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2023/10/25 3:09:21
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

public class GameRequestMgr : ModuleSingletonBase<GameRequestMgr>
{
    public Dictionary<OperationCode, IBaseRequest> RequestDict = new Dictionary<OperationCode, IBaseRequest>();
    public Dictionary<EventCode, IBaseEvent> EventDict = new Dictionary<EventCode, IBaseEvent>();


    public override void Init()
    {
        base.Init();
    }


    public void AddRequest(IBaseRequest request)
    {
        if (!RequestDict.ContainsKey(request.OpCode)) {
            RequestDict.Add(request.OpCode, request);
        }
    }
    public void RemoveRequest(IBaseRequest request)
    {
        RequestDict.Remove(request.OpCode);
    }


    public void AddEvent(IBaseEvent e)
    {
        if (!EventDict.ContainsKey(e.EventCode)) {
            EventDict.Add(e.EventCode, e);
        }
    }
    public void RemoveEvent(IBaseEvent e)
    {
        EventDict.Remove(e.EventCode);
    }

    public void Send<T>(T data, bool IsRecycle = true) where T : C2S_BASE<T>, new()
    {
        Client.Send(data, IsRecycle);
    }
}