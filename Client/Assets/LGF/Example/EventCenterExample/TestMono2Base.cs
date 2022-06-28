/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/5/30 14:41:12
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;

namespace LGF.Example
{
    public class TestMono2Base : Mono2Base
    {
        protected override MonoType monoType => MonoType.OnUpdate | MonoType.OnLateUpdate | MonoType.OnFixedUpdate;

        protected override void OnUpdate()
        {
            Debug.LogError("----TestMono2Base--OnUpdate----");
        }

        protected override void OnLateUpdate()
        {
            Debug.LogError("----TestMono2Base--OnLateUpdate----");
        }

        protected override void OnFixedUpdate()
        {
            Debug.LogError("----TestMono2Base--OnFixedUpdate----");
        }

    }
}

