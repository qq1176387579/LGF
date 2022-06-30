using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Method
{

    /// <summary>
    /// 计算出 一个坐标 围绕center旋转angle 后的的坐标点
    /// </summary>
    /// <param name="position">点</param>
    /// <param name="center">中心点</param>
    /// <param name="axis">基于哪个旋转</param>
    /// <param name="angle">旋转角度</param>
    /// <returns></returns>
    public static Vector3 RotateRound(this Vector3 position, Vector3 center, Vector3 axis, float angle)
    {
        return center + Quaternion.AngleAxis(angle, axis) * (position - center);
    }


    /// <summary>
    /// 计算出向量 围绕center旋转angle后的 得到的坐标
    /// </summary>
    /// <param name="position"></param>
    /// <param name="center"></param>
    /// <param name="axis"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 VectorRotate(this Vector3 vector, Vector3 center, Vector3 axis, float angle)
    {
        return center + Quaternion.AngleAxis(angle, axis) * vector;
    }


    /// <summary>
    /// 算出向量旋转angle后的 得到的向量
    /// </summary>
    /// <param name="position"></param>
    /// <param name="center"></param>
    /// <param name="axis"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 VectorRotate(this Vector3 vector, Vector3 axis, float angle)
    {
        return Quaternion.AngleAxis(angle, axis) * vector;
    }



    /// <summary>
    /// 判断一个向量to和向量from是否小于90度
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static bool IsForward(this Vector3 from, Vector3 to) {
        return Vector3.Dot(from, to) > 0 ? true : false;
    }




    /// <summary>
    /// 判断在基于位置的是否在前后
    /// </summary>
    /// <param name="baseTrans"></param>
    /// <param name="dir"></param>
    /// <returns>true:前；false:后</returns>
    public static bool IsForward(Transform baseTrans, Vector3 pos) {
        return GetIsForwardValue(baseTrans, pos) >= 0 ? true : false;
    }

    /// <summary>
    /// 判断在基于位置的是否在左右
    /// </summary>
    /// <param name="baseTrans"></param>
    /// <param name="dir"></param>
    /// <returns>true:右；false:左</returns>
    public static bool IsRight(Transform baseTrans, Vector3 pos) {
        return GetIsRightValue(baseTrans, pos) >= 0 ? true : false;
    }

    // 大于0，前
    // pos 为 0 ，结果会出错 (做好是相对位置)
    public static float GetIsForwardValue(Transform baseTrans, Vector3 pos) {
        return Vector3.Dot(baseTrans.forward, pos - baseTrans.position);
    }

    // 大于0，右边
    // pos 为 0 ，结果会出错(做好是相对位置)
    public static float GetIsRightValue(Transform baseTrans, Vector3 pos) {
        return Vector3.Cross(baseTrans.forward, pos - baseTrans.position).y;
    }


    /// <summary>
    /// tf 到目标位置的夹角 
    /// </summary>
    /// <param name="tf">基于的tf</param>
    /// <param name="toPos">目标位置点</param>
    /// <returns></returns>
    public static float AngleXZ(this Transform tf, Vector3 toPos)
    {
        Vector3 from = tf.transform.forward;
        Vector3 to = toPos - tf.transform.position;
        from.y = 0;
        to.y = 0;

        return Vector3.Angle(from, to);
    }


}
