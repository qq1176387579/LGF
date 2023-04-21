using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

#if USE_ANDROID
using LitJson;
#endif

using System;

namespace LGF.Android
{
    /// <summary>
    /// 安卓消息  需要开启 USE_ANDROID 才能使用
    /// </summary>
    public partial class AndroidMsgManager : MonoSingleton<AndroidMsgManager>
    {
        public void Init()
        {
            AndroidMsgCenter.Instance.Init();
            
            //sLog.Error(AppConfig.Data.appInfo.PackName);
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            Debug.Log("packname : " + Application.identifier);
            //AppConfig.Instance.serverInfo.path
            //#if UNITY_ANDROID && USE_ANDROID
#if UNITY_ANDROID && !UNITY_EDITOR && USE_ANDROID
            m_GameHelperJavaClass = new AndroidJavaClass(StringPool.Concat(Application.identifier, ".GameHelper"));
#endif
        }


        //#if UNITY_ANDROID && USE_ANDROID
#if UNITY_ANDROID && !UNITY_EDITOR && USE_ANDROID
        AndroidJavaClass m_GameHelperJavaClass = null;

        /// <summary>
        /// 从Unity发送消息到android平台
        /// </summary>
        public void SendUnityMessage(int iMsgId, int iParam1 = 0, int iParam2 = 0, int iParam3 = 0, int iParam4 = 0, string strParam1 = "", string strParam2 = "", string strParam3 = "", string strParam4 = "")
        {
            if (m_GameHelperJavaClass == null) return;

            m_GameHelperJavaClass.CallStatic("SendUnityMessage", iMsgId, iParam1, iParam2, iParam3, iParam4, strParam1, strParam2, strParam3, strParam4);
        }

        /// <summary>
        /// 从平台获取整形数据
        /// </summary>
        public int GetInt(int type)
        {
            if (m_GameHelperJavaClass == null) return 0;

            return m_GameHelperJavaClass.CallStatic<int>("GetInt", type);
        }

        /// <summary>
        /// 从平台获取long数据
        /// </summary>
        public long GetLong(int type)
        {
            if (m_GameHelperJavaClass == null) return 0;

            return m_GameHelperJavaClass.CallStatic<long>("GetLong", type);
        }

        /// <summary>
        /// 从平台获取long数据
        /// </summary>
        public long GetLong2(int type, int iParam1 = 0, int iParam2 = 0, int iParam3 = 0, int iParam4 = 0, string strParam1 = "", string strParam2 = "", string strParam3 = "", string strParam4 = "")
        {
            if (m_GameHelperJavaClass == null) return 0;

            return m_GameHelperJavaClass.CallStatic<long>("GetLong2", type, iParam1, iParam2, iParam3, iParam4, strParam1, strParam2, strParam3, strParam4);
        }

        /// <summary>
        /// 从平台获取string
        /// </summary>
        public string GetString(int type)
        {
            if (m_GameHelperJavaClass == null) return string.Empty;

            return m_GameHelperJavaClass.CallStatic<string>("GetString", type);
        }
#else
        /// <summary>
        /// 从Unity发送消息到android平台
        /// </summary>
        public void SendUnityMessage(int iMsgId, int iParam1 = 0, int iParam2 = 0, int iParam3 = 0, int iParam4 = 0, string strParam1 = "", string strParam2 = "", string strParam3 = "", string strParam4 = "")
        {
            Log.sLog.Debug("-----iMsgId--------" + iMsgId);
            SDKMsg msg = new SDKMsg();
            msg.iMsgId = iMsgId + 100000;

            switch (iMsgId)
            {
                case 100: msg.strParam1 = "Test"; break;
                case 1000: msg.iPararm1 = 1; break;
                default:
                    break;
            }

            msgQueue.Enqueue(msg);
        }

        /// <summary>
        /// 从平台获取整形数据
        /// </summary>
        public int GetInt(int type)
        {
            return 0;
        }

        /// <summary>
        /// 从平台获取long数据
        /// </summary>
        public long GetLong(int type)
        {
            return 0;
        }

        /// <summary>
        /// 从平台获取long数据
        /// </summary>
        public long GetLong(int type, int iParam1 = 0, int iParam2 = 0, int iParam3 = 0, int iParam4 = 0, string strParam1 = "", string strParam2 = "", string strParam3 = "", string strParam4 = "")
        {
            return 0;
        }

        /// <summary>
        /// 从平台获取string
        /// </summary>
        public string GetString(int type)
        {
            return string.Empty;
        }

#endif

        private Queue<SDKMsg> msgQueue = new Queue<SDKMsg>();
        //private ObjectPool<SDKMsg> msgPool = new ObjectPool<SDKMsg>(null, null);
        //后面改成类 ILRuntime 传值类型效果不行。


        void Update()
        {
            while (msgQueue.Count > 0)
            {
                SDKMsg msg = msgQueue.Dequeue();
                EventManager.Instance.BroadCastEvent(GameEventType.AndroidMsg, msg);
            }
        }


#if USE_ANDROID

        /// <summary>
        /// 2022-07-07 22:11:32.452 8916-8965/com.defaultcompany.myframework E/Unity: MacAddress Update  
        /// E/Unity: --------ff---
        /// E/Unity:  msg 100002 0  30f399e6-204a-4d0f-97ee-a826613620f6
        /// E/Unity: --------ff---
        /// E/Unity:  msg 100002 0  30f399e6-204a-4d0f-97ee-a826613620f6
        /// 只会在update之后调用
        /// </summary>
        /// <param name="param"></param>
        void OnMessage(string param)
        {
            JsonData jd = JsonMapper.ToObject(param);
            SDKMsg msg;
            msg.iMsgId = (int)jd["iMsgId"];
            msg.iPararm1 = (int)jd["iPararm1"];
            msg.iPararm2 = (int)jd["iPararm2"];
            msg.iPararm3 = (int)jd["iPararm3"];
            msg.strParam1 = (string)jd["strParam1"];
            msg.strParam2 = (string)jd["strParam2"];
            msg.strParam3 = (string)jd["strParam3"];
            msgQueue.Enqueue(msg);
            //sLog.Error("--------ff---");
        }

#endif



    }

    public struct SDKMsg
    {
        public int iMsgId;
        public int iPararm1;
        public int iPararm2;
        public int iPararm3;
        public string strParam1;
        public string strParam2;
        public string strParam3;
    }



}