/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/5/30 10:52:30
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using static LGF.EvtHelper;

namespace LGF
{

    /// <summary>
    /// 无GC时间系统  产生GC的情况 是list2 里面的数组扩容
    /// </summary>
    public partial class EventCenter : SingletonBase<EventCenter>
    {
        //用字典的话还是有logn 的消耗  如果频繁的删除 添加的  消耗也不低 所以这种还是用数组
        List<EventDispatcherBase> events = new List<EventDispatcherBase>(10);
        public EventCenter() {
            if (CheckInstance())
                this.DebugError("产生多余的 EventCenter 多个实例 有GC  写出来问题  注意");
        }

        public void RegisterEventDispatcher<T>(int type) where T : class, IEntityIndex<T>
        {
            while (type >= events.Count)    //补空位 为了动态支持添加注册
                events.Add(null);

            if (events[type] != null)
            {
                Log.sLog.Error("重复注册");
                return;
            }

            events[type] = new EventDispatcher<T>();
            events[type].isDelayRemove = RegisterNotDelayRemove(type);
        }

        #region int
     
        public static void Add<T>(int type, T obj) where T : class => Instance.events[type].Add(obj);
        public static void Remove<T>(int type, T obj) where T : class => Instance.events[type].Remove(obj);
        public static void Fire(int type) => Instance.events[type].Fire(null);
        public static void Fire(int type, IEventDataBase ev) => Instance.events[type].Fire(ev);
        public static void Fire<T1>(int type, T1 param)
        {
            if (param is IEventDataBase)
            {
#pragma warning disable CS8604 // 引用类型参数可能为
                Fire(type, param as IEventDataBase);
#pragma warning restore CS8604 // 引用类型参数可能为
                return;
            }
            Instance.events[type].Fire(EvtHelper.Event.DataBase<T1>.Get(param));
        }

        public static void Fire<T1, T2>(int type, T1 param, T2 param2) => Instance.events[type].Fire(EvtHelper.Event.DataBase<T1, T2>.Get(param, param2));
        public static void Fire<T1, T2, T3>(int type, T1 param, T2 param2, T3 param3) => Instance.events[type].Fire(EvtHelper.Event.DataBase<T1, T2, T3>.Get(param, param2, param3));
        #endregion


        #region 枚举
        public static void Add<T>(EventsType type, T obj) where T : class => Add((int)type, obj);
        public static void Remove<T>(EventsType type, T obj) where T : class => Remove((int)type, obj);
        public static void Fire(EventsType type) => Fire((int)type);
        public static void Fire(EventsType type, IEventDataBase ev) => Fire((int)type, ev);
        public static void Fire<T1>(EventsType type, T1 param) => Fire((int)type, param);
        public static void Fire<T1, T2>(EventsType type, T1 param, T2 param2) => Fire((int)type, param, param2);
        public static void Fire<T1, T2, T3>(EventsType type, T1 param, T2 param2, T3 param3) => Fire((int)type, param, param2, param3);

        #endregion


    }

    #region 以前的写法 现在不用了 有GC  调用也不方便
    //以前的写法
    //public static void Add<T, T1, T2, T3>(int type, T obj, System.Action<T1, T2, T3> action) where T : IEntityIndex<T>
    //{
    //    (Instance.events[type] as EventDispatcher<T>).Add(obj, (ev) =>
    //    {
    //        if (ev is EvtHelper.Event.DataBase<T1, T2, T3> ee)
    //            action.Invoke(ee.param1, ee.param2, ee.param3);    //会产生GC 这里闭包捕获了action  虽然GC很小一直占用内存  在注销的时候产生
    //    });
    //}
    #endregion

}


#region 泛型枚举  无GC

namespace LGF
{
    namespace Example
    {
        //案例
        public class EventCenterEnumExample : EventCenterEnum<EventsType> { };
    }


    /// <summary>
    /// 
    /// </summary> 
    /// <typeparam name="Enum"></typeparam>
    public class EventCenterEnum<Enum> : EventCenter where Enum : System.Enum 
    {
        public static void RegisterEventDispatcher<T>(Enum type) where T : class, IEntityIndex<T>
        {
            _Instance.RegisterEventDispatcher<T>(type.ToInt());
        }
        public static void Add<T>(Enum type, T obj) where T : class => Add(type.ToInt(), obj);
        public static void Remove<T>(Enum type, T obj) where T : class => Remove(type.ToInt(), obj);
        public static void Fire(Enum type) => Fire(type.ToInt());
        public static void Fire(Enum type, IEventDataBase ev) => Fire(type.ToInt(), ev);
        public static void Fire<T1>(Enum type, T1 param) => Fire(type.ToInt(), param);
        public static void Fire<T1, T2>(Enum type, T1 param, T2 param2) => Fire(type.ToInt(), param, param2);
        public static void Fire<T1, T2, T3>(Enum type, T1 param, T2 param2, T3 param3) => Fire(type.ToInt(), param, param2, param3);

    }
}




#endregion


