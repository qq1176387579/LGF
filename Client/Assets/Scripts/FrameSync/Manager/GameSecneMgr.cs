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

    MainPlayerManager _mainPlayer;
    MainPlayerManager mainPlayer {
        get {
            if (_mainPlayer == null) _mainPlayer = MainPlayerManager.Instance;
            return _mainPlayer;
        } 
    }
    #region isNative
    bool isNative = true;
    public uint _keyid = 0;
    public uint Keyid { get => ++_keyid; }
    #endregion


    List<PlayerUnit> players = new List<PlayerUnit>();
    /// <summary>
    ///  帧同步里面可以用Dict的，只是你不要用它来保存需要遍历的元素。
    ///  如果用foreach 不一定保证顺序一样
    ///  正确理解 是在在帧同步中 不要遍历Dict  因为遍历字典是无序的 不保证所有设备都一样
    /// </summary>
    Dictionary<uint, PlayerUnit> playerUnits = new Dictionary<uint, PlayerUnit>();



    MapRoot mapRoot;
    EnvColliders logocEnv;

    void InitOther()
    {
        //LGFEntry.RegisterOnFixedUpdate(OnFixedUpdate);
    }

    void NativeInit()
    {
        isNative = !ModuleMgr.CheckInstance();    //判断是否有开启 客户端服务
        
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
        if (CurFrame + 1 != msg.curFrame) {
            this.DebugError($" {CurFrame}->{msg.curFrame} 跳帧了 少接收一帧");
        }
        CurFrame = msg.curFrame;
        var opkey = msg.allOpkey;
        if (opkey != null)
        {
            for (int i = 0; i < opkey.Count; i++)
            {
                try {
                    playerUnits[opkey[i].uid].InputKey(opkey[i]);
                }
                catch (System.Exception e) {
                    Debug.LogError($"opkey[i].uid {opkey[i].uid}");
                    foreach (var item in playerUnits) {
                        Debug.Log($"info Key: {item.Key} valinfo playeid:{item.Value.playerid}");
                    }
                    throw;
                }
                //Debug.Log("uid >> "+ opkey[i].uid);
               
            }
        }
      
        LogicTick();

        CheckCharacterPositionRequest.Instance.DefaultRequest(players, CurFrame);
    }




    void OnFixedUpdate()
    {
        if (isNative) {
            allkey.curFrame++;
            OnServerLogicFrame(allkey);
            allkey.allOpkey.ClearReleaseMember();
        }

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
        key.uid = mainPlayer.uid;
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
