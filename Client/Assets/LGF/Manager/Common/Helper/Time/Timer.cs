using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using LGF.Log;


namespace LGF
{
    namespace Timers
    {
        public enum TimeUnit
        {
            Millisecond = 1,
            Second = 1000,
            Minute = 1000 * 60,
            Hour = 1000 * 60 * 60,
            Day = 1000 * 60 * 60 * 24,
        }
    }


    namespace Timers
    {

        /* ----------------------------------------------------------------------------
         *                                关于unity执行思路
         *                   方法1： 任务队列执行完后 不执行 加入unity的主线程队列中执行
         *                   再继续执行  这样才能保证timer时间正确且正常调用unity程序  
         *                   不然系统定时器会出问题 或者 调用unity程序报错
         *                   --ps ： 不知道效果怎么样
         *                   方法2:
         *                   直接在unity 的update执行 LHTimer.update
         * ---------------------------------------------------------------------------
         */

        /// <summary>
        /// 缓冲队列
        /// </summary>
        public class BufferingQueue<T>
        {
            private static readonly object _lock = new object();                 //添加锁
            protected int idx;
            protected List<T>[] queue = new List<T>[2]
            {
            new List<T>(),new List<T>()
            };

            public int Count { get { return queue[idx].Count; } }   //不需要加锁 就算同时读写idx 也没有程序问题

            public void Add(T info)
            {
                lock (_lock)
                    queue[idx].Add(info);   //这里不能缓存tmp = idx操作  不然他可能在idx^1后进行操作 这样两线程就对同一个list进行操作了
            }

            public void Add(List<T> infos)
            {
                lock (_lock)
                    for (int i = 0; i < infos.Count; i++)
                        queue[idx].Add(infos[i]);
            }

            /// <summary>
            /// 不安全使用  保证GetList只在单个线程单方法使用并且管理 //不用ListPool  因为list是自动分配空间的会 新的list可能需要重新分配空间
            /// </summary>
            /// <returns></returns>
            public List<T> Get()
            {
                int lastidx = idx;
                int nexidx = idx ^ 1;
                queue[nexidx].Clear();
                lock (_lock)
                    idx = nexidx;
                return queue[lastidx];
            }




        }



        public class Timer
        {
            private readonly ObjectPool<TimeTask> m_timeTaskPool = new ObjectPool<TimeTask>((p) => p.Init(), null);
            private SortedDictionary<ulong, List<TimeTask>> m_timeTaskQueue = new SortedDictionary<ulong, List<TimeTask>>();  //时间任务队列
            private Dictionary<ulong, TimeTask> m_timeTaskDic = new Dictionary<ulong, TimeTask>();              //任务索引   
            private BufferingQueue<TimeTask> m_tmpAddTaskQueue = new BufferingQueue<TimeTask>();                 //添加任务
            private BufferingQueue<ulong> m_tmpDelTaskQueue = new BufferingQueue<ulong>();                      //删除任务
            private List<ulong> m_delTimeQueue = new List<ulong>();                                //清理任务队列 

            private static readonly object m_tidLock = new object();                 //锁
            private HashSet<ulong> m_tidHash = new HashSet<ulong>();         //id索引
            private List<ulong> m_tidDelHash = new List<ulong>();            //删除id索引  也有判定该事件是否删除作用


            private ulong m_tid = 0;
            private ulong m_nowTime { get { return m_timekeeping; } }
            private ulong m_timekeeping;  //时间
            //private System.Timers.Timer                         m_timer;        //定时出bug 2个不同的线程进行调用内部update  需要调用锁 加深拷贝 感觉一个线程就行 没必要用到线程池重复调用
            private Stopwatch m_stopWatch;
            private bool m_isHasUpdate;
            private Action<string> m_taskLog;
            private Action<Action<int>, int> m_taskHandle;
            uint m_interval;

            #region 注册


            /// <summary>
            /// 
            /// </summary>
            /// <param name="interval">毫秒单位,大于0时自动生成定时器 interval毫秒触发一次定时器</param>
            public Timer(uint interval = 0)
            {
                m_isHasUpdate = false;
                if (interval != 0)
                {
                    m_interval = interval;
                    m_isHasUpdate = true;
                    m_stopWatch = new Stopwatch();
                    m_stopWatch.Start();
                    //m_timer = new System.Timers.Timer(interval)
                    //{
                    //    AutoReset = true
                    //};

                    //m_timer.Elapsed += (object sender, ElapsedEventArgs args) =>
                    //{
                    //    Update();
                    //};
                    //m_timer.Start();
                    Thread thread = new Thread(Update);
                    thread.Start();
                }

            }

            public void SetLog(Action<string> log)
            {
                m_taskLog = log;
            }
            public void SetHandle(Action<Action<int>, int> handle)
            {
                m_taskHandle = handle;
            }

            #endregion


            //void PP()
            //{
            //    ListPool<TimeTask>.Get();
            //    ListPool<TimeTask>.Release();
            //}

            public void Update()
            {
                while (true)
                {
                    ulong lastTime = m_timekeeping;
                    m_timekeeping = (ulong)m_stopWatch.ElapsedMilliseconds;
                    int time = (int)(m_timekeeping - lastTime);
                    time = time >= m_interval ? 0 : (int)m_interval - time;
                    Thread.Sleep(time);
                    OnUpdate();
                }

            }



            /// <summary>
            /// 差值
            /// </summary>
            /// <param name="deltaTime"></param>
            public void Update(ulong deltaTime)
            {
                if (m_isHasUpdate)
                {
                    LogInfo("已有定时器了");
                    //查看自己的问题以及存在定时器了
                    return;
                }

                m_timekeeping += deltaTime;
                OnUpdate();
            }


            private void OnUpdate()
            {
                CheckDelTaskQueue();    //请按顺序执行
                CheckAddTaskQueue();
                CheckTimeTaskQueue();
                RecycleTid();
            }




            void CheckDelTaskQueue()
            {
                if (m_tmpDelTaskQueue.Count == 0) return;
                var dels = m_tmpDelTaskQueue.Get();

                for (int i = 0; i < dels.Count; i++)
                {
                    ulong taskid = dels[i];
                    if (!m_tidDelHash.Contains(taskid))         //已经被执行了。不需要在删除了
                    {
                        if (m_timeTaskDic.ContainsKey(taskid))              //是否在队列中
                        {
                            var task = m_timeTaskDic[taskid];
                            DelTimeTaskQueue(task);//清理任务队列
                            m_timeTaskDic.Remove(taskid);                  //移除
                            m_timeTaskPool.Release(task);
                        }
                        else          //不在队列中  在添加任务队列中
                        {
                            m_tidDelHash.Add(taskid);                      //添加删除   用于删除添加任务队列
                        }
                    }
                }
            }



            void CheckAddTaskQueue()
            {
                if (m_tmpAddTaskQueue.Count == 0) return;
                var adds = m_tmpAddTaskQueue.Get();
                for (int i = 0; i < adds.Count; i++)
                {
                    TimeTask task = adds[i];
                    ulong taskid = task.tid;
                    if (m_tidDelHash.Contains(taskid))
                    {   //在删除队列中    不添加
                        m_timeTaskPool.Release(task); continue;
                    }

                    //try
                    //{
                    //定时器添加
                    if (!m_timeTaskDic.ContainsKey(taskid))
                    {
                        m_timeTaskDic.Add(taskid, task);    //添加队列
                    }
                    //}
                    //catch (Exception e)
                    //{

                    //    LogInfo(e.ToString());
                    //}


                    AddTimeTaskQueue(task); //添加队列
                }
            }


            void CheckTimeTaskQueue()
            {
                if (m_tidDelHash.Count > 0) m_tidDelHash.Clear();   //只能在这里清除  如果有更好的位置的话

                if (m_timeTaskQueue.Count <= 0) return;

                var queue = m_timeTaskQueue.First();

                //记得null的判定
                if (queue.Key <= m_timekeeping)
                {
                    foreach (var item in m_timeTaskQueue)
                    {
                        if (item.Key > m_timekeeping) break;

                        for (int i = 0; i < item.Value.Count; i++)
                        {
                            TimeTask task = item.Value[i];


                            try
                            {
                                task.callback.Invoke(task.tid);
                            }
                            catch (Exception e)
                            {
                                e.DebugError();
                                //LogInfo(e.ToString());
                                //throw;
                            }

                            bool isAdd = true;

                            if (task.count > 0) //默认task.count = 0 表示定时器
                            {
                                task.count--;
                                if (task.count <= 0)
                                {
                                    m_timeTaskDic.Remove(task.tid);
                                    m_tidDelHash.Add(task.tid);     //可以换list
                                    m_timeTaskPool.Release(task);
                                    isAdd = false;
                                }
                            }

                            if (isAdd)
                            {
                                task.time += task.interval;
                                m_tmpAddTaskQueue.Add(task);    //这地方能优化 放入一个队列里面在添加
                            }

                        }
                        ListPool<TimeTask>.Release(item.Value);   //回收
                        m_delTimeQueue.Add(item.Key);     //清除列表
                    }


                    for (int i = 0; i < m_delTimeQueue.Count; i++)
                        m_timeTaskQueue.Remove(m_delTimeQueue[i]);

                    m_delTimeQueue.Clear();
                }
            }



            /// <summary>
            /// 添加任务队列
            /// </summary>
            void AddTimeTaskQueue(TimeTask task)
            {
                List<TimeTask> list;
                if (!m_timeTaskQueue.ContainsKey(task.time))
                {
                    list = ListPool<TimeTask>.Get();
                    m_timeTaskQueue.Add(task.time, list);
                }
                else
                {
                    list = m_timeTaskQueue[task.time];
                }
                list.Add(task);
            }


            /// <summary>
            /// 删除任务队列
            /// </summary>
            void DelTimeTaskQueue(TimeTask task)
            {
                //不错检测  如果出问题  直接报错方便修改  正常这里是不会报错的
                List<TimeTask> list = m_timeTaskQueue[task.time];
                int count = list.Count;
                list.Remove(task);
                if (count == list.Count)
                    LogInfo("------------出错--------");

                if (list.Count == 0)     //回收
                {
                    ListPool<TimeTask>.Release(list);
                    m_timeTaskQueue.Remove(task.time);
                }

            }


            private void LogInfo(string info)
            {
                if (m_taskLog != null)
                {
                    m_taskLog(info);
                }
                else
                {
                    Console.WriteLine(info);
                }
            }

            /// <summary>
            /// 删除任务
            /// </summary>
            /// <param name="tid"></param>
            public bool DelTask(ulong tid)
            {
                lock (m_tidLock)
                {
                    if (m_tidHash.Contains(tid))
                    {
                        m_tmpDelTaskQueue.Add(tid);
                        m_tidHash.Remove(tid);
                        //LogInfo("删除成功");
                        return true;
                    }
                    else
                    {
                        //LogInfo("删除失败");
                        return false;
                    }
                }
            }



            /// <summary>
            /// 延迟回调
            /// </summary>
            /// <param name="callback"></param>
            /// <param name="delay"></param>
            /// <returns></returns>
            public ulong AddTask(Action callback, double delay = 0.005)
            {
                return AddTask((ulong t) => callback.Invoke(), (ulong)(delay * (int)TimeUnit.Second), TimeUnit.Millisecond);
            }

            /// <summary>
            /// 添加任务 间隔interval  timeUnit时间单位  delay延迟  count次数为0时表示定时器  如果delay>0 那么 第一次等待delay
            /// </summary>
            public ulong AddTask(Action callback, ulong interval, TimeUnit timeUnit = TimeUnit.Second, int count = 1, ulong delay = 0)
            {
                return AddTask((ulong t) => callback.Invoke(), interval, timeUnit, count, delay);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public ulong AddTask(Action<ulong> callback, ulong interval, TimeUnit timeUnit = TimeUnit.Second, int count = 1, ulong delay = 0)
            {
                ulong tid = GetTid();
                //var _delay = 
                m_tmpAddTaskQueue.Add(m_timeTaskPool.Get().
                    Init(tid, callback, interval * (ulong)timeUnit, m_nowTime + (delay == 0 ? interval : delay) * (ulong)timeUnit, count));
                return tid;
            }


            private ulong GetTid()
            {
                lock (m_tidLock)
                {
                    ulong count = 1;
                    while (true)
                    {
                        m_tid++;
                        if (m_tid == ulong.MaxValue) m_tid = 1;
                        if (!m_tidHash.Contains(m_tid))
                        {
                            m_tidHash.Add(m_tid); break;
                        }

                        count++;
                        if (count == ulong.MaxValue)
                        {
                            LogInfo("出错 任务数量过多"); return 0;
                        }
                    }
                }
                return m_tid;
            }

            void RecycleTid()
            {
                lock (m_tidLock)
                {
                    foreach (var tid in m_tidDelHash)
                    {
                        if (m_tidHash.Contains(tid))
                            m_tidHash.Remove(tid);
                        //如果m_tidHash没有tid 那边在del已经删除
                    }
                }
            }


            //public void ShowSize()
            //{
            //    int m_timeTaskQueuesize = Marshal.SizeOf(m_timeTaskQueue);
            //    int m_timeTaskDicsize = Marshal.SizeOf(m_timeTaskDic);
            //    LogInfo($" size:  m_timeTaskQueue:{m_timeTaskQueuesize}     m_timeTaskDic:{m_timeTaskDicsize}    ");
            //}

        }



        internal class TimeTask
        {
            public ulong tid;
            public Action<ulong> callback;
            public ulong time;  //单位：毫秒  //触发时间
            public ulong interval;  //间隔
            public int count;   //数量 默认0为定时触发  大于1为
            //public bool isDel;  //是否删除

            public void Init()
            {
                callback = null;
                interval = 0;
                time = 0;
                tid = 0;
                count = 0;
                //isDel = false;
            }

            public TimeTask Init(ulong tid_, Action<ulong> callback_, ulong interval_, ulong time_, int count_)
            {
                callback = callback_;
                interval = interval_;
                time = time_;
                count = count_;
                tid = tid_;
                return this;
            }

        }

    }

}

