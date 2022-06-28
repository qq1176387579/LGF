/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/9 2:27:28
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;

public class GCTest2 : MonoBehaviour
{
    class Test
    {
        int t;
        int y;
    }

    List<Test> list = new List<Test>();

    void Start()
    {
        
    }

    public static int GetToInt<T>(T type) where T : System.Enum 
    {
        return type.ToInt();
    }

    //public static int GetToInt2<T>(T type) where T : System.Enum
    //{
    //    if (type is GameEventType)
    //    {
    //        return ((GameEventType)type).ToInt();
    //    }
    //    return 0;
    //}

    public static int GetToInt3(System.Enum type)
    {
        if (type is GameEventType)
        {
            return ((GameEventType)type).ToInt();
        }
        return 0;
    }

    // Update is called once per frame
    void Update()
    {
        //UnityEngine.Profiling.Profiler.BeginSample("--------1---");
        //var T = new Test();
        //list.Add(T);
        //UnityEngine.Profiling.Profiler.EndSample();
        //UnityEngine.Profiling.Profiler.BeginSample("--------2---");
        //list.Add(new Test());
        //UnityEngine.Profiling.Profiler.EndSample();

        //List<int> numbers = null;
        //int? a = null;

        //(numbers ??= new List<int>()).Add(5);


        //numbers.Add(a ??= 0);

        //UnityEngine.Profiling.Profiler.BeginSample("--------BEGIN---" + EvtHelper.EventsType.BEGIN.ToInt());
        //GetToInt(EvtHelper.EventsType.BEGIN);
        //UnityEngine.Profiling.Profiler.EndSample();
        //测试效率代码
        //e.GetHashCode() 
        //GameEventType.NetStatus;
        //double time = 0;
        UnityEngine.Profiling.Profiler.BeginSample("--------BEGIN-1--");
        for (int i = 0; i < 10000; i++)
        {
            //var now = System.DateTime.Now;
            int t = GetToInt(GameEventType.NetStatus);
           // time += (System.DateTime.Now - now).TotalMilliseconds;
        }
        UnityEngine.Profiling.Profiler.EndSample();
        //UnityEngine.Debug.Log(time);
        //time = 0;
        //UnityEngine.Profiling.Profiler.BeginSample("--------BEGIN-2--");
        //for (int i = 0; i < 10000; i++)
        //{
        //    //var now = System.DateTime.Now;
        //    int t = GetToInt3(GameEventType.NetStatus);
        //    //time += (System.DateTime.Now - now).TotalMilliseconds;
        //}
        //UnityEngine.Profiling.Profiler.EndSample();
        //UnityEngine.Debug.Log(time);
    }
}
