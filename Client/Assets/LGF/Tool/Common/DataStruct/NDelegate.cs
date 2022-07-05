/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/2 13:25:59
/// 功能描述:  委托无GC闭包   测试证明还是挺耗性能的  主要性能消耗在 静态 与 锁那里 后面看情况 要不要写
///           如果要写的话写个生成工具 生成自己需要的结构    可能就多线程里面需要吧 但是感觉又很没用   不过回收生成频率不高 的话 可以用 
///           先把模板放在这
///           
/// 
///           为了在子线程函数在 主线程调用 还是实现了这个
///           效率对比   比普通的事件 多了 17倍消耗  消耗在生成与回收那里   从静态池里拿东西 消耗挺大的  
///           调用无而外消耗
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;



namespace LGF.DataStruct
{
    public interface IDelegateBase
    {
        void Invoke();
        void Release();
    }

    public interface IDelegateBase<T>
    {
        void Invoke(T param);
        void Release();
    }

    #region  NDelegate
    public class NDelegate
    {

        public static Event Get(System.Action evt)
        {
            var t = Event.Get();
            t.tmp.param1 = evt;
            return t;
        }

        public class Event : Poolable<Event>, IDelegateBase
        {
            //初始化的时候调用一次
            internal EvtHelper.Event.DataBase<System.Action> tmp = EvtHelper.Event.DataBase<System.Action>.Get();

            public void Invoke()
            {
                tmp.param1.Invoke();
                Release();  //测试回收
            }

            protected override void OnRelease() => tmp.Clear();
        }


        public static Event<T1> Get<T1>(System.Action<T1> evt, in T1 param2)
        {
            var t = Event<T1>.Get();
            t.tmp.param1 = evt;
            t.tmp.param2 = param2;
            return t;
        }

        public class Event<T1> : Poolable<Event<T1>>, IDelegateBase
        {
            //初始化的时候调用一次
            internal EvtHelper.Event.DataBase<System.Action<T1>, T1> tmp = EvtHelper.Event.DataBase<System.Action<T1>, T1>.Get();

            public void Invoke()
            {
                tmp.param1.Invoke(tmp.param2);
                Release();  //测试回收
            }

            protected override void OnRelease() => tmp.Clear();
        }


        public static Event<T1, T2> Get<T1, T2>(System.Action<T1, T2> evt, in T1 param2, in T2 param3)
        {
            var t = Event<T1, T2>.Get();
            t.tmp.param1 = evt;
            t.tmp.param2 = param2;
            t.tmp.param3 = param3;
            return t;
        }

        public class Event<T1, T2> : Poolable<Event<T1, T2>>, IDelegateBase
        {
            //初始化的时候调用一次
            internal EvtHelper.Event.DataBase<System.Action<T1, T2>, T1, T2> tmp = EvtHelper.Event.DataBase<System.Action<T1, T2>, T1, T2>.Get();

            public void Invoke()
            {
                tmp.param1.Invoke(tmp.param2, tmp.param3);
                Release();  //测试回收
            }

            protected override void OnRelease() => tmp.Clear();
        }




        public static Event<T1, T2, T3> Get<T1, T2, T3>(System.Action<T1, T2, T3> evt, in T1 param2, in T2 param3, in T3 param4)
        {
            var t = Event<T1, T2, T3>.Get();
            t.tmp.param1 = evt;
            t.tmp.param2 = param2;
            t.tmp.param3 = param3;
            t.tmp.param4 = param4;
            return t;
        }

        public class Event<T1, T2, T3> : Poolable<Event<T1, T2, T3>>, IDelegateBase
        {
            //初始化的时候调用一次
            internal EvtHelper.Event.DataBase<System.Action<T1, T2, T3>, T1, T2, T3> tmp = EvtHelper.Event.DataBase<System.Action<T1, T2, T3>, T1, T2, T3>.Get();

            public void Invoke()
            {
                tmp.param1.Invoke(tmp.param2, tmp.param3, tmp.param4);
                Release();  //测试回收
            }

            protected override void OnRelease() => tmp.Clear();

        }

    }
    #endregion



    #region  NDelegate<T>
    public class NDelegate<T>
    {
        public static IDelegateBase<T> Get<T1, T2, T3>(System.Action<T, T1, T2, T3> evt, in T1 param2, in T2 param3, in T3 param4)
        {
            var t = Event<T1, T2, T3>.Get();
            t.tmp.param1 = evt;
            t.tmp.param2 = param2;
            t.tmp.param3 = param3;
            t.tmp.param4 = param4;
            return t;
        }

        public class Event<T1, T2, T3> : Poolable<Event<T1, T2, T3>>, IDelegateBase<T>
        {
            //初始化的时候调用一次
            internal EvtHelper.Event.DataBase<System.Action<T, T1, T2, T3>, T1, T2, T3> tmp = EvtHelper.Event.DataBase<System.Action<T, T1, T2, T3>, T1, T2, T3>.Get();

            public void Invoke(T param)
            {
                tmp.param1.Invoke(param, tmp.param2, tmp.param3, tmp.param4);
                Release();  //测试回收
            }

            protected override void OnRelease() => tmp.Clear();
        }
    }
    #endregion























 }

