/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/30 23:26:51
/// 功能描述:  帧同步 模块
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

public class FrameSyncModule : ModuleBase
{
    protected override void OnInit()
    {
        base.OnInit();

        RegisterClientMsg<S2C_FrameOpKey>(OnServerLogicFrame);
    }

    /// <summary>
    /// 服务器逻辑帧
    /// </summary>
    /// <param name="msg"></param>
    void OnServerLogicFrame(S2C_FrameOpKey msg)
    {
        //感觉不能在这里执行。  
        //可能因为网络问题一下子出现很多帧
        GameSceneMgr.Instance.OnServerLogicFrame(msg);  
    }

}
