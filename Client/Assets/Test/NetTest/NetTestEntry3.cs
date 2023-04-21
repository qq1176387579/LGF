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

public class NetTestEntry3 : GameEntry2
{
   
    //public string name = "";

    protected override void OnStart()
    {
        base.OnStart();

        //如果是单机模式 需要该初始化地方
        C_ModuleMgr.Instance.Init();    //初始客户端

        AppConfig.Instance.serverInfo.GetEndPoint();


        //List<string> strings1 = new List<string>() {"1" };
        //List<string> strings2 = new List<string>() { "2"};

        //PP(strings1, strings2);
        //Debug.LogFormat("{0}  {1}", strings1[0], strings2[0]);

        //PP1(ref strings1, ref strings2);
        //Debug.LogFormat("{0}  {1}", strings1[0], strings2[0]);


        Screen.sleepTimeout = SleepTimeout.NeverSleep;  //防止息屏
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
