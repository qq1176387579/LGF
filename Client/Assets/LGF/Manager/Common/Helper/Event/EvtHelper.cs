/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/5/30 11:00:55
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using static LGF.EvtHelper;
using static LGF.EvtHelper.Event;

namespace LGF
{

    public partial class EvtHelper
    {
        /// <summary>
        /// 这里添加 需要将下面对应的也注册  这是是框架占用用的如果是
        /// </summary>
        public enum EventsType
        {
            BEGIN           = -1,

#if !NOT_UNITY
            OnUpdate        =  0,
            OnLateUpdate    =  1,
            OnFixedUpdate   =  2,
#endif

            TestExample,
            LENGTH,
        }
        //可以用其他方式


        //------------------------------------------注册------------------------------------
        //--------------------------------- 显示继承 不要用隐式继承-------------------------------------
        public interface IOnUpdate : IEntityIndex<IOnUpdate> { }
        public interface IOnLateUpdate : IEntityIndex<IOnLateUpdate> { }
        public interface IOnFixedUpdate : IEntityIndex<IOnFixedUpdate> { }
        public interface ITestExample : IEntityIndex<ITestExample> { }
    }

    public partial class EventCenter
    {
        protected override void OnNew()
        {
            //------------------------------------------注册----------------------------
#if !NOT_UNITY
            RegisterEventDispatcher<IOnUpdate>(EventsType.OnUpdate);
            RegisterEventDispatcher<IOnLateUpdate>(EventsType.OnLateUpdate);
            RegisterEventDispatcher<IOnFixedUpdate>(EventsType.OnFixedUpdate);
            EventMonoHelper.GetSingleton();
#endif

            RegisterEventDispatcher<ITestExample>(EventsType.TestExample);
        }
        
        void RegisterEventDispatcher<T>(EventsType type) where T : class, IEntityIndex<T>
        {
            RegisterEventDispatcher<T>((int)type);
        }


        /// <summary>
        /// 注册不延迟处理 移除 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        bool RegisterNotDelayRemove(int type)
        {
            switch ((EventsType)(type)) //案例
            {
                case EventsType.BEGIN:
                case EventsType.TestExample:
                    return false;
                default:
                    break;
            }
            return true;
        }

    }



    public partial class EvtHelper
    {
      
        public class Event
        {
            public class DataBase<T1> : EventDataBase<DataBase<T1>>
            {
                public T1 param1;
                public static DataBase<T1> Get(T1 param_)
                {
                    var v = Get();
                    v.param1 = param_;
                    return v;
                }
            }

            public class DataBase<T1, T2> : EventDataBase<DataBase<T1, T2>>
            {
                public T1 param1;
                public T2 param2;
                public static DataBase<T1, T2> Get(T1 param1_, T2 param2_)
                {
                    var v = Get();
                    v.param1 = param1_;
                    v.param2 = param2_;
                    return v;
                }
            }

            public class DataBase<T1, T2, T3> : EventDataBase<DataBase<T1, T2, T3>>
            {
                public T1 param1;
                public T2 param2;
                public T3 param3;
                public static DataBase<T1, T2, T3> Get(T1 param1_, T2 param2_, T3 param3_)
                {
                    var v = Get();
                    v.param1 = param1_;
                    v.param2 = param2_;
                    v.param3 = param3_;
                    return v;
                }
            }

          
        }


    }

    public static class EvtHelperExtend
    {

        /// <summary>
        /// 注意GC问题  无捕获的话 无GC
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ev"></param>
        /// <param name="action"></param>
        public static void ToParam<T>(this IEventDataBase ev, System.Action<T> action)
        {
            if (ev is DataBase<T> ee)
            {
                action.Invoke(ee.param1);
            }
        }

        /// <summary>
        /// 注意GC问题  无捕获的话 无GC
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ev"></param>
        /// <param name="action"></param>
        public static void ToThisParam<T2,T>(this IEventDataBase ev, System.Action<T2, T> action, T2 _this)
        {
           
            if (ev is DataBase<T> ee)
            {
                action.Invoke(_this, ee.param1);
            }
        }

    }

}
