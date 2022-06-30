/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/24 3:55:23
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;
using LGF.Config;

namespace LGF
{
    public class AppConfig : SingletonBase<AppConfig>
    {
        ResScriptableObjectConfigRoot m_ResRoot;
        public static ScriptableObjectConfigRoot Data => _Instance.m_ResRoot.root;
        protected override void OnNew()
        {
            m_ResRoot = Resources.Load<ResScriptableObjectConfigRoot>("ResScriptableObjectConfigRoot");
        }

    }
}

