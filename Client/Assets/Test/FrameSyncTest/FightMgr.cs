/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/14 22:37:50
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;
using PEMath;
using PEPhysx;


public class FightMgr : MonoSingleton<FightMgr> ,ILogic 
{
    MapRoot mapRoot;
    EnvColliders logocEnv;

    List<PlayerUnit> players = new List<PlayerUnit>();
    Dictionary<uint, PlayerUnit> playerUnits = new Dictionary<uint, PlayerUnit>();
    public uint _keyid = 0;
    public uint Keyid { get => ++_keyid; }


    // Start is called before the first frame update
    void Start()
    {
        

        Init();
    }

    public void Init()
    {
        InitEnv();
        InitPlayer();

        LogicInit();

       // InitGame(); //初始化游戏
    }

    public void InitGame()
    {
        //var go = Resources.Load<GameObject>("ResChars/Arthur");
        //go = GameObject.Instantiate<GameObject>(go);
        //go.GetComponent<HeroView>().Init(playerUnits[0]);
    }


    void InitEnv()
    {
        mapRoot = GameObject.FindGameObjectWithTag("MapRoot").transform.GetComponent<MapRoot>();

        List<ColliderConfig> cnflist = GenerateEnvCollicfgs(mapRoot.transEnvCollider);
        logocEnv = new EnvColliders
        {
            envColliCfgLst = cnflist
        };

        logocEnv.Init();
    }

    public List<PEColliderBase> GetEnvColliders()
    {
        return logocEnv.GetEnvColliders();
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
            var cnf = new ColliderConfig {
                mPos = new PEVector3(t1.position),
                mRadius = new PEInt(t1.localScale.x / 2),
                mType = ColliderType.Cylinder,
            };

            cnflist.Add(cnf);
        }

        return cnflist;
    }


    void InitPlayer()
    {
  
        HeroData hd = new HeroData
        {
            heroID = 101,
            posIndex = 1,
            userName = "yonghuming",
            unitCfg = ResSvc.Instance.GetUnitConfigByID(101)
        };

        MapCfg mapCfg = ResSvc.Instance.GetMapConfigByID(101);
        //hd.bornPos = new PEVector3(10,0,0);//mapCfg.blueBorn;
        hd.bornPos = new PEVector3(0, 0, 0);//mapCfg.blueBorn;
        PlayerUnit player = new PlayerUnit(hd);
        players.Add(player);
        players.Sort((a1,a2) =>
        {
            return a1.playerid.CompareTo(a2);
        });

        
        playerUnits.Add(player.playerid, player);

        var go = Resources.Load<GameObject>("ResChars/Arthur");
        go = GameObject.Instantiate<GameObject>(go);
        go.GetComponent<HeroView>().Init(player);
    }

    public void LogicInit()
    {
        allkey.allOpkey = new List<C2S_FrameOpKey>();
        for (int i = 0; i < players.Count; i++)
        {
            players[i].LogicInit();
        }
    }
    S2C_FrameOpKey allkey = new S2C_FrameOpKey();

    private void FixedUpdate()
    {
        LogicTick();
    }


    long frameID;
    public void LogicTick()
    {
        frameID++;
        NtfOpKey(allkey);

        for (int i = 0; i < players.Count; i++)
        {
            players[i].LogicTick();
        }
        
    }

    public void LogicUnInit()
    {
       
    }


    public void NtfOpKey(S2C_FrameOpKey key)
    {
        InputKey(key.allOpkey);
        key.allOpkey.Clear();

    }

    public void InputKey(List<C2S_FrameOpKey> opkey)
    {
        for (int i = 0; i < opkey.Count; i++)
        {
            playerUnits[opkey[i].uid].InputKey(opkey[i]);
        }
    }

    #region API Func



    /// <summary>
    /// 发送移动帧操作到服务器
    /// </summary>
    /// <param name="logicDir"></param>
    /// <returns></returns>
    public bool SendMoveKey(PEVector3 logicDir)
    {
        Debug.Log("--logicDir-"+ logicDir);
        C2S_FrameOpKey key = C2S_FrameOpKey.Get();

        key.keytype = Frame_KeyType.Move;
        key.moveKey = Frame_MoveKey.Get();
        key.moveKey.x = logicDir.x.ScaledValue;
        key.moveKey.z = logicDir.z.ScaledValue;
        key.moveKey.Keyid = Keyid;


        //NetSvc.Instance.SendMsg(msg);
        allkey.allOpkey.Add(key);
        return true;
    }
    #endregion

}
