/*************************************************
	作者: Plane
	邮箱: 1785275942@qq.com
	日期: 2021/02/09 21:22
	功能: 资源服务

    //=================*=================\\
           关注微信公众号: PlaneZhong
           关注微信服务号: qiqikertuts
               ~~获取更多教学资讯~~
    \\=================*=================//
*************************************************/

using PEMath;
using PEPhysx;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResSvc : LGF.MonoSingleton<ResSvc> {
    //public static ResSvc Instance;

    //public void InitSvc() {
    //    Instance = this;
    //    //.Log("Init ResSvc Done.");
    //}

    //private Action prgCB = null;
    //public void AsyncLoadScene(string sceneName, Action<float> loadRate, Action loaded) {
    //    AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(sceneName);

    //    prgCB = () => {
    //        float progress = sceneAsync.progress;
    //        loadRate?.Invoke(progress);
    //        if(progress == 1) {
    //            loaded?.Invoke();
    //            prgCB = null;
    //            sceneAsync = null;
    //        }
    //    };
    //}

    //private void Update() {
    //    prgCB?.Invoke();
    //}


    private Dictionary<string, AudioClip> adDic = new Dictionary<string, AudioClip>();
    public AudioClip LoadAudio(string path, bool cache = false) {
        AudioClip au = null;
        if(!adDic.TryGetValue(path, out au)) {
            au = Resources.Load<AudioClip>(path);
            if(cache) {
                adDic.Add(path, au);
            }
        }
        return au;
    }

    private Dictionary<string, Sprite> spDic = new Dictionary<string, Sprite>();
    public Sprite LoadSprite(string path, bool cache = false) {
        Sprite sp = null;
        if(!spDic.TryGetValue(path, out sp)) {
            sp = Resources.Load<Sprite>(path);
            if(cache) {
                spDic.Add(path, sp);
            }
        }
        return sp;
    }

    public UnitCfg GetUnitConfigByID(int unitID) {
        switch(unitID) {
            case 101:
                return new UnitCfg {
                    unitID = 101,
                    unitName = "亚瑟",
                    resName = "arthur",

                    hp = 6500,
                    def = 0,
                    moveSpeed = 5,
                    colliCfg = new ColliderConfig {
                        mType = ColliderType.Cylinder,
                        mRadius = (PEInt)0.5f
                    }
                };
            case 102:
                return new UnitCfg {
                    unitID = 102,
                    unitName = "后羿",
                    resName = "houyi",

                    hp = 3500,
                    def = 10,
                    moveSpeed = 5,
                    colliCfg = new ColliderConfig {
                        mType = ColliderType.Cylinder,
                        mRadius = (PEInt)0.5f
                    }
                };
        }
        return null;
    }

    public MapCfg GetMapConfigByID(int mapID) {
        switch(mapID) {
            case 101:
                return new MapCfg {
                    mapID = 101,

                    bornDelay = 15000,
                    bornInterval = 2000,
                    waveInterval = 50000
                };
            case 102:
                return new MapCfg {
                    mapID = 102,

                    bornDelay = 15000,
                    bornInterval = 2000,
                    waveInterval = 50000
                };
            default:
                return null;
        }
    }
}
