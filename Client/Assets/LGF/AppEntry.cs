/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/5 0:37:34
/// 功能描述:  App入口
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;


namespace LGF
{
    public static class AppEntry
    {
        public static bool IsStartup = false;
        public static void Startup()
        {
            IsStartup = true;
        }

        public static void Update()
        {
            EventManager.Instance.BroadCastEvent(GameEventType.OnUpdate);
            EventCenter.Fire(EvtHelper.EventsType.OnUpdate);
        }

        public static void LateUpdate()
        {
            EventCenter.Fire(EvtHelper.EventsType.OnLateUpdate);
        }

        public static void FixedUpdate()
        {
            EventCenter.Fire(EvtHelper.EventsType.OnFixedUpdate);
        }


        public static void OnDestroy()
        {

        }

    }




}

