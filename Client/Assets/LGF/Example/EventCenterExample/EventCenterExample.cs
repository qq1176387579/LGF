using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using LGF;
using static LGF.EvtHelper;


namespace LGF.Example
{

    //扩展案例   如果想换变量命的话按照下面的操作
    namespace Test
    {
        public interface ITestNewEventCenter : IEntityIndex<ITestNewEventCenter> { }

        //这样方便调用 针对不同枚举  不要生成实例 不然会new一个新对象
        public class EventCenterTest : EventCenterEnum<TestGameEventTypes>
        {
            //protected override void OnNew()   不要用这个参数会出问题  可能有多个继承单例 导致这个无法执行
            public static void Initialize()
            {
                RegisterEventDispatcher<ITestNewEventCenter>(TestGameEventTypes.TestNewEventCenter);   //不会存在实例所以不会调用
            }
        }

        public enum TestGameEventTypes
        {
            TestExample = EventsType.TestExample,
            TestNewEventCenter = EventsType.LENGTH                //长度继承
        }
    }





    public class EventCenterExample : MonoBehaviour, ITestExample, EvtHelper.IOnUpdate, Test.ITestNewEventCenter
    {
        ///// <summary>
        ///// 显示继承 不要用隐式继承
        ///// </summary>

        int IList2Idx<IEntityIndex<IOnUpdate>>.idx { get; set; }
        int IList2Idx<IEntityIndex<ITestExample>>.idx { get; set; }
        int IList2Idx<IEntityIndex<Test.ITestNewEventCenter>>.idx { get; set; }

        /// <summary>
        /// 私有变量测试
        /// </summary>
        int t = 0;

        /// <summary>
        /// 是否看打印效果  打印就无法查看GC效果
        /// </summary>
        bool isDebug = false;

        void IEntityIndex<IOnUpdate>.OnEvent(IEventDataBase data)
        {
            
            //测试GC 记得把debug关掉
            EventCenter.Add(EventsType.TestExample, this);
            EventCenter.Fire(EventsType.TestExample, Event.TestExample.Get());        //TODO3
            EventCenter.Fire(EventsType.TestExample, EvtHelper.Event.DataBase<int>.Get(1));

            Test.EventCenterTest.Fire(Test.TestGameEventTypes.TestExample);
            EventCenter.Remove(EventsType.TestExample, this);

            int testType = Test.TestGameEventTypes.TestNewEventCenter.ToInt();
            EventCenter.Fire(testType);        //测试注册
        }
  
        void IEntityIndex<ITestExample>.OnEvent(IEventDataBase data)
        {
         
            //下列写法 debug 删除是无GC的
            data.ToParam<int>((param1) =>
            {
                 //Debug.LogError(param1);  //这里不能用isdebug  会有捕获GC
            });

            data.ToThisParam<EventCenterExample, int>((_this, param1) =>
            {
                _this.t = param1;
                //Debug.LogError(param1);
            }, this);

            if (isDebug) Debug.LogError(t);

            if (data is Event.TestExample ss)
            {
                if (isDebug) Debug.LogError(ss.t);
            }

            if (data == null)
            {
                if (isDebug) Debug.LogError("Test.TestGameEventTypes.TestExample");
                //Debug.LogError("Test.TestGameEventTypes.TestExample");
            }

        }


        void IEntityIndex<Test.ITestNewEventCenter>.OnEvent(IEventDataBase ev)
        {
            if (isDebug) Debug.LogError("----Test.ITestNewEventCenter-----");
            //Debug.LogError("----Test.ITestNewEventCenter-----");
        }


        void Start()
        {
            EventCenter.Add(EventsType.TestExample, this);
            EventCenter.Add(EventsType.OnUpdate, this);



            EventCenter.Fire(EventsType.TestExample, Event.TestExample.Get());        //TODO3
            EventCenter.Fire(EventsType.TestExample, EvtHelper.Event.DataBase<int>.Get(1));   //TODO2
            EventCenter.Fire(EventsType.TestExample, 2);  //TODO1 
            //test1.Fire(EventsType.TestExample, 2);
            EventCenter.Remove(EventsType.TestExample, this);
            EventCenter.Remove(EventsType.TestExample, this);   //特意删除两次 用来显示多余的效果的
                                                                // EventCenter.Remove<EvtHelper.ITestExample>(EventsType.TestExample, this);
            EventCenter.Fire(EventsType.TestExample, Event.TestExample.Get());


            ////调用前需要先初始化  有GC
            Test.EventCenterTest.Initialize();    //初始广播
            Test.EventCenterTest.Add(Test.TestGameEventTypes.TestNewEventCenter, this);
        }
     

        class Event
        {
            public class TestExample : EventDataBase<TestExample>
            {
                public int t = 4;
            }
        }

    }


    


}

