/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/5 0:37:34
/// 功能描述:  App入口
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using UnityEngine;


namespace LGF
{
    public static class LGFEntry
    {
        public static bool IsStartup = false;

        static event System.Action ev_OnUpdate, ev_OnLateUpdate, ev_OnFixedUpdate;



        //有些需要延迟几帧执行
        public static void Startup()
        {
            IsStartup = true;



#if USE_ANDROID
            Android.AndroidMsgManager.Instance.Init();
#endif
            //Util.MacAddressUtils.Instance.Init();   //初始化唯一标识符


#if !NOT_UNITY
            //弃用 热更新的时候无法配置文件坑 除非ScriptableObject的脚本在热更新意外就加载了 否则无法脚本热更新
            //AppConfig.Instance.Init();
#endif
        }


        public static void Update()
        {
            ev_OnUpdate?.Invoke();
            EventManager.Instance.BroadCastEvent(GameEventType.OnUpdate);
            EventCenter.Fire(EvtHelper.EventsType.OnUpdate);
        }

        public static void LateUpdate()
        {
            ev_OnLateUpdate?.Invoke();
            EventCenter.Fire(EvtHelper.EventsType.OnLateUpdate);
        }

        /// <summary>
        /// 在服务器的时候 会执行在update之前
        /// </summary>
        public static void FixedUpdate()
        {
            ev_OnFixedUpdate?.Invoke();
            EventCenter.Fire(EvtHelper.EventsType.OnFixedUpdate);
        }


      


        public static void RegisterOnUpdate(System.Action ac)
        {
            ev_OnUpdate += ac;
        }
        public static void RegisterOnLateUpdate(System.Action ac)
        {
            ev_OnLateUpdate += ac;
        }
        public static void RegisterOnFixedUpdate(System.Action ac)
        {
            ev_OnFixedUpdate += ac;
        }


        public static void UnRegisterOnUpdate(System.Action ac)
        {
            ev_OnUpdate -= ac;
        }
        public static void UnRegisterOnLateUpdate(System.Action ac)
        {
            ev_OnLateUpdate -= ac;
        }
        public static void UnRegisterOnFixedUpdate(System.Action ac)
        {
            ev_OnFixedUpdate -= ac;
        }

        public static void OnDestroy()
        {
            ev_OnUpdate = null;
            ev_OnLateUpdate = null;
            ev_OnFixedUpdate = null;
        }
    }




}

