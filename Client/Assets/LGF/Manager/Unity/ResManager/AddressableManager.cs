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

    public static class AssetInfoHelper
    {
        //Dictionary<string, IAssetInfo> resDic = new Dictionary<string, IAssetInfo>();
        public static void Error(string str)
        {
            Debug.LogError(str);
        }

        public static T GetAssetInfoBase<T, T1, T2>(this Dictionary<string, IAssetInfo> resDic, string name, bool isNew = false) where T : AssetInfoBase<T2>, new()
            where T1 : UnityEngine.Object
            where T2 : class
        {
            T assstInfo = null;
            if (resDic.TryGetValue(name, out var info)) {
                assstInfo = info as T;
                if (assstInfo == null) {
                    Error($"非法操作 name<{name}> type<{info.GetAssetType()}> as <{typeof(T)}>");
                    return null;
                }
            }
            else {
                if (isNew) {
                    assstInfo = new T();
                    assstInfo.Init(name);
                    resDic.Add(name, assstInfo);
                }
            }
            return assstInfo;
        }

        public static AssetInfoList<T> GetAssetInfoList<T>(this Dictionary<string, IAssetInfo> resDic, string name) where T : UnityEngine.Object
        {
            return resDic.GetAssetInfoBase<AssetInfoList<T>, T, IList<T>>(name);
        }

        public static AssetInfo<T> GetAssetInfo<T>(this Dictionary<string, IAssetInfo> resDic, string name) where T : UnityEngine.Object
        {

            return resDic.GetAssetInfoBase<AssetInfo<T>, T, T>(name);
        }


    }


    public class AddressableManager : SingletonBase<AddressableManager> //, IResourcesManager
    {

        Dictionary<string, IAssetInfo> resDic = new Dictionary<string, IAssetInfo>();

        public AddressableManager()
        {
            ResourceManager.ExceptionHandler = (AsyncOperationHandle handle, Exception exception) => {

                UnityEngine.AddressableAssets.InvalidKeyException ex = exception as UnityEngine.AddressableAssets.InvalidKeyException;
                //Debug.LogError("[[" + lastName + "]] 资源不存在 exception :" + exception.ToString());
                if (ex != null) {
                    Debug.LogError($"[[{(ex != null ? ex.Key : "")}]] 资源不存在 exception : {exception}");
                }
            };
        }



        public AssetInfoList<T> GetAssetInfoList<T>(string name) where T : UnityEngine.Object
        {
            return resDic.GetAssetInfoBase<AssetInfoList<T>, T, IList<T>>(name, true);
        }

        public AssetInfo<T> GetAssetInfo<T>(string name) where T : UnityEngine.Object
        {

            return resDic.GetAssetInfoBase<AssetInfo<T>, T, T>(name, true);
        }


        ///// <summary>
        ///// 加載资源 同步加载
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public T LoadAsset<T>(string name) where T : UnityEngine.Object
        //{
        //    var info = GetAssetInfo<T>(name);
        //    if (info == null) {
        //        return null;
        //    }
        //    return info.Referent();
        //}

        ///// <summary>
        ///// 异步加载资源
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key"></param>
        ///// <param name="finishCallBack"></param>
        //public void AsyncLoadAsset<T>(string name, Action<T> finishCallBack = null) where T : UnityEngine.Object
        //{
        //    var info = GetAssetInfo<T>(name);
        //    if (info == null) {
        //        return;
        //    }
        //    info.AsynReferent(finishCallBack);
        //}


        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        internal void UnLoadRes<T>(AssetInfoBase<T> info) where T : class
        {
            //移除
            resDic.Remove(info.name);   //移除
        }

        public void Clear()
        {
            foreach (var item in resDic) {
                item.Value.Close();
            }

            resDic.Clear();
        }

     
        //public T Instantiate<T>(string name, Transform parent) where T : UnityEngine.Object
        //{
        //    var p = LoadAsset<T>(name);
        //    return GameObject.Instantiate<T>(p, parent);
        //}

        //public T[] LoadAll<T>(string path) where T : UnityEngine.Object
        //{
        //    var info = GetAssetInfoList<T>(path);
        //    var list = info.ReferentToList();
        //    if (list != null) {
        //        return list.ToArray();
        //    }

        //    return null;
        //}

        //public void LoadAllAsync<T>(string path, Action<T> OnComplete) where T : UnityEngine.Object
        //{
        //    var info = GetAssetInfoList<T>(path);
        //    info.AsynReferent(null);
        //    info.SetCompleteOneAssetCallback(OnComplete);
        //}

        //public void GetBytesToSprite(byte[] bytes, Image img)
        //{
        //    //throw new NotImplementedException();
        //}
    }


}

#endif