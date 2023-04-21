/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/15 23:28:20
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using UnityEngine;

public class TestGMSystem : SimpleMonoSingleton<TestGMSystem>
{
   
    private void Start()
    {


        DontDestroyOnLoad(this);
    }

    private Rect labelRect = new Rect(30, 30, 100, 30);
    private float _Interval = 0.5f;
    private int _FrameCount = 0;
    private float _TimeCount = 0;
    private float _FrameRate = 0;

    void Update()
    {
        _FrameCount++;
        _TimeCount += Time.unscaledDeltaTime;
        if (_TimeCount >= _Interval) {
            _FrameRate = _FrameCount / _TimeCount;
            _FrameCount = 0;
            _TimeCount -= _Interval;
        }
    }

    void OnGUI()
    {
        GUI.Label(labelRect, string.Format("FPS：{0:F1}", _FrameRate));
    }

}


