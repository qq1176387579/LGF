using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using System.Diagnostics;
using LGF.Timers;

namespace LGF
{

    public class TimeManager : MonoSingleton<TimeManager>
    {
        #region data
        DateTime InitDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private long m_timeStampMsec;    //毫秒
        private long m_timeStampSec;     //秒
        private long m_netTimeStampSec;    //网络时间戳-秒
        private long m_lastTimeStampSec;    //上一次时间戳
        private long m_lastTimeStampMsec;    //上一次时间戳
        Timers.Timer m_timer = new Timers.Timer();
        Timers.Timer m_timer2 = new Timers.Timer(); //基于时间戳
        //HttpUnixTimestamp m_httpUnixTimestamp; 

        /// <summary>
        /// 本地时间戳 单位秒
        /// </summary>
        public long TimeStampSec { get { return m_timeStampSec; } }     //本地时间

        /// <summary>
        /// 本地时间戳 单位毫秒
        /// </summary>
        public long TimeStampMsec { get { return m_timeStampMsec; } }   //毫秒 本地时间

        /// <summary>
        /// 网络时间戳 秒
        /// </summary>
        public long NetTimeStampSec { get { return m_netTimeStampSec; } }

        #endregion

        private void Awake()
        {
            m_netTimeStampSec = 0;
            m_timer.SetLog(UnityEngine.Debug.LogError);

            //基础值
            TimeSpan ts = DateTime.UtcNow - InitDateTime;
            m_timeStampMsec = Convert.ToInt64(ts.TotalMilliseconds);
            m_lastTimeStampSec = m_timeStampMsec;

            m_timeStampSec = Convert.ToInt64(ts.TotalSeconds);
            m_lastTimeStampSec = m_timeStampSec;

            //m_httpUnixTimestamp = new HttpUnixTimestamp();
            //m_httpUnixTimestamp.Init(MonoManager.Instance.StartCoroutine, MonoManager.Instance.StopCoroutine, OnNetTimeCallback).Start();
          
        }

        void OnNetTimeCallback(long timeStampSec_)
        {
            m_netTimeStampSec = timeStampSec_;
            EventManager.Instance.BroadCastEvent(GameEventType.OnNetTimeChange, m_netTimeStampSec);
        }

        private void Update()
        {
            UpdateTimeStamp();
            //判定切后台 时间会Time.deltaTime吗 

            //后面问策划手机 后台执行要暂定吗 应该是占停的
            ulong tmp = (ulong)(Time.deltaTime * (int)TimeUnit.Second);
            m_timer.Update(tmp);     //受time影响
            m_timer2.Update((ulong)(TimeStampMsec - m_lastTimeStampMsec));      //不受time影响
        }

        /// <summary>
        /// 更新时间戳
        /// </summary>
        private void UpdateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - InitDateTime;
            m_lastTimeStampMsec = m_timeStampMsec;
            m_timeStampMsec = Convert.ToInt64(ts.TotalMilliseconds);

            m_lastTimeStampSec = m_timeStampSec;
            m_timeStampSec = Convert.ToInt64(ts.TotalSeconds);
        }


        #region 基于unity时间

        public bool DelTask(ulong tid)
        {
            return m_timer.DelTask(tid);
        }

        /// <summary>
        /// 延迟回调
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public ulong AddTask(Action callback, double delay = 0.00005)
        {
            return m_timer.AddTask(callback, delay);
        }
        /// <summary>
        /// 添加任务 间隔interval  timeUnit时间单位  delay延迟  count次数为0时表示定时器
        /// </summary>
        public ulong AddTask(Action callback, ulong interval, TimeUnit timeUnit = TimeUnit.Second, int count = 1, ulong delay = 0)
        {
            return m_timer.AddTask(callback, interval, timeUnit, count, delay);
        }

        public ulong AddTask(Action<ulong> callback, ulong interval, TimeUnit timeUnit = TimeUnit.Second, int count = 1, ulong delay = 0)
        {
            return m_timer.AddTask(callback, interval, timeUnit, count, delay);
        }

        #endregion

        #region 基于本地时间戳的定时器 

        /// <summary>
        /// 基于本地时间戳的定时器 
        /// </summary>
        /// <returns></returns>
        public ulong AddTimerByTimeStamp(Action callback, ulong interval, ulong delay = 0, TimeUnit timeUnit = TimeUnit.Second)
        {
            return m_timer2.AddTask(callback, interval, timeUnit, 0, delay);
        }


        /// <summary>
        /// 基于本地时间戳的定时器
        /// </summary>
        /// <returns></returns>
        public ulong AddTaskByTimeStamp(Action callback, ulong interval, TimeUnit timeUnit = TimeUnit.Second, int count = 1, ulong delay = 0)
        {
            return m_timer2.AddTask(callback, interval, timeUnit, count, delay);
        }

        /// <summary>
        /// 基于本地时间戳的定时器
        /// </summary>
        /// <returns></returns>
        public ulong AddTaskByTimeStamp(Action<ulong> callback, ulong interval, TimeUnit timeUnit = TimeUnit.Second, int count = 1, ulong delay = 0)
        {
            return m_timer2.AddTask(callback, interval, timeUnit, count, delay);
        }


        #endregion

        #region 携程延迟

        /// <summary>
        /// 延迟回调 基于携程
        /// </summary>
        /// <param name="time"></param>
        /// <param name=""></param>
        public static Coroutine Delay(Action _action, float time = 0)
        {
            return MonoManager.Instance.StartCoroutine(DelayEvent(_action, time));
        }

        private static IEnumerator DelayEvent(Action _action, float time)
        {
            yield return new WaitForSeconds(time); //WaitForHelper.Instance.GetWaitForSecond(time);
            _action();
        }
        #endregion

        #region 测试性能消耗
        class StopwatchInfo
        {
            public Stopwatch stopwatch;
            public string info;
            public bool isEnd;

            public StopwatchInfo()
            {
                stopwatch = new Stopwatch();
                isEnd = true;
            }
        }

        Dictionary<int, StopwatchInfo> stopwatchDic = new Dictionary<int, StopwatchInfo>();

        public void StartStopwatch(int id, bool isDebug, string info = "default")
        {
            if (!isDebug)
            {
                //if (!GameConst.UNITY_EDITOR) return;
            }

            StopwatchInfo sinfo;
            if (!stopwatchDic.TryGetValue(id, out sinfo))
            {
                sinfo = new StopwatchInfo();
                stopwatchDic.Add(id, sinfo);
            }

            sinfo.info = info;

            if (!sinfo.isEnd)
            {
                Stopwatch_LogError($"StopwatchInfo >> id: {id}  info: {sinfo.info}  is starting".ToColor("#00C5FF"));
                return;
            }

            sinfo.stopwatch.Start();
            sinfo.isEnd = false;
        }

        public void StartStopwatch(int id, string info = "default")
        {
            StartStopwatch(id, false, info);
        }

        public void EndStopwatch(int id,bool isDebug, string str = "")
        {
            if (!isDebug)
            {
                //if (!GameConst.UNITY_EDITOR) return;
            }


            StopwatchInfo sinfo;
            if (!stopwatchDic.TryGetValue(id, out sinfo))
            {
                Stopwatch_LogError($"StopwatchInfo >> id: {id}  info: {sinfo.info}  not start".ToColor("#00C5FF"));
                return;
            }

            if (sinfo.isEnd)
            {
                Stopwatch_LogError($"StopwatchInfo >> id: {id}  info: {sinfo.info}  not start".ToColor("#00C5FF"));
                return;
            }

            Stopwatch_Log($"StopwatchInfo >> id: {id.ToColor("#00C5FF")}  info: {sinfo.info.ToColor("#00C5FF")}   time:{ sinfo.stopwatch.ElapsedMilliseconds.ToColor("#00C5FF")}ms");


            sinfo.isEnd = true;
            sinfo.stopwatch.Reset();
            //sinfo.stopwatch.Stop();
        }

        public void EndStopwatch(int id,string str = "")
        {
            EndStopwatch(id, false, str);
        }

        public void Stopwatch_LogError(string str)
        {
            AddTask(()=> UnityEngine.Debug.LogError(str));
        }


        public void Stopwatch_Log(string str)
        {
            AddTask(() => UnityEngine.Debug.Log(str));
        }

        #endregion




    }

    
}