/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/24 2:36:36
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;

#pragma warning disable IDE0051 // 删除未使用的私有成员
using UnityEditor;
using System.IO;
using System;
using Object = UnityEngine.Object;
using System.Linq;

namespace LGF.Editor
{
    static class ScriptableObjectExtesion
    {
        static List<ScriptableObject> list = new List<ScriptableObject>();
        [MenuItem("Assets/Create/ScriptableObject Asset", priority = 1)]
        static void Create()
        {
            list.Clear();
            foreach (var item in Selection.objects)
            {
                list.Add(CreateAsset(item));
            }
            Selection.objects = list.ToArray();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
        }

        [MenuItem("Assets/Create/ScriptableObject Asset", true)]
        static bool Validate()
        {
            Func<Object, bool> predicate = (obj) =>
            {
                if (obj is MonoScript)
                {
                    var ms = obj as MonoScript;
                    var type = ms.GetClass();
                    if (null != type)
                    {
                        var valid = type.IsSubclassOf(typeof(ScriptableObject));
                        Debug.Assert(valid, $"创建失败：选择了错误的类型 → {type}");
                        return !valid;
                    }
                    sLog.Error($"创建失败: 请避免选择静态类型和没写继承关系的patial类型 → {ms.name}");
                }
                return true;
            };
            return Selection.objects.Length > 0 && !Selection.objects.Any(predicate);
        }
        static ScriptableObject CreateAsset(Object ms)
        {
            var path = AssetDatabase.GetAssetPath(ms);
            path = path.Substring(0, path.LastIndexOf("/"));
            path = Path.Combine(path, "Data");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var type = (ms as MonoScript).GetClass();
            path = AssetDatabase.GenerateUniqueAssetPath($"{path}/ {type.Name}.asset");
            var asset = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}

