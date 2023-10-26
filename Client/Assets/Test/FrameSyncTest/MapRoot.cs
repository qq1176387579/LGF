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


    [Header("�Ƿ��ǻط�")]
    public bool IsPlayback;
    [Header("�ط�·��")]
    public string path;




    private void Start()
    {
        //if (ModuleMgr.CheckInstance()) {

        //}
        //FrameDataMgr.GetSingleton();
        if (!ModuleMgr.CheckInstance() && IsPlayback) {
            GameSceneMgr.GetSingleton();    //������ʼ������
            FrameDataMgr.Instance.StartPath(path);
            Application.targetFrameRate = 60;   //��֡ ��ʱд����

        }
        else {
            GameSceneMgr.Instance.Init();
        }
    }
}
