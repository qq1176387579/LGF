/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/29 20:14:08
/// 功能描述:  
****************************************************/
using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using UnityEngine;

namespace LGF.Util
{
    public static partial class Common
    {

        public static void DontDestroyOnLoad(UnityEngine.Object obj)
        {
            UnityEngine.Object.DontDestroyOnLoad(obj);
        }

    }
}
