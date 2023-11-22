using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ADDRESSABLE_ASSETS
namespace LGF
{
    public class MonoAssetLoader<T> : MonoBehaviour where T : UnityEngine.Object
    {

        SingleAssetLoader<T> loader = new SingleAssetLoader<T>();
        bool isInitLoader = false;
        protected void InitLoader()
        {
            if (isInitLoader) {
                return;
            }
            isInitLoader = true;
            loader.OnLoadComplete += OnLoadComplete;
            //Debug.Log("init");
        }


        ///// <summary>
        ///// 加载完成
        ///// </summary>
        protected virtual void OnLoadComplete(T res)
        {

        }

        public virtual void LoadAsset(string name)
        {
            InitLoader();
            loader.LoadAsset(name);
        }


        /// <summary>
        /// OnDestroy
        /// </summary>
        protected virtual void OnDestroy()
        {
            loader.Release();
        }
    }

}
#endif
