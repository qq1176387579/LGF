using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ADDRESSABLE_ASSETS
namespace LGF
{
    /// <summary>
    /// 单个资源加载器
    /// </summary>
    public class SingleAssetLoader<T> where T : UnityEngine.Object
    {
        string name;
        public T asset => assetInfo.asset;  
        AssetInfo<T> assetInfo;
        public event System.Action<T> OnLoadComplete;

        ///// <summary>
        ///// 加载完成
        ///// </summary>
        //protected virtual void OnLoadComplete(T res)
        //{

        //}

        public void AsyncLoadAsset(string name_)
        {
            if (name == name_) {    //重复不做处理
                return;
            }
            name = name_;
            AsyncLoadAsset();
        }


        public T GetAsset(string name_)
        {
            LoadAsset(name_);
            return assetInfo.asset;
        }


        public void LoadAsset(string name_)
        {
            if (name == name_) {
                return;
            }
            name = name_;
            LoadAsset();
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        void AsyncLoadAsset()
        {
            assetInfo?.Release();
            if (!string.IsNullOrEmpty(name)) {
                assetInfo = AddressableManager.Instance.GetAssetInfo<T>(name);
                //存在GC
                assetInfo.AsynReferent((a, name_) => {
                    OnLoadComplete?.Invoke(a);  //加载资源
                });
            }
            else {  
                //名称设置为空
                assetInfo = null;
                OnLoadComplete?.Invoke(null);
            }
        }


        /// <summary>
        /// 加载资源
        /// </summary>
        void LoadAsset()
        {
            assetInfo?.Release();
            if (!string.IsNullOrEmpty(name)) {
                assetInfo = AddressableManager.Instance.GetAssetInfo<T>(name);
                var tmp = assetInfo.Referent();
                OnLoadComplete?.Invoke(tmp); //加载资源
                //Debug.Log("----1 name: " + name); ;
            }
            else {
                //名称设置为空
                assetInfo = null;
                OnLoadComplete?.Invoke(null);
                //Debug.Log("----2 name: " + name); ;
            }
        }

        public void Release()
        {
            assetInfo?.Release();
            assetInfo = null;
        }
    }


}

#endif