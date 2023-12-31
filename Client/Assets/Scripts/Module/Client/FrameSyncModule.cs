/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/30 23:26:51
/// 功能描述:  帧同步 模块
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;



/// <summary>
/// 帧同步管理
/// </summary>
public class FrameSyncMgr : LGF.SingletonBase<FrameSyncMgr>
{
    ulong CurFrame = 1; //服务器初始帧是1开始
    public bool IsPlayback = false;
    Dictionary<ulong, S2C_FrameOpKey> framesList = new Dictionary<ulong, S2C_FrameOpKey>();

    public int DicCount => framesList.Count;

    public void GameInit()
    {
        CurFrame = 1;

        foreach (var item in framesList) {
            item.Value.Release();
        }
        framesList.Clear();
    }

    public void OnFixedUpdate()
    {
        if (framesList.TryGetValue(CurFrame, out var val)) {
            framesList.Remove(CurFrame);    //减少原来的
            CurFrame++;
            GameSceneMgr.Instance.OnServerLogicFrame(val);
            val.Release();
        }
    }

    public void AddFrame(S2C_FrameOpKey msg)
    {
        framesList.Add(msg.curFrame, msg);

        if (IsPlayback) {
            return;
        }

        if (msg.curFrame != CurFrame) {
            this.Debug($" {CurFrame}->{msg.curFrame} 跳帧了 少接收一帧");
            if (msg.curFrame - CurFrame > 10) {
                this.Debug($" {CurFrame}->{msg.curFrame} 跳帧了 延迟波动");
            }
        }
    }

}



public class FrameSyncModule : ModuleBase
{
    
    
    protected override void OnInit()
    {
        base.OnInit();

        RegisterClientMsg<S2C_FrameOpKey>(OnServerLogicFrame, false);
        LGFEntry.RegisterOnFixedUpdate(OnFixedUpdate);
        
    }


    /// <summary>
    /// 后面看情况 要不要写追帧
    /// 当前 OnFixedUpdate帧率 是 服务器的2被 33ms一次  服务器是66ms一次
    /// 那么正常情况是 双倍追帧
    /// </summary>
    private void OnFixedUpdate()
    {
        FrameSyncMgr.Instance.OnFixedUpdate();
    }

    /// <summary>
    /// 服务器逻辑帧
    /// </summary>
    /// <param name="msg"></param>
    void OnServerLogicFrame(S2C_FrameOpKey msg)
    {

        FrameSyncMgr.Instance.AddFrame(msg);

        //感觉不能在这里执行。  
        //可能因为网络问题一下子出现很多帧
        //GameSceneMgr.Instance.OnServerLogicFrame(msg);  
    }

}
