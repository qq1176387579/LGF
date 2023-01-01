/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/30 23:37:49
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using PEMath;
using PEPhysx;
using UnityEngine;

/// <summary>
/// 游戏场景管理器
/// </summary>
public partial class GameSceneMgr : SingletonBase<GameSceneMgr>
{
    bool isNative = true;
    C_MainPlayerManager _mainPlayer;
    C_MainPlayerManager mainPlayer {
        get {
            if (_mainPlayer == null) _mainPlayer = C_MainPlayerManager.Instance;
            return _mainPlayer;
        } 
    }

    public uint _keyid = 0;
    public uint Keyid { get => ++_keyid; } 
    List<PlayerUnit> players = new List<PlayerUnit>();
    //暂时先用字典   字典有查询消耗
    Dictionary<uint, PlayerUnit> playerUnits = new Dictionary<uint, PlayerUnit>();



    MapRoot mapRoot;
    EnvColliders logocEnv;

    void NativeInit()
    {
        isNative = !C_ModuleMgr.CheckInstance();    //判断是否有开启 客户端服务

        if (isNative)   //单机模式
        {
            MonoManager.Instance.AddFixedUpdateListener(OnFixedUpdate);
            allkey = new S2C_FrameOpKey();
            allkey.allOpkey = new List<C2S_FrameOpKey>();
        }
    }


    public void LogicInit()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].LogicInit();
        }
    }



    public void LogicTick()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].LogicTick();
        }
    }



    public List<PEColliderBase> GetEnvColliders()
    {
        return logocEnv.GetEnvColliders();
    }




    #region accept


    S2C_FrameOpKey allkey;
    public ulong CurFrame { get; private set; }
    public void OnServerLogicFrame(S2C_FrameOpKey msg)
    {
        CurFrame = msg.curFrame;
        var opkey = msg.allOpkey;
        if (opkey != null)
        {
            for (int i = 0; i < opkey.Count; i++)
            {
                playerUnits[opkey[i].uid].InputKey(opkey[i]);
            }
        }
      

        LogicTick();
    }

    void OnFixedUpdate()
    {
        allkey.curFrame++;
        OnServerLogicFrame(allkey);
        allkey.allOpkey.ClearReleaseMember();
    }

    #endregion






    #region Send


    /// <summary>
    /// 发送移动帧操作到服务器
    /// </summary>
    /// <param name="logicDir"></param>
    /// <returns></returns>
    public bool SendMoveKey(in PEVector3 logicDir)
    {
        if (!isNative)
        {
            //后面可以优化  客户端的逻辑帧的时候 发送
            mainPlayer.SendMoveKey(logicDir.x.ScaledValue, logicDir.z.ScaledValue, Keyid);
            return true;
        }

        C2S_FrameOpKey key = C2S_FrameOpKey.Get();

        key.keytype = Frame_KeyType.Move;
        key.moveKey = Frame_MoveKey.Get();
        key.moveKey.x = logicDir.x.ScaledValue;
        key.moveKey.z = logicDir.z.ScaledValue;
        key.moveKey.Keyid = Keyid;

        allkey.allOpkey.Add(key);
        return true;
    }


    #endregion







}
