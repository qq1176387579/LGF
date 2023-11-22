using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ADDRESSABLE_ASSETS
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Exceptions;
using UnityEngine.UI;


namespace LGF
{
    
    /// <summary>
    /// 资源 加载器 用来加载资源 释放资源的
    /// 多个资源管理 一般统一释放
    /// </summary>
    public class AssetLoader 
    {
        Dictionary<string, IAssetInfo> resDic = new Dictionary<string, IAssetInfo>();

        /// <summary>
        /// 加載资源 同步加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T LoadAsset<T>(string name) where T : UnityEngine.Object
        {
            var info = resDic.GetAssetInfo<T>(name);
            if (info != null && info.asset != null)  {
                return info.asset;      //本地引用  info.asset == null 可能存在异步的加载的情况
            }

            info = AddressableManager.Instance.GetAssetInfo<T>(name);
            T res = info.Referent();     //引用  
            if (res != null) {
                resDic.Add(name, info);
            }
            else {
                Debug.LogError($"name:<{name}> res is null !!!");
            }
            return res; 
        }


        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="finishCallBack"></param>
        public void AsyncLoadAsset<T>(string name, Action<T> finishCallBack) where T : UnityEngine.Object
        {
            var info = resDic.GetAssetInfo<T>(name);  
            if (info != null && info.asset != null) {
                finishCallBack?.Invoke(info.asset);   //资源加载完成 直接调用
                return;
            }

            //存在GC 这里没有库 后面写个无GC版本的 替换掉这里面的数据 传输数据有  finishCallBack,this,
            System.Action<T,string> action =(a, name_) => {
                if (a == null) {
                    resDic.Remove(name_);
                    return; //后面看需求  需要返回值的null  在把这个干掉
                }
                finishCallBack?.Invoke(a);
            };

            if (info != null) { //资源未加载完成
                info.FinishCallBack += action;
                return;
            }
          
            info = AddressableManager.Instance.GetAssetInfo<T>(name);
            info.AsynReferent(action);  //引用
            resDic.Add(name,info);     
        }

        public void ReleaseAll()
        {
            foreach (var item in resDic) {
                item.Value.Release();
            }
            resDic.Clear();
        }

    }



}
#endif