/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/5/31 20:11:47
/// 功能描述:  FSM 有限状态机
****************************************************/

using System;
using System.Collections.Generic;
using System.Text;


namespace LGF
{
    public interface IStateBase
    {
        void Update();
        void LateUpdate();
        void FixedUpdate();
    }


    #region 完整继承 版本

    #region 案例
    namespace Example
    {
        public enum tests111 { sggg }
        public class sssValue { }

        public class tttStateBase : StateBase<tests111, sssValue, tttBaseStateMachine, tttStateBase>
        {
            public override void OnEnter(tests111 lastState = default)
            {
                throw new System.NotImplementedException();
            }

            public override void OnExit(tests111 nextState = default)
            {
                throw new System.NotImplementedException();
            }
        }

        public class tttBaseStateMachine : StateMachine<tests111, sssValue, tttBaseStateMachine, tttStateBase>
        {
            public tttBaseStateMachine(sssValue value_) : base(value_) { }

        }
    }
    #endregion


    public abstract class StateBase<StateID, Value, _StateMachine, _StateBase>
        : StateBase1<StateID, _StateBase>
        where StateID : System.Enum
        where _StateMachine : StateMachine<StateID, Value, _StateMachine, _StateBase>
        where _StateBase : StateBase<StateID, Value, _StateMachine, _StateBase>
    {
        protected _StateMachine m_StateMachine;
        protected Value m_Value => m_StateMachine.CurValue;
        public override void Init(object stateMachine)
        {
            m_StateMachine = (_StateMachine)stateMachine;
        }
    }



    #endregion


    #region 提供单独继承
    public abstract class StateBase<StateID, Value>
       : StateBase<StateID, Value, StateMachine<StateID, Value>>
        where StateID : System.Enum
    {

    }

    public abstract class StateBase<StateID, Value, _StateMachine>
        : StateBase1<StateID, StateBase<StateID, Value, _StateMachine>>
        where StateID : System.Enum
        where _StateMachine : StateMachine<StateID, Value, _StateMachine>
    {
        protected _StateMachine _stateMachine;
        protected Value _value => _stateMachine.CurValue;
        public override void Init(object stateMachine)
        {
            _stateMachine = (_StateMachine)stateMachine;
        }
    }


    public abstract class StateBase<StateID> : StateBase1<StateID, StateBase<StateID>>
        where StateID : System.Enum
    {
        protected StateMachine<StateID> _stateMachine;
        public override void Init(object stateMachine)
        {
            _stateMachine = (StateMachine<StateID>)stateMachine;
        }
    }
    #endregion



    public abstract class StateBase1<StateID, StateBase_> : IStateBase
        where StateID : System.Enum
        where StateBase_ : StateBase1<StateID, StateBase_>
    {
        private int m_Flag = StateMachineDefine.DEFAULT_VALUE;
        public int Flag { get => m_Flag;}

        /// <summary>
        /// 进入状态
        /// </summary>
        public abstract void OnEnter(StateID lastState = default(StateID));
        /// <summary>
        /// 退出状态
        /// </summary>
        public abstract void OnExit(StateID nextState = default(StateID));

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update() { }
        /// <summary>
        /// 延迟帧更新
        /// </summary>
        public virtual void LateUpdate() { }
        /// <summary>
        /// 物理帧更新
        /// </summary>
        public virtual void FixedUpdate() { }


        /// <summary>
        /// 是否可以切换至
        /// </summary>
        /// <param name="stateID"></param>
        /// <returns></returns>
        public bool CanTransitionTo(int stateID) => (m_Flag & stateID) == stateID;
        /// <summary>
        /// 激活可切换状态
        /// </summary>
        /// <param name="stateID"></param>
        public StateBase1<StateID, StateBase_> EnableTransition(StateID stateID)
        {
            m_Flag |= stateID.ToInt();
            return this;
        }

        /// <summary>
        /// 禁用可切换状态
        /// </summary>
        /// <param name="stateID"></param>
        public StateBase1<StateID, StateBase_> DisableTransition(StateID stateID)
        {
            m_Flag &= ~stateID.ToInt();
            return this;
        }

        /// <summary>
        /// 初始化 状态机
        /// </summary>
        /// <param name="stateMachine"></param>
        public virtual void Init(object stateMachine)
        {
        }
    }


}

