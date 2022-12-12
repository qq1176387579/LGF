/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/24 3:52:46
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;

namespace LGF.Config
{
    public class AppInfoConfig : ScriptableObject
    {
        [Header("安卓打包包名")]
        public string PackName;
    }


}
