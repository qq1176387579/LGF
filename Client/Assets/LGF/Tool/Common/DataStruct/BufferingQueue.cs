/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/5 0:04:00
/// 功能描述:  缓冲队列 
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

namespace LGF.DataStruct
{
    /// <summary>
    /// 缓冲队列   该队列 线程不安全 请谨慎使用
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
            queue[nexidx].Clear();  //定时器也是这里的  只不过定时器那边外面有进行回收  其他程序要使用要保证 单线程
            lock (_lock)
                idx = nexidx;
            return queue[lastidx];
        }

    }





    /// <summary>
    /// 线程安全
    /// 缓冲队列  跟上面不一样的地方是 不在Get的时候清理数据
    /// </summary>
    public class NBufferingQueue<T>
    {        
        protected int idx;
        protected List<T>[] queue = new List<T>[2]
        {
            new List<T>(),new List<T>()
        };
        bool needClear = false;
        System.Action<T> OnClearEvt;


        public int Count { get { return queue[idx].Count; } }   //

        public void Add(T info)
        {
            lock (queue)
            {
                queue[idx].Add(info);   //
            }
               
        }

        //public void Add(List<T> infos)
        //{
        //    lock (queue)
        //    {
        //        CheckClear();
        //        for (int i = 0; i < infos.Count; i++)
        //            queue[idx].Add(infos[i]);
        //    }
               
        //}


        public bool CanGet()
        {
            return Count > 0;
        }

        /// <summary>
        /// 不安全使用  需要配合 CanGet 使用  不然出问题
        /// </summary>
        /// <returns></returns>
        public List<T> Get()
        {
            if (!CanGet()) return null;

            if (needClear)
            {
                sLog.Error("请先 Clear"); //请先清理
                return null;
            }

            int lastidx = idx;
            int nexidx = idx ^ 1;
          
            lock (queue)
            {
                needClear = true;
                idx = nexidx;
            }
               
            return queue[lastidx];
        }

        public void Clear()
        {
            if (!needClear)
                return;

            int lastidx = idx ^ 1;
            needClear = false;
            var list = queue[lastidx];
            //出现报错信息 System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            //之前的逻辑可能 在访问的时候删除  所以Clear 改为Get调用玩后执行
            for (int i = list.Count - 1; i >= 0; i--)   
            {
                OnClearEvt?.Invoke(list[i]);
                list.RemoveAt(i);
            }
        }
        public void OnClear(System.Action<T> evt)
        {
            OnClearEvt = evt;
        }
    }

}