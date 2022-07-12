/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/6 21:05:07
/// 修改日期:  
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;
using LGF.DataStruct;

namespace LGF.Example
{
    public class TestQuadTreeFind : MonoBehaviour
    {
        List<Deque2<QuadTree.IPos>> list = new List<Deque2<QuadTree.IPos>>();
        public List<QuadTree.IPos> list2 = new List<QuadTree.IPos>();
        public Vector2 size = new Vector2(10, 10);


        QuadTree tree;
        int count1 = 0;
        public int count1_1 = 0;
        int count2 = 0;
        // Update is called once per frame
        Rect rect = new Rect();
        void Update()
        {
            if (tree == null)
                tree = QuadTreeExample.Instance.tree;

            for (int i = 0; i < list2.Count; i++)
            {
                (list2[i] as TestQuadTreePos).ResetColor();
            }
            list2.Clear();

            //查找性能对比
            UnityEngine.Profiling.Profiler.BeginSample("-----TestQuadTreeFind--1---------");
            rect.size = size;
            rect.center = new Vector2(transform.position.x, transform.position.z);
            tree.FindRect(rect, list);

            count1 = 0;
            count1_1 = list.Count;
            TestQuadTreeFind t = this;

            for (int i = 0; i < list.Count; i++)
            {
                list[i].foreach_root((a, _this) =>
                {
                    _this.list2.Add(a);
                    if (_this.CanInsert(ref _this.rect, a))
                        _this.count1++;
                }, t);
            };



            UnityEngine.Profiling.Profiler.EndSample();
            list.Clear();

            UnityEngine.Profiling.Profiler.BeginSample("----TestQuadTreeFind---2---------");
            var listt = QuadTreeExample.Instance.list;
            count2 = 0;
            for (int i = 0; i < listt.Count; i++)
                if (CanInsert(ref rect, listt[i]))
                    count2++;
            UnityEngine.Profiling.Profiler.EndSample();

            for (int i = 0; i < list2.Count; i++)
                (list2[i] as TestQuadTreePos).SetColor();

            //sLog.Debug($" {count1}  {count1_1} |  {count2}");
            if (count1 != count2)
            {
                transform.debugError($"----error------{count1}  {count1_1} |  {count2}---");
            }

        }

        public bool CanInsert(ref Rect rect, QuadTree.IPos pos)
        {
            return rect.xMin < pos.x && rect.yMin < pos.y && rect.xMax > pos.x && rect.yMax > pos.y;
        }




        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                rect.size = size;
                rect.center = new Vector2(transform.position.x, transform.position.z);
            }

            Gizmos.color = Color.red;

            Vector3 p1 = new Vector3(rect.x, 0.1f, rect.y);
            Vector3 p2 = new Vector3(rect.xMax, 0.1f, rect.y);
            Vector3 p3 = new Vector3(rect.xMax, 0.1f, rect.yMax);
            Vector3 p4 = new Vector3(rect.x, 0.1f, rect.yMax);

            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);
        }

    }

}



