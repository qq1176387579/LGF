/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/30 18:22:00
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;

#if UNITY_EDITOR
[System.Serializable]
#endif
public struct ViewUnitData
{
    //Pos
    [Header("同步位置")]
    public bool IsSyncPos;
    [Header("预测点")]
    public bool PredictPos; //预测点
    [Header("预测最大数量")]
    public int PredictMaxCount; //预测最大数量
    [Header("平滑")]
    public bool SmoothPos;      //平滑
    [Header("平滑加速")]
    public float viewPosAccer;  //平滑加速

    //Dir
    [Header("是否同步方向")]
    public bool IsSyncDir;
    [Header("平滑方向")]
    public bool SmoothDir;

    [Header("视图方向加速值")]
    public float viewDirAccer;      //视图方向加速值   
    [Header("角度赔率  角度相差的值乘以这个倍率 ")]
    public float AngleMultiplier;   //角度赔率  角度相差的值乘以这个倍率 
}

/// <summary>
/// 配置暂时  
/// </summary>
public class ViewUnitConfig : ScriptableObject
{
    public ViewUnitData data;

}
