/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/24 3:44:51
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;

namespace LGF.Config
{
    public class ScriptableObjectConfigRoot : ScriptableObject
    {
        public AppInfoConfig AppInfo;
        public GeneratedConfig Generated;

    }
}

