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


///// <summary>
///// 帧同步管理器
///// </summary>
//public class S_FrameSyncModule : S_ModuleBase
//{
//    ulong taskID;

//    protected override void OnInit()
//    {
//        base.OnInit();

//        taskID = timerMgr.AddTask(FrameSync, GameFrameConfig.ServerLogicFrameIntervelMs, TimeUnit.Millisecond, 0);
//    }


//    void FrameSync(ulong tid)
//    {

//    }


//}
