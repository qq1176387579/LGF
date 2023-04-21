/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/1 22:32:47
/// 功能描述:  时间管理器帮助类
****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

namespace LGF
{
    public static class TimeManagerHelper
    {
        public static ulong Delay(this object obj, System.Action callback, float delayTime = 0.001f) => TimeManager.Instance.AddTask(callback, delayTime);
        public static ulong Delay(this object obj, Action<ulong> callback, float delayTime = 0.001f) => TimeManager.Instance.AddTask(callback, delayTime);

        /// <summary>
        /// 延迟函数  有GC
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="delayTime"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static ulong Delay(this object obj, float delayTime, Action callback) => TimeManager.Instance.AddTask(callback, delayTime);

        /// <summary>
        /// 该版本如果缓存callback 就无GC
        /// 参考 AudioManager.DelayRelease 使用
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="delayTime"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static ulong Delay(this object obj, float delayTime, Action<ulong> callback) => TimeManager.Instance.AddTask(callback, delayTime);
    }

}
