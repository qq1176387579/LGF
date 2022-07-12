/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/12 21:24:23
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

/// <summary>
/// unity Module管理
/// </summary>
public class ModuleMgr : SingletonBase<ModuleMgr>
{
    /// <summary>
    /// unity 都调用  控制台只调用自己的  后看情况要不要加宏定义
    /// </summary>
    public override void Init()
    {
        C_ModuleMgr.Instance.Init();    

        S_ModuleMgr.Instance.Init();    
    }


}
