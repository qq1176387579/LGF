using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LGF.EvtHelper;
using LGF.DataStruct;

namespace LGF
{
    /// <summary>
    /// 会有GC  晚点看怎么处理
    /// </summary>
    public abstract class MonoBase : MonoBehaviour, IOnUpdate, IOnLateUpdate, IOnFixedUpdate
    {
        SemctlBool<MonoBase> _updateSem, _fixedUpdateSem, _lateUpdateSem;

        int IList2Idx<IEntityIndex<IOnUpdate>>.idx { get; set; }
        int IList2Idx<IEntityIndex<IOnLateUpdate>>.idx { get; set; }
        int IList2Idx<IEntityIndex<IOnFixedUpdate>>.idx { get; set; }

        #region OnUpdate
        protected void AddOnUpdate()
        {
            if (_updateSem == null)
                _updateSem = new SemctlBool<MonoBase>(this,
                    (_this) => EventCenter.Add(EventsType.OnUpdate, _this),
                    (_this) => EventCenter.Remove(EventsType.OnUpdate, _this)); //这样无GC
            _updateSem.Begin();
        }


        protected void RemoveOnUpdate() => _updateSem?.End();

        #endregion

        #region OnFixedUpdate
        protected void AddOnFixedUpdate()
        {
            if (_fixedUpdateSem == null)
                _fixedUpdateSem = new SemctlBool<MonoBase>(this,
                    (_this) => EventCenter.Add(EventsType.OnFixedUpdate, _this),
                    (_this) => EventCenter.Remove(EventsType.OnFixedUpdate, _this));
            _fixedUpdateSem.Begin();
        }
        protected void RemoveOnFixedUpdate() => _fixedUpdateSem?.End();
        #endregion

        #region OnOnLateUpdate
        protected void AddOnLateUpdate()
        {
            if (_lateUpdateSem == null)
                _lateUpdateSem = new SemctlBool<MonoBase>(this,
                    (_this) => EventCenter.Add(EventsType.OnLateUpdate, _this),
                    (_this) => EventCenter.Remove(EventsType.OnLateUpdate, _this));
            _lateUpdateSem.Begin();
        }
        protected void RemoveOnLateUpdate() => _lateUpdateSem?.End();

        #endregion
        void IEntityIndex<IOnUpdate>.OnEvent(IEventDataBase ev) { if (ev == null) OnUpdate(); }
        void IEntityIndex<IOnLateUpdate>.OnEvent(IEventDataBase ev) { if (ev == null) OnFixedUpdate(); }
        void IEntityIndex<IOnFixedUpdate>.OnEvent(IEventDataBase ev) { if (ev == null) OnLateUpdate(); }

        protected virtual void OnUpdate() { }
        protected virtual void OnFixedUpdate() { }
        protected virtual void OnLateUpdate() { }



    }

}

