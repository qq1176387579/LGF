/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/31 0:56:40
/// 功能描述:  
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Serializable;
using PEMath;
using PEPhysx;
using UnityEngine;





public partial class GameSceneMgr
{

    protected override void OnNew()
    {
        base.OnNew();
        EventManager.Instance.AddListener<GameSceneInitData>(GameEventType.ClientEvent_StartPlayback, OnStartPlayback);


    }

    private void OnStartPlayback(GameSceneInitData data)
    {
        isPlayback = true;
        sceneInitData = data;
        Init();
    }

    public override void Init()
    {
        if (!EventCenter.CheckInstance()) EventCenter.Instance.Init();  //TestPlayer1 依赖这个
        
        NativeInit();
        InitData();


        InitEnv();
        InitPlayer();

        LogicInit();

        InitOther();
    }

    ///// <summary>
    ///// 如果是回放
    ///// </summary>
    ///// <param name="s"></param>
    ///// <returns></returns>
    //public void InitPlayback()
    //{
    //    isPlayback = true;
    //}




    void InitData()
    {
        if (isNative) {
            return;
        }
        if (isPlayback)  {
            return;
        }
        sceneInitData = new GameSceneInitData();
        sceneInitData.allUserInfo = RoomManager.Instance.GetAllUserInfoByList();
        sceneInitData.mainPlayerID = mainPlayer.uid;
        //isPlayback = false;

        EventManager.Instance.BroadCastEvent<GameSceneInitData>(GameEventType.ClientEvent_GameSceneInitData, sceneInitData);
    }



    #region NativeInit
    void NativeInit()
    {
        
        isNative = !ModuleMgr.CheckInstance();    //判断是否有开启 客户端服务

        if (isNative && !isPlayback)   //单机模式
        {
            MonoManager.Instance.AddFixedUpdateListener(OnFixedUpdate);
            allkey = new S2C_FrameOpKey();
            allkey.allOpkey = new List<C2S_FrameOpKey>();
        }
    }
    #endregion


    #region InitEnv



    void InitEnv()
    {
        mapRoot = UnityEngine.GameObject.FindGameObjectWithTag("MapRoot").transform.GetComponent<MapRoot>();

        List<ColliderConfig> cnflist = GenerateEnvCollicfgs(mapRoot.transEnvCollider);
        logocEnv = new EnvColliders
        {
            envColliCfgLst = cnflist
        };

        logocEnv.Init();
    }

    List<ColliderConfig> GenerateEnvCollicfgs(Transform tf)
    {
        List<ColliderConfig> cnflist = new List<ColliderConfig>();

        BoxCollider[] boxArr = tf.GetComponentsInChildren<BoxCollider>();
        for (int i = 0; i < boxArr.Length; i++)
        {
            Transform t1 = boxArr[i].transform;
            //后面封装吧
            var cnf = new ColliderConfig
            {
                mPos = new PEVector3(t1.position),
                mSize = new PEVector3(t1.localScale / 2),
                mType = ColliderType.Box,
                mAxis = new PEVector3[3] {
                    new PEVector3(tf.right) ,
                    new PEVector3(tf.up) ,
                    new PEVector3(tf.forward)
                }
            };

            cnflist.Add(cnf);
        }

        CapsuleCollider[] cyArr = tf.GetComponentsInChildren<CapsuleCollider>();
        for (int i = 0; i < cyArr.Length; i++)
        {
            Transform t1 = cyArr[i].transform;
            //后面封装吧
            var cnf = new ColliderConfig
            {
                mPos = new PEVector3(t1.position),
                mRadius = new PEInt(t1.localScale.x / 2),
                mType = ColliderType.Cylinder,
            };

            cnflist.Add(cnf);
        }

        return cnflist;
    }

    #endregion

    #region InitPlayer

    void InitPlayer()
    {
        var gop = Resources.Load<GameObject>("ResChars/Arthur");
        if (isNative && !isPlayback)
        {
            var go = GameObject.Instantiate<GameObject>(gop);
            TestPlayer1 player1 = go.GetComponent<TestPlayer1>();
            player1.Init();
            //player1.logicUnit.playerid = mainPlayer.uid;
            //player1.logicUnit.testuid = mainPlayer.uid;
            players.Add(player1.logicUnit);
            playerUnits.Add(player1.logicUnit.playerid, player1.logicUnit);
            CameraFollow.Instance.CameraFollowOb = player1.transform;
        }
        else
        {
            sLog.OpenMsgInfo = false;
            var allUserInfo = RoomManager.Instance.GetAllUserInfo();    //帧同步不能用这个Dic 来处理 因为字典是无序的 所以这里出问题
            int count = 0;
            List<CMD_UserRoomInfo> info = sceneInitData.allUserInfo;

            foreach (var user in info)
            {
                count++;
                var go = GameObject.Instantiate<GameObject>(gop);
                TestPlayer1 player1 = go.GetComponent<TestPlayer1>();
                player1.Init();
 
                player1.logicUnit.playerid = user.useinfo.uid;
                player1.logicUnit.testuid = user.useinfo.uid.ToString();
                sLog.Error("-----InitPlayer---" + player1.logicUnit.playerid);

                players.Add(player1.logicUnit);
                playerUnits.Add(player1.logicUnit.playerid, player1.logicUnit);

                player1.SetPos(new Vector3(count - 1, 0, count - 1));   //暂时先这样写
                if (sceneInitData.mainPlayerID == player1.logicUnit.playerid) {
                    CameraFollow.Instance.CameraFollowOb = player1.transform;
                }
            }

            players.Sort((a1, a2) =>
            {
                return a1.playerid.CompareTo(a2.playerid);
            });
        }

      
      
    }

    #endregion


}
