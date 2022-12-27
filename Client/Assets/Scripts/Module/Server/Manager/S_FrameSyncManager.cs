/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/24 18:29:06
/// 功能描述:  帧同步管理器 帧同步消息处理
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Timers;



public class GameFrameConfig
{
    /// <summary>
    /// 服务器逻辑帧间隔
    /// </summary>
    public const int ServerLogicFrameIntervelMs = 66;   

}


/// <summary>
/// 帧同步管理器
/// </summary>
public class S_FrameSyncManager : S_SingletonBase<S_FrameSyncManager>
{
    ulong taskID;

    event System.Action m_OnLogicFrameEvt;

    public override void Init()
    {
        taskID = moduleMgr.timerMgr.AddTask(OnLogicFrame, GameFrameConfig.ServerLogicFrameIntervelMs, TimeUnit.Millisecond, 0);    //扩展定时器
    }

    void OnLogicFrame(ulong tid)
    {
        //sLog.Debug("OnLogicFrame");
        //逻辑帧
        //EventManager.Instance.BroadCastEvent(GameEventType.ServerEvent_OnLogicFrame);

        m_OnLogicFrameEvt?.Invoke();
    }

    /// <summary>
    /// 注册逻辑帧
    /// </summary>
    public void RegisterLogicFrame(System.Action action)
    {
        m_OnLogicFrameEvt += action;
    }

    /// <summary>
    /// 注销逻辑帧
    /// </summary>
    public void UnRegisterLogicFrame(System.Action action)
    {
        m_OnLogicFrameEvt -= action;
    }

}
