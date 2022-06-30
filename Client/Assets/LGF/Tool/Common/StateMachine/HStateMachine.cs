/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/16 14:43:00
/// 功能描述:  HFSM 分层状态机
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

namespace LGF
{
    public abstract class HStateMachineBase<StateID, Value, _HStateMachine, _StateBase>
    : StateMachine<StateID, Value, _HStateMachine, _StateBase>
    where _HStateMachine : HStateMachineBase<StateID, Value, _HStateMachine, _StateBase>
    where StateID : System.Enum
    where _StateBase : StateBase<StateID, Value, _HStateMachine, _StateBase>
    {
        public HStateMachineBase(Value value_) : base(value_) { }
        protected virtual IStateMachine nextLayerStateMachine { get; }

        public override void Update()
        {
            CurrentState.Update();
            nextLayerStateMachine?.Update();
        }

        public override void FixedUpdate()
        {
            CurrentState.FixedUpdate();
            nextLayerStateMachine?.Update();
        }

        public override void LateUpdate()
        {
            CurrentState.LateUpdate();
            nextLayerStateMachine?.LateUpdate();
        }
    }


    public abstract class HStateMachineBase<StateID, Value, _HStateMachine, _StateBase, _relevancyStateMachine>
        : HStateMachineBase<StateID, Value, _HStateMachine, _StateBase>
        where _relevancyStateMachine : IStateMachine
        where StateID : System.Enum
        where _StateBase : StateBase<StateID, Value, _HStateMachine, _StateBase>
        where _HStateMachine : HStateMachineBase<StateID, Value, _HStateMachine, _StateBase, _relevancyStateMachine>
    {
        public HStateMachineBase(Value value_) : base(value_) { }
    }

    /// <summary>
    /// HFSM 顶部状态机  
    /// </summary>
    /// <typeparam name="_relevancyStateMachine"></typeparam>
    public abstract class THStateMachine<StateID, Value, _HStateMachine, _StateBase, _relevancyStateMachine>
        : HStateMachineBase<StateID, Value, _HStateMachine, _StateBase, _relevancyStateMachine>
        where _relevancyStateMachine : IStateMachine
        where StateID : System.Enum
        where _StateBase : StateBase<StateID, Value, _HStateMachine, _StateBase>
        where _HStateMachine : THStateMachine<StateID, Value, _HStateMachine, _StateBase, _relevancyStateMachine>
    {
        protected override IStateMachine nextLayerStateMachine => SNextState;
        public _relevancyStateMachine SNextState;

        public THStateMachine(Value value_) : base(value_) { }

        public void Bing(_relevancyStateMachine next_) => SNextState = next_;
    }


    /// <summary>
    /// HFSM 底部状态机  
    /// </summary>
    /// <typeparam name="_relevancyStateMachine"></typeparam>
    public abstract class BHStateMachine<StateID, Value, _HStateMachine, _StateBase, _relevancyStateMachine>
          : HStateMachineBase<StateID, Value, _HStateMachine, _StateBase, _relevancyStateMachine>
        where _relevancyStateMachine : IStateMachine
        where StateID : System.Enum
        where _StateBase : StateBase<StateID, Value, _HStateMachine, _StateBase>
        where _HStateMachine : BHStateMachine<StateID, Value, _HStateMachine, _StateBase, _relevancyStateMachine>
    {
        protected override IStateMachine nextLayerStateMachine => null;
        public _relevancyStateMachine SFrontState;

        public BHStateMachine(Value value_) : base(value_) { }

        public void Bing(_relevancyStateMachine front_) => SFrontState = front_;

    }


    public abstract class HStateMachine<StateID, Value, _HStateMachine, _StateBase, _frontStateMachine, _nextStateMachine>
         : HStateMachineBase<StateID, Value, _HStateMachine, _StateBase>
        where _frontStateMachine : IStateMachine
        where _nextStateMachine : IStateMachine
        where StateID : System.Enum
        where _StateBase : StateBase<StateID, Value, _HStateMachine, _StateBase>
        where _HStateMachine : HStateMachine<StateID, Value, _HStateMachine, _StateBase, _frontStateMachine, _nextStateMachine>
    {
        protected override IStateMachine nextLayerStateMachine => SNextState;
        public _frontStateMachine SFrontState;
        public _nextStateMachine SNextState;

        public HStateMachine(Value value_) : base(value_) { }
        public void Bing(_frontStateMachine front_, _nextStateMachine next_)
        {
            SFrontState = front_;
            SNextState = next_;
        }


    }




    #region 原来写法  太麻烦了 没必要  实际只需要 一个接口就行了

    //public abstract class HStateMachineBase<StateID, Value, _HStateMachine, _StateBase> : StateMachine<StateID, Value, _HStateMachine, _StateBase>
    //    where _HStateMachine : HStateMachineBase<StateID, Value, _HStateMachine, _StateBase>
    //    where StateID : System.Enum
    //    where _StateBase : StateBase<StateID, Value, _HStateMachine, _StateBase>
    //{
    //    public HStateMachineBase(Value value_) : base(value_) { }
    //}


    //public abstract class StateMachine<StateID, Value, _HStateMachine, _StateBase, StateID2, Value2, _relevancyStateMachine, _StateBase2> : HStateMachineBase<StateID, Value, _HStateMachine, _StateBase>
    //    where StateID2 : System.Enum
    //    where _relevancyStateMachine : HStateMachineBase<StateID2, Value2, _relevancyStateMachine, _StateBase2>
    //    where StateID : System.Enum
    //    where _StateBase : StateBase<StateID, Value, _HStateMachine, _StateBase>
    //    where _StateBase2 : StateBase<StateID2, Value2, _relevancyStateMachine, _StateBase2>
    //    where _HStateMachine : StateMachine<StateID, Value, _HStateMachine, _StateBase, StateID2, Value2, _relevancyStateMachine, _StateBase2>
    //{
    //    public StateMachine(Value value_) : base(value_) { }
    //}

    ///// <summary>
    ///// 顶层状态机
    ///// </summary>
    ///// <typeparam name="_relevancyStateMachine"></typeparam>
    //public abstract class THStateMachine<StateID, Value, _HStateMachine, _StateBase, StateID2, Value2, _relevancyStateMachine, _StateBase2> : StateMachine<StateID, Value, _HStateMachine, _StateBase, StateID2, Value2, _relevancyStateMachine, _StateBase2>
    //    where StateID2 : System.Enum
    //    where _relevancyStateMachine : HStateMachineBase<StateID2, Value2, _relevancyStateMachine, _StateBase2>
    //    where StateID : System.Enum
    //    where _StateBase : StateBase<StateID, Value, _HStateMachine, _StateBase>
    //    where _StateBase2 : StateBase<StateID2, Value2, _relevancyStateMachine, _StateBase2>
    //    where _HStateMachine : THStateMachine<StateID, Value, _HStateMachine, _StateBase, StateID2, Value2, _relevancyStateMachine, _StateBase2>

    //{
    //    public THStateMachine(Value value_) : base(value_) { }
    //}


    ///// <summary>
    ///// 顶层状态机  
    ///// </summary>
    ///// <typeparam name="_relevancyStateMachine"></typeparam>
    //public abstract class BHStateMachine<StateID, Value, _HStateMachine, _StateBase, StateID2, Value2, _relevancyStateMachine, _StateBase2> : StateMachine<StateID, Value, _HStateMachine, _StateBase, StateID2, Value2, _relevancyStateMachine, _StateBase2>
    //    where StateID2 : System.Enum
    //    where _relevancyStateMachine : HStateMachineBase<StateID2, Value2, _relevancyStateMachine, _StateBase2>
    //    where StateID : System.Enum
    //    where _StateBase : StateBase<StateID, Value, _HStateMachine, _StateBase>
    //    where _StateBase2 : StateBase<StateID2, Value2, _relevancyStateMachine, _StateBase2>
    //    where _HStateMachine : BHStateMachine<StateID, Value, _HStateMachine, _StateBase, StateID2, Value2, _relevancyStateMachine, _StateBase2>

    //{
    //    public BHStateMachine(Value value_) : base(value_) { }
    //}

    ///// <summary>
    ///// 这里其实可以简化  直接用objcet  用接口解耦  暂时先不
    ///// </summary>
    //public abstract class StateMachine<StateID, Value, _HStateMachine, _StateBase, StateID2, Value2, _frontStateMachine, _StateBase2,
    //    StateID3, Value3, _nextStateMachine, _StateBase3> : HStateMachineBase<StateID, Value, _HStateMachine, _StateBase>
    //    where StateID3 : System.Enum
    //    where StateID2 : System.Enum
    //    where _frontStateMachine : HStateMachineBase<StateID2, Value2, _frontStateMachine, _StateBase2>
    //    where _nextStateMachine : HStateMachineBase<StateID3, Value3, _nextStateMachine, _StateBase3>
    //    where StateID : System.Enum
    //    where _StateBase : StateBase<StateID, Value, _HStateMachine, _StateBase>
    //    where _StateBase2 : StateBase<StateID2, Value2, _frontStateMachine, _StateBase2>
    //    where _StateBase3 : StateBase<StateID3, Value3, _nextStateMachine, _StateBase3>
    //    where _HStateMachine : StateMachine<StateID, Value, _HStateMachine, _StateBase, StateID2, Value2, _frontStateMachine, _StateBase2,
    //    StateID3, Value3, _nextStateMachine, _StateBase3>
    //{
    //    public StateMachine(Value value_) : base(value_) { }
    //}

    #endregion

}



