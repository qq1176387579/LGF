/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/8 23:12:58
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;
using LGF.DataStruct;

public class GCTest : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        _tree = new QuadTree(new Rect(0, 0, 500, 500), 30, 30);
        ass2 = sdsad.PP3;
    }
    QuadTree _tree;

    List<QuadTree.Node> list = new List<QuadTree.Node>();
    int count = 1000;
    System.Action<QuadTree.Node> ass;
    System.Action<QuadTree.Node> ass2;
   
    void Update()
    {

        UnityEngine.Profiling.Profiler.BeginSample("--------Rect---");
        for (int i = 0; i < count; i++)
        {
            new Rect(1, 1, 2, 3);
        }
        UnityEngine.Profiling.Profiler.EndSample();


        UnityEngine.Profiling.Profiler.BeginSample("--------QuadTree.Node.Get---");
        for (int i = 0; i < count; i++)
        {
            list.Add(QuadTree.Node.Get());
            list[i].Init(_tree.root, new Rect());
        }
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("--------PPsss---");
        for (int i = 0; i < count; i++)
            PPsss((a) => { int t = a; });
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("--------PPsss--2-");
        for (int i = 0; i < count; i++)
            PPsss2((a) => { int t = a; });
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("--------PPsss--3-");
        for (int i = 0; i < count; i++)
            PPsss2((a) => { PPsss3(); });	//有GC 
        UnityEngine.Profiling.Profiler.EndSample();


        UnityEngine.Profiling.Profiler.BeginSample("--------list.ForEach---");
        list.ForEach((a) =>
        {
            //int t = a.layer;
            GCTest s = this;
        });
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("--------list.ForEach--2-");

        list.ForEach(PP);
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("--------list.ForEach--3-");
        list.ForEach(PP2);
        UnityEngine.Profiling.Profiler.EndSample();


        UnityEngine.Profiling.Profiler.BeginSample("--------list.ForEach--4-");
        list.ForEach(sdsad.PP3);
        UnityEngine.Profiling.Profiler.EndSample();



        UnityEngine.Profiling.Profiler.BeginSample("--------System.Action<QuadTree.Node>--1-");
        ass = sdsad.PP3;
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("--------list.ForEach--5-");
        list.ForEach(ref ass);
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("--------System.Action<QuadTree.Node>--2-");
        ass = PP2;
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("--------System.Action<QuadTree.Node>--3-");
        ass = sdsad.PP3;
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("--------System.Action<QuadTree.Node>--ass2-");
        for (int i = 0; i < count; i++)
        {
            ass2.Invoke(list[i]);
        }
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("--------list.ForEach--bibaoTest-");
        list.ForEach((a) =>
        {
            sdsad.PP3(a);
        });
        UnityEngine.Profiling.Profiler.EndSample();


        UnityEngine.Profiling.Profiler.BeginSample("--------QuadTree.Node.Release---");
        for (int i = 0; i < count; i++)
        {
            list[i].Release();
        }
        UnityEngine.Profiling.Profiler.EndSample();
        list.Clear();
    }

    void PP(QuadTree.Node a)
    {
        int t = a.layer;
    }
    
    static void PP2(QuadTree.Node a)
    {
        int t = a.layer;
    }

    void PPsss(System.Action<int> s)
    {
        PPsss2(s);
    }

    void PPsss2(System.Action<int> s)
    {
        s.Invoke(1);
    }


    void PPsss3()
    {
    }
}

public static class sdsad
{
    public static void PP3(QuadTree.Node a)
    {
        int t = a.layer;
    }

    public static void PP4(ref System.Action<QuadTree.Node> a)
    {
        
    }

    public static void ForEach<T>(this List<T> s ,ref System.Action<T> a)
    {
        for (int i = 0; i < s.Count; i++)
        {
            a.Invoke(s[i]);
        }
    }

}