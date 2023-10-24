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

    bool isConnectServer = false;
    private void Start()
    {

        if (ModuleMgr.GetSession().IsDisconnection()) {
            //text.text = "连接不上服务器";
        }
        else {
            //text.text = "连接中";
        }

        DontDestroyOnLoad(this);
    }

    private Rect labelRect = new Rect(30, 30, 100, 30);
    private Rect labelRect2 = new Rect(30, 70, 100, 30);
    private Rect labelRect3 = new Rect(30, 100, 100, 30);
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

        if (ModuleMgr.GetSession() == null) {
            isConnectServer = false;
        }
        else {
            isConnectServer = !ModuleMgr.GetSession().IsDisconnection();
        }
     
    }

    void OnGUI()
    {
        GUI.Label(labelRect, string.Format("FPS：{0:F1}", _FrameRate));
        GUI.Label(labelRect2, isConnectServer ? "服务器连接中" : "连接不上服务器");
        if (Reporter.Instance != null&& !Reporter.Instance.show) {
            if (GUI.Button(labelRect3, "显示日志")) {
                //Debug.Log("---fff--");
                Reporter.Instance.DoShow();
            }
        }
       
      
    }

}


