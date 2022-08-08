/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/12 21:41:53
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

public class GameEntry2 : GameEntry
{
    protected override void OnStart()
    {
        base.OnStart();

        //ModuleMgr.Instance.Init();
    }


    protected override void OnDisable()
    {
        base.OnDisable();

        if (C_ModuleMgr.CheckInstance()) C_ModuleMgr.Instance.Close();
        if (S_ModuleMgr.CheckInstance()) S_ModuleMgr.Instance.Close();
    }

}
