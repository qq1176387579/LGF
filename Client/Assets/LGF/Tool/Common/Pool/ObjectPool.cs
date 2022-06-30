using LGF.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 后面有时间去实现一个 线程安全队列 做缓冲池
/// </summary>

namespace LGF
{
    public class ObjectPool<T> where T : new()
    {
        private readonly Stack<T> m_Stack = new Stack<T>();
        private readonly Action<T> m_ActionOnGet;
        private readonly Action<T> m_ActionOnRelease;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }

        public ObjectPool(Action<T> actionOnGet, Action<T> actionOnRelease)
        {
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
        }

        public T Get()
        {
            T element;
            lock (m_Stack)
                if (m_Stack.Count == 0)
                {
                    element = new T();
                    countAll++;
                }
                else
                {
                    element = m_Stack.Pop();
                }
            m_ActionOnGet?.Invoke(element);
            return element;
        }

        public void Release(T element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                this.DebugError("Internal error. Trying to destroy object that is already released to pool.");
            m_ActionOnRelease?.Invoke(element);
            lock (m_Stack)
                m_Stack.Push(element);
        }

        public void Clear()
        {
            m_Stack.Clear();
        }
    }





    //public static class StringPoolHelper
    //{
    //    public static void Release(this StringBuilder builder)
    //    {
    //        StringPool.Release(builder);
    //    }
    //}


}

