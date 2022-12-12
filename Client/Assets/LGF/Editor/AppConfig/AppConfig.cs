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
using UnityEditor;

namespace LGF.Editor
{
    public class AppConfig : SingletonBase<AppConfig>
    {
        public  EditortConfigRoot data;
        public static EditortConfigRoot Data => Instance.data;
        protected override void OnNew()
        {
            data = AssetDatabase.LoadAssetAtPath<EditortConfigRoot>("Assets/LGF/Editor/AppConfig/ConfigScirpts/Data/EditortConfigRoot.asset");
            if (data == null)
            {
                Debug.Log("------data == null---");
            }
         
        }
    }
}

