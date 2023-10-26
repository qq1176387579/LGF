/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2023/10/25 2:46:39
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using Protocol;
using UnityEngine;
using UnityEngine.SceneManagement;


public interface IBaseEvent
{
    EventCode EventCode { get; }
    void OnEvent(S2C_GameEvent eventData);
}



public abstract class BaseEvent : MonoBehaviour, IBaseEvent
{
    public abstract EventCode EventCode { get; }

    public abstract void OnEvent(S2C_GameEvent eventData);

    public  void Add()
    {
        GameRequestMgr.Instance.AddEvent(this);
    }
    public void Remove()
    {
        GameRequestMgr.Instance.RemoveEvent(this);
    }
}


public interface IBaseRequest
{
    abstract OperationCode OpCode { get; }
    void OnOperationResponse(S2C_GameMsg msg);
}


public abstract class BaseRequest<T> : SingletonBase<T>, IBaseRequest where T : BaseRequest<T>, new()
{
    public abstract OperationCode OpCode { get; }
    public virtual void DefaultRequest()
    {
        Debug.Log("发送请求  >>>> type : " + GetType().Name);
        Send();
    }

    public abstract void OnOperationResponse(S2C_GameMsg msg);

    protected override void OnNew()
    {
        GameRequestMgr.Instance.AddRequest(this);
    }

    public void Remove()
    {
        GameRequestMgr.Instance.RemoveRequest(this);
    }

    public void Send(IProtocol protocol) 
    {
        Send(protocol.ToJson());
    }

    public void Send(string _data = null)
    {
        var tmp = C2S_GameMsg.Get();
        tmp.type = (uint)OpCode;
        if (_data != null) {
            tmp.data = _data;
        }
        GameRequestMgr.Instance.Send(tmp);
    }
}