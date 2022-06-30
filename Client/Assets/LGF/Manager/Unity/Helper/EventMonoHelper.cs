/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/5/30 14:10:28
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

namespace LGF
{
    internal class EventMonoHelper : MonoSingleton<EventMonoHelper>
    {
        private void Update() => EventCenter.Fire(EvtHelper.EventsType.OnUpdate);
   
        private void LateUpdate() => EventCenter.Fire(EvtHelper.EventsType.OnLateUpdate);

        private void FixedUpdate() => EventCenter.Fire(EvtHelper.EventsType.OnFixedUpdate);

    }
}

