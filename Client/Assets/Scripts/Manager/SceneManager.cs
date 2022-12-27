/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/27 16:47:20
/// 功能描述:  
****************************************************/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.DataStruct;

public class SceneManager : SingletonBase<SceneManager>
{
    protected override void OnNew()
    {
        base.OnNew();
        LGFEntry.RegisterOnUpdate(OnUpdate);

        EventManager.Instance.AddListener(GameEventType.ClientEvent_StartPlay, OnStartPlay); //用于显示
    }



    bool startPlay;
    //场景管理器  用于跳转场景
    private Action prgCB = null;
    float nextTime;
    public void AsyncLoadScene(string sceneName, Action<float> loadRate, Action loaded)
    {
        
        AsyncOperation sceneAsync = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        nextTime = 0;
        //不管GC了  调用次数不多
        startPlay = false;
        GC.Collect();   //加载场景前 GC
        sceneAsync.allowSceneActivation = false; //防止卡90%
        prgCB = () =>
        {
            float progress = sceneAsync.progress;
            Debug.Log("---------" + progress);
            if (!sceneAsync.isDone && progress < 0.9f)
            {
                loadRate?.Invoke(progress / 0.9f);
                return;
            }

            if (!startPlay)
            {
                if (loaded != null)
                {
                    loaded?.Invoke();
                    loaded = null;
                }
                sLog.Debug("加载完成 ---暂时不跳转");
                return; //不跳转
            }

            // 测试一下
            prgCB = null;
            sceneAsync.allowSceneActivation = true; //开始跳转
            sceneAsync = null;

            Debug.Log("加载完成----");
        };
    }

    void OnUpdate()
    {
        if (prgCB == null)
        {
            return;
        }

        if (Time.time > nextTime)
        {
            nextTime = Time.time + 0.3f;
            prgCB?.Invoke();
        }
        
    }



    void OnStartPlay()
    {
        startPlay = true;
        prgCB.Invoke();
    }

}
