/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/16 15:25:17
/// 功能描述:  分层状态机 帮助类 
/// PS:  暂时先这样处理  后面可能一个父FSM有 多个子FSM状态  当前只是处理了单个FSM的情况
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;

namespace LGF
{
    public class HFsmHelper<StateID, Value, _frontStateMachine, _nextStateMachine>
        where _frontStateMachine : IStateMachine
        where _nextStateMachine : IStateMachine
        where StateID : System.Enum
    {

        /// <summary>
        /// HFSM 中间层
        /// </summary>
        public class HFsm<_StateBase, _SelfStateMachine> : HStateMachine<StateID, Value, _SelfStateMachine, _StateBase, _frontStateMachine, _nextStateMachine>
            where _StateBase : State<_StateBase, _SelfStateMachine>
            where _SelfStateMachine : HFsm<_StateBase, _SelfStateMachine>
        {
            public HFsm(Value value_) : base(value_) { }
        }

        public abstract class State<_StateBase, _SelfStateMachine> : StateBase<StateID, Value, _SelfStateMachine, _StateBase>
        where _StateBase : StateBase<StateID, Value, _SelfStateMachine, _StateBase>
        where _SelfStateMachine : StateMachine<StateID, Value, _SelfStateMachine, _StateBase>
        {

        }
    }




    public class HFsmHelper<StateID, Value, _IStateMachine>
        where _IStateMachine : IStateMachine
        where StateID : System.Enum
    {

        /// <summary>
        /// HFSM 底部状态机  
        /// </summary>
        public class BHFsm<_StateBase, _SelfStateMachine> : BHStateMachine<StateID, Value, _SelfStateMachine, _StateBase, _IStateMachine>
               where _StateBase : State<_StateBase, _SelfStateMachine>
            where _SelfStateMachine : BHFsm<_StateBase, _SelfStateMachine>
        {
            public BHFsm(Value value_) : base(value_) { }
        }

        /// <summary>
        /// HFSM 顶部状态机  
        /// </summary>
        public class THFsm<_StateBase, _SelfStateMachine> : THStateMachine<StateID, Value, _SelfStateMachine, _StateBase, _IStateMachine>
               where _StateBase : State<_StateBase, _SelfStateMachine>
            where _SelfStateMachine : THFsm<_StateBase, _SelfStateMachine>
        {
            public THFsm(Value value_) : base(value_) { }
        }

        public abstract class State<_StateBase, _SelfStateMachine> : StateBase<StateID, Value, _SelfStateMachine, _StateBase>
        where _StateBase : StateBase<StateID, Value, _SelfStateMachine, _StateBase>
        where _SelfStateMachine : StateMachine<StateID, Value, _SelfStateMachine, _StateBase>
        {

        }
    }


    //public class HFsmHelper<StateID, Value>
    //     where StateID : System.Enum
    //{
    //    public abstract class State<_StateBase, _SelfStateMachine> : StateBase<StateID, Value, _SelfStateMachine, _StateBase>
    //      where _StateBase : StateBase<StateID, Value, _SelfStateMachine, _StateBase>
    //      where _SelfStateMachine : StateMachine<StateID, Value, _SelfStateMachine, _StateBase>
    //    {

    //    }
    //}
}

//下面时使用案例
namespace LGF.Example
{
   
    public class HFsmHelperExample 
    {
        /// <summary>
        /// 绑定案例   
        /// </summary>
        public static void BingExample()
        {
            Helper.Fsm fsm1 = new Helper.Fsm(new Data());
            Helper2.Fsm fsm2 = new Helper2.Fsm(new Data());
            Helper3.Fsm fsm3 = new Helper3.Fsm(new Data3());
            //------------绑定状态机-------------
            fsm1.Bing(fsm3);
            fsm2.Bing(fsm3);
            fsm3.Bing(fsm2, fsm1);
        }

        #region 少参数的话用这个代替
        // 如  public class Helper : HFsmHelper<TestHFsmHelper, Data, Helper2.Fsm>
        public class TestIStateMachine : IStateMachine
        {
            public void FixedUpdate() => throw new System.NotImplementedException();
            public void LateUpdate() => throw new System.NotImplementedException();
            public void Update() => throw new System.NotImplementedException();
        }
        #endregion

        #region 底部
        public class Data { }
        public enum TestHFsmHelper { }
        public class Helper : HFsmHelper<TestHFsmHelper, Data, Helper3.Fsm>
        {
            public class Fsm : BHFsm<StateBase, Fsm> { public Fsm(Data value_) : base(value_) { } }
            public abstract class StateBase : State<StateBase, Fsm> { }
        }
        #endregion

        public class Data2 { }
        public enum TestHFsmHelper2 { }
        public class Helper2 : HFsmHelper<TestHFsmHelper2, Data, Helper3.Fsm>
        {
            public class Fsm : THFsm<StateBase, Fsm>{public Fsm(Data value_) : base(value_) { } }
            public abstract class StateBase : State<StateBase, Fsm> { }
        }


        public class Data3 { }

        public enum TestHFsmHelper3 { }

        public class Helper3 : HFsmHelper<TestHFsmHelper3, Data3, Helper2.Fsm, Helper.Fsm>
        {
            public class Fsm : HFsm<StateBase, Fsm> { public Fsm(Data3 value_) : base(value_) { } }
            public abstract class StateBase : State<StateBase, Fsm> { }
        }


    }
}
