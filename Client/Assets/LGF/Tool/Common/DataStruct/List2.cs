/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/5 11:12:08
/// 修改日期:  
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
#if MODULE_MATHEMATICS
using Unity.Mathematics;
#endif
using LGF;
using LGF.Log;

namespace LGF.DataStruct
{
    public interface IList2Idx<T>
    {
        int idx { get; set; }
    }


    internal class List2_IEnumerator<T> : Poolable<List2_IEnumerator<T>>, IEnumerator<T> where T : IList2Idx<T>
    {
        List2<T> list;
        int position = -1;
        public static List2_IEnumerator<T> Get(List2<T> list_)
        {
            var tmp = Get();
            tmp.list = list_;
            return tmp;
        }

        bool IEnumerator.MoveNext()
        {
            position++;
            return position < list.Count;
        }

        public void Reset()
        {
            //Debug.LogError("List2  Reset ");
            position = -1;
        }

        T IEnumerator<T>.Current => list[position];

        object IEnumerator.Current => list[position];


        public override void Dispose()
        {
            list = null;
            Reset();
            base.Dispose();     //回收
        }

    }

    public class List2<T> : IEnumerable<T> where T : IList2Idx<T>
    {
        const int IdxOffset = 30;   //索引偏差值 用来判断是否初始化的
        const int IdxOffsetVal = 1 << IdxOffset;

        public List2() => Count = 0;

        /// <summary>
        /// 默认分配大小
        /// </summary>
        /// <param name="size"></param>
        public List2(int size) : this()
        {
            entitys = (T[])System.Array.CreateInstance(typeof(T), size); 
        }

        T[] entitys;
        public bool InForEach { get; private set; }

        public T this[int idx] { get => entitys[idx]; }

        public int Count { get; private set; }
        public void Add(T obj)
        {
            if (obj.idx > 0)
            {
                this.DebugError($"list2 {typeof(T).Name} 已经添加了 无法在添加了");
                return;
            }

            if (InForEach)
            {
                 this.DebugError($"list2 {typeof(T).Name} 不能在ForEach时候 删除 添加");
                return ;
            }

            Count++;
            if (entitys == null || Count > entitys.Length)
            {
#if !NOT_UNITY
                UnityEngine.Profiling.Profiler.BeginSample("--------newObjs---GC---");
#endif
#if MODULE_MATHEMATICS
                var newObjs = System.Array.CreateInstance(typeof(T), math.ceilpow2(Count)); //2的倍数
#else
                var newObjs = System.Array.CreateInstance(typeof(T), entitys == null ? 1 : entitys.Length * 2); //2的倍数
#endif

#if !NOT_UNITY
                UnityEngine.Profiling.Profiler.EndSample();
#endif

                if (entitys != null) entitys.CopyTo(newObjs, 0);
                entitys = (T[])newObjs;
            }

            entitys[Count - 1] = obj;
            obj.idx = (Count - 1) | IdxOffsetVal;
        }

        public void Remove(T obj)
        {
            if (obj.idx == 0)
            {
                 this.DebugError($"list2 {typeof(T).Name} 未注册过 或者 重复删除");
                return;
            }

          
            int idx = obj.idx & ~IdxOffsetVal;  //获得

            if (!obj.Equals(entitys[idx]))
            {
                 this.DebugError($"list2 {typeof(T).Name} 不在同一个容器里面");
                return;
            }

            if (InForEach)
            {
                 this.DebugError($"list2 {typeof(T).Name} 不能在ForEach时候 删除");
                return;
            }


            Count--;
            entitys[idx] = entitys[Count];
            entitys[idx].idx = idx | IdxOffsetVal;   //获得新索引

            entitys[Count] = default(T);     //清空
            obj.idx = 0;                    //状态清零
        }

        /// <summary>
        /// 请用lambda表达式
        /// </summary>
        /// <param name="action"></param>
        public void Clear(System.Action<T> action = null)
        {
            InForEach = true;
            for (int i = Count - 1; i >= 0; i--)
            {
                entitys[i].idx = 0;          
                action?.Invoke(entitys[i]);
                entitys[i] = default(T);     //清空
            }
            InForEach = false;
            Count = 0;
        }


        /// <summary>
        /// 为了无GC传参  请用lambda表达式传参
        /// 如果是一个方法 请用下面实现方式
        /// Clear(param.add) 转换为下面的形式
        /// Clear（ (val,param)=>{ param.add(v)} )
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="action"></param>
        /// <param name="param12"></param>
        public void Clear<T1>(System.Action<T, T1> action, T1 param12)
        {
            InForEach = true;
            for (int i = Count - 1; i >= 0; i--)
            {
                entitys[i].idx = 0;
                action?.Invoke(entitys[i], param12);
                entitys[i] = default(T);     //清空
            }
            InForEach = false;
            Count = 0;
        }



        public void ForEach(System.Action<T> action)
        {
            if (Count == 0 || action == null)
                return;

            InForEach = true;
            try
            {
                for (int i = Count-1; i >= 0; i--)
                    action(entitys[i]);
            }
            catch (System.Exception e)
            {
                e.DebugError();
            }
            finally
            {
                InForEach = false;
            }
        }

        public void ForEach<T1>(System.Action<T, T1> action, T1 param1)
        {
            if (Count == 0 || action == null)
                return;

            InForEach = true;
            try
            {
                for (int i = Count - 1; i >= 0; i--)
                    action(entitys[i], param1);
            }
            catch (System.Exception e)
            {
                e.DebugError();
            }
            finally
            {
                InForEach = false;
            }
        }



        public void CpyeTo(List<T> list_)
        {
            if (Count == 0)
                return;

            for (int i = Count - 1; i >= 0; i--)
                list_.Add(entitys[i]);
        }

        /// <summary>
        /// 清除拷贝
        /// </summary>
        /// <param name="list_"></param>
        public void ClearTo(List<T> list_)
        {
            if (Count == 0)
                return;
            InForEach = true;
            for (int i = Count - 1; i >= 0; i--)
            {
                entitys[i].idx = 0;
                list_.Add(entitys[i]);
                entitys[i] = default(T);     //清空
            }
            InForEach = false;
            Count = 0;
        }


        public void MoveTo(List2<T> list_)
        {
            InForEach = true;
            for (int i = Count - 1; i >= 0; i--)
            {
                entitys[i].idx = 0;
                list_.Add(entitys[i]);
                entitys[i] = default(T);     //清空
            }
            InForEach = false;
            Count = 0;
        }


        public static List2<T> Get()
        {
            return List2Pool<T>.Get();
        }

        public void Release()
        {
            List2Pool<T>.Release(this);
        }

        #region IEnumerator


        IEnumerator<T> IEnumerable<T>.GetEnumerator() => List2_IEnumerator<T>.Get(this);


        IEnumerator IEnumerable.GetEnumerator() => List2_IEnumerator<T>.Get(this);

        #endregion

    }


    //------------------  没用上   写着好玩
    /// <summary>
    /// Poolable 和 list2都存在
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PoolableOrList2<T> : Poolable<T>, IList2Idx<T> where T : PoolableOrList2<T>, new()
    {
        int IList2Idx<T>.idx { get; set; }
        static List2<T> objs = new List2<T>();

        protected override void OnGet()
        {
            base.OnGet();
            objs.Add((T)this);
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            objs.Add((T)this);
        }

        public static void ForEach(System.Action<T> action)
        {
            objs.ForEach(action);
        }
    }



    /// <summary>
    /// 创建List<Vector3> m_Positions = ListPool<Vector3>.Get();
    /// 销毁ListPool<Vector3>.Release(m_Positions);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class List2Pool<T> where T : IList2Idx<T>
    {
        private static readonly ObjectPool<List2<T>> s_ListPool = new ObjectPool<List2<T>>(null, l => l.Clear());

        public static List2<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List2<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }
    }


}


