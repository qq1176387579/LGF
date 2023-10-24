/***************************************************
/// ����:      liuhuan
/// ��������:  2023/3/10 15:10:39
/// ��������:  ��Ϸ���
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
    /// ���������ĵ�λ  ����ֱ�ӻ��յ�  ʡ�Թ�������ѯ���� �� ����ع���������
    /// </summary>
    public interface IGameObjectPoolUnit<T> where T : MonoBehaviour
    {
        public GameObjectPoolByMono<T> Pool { get; set; }
        public GameObjectUnitInfo GoMgrUnitInfo { get; set; }
    }





    public static class GameObjectPoolHelper
    {
        /// <summary>
        /// ���������ʵ����Ϣ
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
        /// ֱ�ӻ��� ʡ�Թ������м��ѯ����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unit"></param>
        public static void Release<T>(this IGameObjectPoolUnit<T> unit) where T : MonoBehaviour
        {
            T cl = unit as T;
            if (cl == null) {
                Debug.LogError("�Ƿ�����");
                return;
            }
            var info = unit.GoMgrUnitInfo;
            if (info.isRelease) {
                Debug.LogError("�Ƿ����ա�  �ö����Ѿ���������!!");
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
                Debug.LogError("�Ƿ����ա�  �ö����Ѿ���������!!");
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
    /// GameObject ������
    /// </summary>
    public class GameObjectManager : SingletonBase<GameObjectManager>
    {
        Transform rootPath;
        //Ԥ���� 
        Dictionary<string, IGameObjectPool> m_pools = new Dictionary<string, IGameObjectPool>();    //�����
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
                T prefab = GameObjectPoolHelper.LoadPrefab<T>(path); //��ʱ������ʵ��
                if (prefab == null) return null;
                pool2 = new TT();
                pool2.Init(prefab);
                pool2.SetOther(GameObjectPoolHelper.GetNewPoolLocation(path, rootPath));
                m_pools.Add(path, pool2);
            }
            else {
                pool2 = (pool as TT);
                if (pool2 == null) {
                    //������һ������ ���ǲ���һ�����  ��Ҫ����ͬһ���
                    this.DebugError($"---�Ƿ�����--������һ������ ���ǲ���һ�����- type : {typeof(T)}  pool is{pool.GetPrefabType()} \n ��ֻ����һ������");
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
            bool isNew = pool2.CountInactive == 0;  //�´�����
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
                    info.GoRelease(instanceID);   //����
                }
                else {
                    this.DebugError("�ö����ظ����� ���� ����" + go.name);
                }
            }
            else {
                //Debug.Log("  Release GetInstanceID : " + tmp.GetInstanceID());
                this.DebugError("�ö��� ���� GameObjectManager ������� ����ű� go: " + go.name);
            }
        }
    }


    ///// <summary>
    ///// ���þֲ�
    ///// </summary>
    //public class GameObjectLocalPool : GameObjectPoolBase
    //{

    //}

    ///// <summary>
    ///// ���þֲ� �ķ���  �米�����ӡ�  ����Ҫ�ⲿ���������
    ///// </summary>
    //public class GameObjectLocalPool<T> : MonoBehaviour where T : MonoBehaviour
    //{
    //    T prefab;    //Ԥ����
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
    /// ʵ������ǩ  ֻ�з�������
    /// </summary>
    public enum InstantiateLabel
    {
        TAG_01,
        TAG_02,
        TAG_03,
    }

    /// <summary>
    /// �ö���ز��� ����������Ӱ��
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

        protected Transform m_root;   //�ռ���ʱ����ڵ�
        Stack<T> m_Stack = new Stack<T>();
        //ʵ������
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
                this.DebugError($" type {typeof(T)}  Prefab δ��ʼ��,���ȳ�ʼ�� ");
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

        //���濴���Ҫ��Ҫ�����  gameobjcet ���ܲ���Ҫ  �������Ҫ����Ҫ���ദ��
        //��gameobjcet��GetInstanceIDΪ��׼ ���������е��� ��mono.gameObject
        //��Ȼ��ͬ����� Component������
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
        /// �÷�������ʵ��������֤ Ӱ��Ч��
        /// </summary>
        /// <param name="element"></param>
        public void Release(T element)
        {
            if (element == null) {
                Debug.Log("��������!!  obj is null");
                return;
            }

            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                this.DebugError($"Internal error. Trying to destroy object that is already released to pool. type: {typeof(T)}");
            m_ActionOnRelease?.Invoke(element);
            OnRelease(element);
            m_Stack.Push(element);  //����
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
                this.DebugError("�Ƿ�����  �ö��������������� ���ɵĶ��� �޷�������");
            }
        }

        public virtual void ReleaseByID(GameObject go)
        {
            ReleaseByID(go.GetInstanceID());
        }

        public abstract void SetUnitInfo(T element, GameObjectUnitInfo info);
      
    }


}


