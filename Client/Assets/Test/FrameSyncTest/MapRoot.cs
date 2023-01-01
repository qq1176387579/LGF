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


    private void Start()
    {
        GameSceneMgr.Instance.Init();
    }
}
