/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/24 2:18:03
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject2", order = 1)]
public class SpawnManagerScriptableObject2 : ScriptableObject
{
    public string str;
    public SpawnManagerScriptableObject data;

    public ScriptableObject GetInfo()
    {
        return data;
    }
}