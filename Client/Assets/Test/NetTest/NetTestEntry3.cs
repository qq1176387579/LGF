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
   
    public string name = "";

    protected override void OnStart()
    {
        base.OnStart();

        C_ModuleMgr.Instance.Init();    //初始客户端

    }



}
