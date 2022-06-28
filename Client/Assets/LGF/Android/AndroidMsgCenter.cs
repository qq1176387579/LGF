/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/24 4:19:09
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;
using System;

namespace LGF.Android
{
    public class AndroidMsgCenter : SingletonBase<AndroidMsgCenter>
    {
        Dictionary<AndroidMsgDefine, Action<SDKMsg>> OnMsgEvent = new Dictionary<AndroidMsgDefine, Action<SDKMsg>>();
        public override void Init()
        {
            EventManager.Instance.AddListener<SDKMsg>(GameEventType.AndroidMsg, OnSDKMsg);
            //SendMsg(1, 2, 3, 4, 5, "1", "2", "3", "4");
        }

        /// <summary>
        /// SDK 注册回调    先这样简单写 
        /// </summary>
        public void RegisterOnMsg(AndroidMsgDefine type, Action<SDKMsg> action)
        {
            _Instance.RegisterOnMsgEx(type, action);
        }

        public void RegisterOnMsgEx(AndroidMsgDefine type, Action<SDKMsg> action)
        {
            if (!OnMsgEvent.ContainsKey(type))
            {
                OnMsgEvent.Add(type, action);
            }
            else
            {
                OnMsgEvent[type] += action;
            }
        }



        public static void SendMsgStr(AndroidMsgDefine msgid, string strParam1 = "", string strParam2 = "", string strParam3 = "", string strParam4 = "")
        {
            AndroidMsgManager.Instance.SendUnityMessage((int)msgid, 0, 0, 0, 0, strParam1, strParam2, strParam3, strParam4);
        }

        public static void SendMsg(int iMsgId, int iParam1 = 0, int iParam2 = 0, int iParam3 = 0, int iParam4 = 0, string strParam1 = "", string strParam2 = "", string strParam3 = "", string strParam4 = "")
        {
            AndroidMsgManager.Instance.SendUnityMessage(iMsgId, iParam1, iParam2, iParam3, iParam4, strParam1, strParam2, strParam3, strParam4);
        }

        ///  以后有时间优先流程 感觉传2个数组就行
        public void SendMsg(AndroidMsgDefine msgid, int iParam1 = 0, int iParam2 = 0, int iParam3 = 0, int iParam4 = 0, string strParam1 = "", string strParam2 = "", string strParam3 = "", string strParam4 = "")
        {
            AndroidMsgManager.Instance.SendUnityMessage((int)msgid, iParam1, iParam2, iParam3, iParam4, strParam1, strParam2, strParam3, strParam4);
        }

        /// <summary>
        /// OnSDKMsg 回调
        /// </summary>
        /// <param name="msg"></param>
        private void OnSDKMsg(SDKMsg msg)
        {
            Debug.LogError(" msg " + msg.iMsgId + " " + msg.iPararm1 + "  " + msg.strParam1);
            if (OnMsgEvent.TryGetValue((AndroidMsgDefine)msg.iMsgId, out Action<SDKMsg> onSDKMsg))
            {
                onSDKMsg?.Invoke(msg);
            }
        }




    }

}

