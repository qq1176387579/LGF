using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LGF
{
    public abstract class Mono2Base : MonoBase
    {
       
        [Flags]
        protected enum MonoType
        {
            OnUpdate        = 1,
            OnLateUpdate    = 2,
            OnFixedUpdate   = 4,
        }

        /// <summary>
        /// 注册标签
        /// </summary>
        protected abstract MonoType monoType { get; }

        protected virtual void OnEnable()
        {
            if (!Application.isPlaying) return; //只在运行模式执行
            if ((monoType & MonoType.OnFixedUpdate) != 0)   AddOnFixedUpdate();
            if ((monoType & MonoType.OnLateUpdate) != 0)    AddOnLateUpdate();
            if ((monoType & MonoType.OnUpdate) != 0)        AddOnUpdate();
        }

        protected virtual void OnDisable()
        {
            if (!Application.isPlaying) return; //只在运行模式执行
            if ((monoType & MonoType.OnFixedUpdate) != 0)   RemoveOnFixedUpdate();
            if ((monoType & MonoType.OnLateUpdate) != 0)    RemoveOnLateUpdate();
            if ((monoType & MonoType.OnUpdate) != 0)        RemoveOnUpdate();
        }

    }
}
