/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/24 20:17:40
/// 功能描述:  
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using LGF;
using LGF.Log;
using LGF.Timers;

namespace LGF.Server
{
    public class TimeManager : ModuleSingletonBase<TimeManager>
    {
        LGF.Timers.Timer timer = new LGF.Timers.Timer();
        public Stopwatch stopwatch = new Stopwatch();
        public ulong curTime => lastTime;   //当前时间
        ulong lastTime;
        public override void Init()
        {
            base.Init();
            //OnFixedUpdate 在局域网房间对战时  比较稳定 所以用这个
            LGFEntry.RegisterOnFixedUpdate(OnFixedUpdate);
            stopwatch.Start();
            sLog.Debug("S_TimeManager Init");
        }


        private void OnFixedUpdate()
        {
            var curTime = (ulong)stopwatch.ElapsedMilliseconds;
            //sLog.Debug("OnFixedUpdate time {0} ", curTime - lastTime);
            timer.Update(curTime - lastTime);
            //sLog.Debug("----OnFixedUpdate--");
            lastTime = curTime;
        }

        /// <summary>
        /// 添加任务 间隔interval  timeUnit时间单位  delay延迟  count次数为0时表示定时器  如果delay>0 那么 第一次等待delay
        /// </summary>
        public ulong AddTask(Action callback, ulong interval, TimeUnit timeUnit = TimeUnit.Second, int count = 1, ulong delay = 0)
        {
            return timer.AddTask(callback, interval, timeUnit, count, delay);
        }



        public ulong AddTask(Action<ulong> callback, ulong interval, TimeUnit timeUnit = TimeUnit.Second, int count = 1, ulong delay = 0)
        {
            return timer.AddTask(callback, interval, timeUnit, count, delay);
        }


        public override void Close()
        {
            LGFEntry.UnRegisterOnFixedUpdate(OnFixedUpdate);

            base.Close();
        }
    }


}


