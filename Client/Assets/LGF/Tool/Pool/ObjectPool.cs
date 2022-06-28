using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 后面有时间去实现一个 线程安全队列 做缓冲池
/// </summary>

namespace LGF
{
    public class ObjectPool<T> where T : new()
    {
        private readonly Stack<T> m_Stack = new Stack<T>();
        private readonly UnityAction<T> m_ActionOnGet;
        private readonly UnityAction<T> m_ActionOnRelease;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }

        public ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
        {
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
        }

        public T Get()
        {
            T element;
            lock(m_Stack)
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
                UnityEngine.Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            m_ActionOnRelease?.Invoke(element);
            lock (m_Stack)
                m_Stack.Push(element);
        }

        public void Clear()
        {
            m_Stack.Clear();
        }
    }

    /// <summary>
    /// 创建List<Vector3> m_Positions = ListPool<Vector3>.Get();
    /// 销毁ListPool<Vector3>.Release(m_Positions);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ListPool<T>
    {
        private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, l => l.Clear());

        public static List<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }

    }



    //线程不安全
    public static class StringPool
    {
        private static int MaxCount = 10000;

        private static readonly ObjectPool<StringBuilder> Pool = new ObjectPool<StringBuilder>(Clear, null);

        static void Clear(StringBuilder s)
        {
            if (Pool.countAll >= MaxCount)
            {
                UnityEngine.Debug.LogError("Pool count reach to MaxCount.");
            }
            s.Remove(0, s.Length);
        }

        public static StringBuilder GetStringBuilder()
        {
            StringBuilder stringBuilder = Pool.Get();
            return stringBuilder;
        }

        public static void Release(StringBuilder toRelease)
        {
            if (Pool.countAll >= MaxCount)
            {
                UnityEngine.Debug.LogError("Pool count reach to MaxCount.");
            }

            Pool.Release(toRelease);
        }

        public static string Concat(string s1, string s2)
        {
            StringBuilder stringBuilder = Pool.Get();
            stringBuilder.Append(s1);
            stringBuilder.Append(s2);
            string result = stringBuilder.ToString();
            Release(stringBuilder);
            return result;
        }

        public static string Concat(string s1, string s2, string s3)
        {
            StringBuilder stringBuilder = Pool.Get();
            stringBuilder.Append(s1);
            stringBuilder.Append(s2);
            stringBuilder.Append(s3);
            string result = stringBuilder.ToString();
            Release(stringBuilder);
            return result;
        }

        public static string Concat(string s1, string s2, string s3, string s4)
        {
            StringBuilder stringBuilder = Pool.Get();
            stringBuilder.Append(s1);
            stringBuilder.Append(s2);
            stringBuilder.Append(s3);
            stringBuilder.Append(s4);
            string result = stringBuilder.ToString();
            Release(stringBuilder);
            return result;
        }

        public static string Format(string src, params object[] args)
        {
            StringBuilder stringBuilder = Pool.Get();
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.AppendFormat(src, args);
            string result = stringBuilder.ToString();
            Release(stringBuilder);
            return result;
        }



    }


    public class Poolable<T> : System.IDisposable where T : Poolable<T>, new()
    {
        [System.NonSerialized]
        private static readonly ObjectPool<T> _pool = new ObjectPool<T>(_OnGet, _OnRelease);
        [System.NonSerialized]
        protected int releaseCount;   //回收次数
        public static T GetPooled()
        {
            return _pool.Get();
        }

        public static T Get()
        {
            return _pool.Get();
        }

        public static void ClearPool()
        {
            _pool.Clear();
        }
        static void _OnRelease(Poolable<T> _this)
        {
            _this.OnRelease();
        }
        static void _OnGet(Poolable<T> _this)
        {
            _this.OnGet();
        }

        public void Release()
        {
            if (!IsRelease())
            {
                //lock(_pool)   //后续看情况要不要加锁  多线程
                _pool.Release((T)this);
            }
            else
            {
                UnityEngine.Debug.LogError($">>>>>>>>>>>> 重复回收了  releaseCount: {releaseCount}  GetType: {GetType().UnderlyingSystemType}");
            }
                
        }


        protected virtual void OnRelease()
        {
            releaseCount++;
        }

        protected virtual void OnGet()
        {
            releaseCount = 0;
        }

        /// <summary>
        /// 是否时回收状态
        /// </summary>
        /// <returns></returns>
        public bool IsRelease()
        {
            return releaseCount >= 1;
        }

        public void Dispose()
        {
             Release();    //回收过不回收
        }
    }


}

