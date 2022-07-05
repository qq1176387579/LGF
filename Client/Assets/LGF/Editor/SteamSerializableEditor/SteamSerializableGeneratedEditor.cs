/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/3 18:02:14
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using UnityEditor;
using LGF.Serializable;

namespace LGF.Editor
{
    public class SteamSerializableGeneratedEditor : EditorWindow
    {
        [MenuItem("数据工具/SteamSerializable/Generated")]
        public static void Generated()
        {
            LGF.Serializable.SerializerHelper.GeneratedAll();
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("Generated 生成完成");

        }



    }

}
