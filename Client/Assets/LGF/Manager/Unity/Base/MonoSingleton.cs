using UnityEngine;
using System.Collections;
using System.Dynamic;
using SingletonHelperNamespace;
using LGF.Log;

namespace SingletonHelperNamespace
{
    public static  class SingletonHelper
    {
        static GameObject _root = null;

        public static GameObject GetRoot()
        {
            if (null == _root)
            {
                _root = new GameObject("MonoSingletonRoot");
                MonoBehaviour.DontDestroyOnLoad(_root);      //保证改单例类不会进行销毁
            }
            return _root;
        }
    }



}

namespace LGF
{

    /// <summary>
    /// 继承MonoBehaviour类的单例!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        static protected T _instance = default(T);
        static bool _isNewObj = true;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    //判断是否多余一个的数量
                    if (FindObjectsOfType<T>().Length > 1)
                    {
                        sLog.Error(typeof(T).Name + "该单例类的数量大于1");
                        return _instance;
                    }

                    //判断是否一个都没有  没有就要思考如何创建一个单例对象
                    if (_instance == null)
                    {
                        GameObject go = null;
                        //如果这个GameObject物体不存在
                        var _root = SingletonHelper.GetRoot();

                        go = _isNewObj ? new GameObject(typeof(T).Name) : _root;

                        if (_isNewObj)
                            go.transform.SetParent(_root.transform);

                        //在这个物体上添加这个组件(这个单例类)
                        _instance = go.AddComponent<T>();
                        _instance.Init();
                    }
                    else {
                        _instance.RestParent();
                    }
                }
                return _instance;
            }
        }

        public static T GetSingleton() => Instance;

        public virtual void Init() { }
        void RestParent()
        {
            if (transform.parent != SingletonHelper.GetRoot().transform) {
                transform.SetParent(SingletonHelper.GetRoot().transform);
            }
        }
    }


    /// <summary>
    /// 简单mono单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleMonoSingleton<T> : MonoBehaviour where T : SimpleMonoSingleton<T>
    {
        static protected T _instance;
        public static T Instance { get => _instance; }

        private void Awake()
        {
            _instance = this as T;
            OnAwake();
        }


        public virtual void OnAwake()
        {

        }


    }


}


