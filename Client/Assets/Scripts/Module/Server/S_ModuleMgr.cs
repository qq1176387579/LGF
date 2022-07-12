/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/12 22:01:51
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

public class S_ModuleMgr : SingletonBase<S_ModuleMgr>
{
    public override void Init()
    {
        S_ChatMgr.Instance.Init();
    }

}
