/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/29 20:45:53
/// 功能描述:  
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Util;

namespace LGF
{
    public interface IWeight
    {
        int Weight { get; }
    }
}

namespace LGF
{

    public static class ArrayUtils 
    {
        #region 随机

        public static bool RandomProResult(this int pro) => Common.RandomProResult(pro);

        /// <summary>
        /// 随机抽取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T RandomDrawing<T>(this T[] list, int count = 0) where T : class, IWeight
        {
            if (list == null || list.Length == 0) return null;

            int weight = 0;
            int _count = count > 0 && count <= list.Length ? count : list.Length;

            for (int i = 0; i < _count; i++) weight += list[i].Weight;

            weight = Common.Random(weight);    //0 ~ weight - 1

            int tmp = 0;
            for (int i = 0; i < _count; i++)
            {
                tmp += list[i].Weight;          // 1 ~ weight
                if (tmp > weight)
                {
                    //UnityEngine.Debug.LogError($"weight:{weight} tmp :{tmp}");
                    return list[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 随机抽取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T RandomDrawing<T>(this List<T> list) where T : class, IWeight
        {
            if (list == null || list.Count == 0) return null;

            int weight = 0;
            for (int i = 0; i < list.Count; i++) weight += list[i].Weight;

            weight = Common.Random(weight);    //0 ~ weight - 1
            int tmp = 0;
            for (int i = 0; i < list.Count; i++)
            {
                tmp += list[i].Weight;          // 1 ~ weight
                if (tmp > weight)
                {
                    return list[i];
                }
            }
            return null;
        }

        public static T RandomNext<T>(this List<T> list)
        {
            if (list.Count == 0) return default(T);
            return list[Common.Random(list.Count)];
        }


        public static T RandomNext<T>(this T[] arrs)
        {
            if (arrs.Length == 0) return default(T);
            return arrs[Common.Random(arrs.Length)];
        }

        #endregion

        #region 数组相关操作

        public static void Swap<T>(this T[] list, int idx1, int idx2)
        {
            if (idx1 < 0 || idx2 < 0 || idx1 >= list.Length || idx2 >= list.Length) return;
            T tmp = list[idx1];
            list[idx1] = list[idx2];
            list[idx2] = tmp;
        }

        public static void Swap<T>(this List<T> list, int idx1, int idx2)
        {
            if (idx1 < 0 || idx2 < 0 || idx1 >= list.Count || idx2 >= list.Count) return;
            T tmp = list[idx1];
            list[idx1] = list[idx2];
            list[idx2] = tmp;
        }

        public static void ClearReleaseMember<T>(this List<T> list) where T : Poolable<T>, new() 
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                list[i].Release();
                list.RemoveAt(i);
            }
        }


        /// <summary>
        /// 清理回收成员
        /// </summary>
        public static void ClearReleaseMember<T2,T>(this Dictionary<T2,T> valuePairs) where T : Poolable<T>, new() where T2 : struct
        {
            foreach (var item in valuePairs)
            {
                item.Value.Release();
            }
            valuePairs.Clear();
        }


        /// <summary>
        /// 回收到list缓冲中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void ReleaseAll<T>(this List<T> list) where T : Poolable<T>, new()
        {
            if (list == null) return;

            list.ForEach((info) =>
            {
                info.Release();
            });
            ListPool<T>.Release(list);
        }



        public static T GetByID<T>(this T[] arr, int id)
        {
            if (arr.CheckeID(id))
            {
                return arr[id];
            }
            return default(T);
        }

        public static T GetByID<T>(this List<T> arr, int id)
        {
            if (arr.CheckeID(id))
            {
                return arr[id];
            }
            return default(T);
        }



        public static T1 GetByID<T, T1>(this Dictionary<T, T1> dic, T id)
        {
            if (dic.ContainsKey(id))
            {
                return dic[id];
            }
            return default(T1);
        }


        public static bool CheckeID<T>(this List<T> arr, int id)
        {
            if (arr == null || arr.Count <= id || id < 0) return false;
            return true;
        }

        public static bool CheckeID<T>(this T[] arr, int id)
        {
            if (arr == null || arr.Length <= id || id < 0) return false;
            return true;
        }

        public static bool CheckeID<T, T1>(this Dictionary<T, T1> dic, T id)
        {
            if (dic == null || !dic.ContainsKey(id)) return false;
            return true;
        }


        public static bool HasData<T>(this List<T> arr)
        {
            if (arr == null || arr.Count == 0) return false;
            return true;
        }

        public static void CopyTo<T>(this List<T> arr, List<T> list)
        {
            if (arr == null)
            {
                if (list != null)
                    list.Clear();
                return;
            }

            list.Clear();
            for (int i = 0; i < arr.Count; i++)
                list.Add(arr[i]);
        }

        public static void ForEach<T>(this T[] list, System.Action<T> func)
        {
            if (list == null)
                return;

            //sLog.Error("-----12---");
            //Array.ForEach(list, func);
            for (int i = 0; i < list.Length; i++)
            {
                func.Invoke(list[i]);
            }
        }


        public static T FirstFunc<T>(this List<T> list, Func<T, bool> func)
        {
            if (list == null)
                return default;

            for (int i = 0; i < list.Count; i++)
            {
                if (func(list[i]))
                    return list[i];
            }
            return default;
        }


        public static T FirstFunc<T,T2>(this List<T> list, Func<T,T2, bool> func,T2 t2)
        {
            if (list == null)
                return default;

            for (int i = 0; i < list.Count; i++)
            {
                if (func(list[i], t2))
                    return list[i];
            }

            return default;
        }

        public static T FirstFunc<T, T2, T3>(this List<T> list, Func<T, T2, T3, bool> func, T2 t2, T3 t3)
        {
            if (list == null)
                return default;

            for (int i = 0; i < list.Count; i++)
            {
                if (func(list[i], t2, t3))
                    return list[i];
            }

            return default;
        }



        public static void RemoveFunc<T>(this List<T> list, Func<T, bool> func)
        {
            if (list == null)
                return;

            // 找出第一个空元素 O(n)
            int count = list.Count;
            for (int i = 0; i < count; i++)
                if (func.Invoke(list[i]))
                {
                    // 记录当前位置
                    int newCount = i++;

                    // 对每个非空元素，复制至当前位置 O(n)
                    for (; i < count; i++)
                        if (!func.Invoke(list[i]))
                            list[newCount++] = list[i];

                    // 移除多余的元素 O(n)
                    list.RemoveRange(newCount, count - newCount);
                    break;
                }
        }


        public static void RemoveFunc<T>(this List<T> list, Func<T, bool> func, Action<T> OnInvoke)
        {
            if (list == null)
                return;

            // 找出第一个空元素 O(n)
            int count = list.Count;
            for (int i = 0; i < count; i++)
                if (func.Invoke(list[i]))
                {
                    OnInvoke.Invoke(list[i]);
                    // 记录当前位置
                    int newCount = i++;

                    // 对每个非空元素，复制至当前位置 O(n)
                    for (; i < count; i++)
                        if (!func.Invoke(list[i]))
                        {
                            list[newCount++] = list[i];
                        }
                        else
                        {
                            OnInvoke.Invoke(list[i]);
                        }

                    // 移除多余的元素 O(n)
                    list.RemoveRange(newCount, count - newCount);
                    break;
                }
        }

        /// <summary>
        /// 取出最上面的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Pop<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
                return default;

            T val = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return val;
        }


        public static T Top<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
                return default;

            T val = list[list.Count - 1];

            return val;
        }

        /// <summary>
        /// 插入到最前面
        /// </summary>
        public static void Enqueue<T>(this List<T> list, T val)
        {
            if (list == null)
                return;

            list.Add(val);
            for (int i = 0; i < list.Count - 1; i++)
            {
                list[i + 1] = list[i];
            }

            list[0] = val;
        }

        #endregion
    }

}