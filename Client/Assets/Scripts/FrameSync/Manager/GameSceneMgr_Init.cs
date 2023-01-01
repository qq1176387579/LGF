/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/31 0:56:40
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using PEMath;
using PEMath;
using PEPhysx;
using UnityEngine;

public partial class GameSceneMgr
{

    public override void Init()
    {
        if (!EventCenter.CheckInstance()) EventCenter.Instance.Init();  //TestPlayer1 依赖这个
        
        NativeInit();

        InitEnv();
        InitPlayer();

        LogicInit();
    }


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
        var go = Resources.Load<GameObject>("ResChars/Arthur");
        if (!isNative)
        {
            go = GameObject.Instantiate<GameObject>(go);
            TestPlayer1 player1 = go.GetComponent<TestPlayer1>();
            player1.Init();
            player1.logicUnit.playerid = mainPlayer.uid;
            players.Add(player1.logicUnit);
            playerUnits.Add(player1.logicUnit.playerid, player1.logicUnit);

        }
        else
        {
            var allUserInfo = C_RoomManager.Instance.GetAllUserInfo();
            int count = 0;
            foreach (var user in allUserInfo)
            {
                count++;
                go = GameObject.Instantiate<GameObject>(go);
                TestPlayer1 player1 = go.GetComponent<TestPlayer1>();
                player1.Init();

                if (!isNative)
                    player1.logicUnit.playerid = user.Value.useinfo.uid;

                players.Add(player1.logicUnit);
                playerUnits.Add(player1.logicUnit.playerid, player1.logicUnit);

                player1.SetPos(new Vector3(count - 1, 0, count - 1));   //暂时先这样写
            }

            players.Sort((a1, a2) =>
            {
                return a1.playerid.CompareTo(a2);
            });
        }

      
      
    }

    #endregion


}
