using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace LGF
{
    public class MonoController : MonoBehaviour
    {
        private event UnityAction updateEvent;
        private event UnityAction lateUpdateEvent;
        private event UnityAction fixedUpdateEvent;
        private event UnityAction onDisableEvent;
        private event UnityAction onDestoryEvent;
        private event UnityAction onGUIEvent;
        private event UnityAction<bool> onApplicationPauseEvent;
        private event UnityAction<bool> onApplicationFocusEvent;
        private event UnityAction onApplicationQuitEvent;

        private void Awake() => DontDestroyOnLoad(gameObject);

        private void FixedUpdate() => fixedUpdateEvent?.Invoke();
        private void Update() => updateEvent?.Invoke();
        private void LateUpdate() => lateUpdateEvent?.Invoke();
        private void OnDisable() => onDisableEvent?.Invoke();
        private void OnDestroy() => onDestoryEvent?.Invoke();
        private void OnGUI() => onGUIEvent?.Invoke();
        private void OnApplicationPause(bool pause) => onApplicationPauseEvent?.Invoke(pause);
        private void OnApplicationFocus(bool focus) => onApplicationFocusEvent?.Invoke(focus);
        private void OnApplicationQuit() => onApplicationQuitEvent?.Invoke();

        public void AddUpdateListener(UnityAction action) => updateEvent += action;
        public void RemoveUpdateListener(UnityAction action) => updateEvent -= action;

        public void AddLateUpdateListener(UnityAction action) => lateUpdateEvent += action;
        public void RemoveLateUpdateListener(UnityAction action) => lateUpdateEvent -= action;

        public void AddFixedUpdateListener(UnityAction action) => fixedUpdateEvent += action;
        public void RemoveFixedUpdateListener(UnityAction action) => fixedUpdateEvent -= action;

        public void AddOnDisableListener(UnityAction action) => onDisableEvent += action;
        public void RemoveOnDisableListener(UnityAction action) => onDisableEvent -= action;

        public void AddOnDestoryListener(UnityAction action) => onDestoryEvent += action;
        public void RemoveOnDestoryListener(UnityAction action) => onDestoryEvent -= action;

        public void AddOnGUIListener(UnityAction action) => onGUIEvent += action;
        public void RemoveOnGUIListener(UnityAction action) => onGUIEvent -= action;

        public void AddOnApplicationPauseListener(UnityAction<bool> action) => onApplicationPauseEvent += action;
        public void RemoveOnApplicationPauseListener(UnityAction<bool> action) => onApplicationPauseEvent -= action;

        public void AddOnApplicationFocusListener(UnityAction<bool> action) => onApplicationFocusEvent += action;
        public void RemoveOnApplicationFocusListener(UnityAction<bool> action) => onApplicationFocusEvent -= action;

        public void AddOnApplicationQuitListener(UnityAction action) => onApplicationQuitEvent += action;
        public void RemoveOnApplicationQuitListener(UnityAction action) => onApplicationQuitEvent -= action;


        public void RemoveAll()
        {
            updateEvent = null;
            lateUpdateEvent = null;
            fixedUpdateEvent = null;
            onDisableEvent = null;
            onDestoryEvent = null;
            onApplicationPauseEvent = null;
            onApplicationFocusEvent = null;
            onApplicationQuitEvent = null;
        }

    }
}