using LGF.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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

        int m_SpinLock = 0;
        public T Get()
        {
            T element = default;
            bool isNew = false;
            //自旋锁  感觉比SpinLock效率高
            while (Interlocked.Exchange(ref m_SpinLock, 1) != 0)    {
                Thread.SpinWait(1);//自旋锁等待
            }

            if (m_Stack.Count == 0)
            {
                isNew = true;
                countAll++;
            }
            else
            {
                element = m_Stack.Pop();
            }
            //释放锁：将_SpinLock重置会0；
            Interlocked.Exchange(ref m_SpinLock, 0);

            if (isNew)
                element = new T();


            m_ActionOnGet?.Invoke(element);
            return element;
        }

        public void Release(T element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                this.DebugError($"Internal error. Trying to destroy object that is already released to pool. type: {typeof(T)}");
            m_ActionOnRelease?.Invoke(element);

            //自旋锁 
            while (Interlocked.Exchange(ref m_SpinLock, 1) != 0)  {   
                Thread.SpinWait(1);//自旋锁等待
            }
            m_Stack.Push(element);
            Interlocked.Exchange(ref m_SpinLock, 0);
         

        }

        public void Clear()
        {
            m_Stack.Clear();
        }
    }



}
