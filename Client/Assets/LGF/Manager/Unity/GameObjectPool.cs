/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2023/3/10 15:10:39
/// 功能描述:  游戏入口
****************************************************/

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LGF;
using LGF.Util;
using LGF.Log;

namespace LGF
{

    public interface IGameObjectPool
    {
        Type GetPrefabType();
        void ReleaseByID(int instanceID);
        void ReleaseByID(GameObject go);
    }



    /// <summary>
    /// 缓冲池里面的单位  用来直接回收的  省略管理器查询部分 与 缓冲池管理器兼容
    /// </summary>
    public interface IGameObjectPoolUnit<T> where T : MonoBehaviour
    {
        public GameObjectPoolByMono<T> Pool { get; set; }
        public GameObjectUnitInfo GoMgrUnitInfo { get; set; }
    }





    public static class GameObjectPoolHelper
    {
        /// <summary>
        /// 缓冲池里面实例信息
        /// </summary>

        public static T LoadPrefab<T>(string path) where T : UnityEngine.Object
        {
            T prefab = Resources.Load<T>(path);
            if (prefab == null) {
                Debug.LogError($"path not has prefab : {path}");
            }
            return prefab;
        }

        public static Transform GetNewPoolLocation(string name, Transform parent = null)
        {
            Transform tf = new GameObject(name).transform;
            tf.SetParent(parent);
            tf.Reset();
            return tf;
        }

        public static void Release(this GameObject go)
        {
            GameObjectManager.Instance.Release(go);
        }

        public static void Release(this Transform tf)
        {
            GameObjectManager.Instance.Release(tf.gameObject);
        }

        /// <summary>
        /// 直接回收 省略管理器中间查询步骤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unit"></param>
        public static void Release<T>(this IGameObjectPoolUnit<T> unit) where T : MonoBehaviour
        {
            T cl = unit as T;
            if (cl == null) {
                Debug.LogError("非法回收");
                return;
            }
            var info = unit.GoMgrUnitInfo;
            if (info.isRelease) {
                Debug.LogError("非法回收。  该对象已经被回收了!!");
                return;
            }
            else {
                unit.Pool.Release(cl);
                info.isRelease = true;
            }
        }

    }

    public class GameObjectUnitInfo : Poolable<GameObjectUnitInfo>
    {
        public bool isRelease;
        public IGameObjectPool pool;

        public GameObjectUnitInfo Init(IGameObjectPool pool_)
        {
            isRelease = false;
            pool = pool_;
            return this;
        }

        public bool CanRelease()
        {
            return !isRelease;
        }

        public void GoRelease(int instanceID)
        {
            if (isRelease) {
                Debug.LogError("非法回收。  该对象已经被回收了!!");
                return;
            }
            isRelease = true;
            pool.ReleaseByID(instanceID);
        }

        public void GoGet()
        {
            isRelease = false;
        }

    }



    /// <summary>
    /// GameObject 管理器
    /// </summary>
    public class GameObjectManager : SingletonBase<GameObjectManager>
    {
        Transform rootPath;
        //预制体 
        Dictionary<string, IGameObjectPool> m_pools = new Dictionary<string, IGameObjectPool>();    //对象池
        Dictionary<int, GameObjectUnitInfo> m_ins2pool = new Dictionary<int, GameObjectUnitInfo>();

        protected override void OnNew()
        {
            rootPath = GameObjectPoolHelper.GetNewPoolLocation("GameObjectPoolRoot"); //
            Common.DontDestroyOnLoad(rootPath);
        }
      

        public TT GetPool<T, TT>(string path)
           where T : UnityEngine.Object
           where TT : GameObjectPoolBase<T>, new()
        {
            TT pool2;
            if (!m_pools.TryGetValue(path, out var pool)) {
                T prefab = GameObjectPoolHelper.LoadPrefab<T>(path); //暂时先这样实现
                if (prefab == null) return null;
                pool2 = new TT();
                pool2.Init(prefab);
                pool2.SetOther(GameObjectPoolHelper.GetNewPoolLocation(path, rootPath));
                m_pools.Add(path, pool2);
            }
            else {
                pool2 = (pool as TT);
                if (pool2 == null) {
                    //可能是一个对象 但是不是一个组件  需要设置同一组件
                    this.DebugError($"---非法操作--可能是一个对象 但是不是一个组件- type : {typeof(T)}  pool is{pool.GetPrefabType()} \n 请只设置一个类型");
                    return null;
                }
            }
            return pool2;
        }

        T GetEx<T,TT>(string path) where T : UnityEngine.Object
               where TT : GameObjectPoolBase<T>, new()
        {
            TT pool2 = GetPool<T,TT>(path);
            if (pool2 == null) {
                return null;
            }
            bool isNew = pool2.CountInactive == 0;  //新创建的
            T tmp = pool2.Get();

            int InstanceID = tmp is GameObject ?  tmp.GetInstanceID() : (tmp as MonoBehaviour).gameObject.GetInstanceID();

            if (isNew) {
                var info = GameObjectUnitInfo.Get().Init(pool2);
                m_ins2pool.Add(InstanceID, info);
                pool2.SetUnitInfo(tmp, info);
            }
            else {
                m_ins2pool[InstanceID].GoGet();
            }
            return tmp;
        }

        public GameObject Get(string path) 
        {
            return GetEx<GameObject, GameObjectPoolByGo>(path);
        }

        public T Get<T>(string path) where T : UnityEngine.MonoBehaviour
        {
            return GetEx<T, GameObjectPoolByMono<T>>(path);
        }


        public void Release(GameObject go)
        {
            int instanceID = go.GetInstanceID();
            if (m_ins2pool.TryGetValue(instanceID, out var info)) {
                if (!info.isRelease) {
                    info.GoRelease(instanceID);   //回收
                }
                else {
                    this.DebugError("该对象重复回收 请检查 代码" + go.name);
                }
            }
            else {
                //Debug.Log("  Release GetInstanceID : " + tmp.GetInstanceID());
                this.DebugError("该对象 不是 GameObjectManager 所管理的 请检查脚本 go: " + go.name);
            }
        }
    }


    ///// <summary>
    ///// 作用局部
    ///// </summary>
    //public class GameObjectLocalPool : GameObjectPoolBase
    //{

    //}

    ///// <summary>
    ///// 作用局部 的泛型  如背包格子。  不需要外部调用与回收
    ///// </summary>
    //public class GameObjectLocalPool<T> : MonoBehaviour where T : MonoBehaviour
    //{
    //    T prefab;    //预制体
    //    public GameObjectLocalPool<T> Init(T _prefab)
    //    {
    //        prefab = _prefab;
    //        return this;
    //    }
    //}



    public class GameObjectPoolByGo : GameObjectPoolBase<GameObject>
    {
        public override void SetUnitInfo(GameObject element, GameObjectUnitInfo info)
        {
            
        }

        protected override void AddToInstanceMgrAndInitInstance(GameObject element)
        {
            instanceMgr.Add(element.GetInstanceID(), element);
        }

        protected override void InitPos(InstantiateLabel label, GameObject element, in Vector3 position, in Quaternion rotation, Transform parent = null)
        {
            InitPos(label, element.transform, position, rotation, parent);
        }

        protected override void OnGet(GameObject element)
        {
            element.SetActive(true);
        }

        protected override void OnRelease(GameObject element)
        {
            element?.SetActive(false);
            element?.transform.SetParent(m_root);
        }
    }


    public class GameObjectPoolByMono<T> : GameObjectPoolBase<T> where T : MonoBehaviour
    {
        public override void SetUnitInfo(T element, GameObjectUnitInfo info)
        {
            if (element is IGameObjectPoolUnit<T>) {
                (element as IGameObjectPoolUnit<T>).GoMgrUnitInfo = info;
            }
        }

        protected override void AddToInstanceMgrAndInitInstance(T element)
        {
            instanceMgr.Add(element.gameObject.GetInstanceID(), element);
            if (element is IGameObjectPoolUnit<T>) {
                (element as IGameObjectPoolUnit<T>).Pool = this;
            }
        }

        protected override void InitPos(InstantiateLabel label, T element, in Vector3 position, in Quaternion rotation, Transform parent = null)
        {
            InitPos(label, element.transform, position, rotation, parent);
        }

        protected override void OnGet(T element)
        {
            element.gameObject.SetActive(true);
        }

        protected override void OnRelease(T element)
        {
            element.gameObject.SetActive(false);
            element.transform.SetParent(m_root);
        }

    }

    /// <summary>
    /// 实例话标签  只有分类作用
    /// </summary>
    public enum InstantiateLabel
    {
        TAG_01,
        TAG_02,
        TAG_03,
    }

    /// <summary>
    /// 该对象池不受 管理器管理影响
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GameObjectPoolByBehaviour<T> : GameObjectPoolBase<T> where T : Behaviour
    {
        public override void SetUnitInfo(T element, GameObjectUnitInfo info)
        {
            throw new NotImplementedException();
        }

        protected override void AddToInstanceMgrAndInitInstance(T element)
        {
            
        }

        protected override void OnGet(T element)
        {
            element.gameObject.SetActive(true);
        }

        protected override void OnRelease(T element)
        {
            element.gameObject.SetActive(false);
            element.transform.SetParent(m_root);
        }

        protected override bool CheckPrefab()
        {
            return true;
        }

        public override T Instantiate(InstantiateLabel label, in Vector3 position, in Quaternion rotation, Transform parent = null)
        {
            T tmp = new GameObject().AddComponent<T>();
            tmp.transform.SetParent(parent);
            switch (label) {
                case InstantiateLabel.TAG_01:
                    break;
                case InstantiateLabel.TAG_02:
                    tmp.transform.localPosition = position;
                    break;
                case InstantiateLabel.TAG_03:
                    tmp.transform.position = position;
                    tmp.transform.rotation = rotation;
                    break;
                default:
                    break;
            }
            return tmp;
        }

        public override void ReleaseByID(GameObject go)
        {
            throw new NotImplementedException();
        }
        public override void ReleaseByID(int instanceID)
        {
            throw new NotImplementedException();
        }

        protected override void InitPos(InstantiateLabel label, T element, in Vector3 position, in Quaternion rotation, Transform parent = null)
        {
            InitPos(label, element.transform, position, rotation, parent);
        }
    }




    public abstract class GameObjectPoolBase<T> : IGameObjectPool where T : UnityEngine.Object
    {

        protected Transform m_root;   //收集的时候根节点
        Stack<T> m_Stack = new Stack<T>();
        //实例管理
        protected Dictionary<int, T> instanceMgr { get; private set; }

        public T Prefab { get; protected set; }
        public Stack<T> Stack { get => m_Stack; }

        public int CountAll { get; protected set; }
        public int CountActive { get => CountAll - CountInactive; }
        public virtual int CountInactive { get => Stack.Count; }

        private Action<T> m_ActionOnGet;
        private Action<T> m_ActionOnRelease;
        //public event Action<T> OnInstantiate;

        public GameObjectPoolBase<T> Init(T prefab, Action<T> _ActionOnGet = null, Action<T> _ActionOnRelease = null)
        {
            Prefab = prefab;
            m_ActionOnGet = _ActionOnGet;
            m_ActionOnRelease = _ActionOnRelease;
            instanceMgr = new Dictionary<int, T>();
            return this;
        }

        public void SetOther(Transform _root)
        {
            m_root = _root;
        }



      

        protected virtual bool CheckPrefab()
        {
            if (Prefab == null) {
                this.DebugError($" type {typeof(T)}  Prefab 未初始化,请先初始化 ");
                return false;
            }
            return true;
        }


        public virtual T Get(Transform parent = null)
        {
            return GetEx(InstantiateLabel.TAG_01, Vector3.zero, Quaternion.identity, parent);
        }

        public T Get(in Vector3 position, Transform parent = null)
        {

            return GetEx(InstantiateLabel.TAG_02, position, default, parent);
        }

        public T Get(in Vector3 position, in Quaternion rotation, Transform parent = null)
        {

            return GetEx(InstantiateLabel.TAG_03, position, rotation, parent);
        }

        protected virtual T GetEx(InstantiateLabel label, in Vector3 position, in Quaternion rotation, Transform parent = null)
        {
            if (!CheckPrefab()) {
                return null;
            }
            T element = null;
            if (CountInactive == 0) {
                element = Instantiate(label, position, rotation, parent);
                AddToInstanceMgrAndInitInstance(element);
                CountAll++;
            }
            else {
                element = m_Stack.Pop();
                InitPos(label, element, position, rotation, parent);
            }
            m_ActionOnGet?.Invoke(element);
            OnGet(element);
            return element;
        }

        //后面看情况要不要做检测  gameobjcet 可能不需要  如果不需要就需要分类处理
        //以gameobjcet的GetInstanceID为基准 其他不进行调用 如mono.gameObject
        //不然不同的组件 Component有问题
        protected abstract void AddToInstanceMgrAndInitInstance(T element);

        protected abstract void InitPos(InstantiateLabel label, T element,in Vector3 position, in Quaternion rotation, Transform parent = null);

        protected virtual void InitPos(InstantiateLabel label, Transform element, in Vector3 position, in Quaternion rotation, Transform parent = null)
        {
            if (element.transform.parent != parent) {
                element.transform.SetParent(parent);
            }
           
            switch (label) {
                case InstantiateLabel.TAG_01:
                    break;
                case InstantiateLabel.TAG_02:
                    element.position = position;
                    break;
                case InstantiateLabel.TAG_03:
                    element.position = position;
                    element.rotation = rotation;
                    break;
                default:
                    break;
            }
        }

        public virtual T Instantiate(InstantiateLabel label, in Vector3 position, in Quaternion rotation, Transform parent = null)
        {
            switch (label) {
                case InstantiateLabel.TAG_01:
                    return GameObject.Instantiate<T>(Prefab, parent);
                case InstantiateLabel.TAG_02:
                    return GameObject.Instantiate<T>(Prefab, position, Quaternion.identity, parent);
                case InstantiateLabel.TAG_03:
                    return GameObject.Instantiate<T>(Prefab, position, rotation, parent);
            }
            return GameObject.Instantiate<T>(Prefab, position, rotation, parent);
        }


        protected abstract void OnGet(T element);
        protected abstract void OnRelease(T element);

        /// <summary>
        /// 该方法不做实例对象验证 影响效率
        /// </summary>
        /// <param name="element"></param>
        public void Release(T element)
        {
            if (element == null) {
                Debug.Log("请检查数据!!  obj is null");
                return;
            }

            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                this.DebugError($"Internal error. Trying to destroy object that is already released to pool. type: {typeof(T)}");
            m_ActionOnRelease?.Invoke(element);
            OnRelease(element);
            m_Stack.Push(element);  //回收
        }

        public Type GetPrefabType()
        {
            return Prefab.GetType();
        }

        public virtual void ReleaseByID(int instanceID)
        {
            if (instanceMgr.TryGetValue(instanceID, out T val)) {
                Release(val);
            }
            else {
                this.DebugError("非法回收  该对象不是这个缓冲池内 生成的对象 无法做回收");
            }
        }

        public virtual void ReleaseByID(GameObject go)
        {
            ReleaseByID(go.GetInstanceID());
        }

        public abstract void SetUnitInfo(T element, GameObjectUnitInfo info);
      
    }


}


