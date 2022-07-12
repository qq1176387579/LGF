/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/12 22:00:29
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

public class C_ModuleMgr : SingletonBase<C_ModuleMgr>
{
    public override void Init()
    {
        C_ChatMgr.Instance.Init();

    }

}
