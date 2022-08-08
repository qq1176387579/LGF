/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/7 23:06:32
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using System.Net;
using System.Net.NetworkInformation;

namespace LGF.Util
{
    /// <summary>
    /// 安卓端需要等待android的数据传过来 pc端不需要
    /// </summary>
    public class MacAddressUtils : SingletonBase<MacAddressUtils>
    {
        string uuid; //获得UUID
        protected override void OnNew()
        {
            base.OnNew();
#if UNITY_ANDROID && !UNITY_EDITOR 
            Android.AndroidMsgCenter.RegisterOnMsg(Android.AndroidMsgDefine.GetUUIDResp, (a) =>
            {
                uuid = a.strParam1;
            });
#endif
        }

        public bool HasUUID()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return uuid != null;
#else
            return true;
#endif
        }


        public string GetUUID()
        {
            if (!HasUUID())
                return null;

#if !USE_ANDROID || UNITY_EDITOR
            if (uuid == null) uuid = GetMacAddress(); 
#endif
            return uuid;
        }



        static string GetMacAddress()
        {
            string physicalAddress = "";
            NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adaper in nice)
            {
                //Console.WriteLine(adaper.Description);
                if (adaper.Description == "en0")
                {
                    physicalAddress = adaper.GetPhysicalAddress().ToString();
                    break;
                }
                else
                {
                    physicalAddress = adaper.GetPhysicalAddress().ToString();
                    if (physicalAddress != "")
                    {
                        break;
                    };
                }
            }
            return physicalAddress;
        }
    }

}

