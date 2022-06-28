/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/5/31 20:11:47
/// 功能描述:  FSM 有限状态机
****************************************************/


using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
///互相依赖才能不报错 神奇的 鸡生蛋 蛋生鸡  = =  
///StateMachine依赖 StateBase    StateBase 又依赖StateMachine依赖  = =
///有点离谱
/// </summary>

namespace LGF
{

    public interface IStateMachine
    {
        void Update();
        void LateUpdate();
        void FixedUpdate();
    }


    #region 完整继承版本


    public class StateMachine<StateID, Value, _StateMachine, _StateBase>
    : StateMachineBase<StateID, _StateBase, _StateMachine>
    where StateID : System.Enum
    where _StateBase : StateBase<StateID, Value, _StateMachine, _StateBase>
    where _StateMachine : StateMachine<StateID, Value, _StateMachine, _StateBase>   //继承了自己
    {
        public Value CurValue { get; private set; }
        public StateMachine(Value value_) { CurValue = value_; }

    }

    #endregion

    #region simple 继承版本
    public class StateMachine<StateID, Value> : StateMachine<StateID, Value, StateMachine<StateID, Value>>
        where StateID : System.Enum
    {
        public StateMachine(Value value_) : base(value_) { }
    }

    public class StateMachine<StateID, Value, _StateMachine> 
        : StateMachineBase<StateID, StateBase<StateID, Value, _StateMachine>, _StateMachine>
        where StateID : System.Enum
        //where _StateMachine : StateMachineBase<StateID, StateBase<StateID, Value, _StateMachine>, _StateMachine>
        where _StateMachine : StateMachine<StateID, Value, _StateMachine>   //继承了自己
    {
        public Value CurValue { get; private set; }

        public StateMachine(Value value_) { CurValue = value_; }

    }

    
    public class StateMachine<StateID>
        : StateMachineBase<StateID, StateBase<StateID>, StateMachine<StateID>>
        where StateID : System.Enum
    {
    }
    #endregion


    public class StateMachineBase<StateID, StateBase, StateMachine> : IStateMachine
        where StateID : System.Enum
        where StateMachine : StateMachineBase<StateID, StateBase, StateMachine>
        where StateBase : StateBase1<StateID, StateBase>
    {
        public StateID CurrentStateID { get; private set; }
        public StateBase CurrentState { get; private set; }
        protected Dictionary<StateID, StateBase> m_StateMap = new Dictionary<StateID, StateBase>();

        /// <summary>
        /// 设置默认状态
        /// </summary>
        /// <param name="stateID"></param>
        /// <returns></returns>
        public StateMachineBase<StateID, StateBase, StateMachine> SetDefaultState(StateID stateID)
        {
            if (m_StateMap.ContainsKey(stateID))
                SetDefaultState(stateID, m_StateMap[stateID]);
            return this;
        }


        /// <summary>
        /// 设置默认状态
        /// </summary>
        /// <param name="stateID"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public StateMachine SetDefaultState(StateID stateID, StateBase state)
        {
            if (CurrentState == null)
            {
                CurrentStateID = stateID;
                CurrentState = state;
                CurrentState.OnEnter();
            }
            return (StateMachine)this;
        }


        public  void ClearState()
        {
            CurrentState = null;
            CurrentStateID = default;
        }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="stateID">添加的状态ID</param>
        /// <param name="state">对应的状态对象</param>
        public StateMachine AddState(StateID stateID, StateBase state)
        {
            if (m_StateMap.Count >= StateMachineDefine.MAX_CAPACITY)
            {
                DeLog($"已经达到状态机最大容量");
                return (StateMachine)this;
            }

            if (m_StateMap.ContainsKey(stateID))
            {
                DeLog($"状态ID:{stateID}已经存在，不能重复添加");
                return (StateMachine)this;
            }

            m_StateMap.Add(stateID, state);
            state.Init(this);
            //DeLog($"  AddState ");
            return (StateMachine)this;
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="stateID">要移除的状态ID</param>
        public StateMachine RemoveState(StateID stateID)
        {
            if (!m_StateMap.ContainsKey(stateID))
            {
                DeLog($"状态ID:{stateID}不存在，不需要移除");
                return (StateMachine)this;
            }
            m_StateMap.Remove(stateID);

            return (StateMachine)this;
        }

        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="stateID">需要转换到的目标状态ID</param>
        /// <param name="canChangeSelf">当前状态也能改变</param>
        public void ChangeState(StateID stateID, bool canChangeSelf = false, bool canOnExit = true)
        {
            if (!m_StateMap.ContainsKey(stateID))
            {
                DeLog($"状态ID:{stateID}不存在！");
                return;
            }

            if (CurrentState != null && !CurrentState.CanTransitionTo(1 << stateID.ToInt()))
            {
                DeLog($"无法切换至{stateID}");
                return;
            }

            if (!canChangeSelf && CurrentStateID.ToInt() == stateID.ToInt())
            {
                DeLog($"处于当前状态中 无法切换{stateID}");
                return;
            }


            var lastStateID = CurrentStateID;
            CurrentStateID = stateID;
            if (canOnExit)
                CurrentState?.OnExit(stateID);
            CurrentState = m_StateMap[stateID];
            CurrentState.OnEnter(lastStateID);
        }


        public void FsmChangeState(StateID stateID)
        {
            if (CurrentStateID.ToInt() != stateID.ToInt())
                ChangeState(stateID);
        }


        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update() => CurrentState?.Update();
        /// <summary>
        /// 延迟更新
        /// </summary>
        public virtual void LateUpdate() => CurrentState?.LateUpdate();
        /// <summary>
        /// 物理更新
        /// </summary>
        public virtual void FixedUpdate() => CurrentState?.FixedUpdate();


        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="content"></param>
        public void DeLog(string content)
        {
#if UNITY_STANDALONE
            UnityEngine.Debug.LogError(content);
#else
            Console.WriteLine(content);
#endif
        }
    }



}
