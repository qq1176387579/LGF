/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/27 17:23:22
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;
using System.Net;
using System;

public class NetTestEntry3 : GameEntry2
{

    //public string name = "";
    static DateTime _initialDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    protected override void OnStart()
    {
        

        base.OnStart();
        Application.targetFrameRate = 60;   //锁帧
        //如果是单机模式 需要该初始化地方
        ModuleMgr.Instance.Init();    //初始客户端


        //List<string> strings1 = new List<string>() {"1" };
        //List<string> strings2 = new List<string>() { "2"};

        //PP(strings1, strings2);
        //Debug.LogFormat("{0}  {1}", strings1[0], strings2[0]);

        //PP1(ref strings1, ref strings2);
        //Debug.LogFormat("{0}  {1}", strings1[0], strings2[0]);


        var ts = DateTime.UtcNow.AddHours(8);
        this.Debug($"ts : {ts}   DayOfWeek<{(int)ts.DayOfWeek}>  DayOfYear<{ts.DayOfYear}>" );

        Screen.sleepTimeout = SleepTimeout.NeverSleep;  //防止息屏

        
        string BasePath = Application.persistentDataPath + "/FrameData/";

        FrameDataMgr.Instance.Init();

    }

  

    //void PP<T>(List<T> t ,List<T> t1)
    //{
    //    var tmp = t;
    //    t = t1;
    //    t1 = tmp;
    //}

    //void PP1<T>(ref List<T> t, ref List<T> t1)
    //{
    //    var tmp = t;
    //    t = t1;
    //    t1 = tmp;
    //}


}
