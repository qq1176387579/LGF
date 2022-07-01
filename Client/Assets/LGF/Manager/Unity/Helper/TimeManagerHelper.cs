/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/1 22:32:47
/// 功能描述:  时间管理器帮助类
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;


namespace LGF
{
    public static class TimeManagerHelper
    {
        public static ulong Delay(this object obj, System.Action callback, float delayTime = 0.1f) => TimeManager.Instance.AddTask(callback, delayTime);
        public static ulong Delay(this object obj, System.Action<ulong> callback, float delayTime = 0.1f) => TimeManager.Instance.AddTask(callback, delayTime);
    }

}
