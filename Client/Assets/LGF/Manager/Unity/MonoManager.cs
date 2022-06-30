using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using SingletonHelperNamespace;

namespace LGF
{ 
    /// <summary>
    /// 单例类使用Mono Update    暂时先这样后面有时间做优化
    /// </summary>
    public class MonoManager : SingletonBase<MonoManager>
    {
        private MonoController controller;

        public MonoManager()
        {
            if(controller == null)
                controller = new GameObject("MonoController").AddComponent<MonoController>();

            controller.transform.SetParent(SingletonHelper.GetRoot().transform);
        }

        public void AddUpdateListener(UnityAction action)=> controller.AddUpdateListener(action);
        public void RemoveUpdateListener(UnityAction action)=> controller.RemoveUpdateListener(action);

        public void AddLateUpdateListener(UnityAction action)=> controller.AddLateUpdateListener(action);
        public void RemoveLateUpdateListener(UnityAction action)=> controller.RemoveLateUpdateListener(action);

        public void AddFixedUpdateListener(UnityAction action)=> controller.AddFixedUpdateListener(action);
        public void RemoveFixedUpdateListener(UnityAction action)=>controller.RemoveFixedUpdateListener(action);

        public void AddOnDisableListener(UnityAction action) => controller.AddOnDisableListener(action);
        public void RemoveOnDisableListener(UnityAction action) => controller.RemoveOnDisableListener(action);

        public void AddOnDestroyListener(UnityAction action)=> controller.AddOnDestoryListener(action);
        public void RemoveOnDestroyListener(UnityAction action)=> controller.RemoveOnDestoryListener(action);

        public void AddOnGUIListener(UnityAction action) => controller.AddOnGUIListener(action);
        public void RemoveOnGUIListener(UnityAction action) => controller.RemoveOnGUIListener(action);

        public void AddOnApplicationPauseListener(UnityAction<bool> action)=> controller.AddOnApplicationPauseListener(action);
        public void RemoveOnApplicationPauseListener(UnityAction<bool> action)=> controller.RemoveOnApplicationPauseListener(action);

        public void AddOnApplicationFocusListener(UnityAction<bool> action) => controller.AddOnApplicationFocusListener(action);
        public void RemoveOnApplicationFocusListener(UnityAction<bool> action) => controller.RemoveOnApplicationFocusListener(action);

        public void AddOnApplicationQuitListener(UnityAction action)=> controller.AddOnApplicationQuitListener(action);
        public void RemoveOnApplicationQuitListener(UnityAction action)=> controller.RemoveOnApplicationQuitListener(action);

        public Coroutine StartCoroutine(IEnumerator routine)=> controller.StartCoroutine(routine);
        public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)=> controller.StartCoroutine(methodName, value);
        public Coroutine StartCoroutine(string methodName)=> controller.StartCoroutine(methodName);
        public void StopCoroutine(IEnumerator routine)=> controller.StopCoroutine(routine);
        public void StopCoroutine(string methodName)=> controller.StopCoroutine(methodName);
        public void StopCoroutine(Coroutine routine)=> controller.StopCoroutine(routine);

        public void RemoveAll() => controller.RemoveAll();
    }
}