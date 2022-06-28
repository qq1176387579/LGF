/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/5/30 10:59:00
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if MODULE_MATHEMATICS
using Unity.Mathematics;
#endif
using LGF;
using LGF.Log;


namespace LGF
{

    internal abstract class EventDispatcherBase
    {
        /// <summary>
        /// 类型id
        /// </summary>
        public bool isDelayRemove; //是否延迟移除  用于 ForEach时候的处理 IList2Idx 要换成结构体
        internal abstract void Fire(IEventDataBase msg);
        internal abstract void Add<T>(T obj) where T : class;
        internal abstract void Remove<T>(T obj) where T : class;
    }


    /// <summary>
    /// 事件分配器  回去后在优化下  要处理同时删除与添加   删除后又添加的状态
    /// 思路 用2个list2组成 用1个自定义结构体 继承 2个list对应的index
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class EventDispatcher<T> : EventDispatcherBase where T : class, IEntityIndex<T>
    {
        List2<IEntityIndex<T>> entitys = new List2<IEntityIndex<T>>();
        List<IEntityIndex<T>> removeList;
        void AddEx(T obj)
        {
            entitys.Add(obj);
        }

        void RemoveEx(T obj)
        {
            entitys.Remove(obj);
        }

        internal override void Fire(IEventDataBase msg)
        {
            entitys.ForEach((a, _msg) => a.OnEvent(_msg), msg); //0GC
            msg?.Release();  //回收
            if (removeList != null && removeList.Count > 0)
            {
                for (int i = 0; i < removeList.Count; i++)
                    entitys.Remove(removeList[i]);
                removeList.Clear();
            }  
        }

        internal override void Add<T1>(T1 obj)
        {
            if (!(obj is T))
            {
                Debug.LogError($"非法添加 >> {obj.GetType().Name} not is {typeof(T).Name} ");
                return;
            }
            AddEx(obj as T);
        }


        internal override void Remove<T1>(T1 obj)
        {
            if (!(obj is T))
            {
                Debug.LogError($"非法添加 >> {obj.GetType().Name} not is {typeof(T).Name} ");
                return;
            }

            if (entitys.InForEach)
            {
                if (removeList == null)
                    removeList = new List<IEntityIndex<T>>();
                removeList.Add(obj as T);
                return;
            }
            RemoveEx(obj as T);
        }
    }



}
