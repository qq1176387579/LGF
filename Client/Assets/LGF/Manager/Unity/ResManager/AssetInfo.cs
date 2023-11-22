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
using LGF.DataStruct;

namespace LGF
{

    public interface IAssetInfo
    {
        Type GetAssetType();
        void Close();

        void Release();

    }

    /// <summary>
    /// 多个资源加载 资源管理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AssetInfoList<T> : AssetInfoBase<IList<T>> where T : UnityEngine.Object
    {
        public List<T> assetsComplete;   //完成的资源
        event System.Action<T> OneAssetComplete;  //单个资源完成

        protected override void AddressablesLoad()
        {
            assetsComplete = new List<T>(); //完成的资源
            //base.AddressablesLoad();
            OpHandle = Addressables.LoadAssetsAsync<T>(name, OnCompleteOneAsset);
            AddOnCompleted();
            OneAssetComplete = null;
        }

        void OnCompleteOneAsset(T oneAsset)
        {
            assetsComplete.Add(oneAsset);
            OneAssetComplete.Invoke(oneAsset);
        }

        /// <summary>
        /// 设置完成单个资源时的回调
        /// </summary>
        public void SetCompleteOneAssetCallback(System.Action<T> action)
        {
            if (OpHandle.Status == AsyncOperationStatus.Failed) {
                return;
            }

            if (OpHandle.Status != AsyncOperationStatus.Succeeded) {
                OneAssetComplete += action;
            }

            for (int i = 0; i < assetsComplete.Count; i++) {
                action.Invoke(assetsComplete[i]);
            }
        }

        public List<T> ReferentToList()
        {
            if (Referent() == null) {
                return null;
            }
            return assetsComplete;
        }

        protected override void OnCompleted(AsyncOperationHandle<IList<T>> op)
        {
            base.OnCompleted(op);
            OneAssetComplete = null;
        }

        public override void Close()
        {
            base.Close();
            assetsComplete.Clear();
            assetsComplete = null;
            OneAssetComplete = null;
        }

    }


    public class AssetInfo<T> : AssetInfoBase<T> where T : UnityEngine.Object
    {


    }


    public class AssetInfoBase<T> : IAssetInfo where T : class
    {
        public string name; //资源名称
        public T asset => OpHandle.Result;   //资源
        public AsyncOperationHandle<T> OpHandle;
        bool addCompleted = false;
        SemctlCount semctlCount;
        public event Action<T, string> FinishCallBack;
        public bool IsLoadOver = false;  //加载完成
        //bool IsWaiting = false;   //等待中
        bool isDispose = false;   //是否释放掉

        public void Init(string name_)
        {

            semctlCount = new LH.SemctlCount(null, Dispose);
            semctlCount.name = name_;
            this.name = name_;
            AddressablesLoad();
            FinishCallBack = null;
            //OpHandle.Completed += OnCompleted;
        }

        protected virtual void AddressablesLoad()
        {
            OpHandle = Addressables.LoadAssetAsync<T>(name);   //加载资源
        }


        public Type GetAssetType()
        {
            return typeof(T);
        }

        /// <summary>
        /// 异步引用
        /// </summary>
        /// <returns></returns>
        public virtual void AsynReferent(Action<T,string> finishCallBack)
        {

            if (OpHandle.Status == AsyncOperationStatus.Failed) {
                finishCallBack?.Invoke(null, name);
                return;
            }

            semctlCount.Begin();
            if (OpHandle.Status == AsyncOperationStatus.Succeeded) {
                finishCallBack?.Invoke(asset, name);
            }
            else {
                AddOnCompleted();
                if (finishCallBack != null) {
                    FinishCallBack += finishCallBack;   //异步的情况
                }
            }
        }

        protected void AddOnCompleted()
        {
            if (!addCompleted) {   //未初始过
                addCompleted = true;
                OpHandle.Completed += OnCompleted;  //添加回调
            }
        }

        /// <summary>
        /// 异步的回调
        /// 就算调用 Addressables.Release 后 这个回调还是会继续调用
        /// </summary>
        /// <param name="op"></param>
        protected virtual void OnCompleted(AsyncOperationHandle<T> op)
        {
            if (isDispose) {
                return; //已经强制回收了 不能再操作了
            }
            IsLoadOver = true;
            //if (!IsWaiting && semctlCount.Count == 0) {
            if (semctlCount.Count == 0) {
                //没有在等待引用 且 引用次数为0 那么卸载这个类
                Dispose();
                return;
            }

            if (op.Status == AsyncOperationStatus.Succeeded) {
                FinishCallBack?.Invoke(op.Result, name);   //加载成功
            }
            else if (op.Status == AsyncOperationStatus.Failed) {
                //加载失败
                FinishCallBack?.Invoke(null, name);
                semctlCount.End();
            }
            else {
                Debug.LogError("出错");
            }
            FinishCallBack = null;
        }

        /// <summary>
        /// 引用
        /// </summary>
        /// <returns></returns>
        public virtual T Referent()
        {
            semctlCount.Begin();
            //存在异步加载加载中  需要等待  如果本身是异步引用 那边需要
            if (OpHandle.Status == AsyncOperationStatus.None) {
                //IsWaiting = true;
                //如果当前异步中 那么等待
                OpHandle.WaitForCompletion();
                //IsWaiting = false;
            }

            IsLoadOver = true;
            if (OpHandle.Status == AsyncOperationStatus.Failed) {
                semctlCount.End();  //地址出错时候回收
                return default;
            }

            return asset;
        }

        /// <summary>
        /// 回收
        /// </summary>
        public void Release()
        {
            if (isDispose) {
                return;
            }

            if (semctlCount.Count == 0) {
                UnityEngine.Debug.Log("非法Release 请检查代码");
                return;
            }
            semctlCount.End();
        }

        public void Dispose()
        {
            if (isDispose) return;

            if (!IsLoadOver) {
                //如果没有加载完成 先不回收 等加载完成后进行回收
                return;
            }
            Close();
            AddressableManager.Instance.UnLoadRes(this);
        }

        /// <summary>
        /// 强制关闭
        /// </summary>
        public virtual void Close()
        {
            if (isDispose) {
                Debug.LogError("非法操作 已经关闭掉了");
                return;
            }
            isDispose = true;
            Addressables.Release(OpHandle); //强制回收

        }

    }
}


#endif