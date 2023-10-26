using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoot : MonoBehaviour {
    public Transform transCameraRoot;
    public Transform transEnvCollider;

    public Transform blueTower;
    public Transform redTower;
    public Transform blueCrystal;
    public Transform redCrystal;


    [Header("是否是回放")]
    public bool IsPlayback;
    [Header("回放路径")]
    public string path;




    private void Start()
    {
        //if (ModuleMgr.CheckInstance()) {

        //}
        //FrameDataMgr.GetSingleton();
        if (!ModuleMgr.CheckInstance() && IsPlayback) {
            GameSceneMgr.GetSingleton();    //不做初始化处理
            FrameDataMgr.Instance.StartPath(path);
            Application.targetFrameRate = 60;   //锁帧 暂时写在这

        }
        else {
            GameSceneMgr.Instance.Init();
        }
    }
}
