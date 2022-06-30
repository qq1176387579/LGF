/***************************************************
/// 作者:      刘欢
/// 创建日期:  2022/6/5 14:01:36
/// 修改日期:  
/// 功能描述:  优先队列 案例
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;
using System;
using LGF.DataStruct;

namespace LGF.Example
{
    public class PriorityQueueExample : MonoBehaviour
    {

        private void Start()
        {
            //priority_queue<int> s = new priority_queue<int>(PP);
            //priority_queue<int> s = new priority_queue<int>((x, y) => { return PP(x, y) ? -1 : 1; });
            priority_queue<int> s = new priority_queue<int>();

            int[] a = { 3, 4, -1, 7, 10, 9, 5, 2 };
            for (int i = 0; i < a.Length; i++)
            {
                s.push(a[i]);
                s.Debug();

                if (i % 3 == 0 && i != 0)
                {
                    s.pop();
                    s.Debug();
                }
            }

            List<int> list = new List<int>();
            //Debug.LogError(int.IComparer())
            list.AddRange(a);
            list.Sort((a, b) => { return PP(a, b) ? -1 : 1; });
            string str = "";
            for (int i = 0; i < list.Count; i++)
            {
                str += " " + list[i];
            }

            Debug.LogError(str);


            priority_queue<Test2> test2sss = new priority_queue<Test2>();

            test2sss.push(new Test2 { a = 3 });
            test2sss.push(new Test2 { a = 1 });
            test2sss.push(new Test2 { a = 2 });

            test2sss.Debug();
            test2sss.pop();
            test2sss.Debug();
            //new priority_queue<int>((a,b) => { return a.CompareTo(b); });
            //priority_queue<int> priority_Queue = new priority_queue<int>();
        }

        

        static bool PP(int x, int y)
        {
            return x < y;
        }

        public struct Test2 : System.IComparable<Test2>
        {
            public int a;

            public override string ToString()
            {
                return a.ToString();
            }

            int IComparable<Test2>.CompareTo(Test2 other)
            {
                return a.CompareTo(other.a); ;
            }
        }
    }

}

