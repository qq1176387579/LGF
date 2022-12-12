/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/3 9:37:35
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;

namespace LGF.Config
{
    public class GeneratedConfig : ScriptableObject
    {
        /// <summary>
        /// 流序列化地址
        /// </summary>
        public string SteamSerializablePath = "Assets/Scripts/Generated/SteamSerializable/";  //流序列化地址

        [Header("拷贝Asmref 引用地址")]
        public string SteamSerializableCpyeAsmrefPath = "Assets/Plugins/PluginsByLGF.asmref";  //流序列化地址


        //unity程序集地址
        public List<TextAsset> unityAsmdef = new List<TextAsset>();

    }
}


