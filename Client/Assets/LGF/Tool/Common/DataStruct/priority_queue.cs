/***************************************************
/// 作者:      刘欢
/// 创建日期:  2022/6/5 13:58:22
/// 修改日期:  
/// 功能描述:  优先队列
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using System;

namespace LGF.DataStruct
{
    /// <summary>
    /// 优先队列  默认是小堆  默认比较IComparable
    /// System.Func<T, T, bool> 表示按这个顺序排序 如 a < b --小堆,跟C++一样
    /// System.Func<T, T, int> 规则 IComparable 一样 a < b = -1
    /// 必须实现IComparable 不然会有拆箱装箱 where T : IComparable<T>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class priority_queue<T>  where T : IComparable<T>
    {
        List<T> list = new List<T>();
        System.Func<T, T, bool> _compare;   //默认是 a < b 

        public int Count { get => list.Count; }

        public priority_queue() { }
        public priority_queue(System.Func<T, T, bool> compare_) => _compare = compare_;

        public priority_queue(System.Func<T, T, int> compare2_) => _compare = (T a, T b) => { return compare2_(a, b) < 0; }; //调用的时候是 !compare


        /// <summary>
        /// 入队列
        /// </summary>
        /// <param name="x"></param>
        public void push(T x)
        {
            list.Add(x);

            int index = list.Count - 1;
            int parent = (index - 1) / 2;

            while (index > 0)
            {
                if (compare(list[index], list[parent]))  //当前节点小于他父节点
                {
                    list.Swap(index, parent);
                    index = parent;
                    parent = (index - 1) / 2;
                }
                else break;
            }
        }

        /// <summary>
        /// 出队列
        /// </summary>
        public void pop()  //出队列
        {

            int index = 0;
            int cl, cr; //cl 左节点  cr 右节点
            int tidx;

            list.Swap(list.Count - 1, 0);   //头与尾交换
            list.RemoveAt(list.Count - 1);  //出队列


            while (true)
            {
                if (index * 2 + 1 < list.Count)
                    cl = index * 2 + 1;
                else  //没有儿子
                    break;

                if (index * 2 + 2 < list.Count)
                    cr = index * 2 + 2;
                else
                    cr = -1; //标记

                //两个儿子
                if (cr != -1)
                {
                    if (compare(list[cl], list[cr]))   //比较两边大小
                        tidx = cl;
                    else tidx = cr;
                }
                else
                {
                    tidx = cl;   //一个儿子
                }

                //小于父节点 交换
                if (compare(list[tidx], list[index]))
                {
                    list.Swap(tidx, index);
                    index = tidx;  //更新父节点
                }
                else break; //父节点 小于等于2个子节点 
            }
        }


        /// <summary>
        /// 比较  在方法内是左右互换
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        bool compare(T a, T b)
        {
            if (_compare != null) return _compare(a, b);
            //不能用强转  因为强转会有装箱 拆箱问题  比如int
            return a.CompareTo(b) < 0;  

            //降序  和list一样操作  默认是小堆
            //a.CompareTo(b) =====>   a < b = -1   a == b = -1  a > b  = 1
            //int[] a = { 3, 4, -1, 7, 10, 9, 5, 2 };
            //list.Sort((a,b) => { return a < b ? -1 : 1;  });
            //-1 2 3 4 5 7 9 10
        }



        public T top()
        {
            return list.Count > 0 ? list[0] : default;
        }

        public int size()
        {
            return list.Count;
        }


        public void Log()
        {
            var strb = StringPool.GetStringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                strb.Append(" ");
                strb.Append(list[i]);
            }
            sLog.Debug(strb.ToString());
            strb.Release();
        }

    }



}
