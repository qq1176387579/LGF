/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/29 20:58:49
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

namespace LGF
{
    /// <summary>
    /// 创建List<Vector3> m_Positions = ListPool<Vector3>.Get();
    /// 销毁ListPool<Vector3>.Release(m_Positions);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ListPool<T>
    {
        private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, l => l.Clear());

        public static List<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }

    }

    public static class ListPoolHelper
    {
        /// <summary>
        /// 回收到list缓冲中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Release<T>(this List<T> list)
        {
            if (list != null) ListPool<T>.Release(list);
        }
    }

}