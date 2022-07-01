/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/30 23:38:33
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using LGF;
using LGF.Log;

namespace LGF.Timers
{
    /// <summary>
    /// 简单异步定时器
    /// </summary>
    public class SimpleAsynTimer
    {
        System.Action m_ac;
        uint m_interval;
        Stopwatch m_stopWatch;
        bool m_disposed = false;
        private ulong m_timekeeping;  //时间
        Task m_task;
        bool isdelay = false;   //后为了保证安全 换成原子
        public SimpleAsynTimer(System.Action ac, uint interval = 10)
        {
            m_interval = interval;
            m_stopWatch = new Stopwatch();
            m_stopWatch.Start();
            m_ac = ac;
            m_task = Task.Run(() =>
            {
                isdelay = true; //初始化
                while (!m_disposed)
                {
                    ulong lastTime = m_timekeeping;
                    m_timekeeping = (ulong)m_stopWatch.ElapsedMilliseconds;
                    int time = (int)(m_timekeeping - lastTime);
                    time = time >= m_interval ? 0 : (int)m_interval - time;
                    Thread.Sleep(time);
                    //Task.Delay(time);   //严格时间
                    isdelay = false;    //延迟中
                    if (m_disposed) return;    
                    OnRun();
                }
            });
            
        }

        void OnRun()
        {
            m_ac?.Invoke();
            isdelay = true;
        }


        /// <summary>
        /// 线程安全
        /// </summary>
        public void Disposed()
        {
            m_disposed = true;
            m_ac = null;

            //延迟等待中 不做处理  表示没有执行OnRun 所以不需要阻塞
            //如果非等待中 表示需要阻塞线程 等待OnRun完成
            if (!isdelay)   //非延迟中
                m_task.Wait();

            m_task = null;
            m_stopWatch = null;
        }
    
    }

}
