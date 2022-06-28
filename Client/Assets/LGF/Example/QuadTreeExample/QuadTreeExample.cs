/***************************************************
/// 作者:      tttt
/// 创建日期:  2022/6/5 10:42:38
/// 修改日期:  
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LGF;
using LGF.Log;

namespace LGF.Example
{
    public class QuadTreeExample : SimpleMonoSingleton<QuadTreeExample>
    {
        [System.NonSerialized]
        public List<TestQuadTreePos> list = new List<TestQuadTreePos>();

        public List<Vector3> speed = new List<Vector3>();   //速度

        public TestQuadTreePos treePos;

        public int MaxBodies = 1000;

        [Header("QuadTree Settings")]
        public Rect WorldSize = new Rect(-500, -500, 1000, 1000);
        public int BodiesPerNode = 4;
        public int MaxSplits = 10;

        public QuadTree tree;


        private void Start()
        {

            UnityEngine.Profiling.Profiler.BeginSample("-------tree Init-------");
            tree = new QuadTree(WorldSize, MaxSplits, BodiesPerNode);
            UnityEngine.Profiling.Profiler.EndSample();

            for (int i = 0; i < MaxBodies; i++)
            {
                //var body = GameObject.Instantiate<TestQuadTreePos>(treePos, transform);
                UnityEngine.Profiling.Profiler.BeginSample("-------1-------");
                var body = GameObject.Instantiate<TestQuadTreePos>(treePos, transform);
                float x = UnityEngine.Random.Range(WorldSize.xMin, WorldSize.xMax);
                float y = UnityEngine.Random.Range(WorldSize.yMin, WorldSize.yMax);
                body.transform.position = new Vector3(x, 0, y);
                tree.Insert(body);
                UnityEngine.Profiling.Profiler.EndSample();

                speed.Add(GetNewSpeed());
                list.Add(body);
            }

        }

        public Vector3 GetNewSpeed()
        {
            float t1 = UnityEngine.Random.Range(-1f, 1);
            float t3 = UnityEngine.Random.Range(-1f, 1);
            return new Vector3(t1, 0, t3).normalized * 3f;
        }


        private void Update()
        {


            //----------------------分界线----------------------------------

            //UnityEngine.Profiling.Profiler.BeginSample("-------OnMove--speed-----");
            //for (int i = list.Count - 1; i >= 0; i--)
            //{
            //    if (list[i].node == null) continue;

            //    list[i].transform.position += speed[i] * Time.deltaTime;    //list[i].transform.position += speed[i] * Time.deltaTime;  赋值会有延迟 操蛋了
            //    //UnityEngine.Profiling.Profiler.BeginSample("-------OnMove-------");
            //    tree.OnMove(list[i]); //放在这里没有问题
            //    //UnityEngine.Profiling.Profiler.EndSample();
            //}
            //UnityEngine.Profiling.Profiler.EndSample();


            UnityEngine.Profiling.Profiler.BeginSample("-------OnMove--1-----");
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].node == null) continue;
                list[i].transform.position += speed[i] * Time.deltaTime;    //list[i].transform.position += speed[i] * Time.deltaTime;  赋值会有延迟 操蛋了
            }
            UnityEngine.Profiling.Profiler.EndSample();

            //维护的代价比想想中低很多
            UnityEngine.Profiling.Profiler.BeginSample("-------OnMove---2----");
            tree.OnMove2(list);

            //for (int i = list.Count - 1; i >= 0; i--)
            //{
            //    if (list[i].node == null) continue;
            //    tree.OnMove(list[i]);
            //    // 存在逻辑问题  假设树里面的list xy
            //    //如果 tree.OnMove()时候  tree.list[4] 移动一个新的 节点 执行Split(),  然而tree.list[0] 这时候已经不在当前节点内了 
            //    //这时候会出现bug  tree.list[0] 无法进入任何4个节点  当前节点也进入不了 
            //}
            UnityEngine.Profiling.Profiler.EndSample();

            //transform.debugInfo("--->>--");
        }



        private void OnDrawGizmos()
        {
            tree?.OnDrawGizmos();

            if (!QuadTreeHelper.IsOpenOnDrawGizmos) return;

            if (!Application.isPlaying)
            {
                Gizmos.color = Color.cyan;
                Rect rect = WorldSize;
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


}









