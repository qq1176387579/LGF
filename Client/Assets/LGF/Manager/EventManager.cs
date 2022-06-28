﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace LGF
{
    /// <summary>
    /// 后面优化下  改为int执行  事件不暴露
    /// </summary>
    public class EventManager : SingletonBase<EventManager>
    {
        //自已私有的字典 管理所有的事件
        private Dictionary<GameEventType, List<Delegate>> m_eventDic = new Dictionary<GameEventType, List<Delegate>>();
        /// <summary>
        /// 判断字典里面是否包含了某一类事件
        /// </summary>
        /// <param name="GameEventType"></param>
        /// <returns></returns>
        public bool CheckEvent(GameEventType evtType)
        {
            bool isContanis = false;
            //如果字典不包含该类型 表明是第一次添加该事件
            if (m_eventDic.ContainsKey(evtType))
            {
                isContanis = true;
            }
            return isContanis;
        }
        /// <summary>
        /// 事件类型跟事件的参数
        /// </summary>
        /// <param name="eType"></param>
        /// <param name="ac"></param>
        public void AddListener(GameEventType eType, UnityAction ac)
        {
            //如果已经包含了 不是第一次添加
            if (CheckEvent(eType))
            {
                //使用了里市替换
                m_eventDic[eType].Add(ac);
            }
            //如果是第一次,就直接添加到字典里面
            else
            {
                List<Delegate> list = new List<Delegate>();
                list.Add(ac);
                m_eventDic.Add(eType, list);
            }
        }

        public void AddListener<T>(GameEventType eType, UnityAction<T> ac)
        {
            //如果已经包含了 不是第一次添加
            if (CheckEvent(eType))
            {
                //使用了里市替换
                m_eventDic[eType].Add(ac);
            }
            //如果是第一次,就直接添加到字典里面
            else
            {
                List<Delegate> list = new List<Delegate>();
                list.Add(ac);
                m_eventDic.Add(eType, list);
            }
        }

        public void AddListener<T, K>(GameEventType eType, UnityAction<T, K> ac)
        {
            //如果已经包含了 不是第一次添加
            if (CheckEvent(eType))
            {
                //使用了里市替换
                m_eventDic[eType].Add(ac);
            }
            //如果是第一次,就直接添加到字典里面
            else
            {
                List<Delegate> list = new List<Delegate>();
                list.Add(ac);
                m_eventDic.Add(eType, list);
            }
        }

        public void AddListener<T, K, M>(GameEventType eType, UnityAction<T, K, M> ac)
        {
            //如果已经包含了 不是第一次添加
            if (CheckEvent(eType))
            {
                //使用了里市替换
                m_eventDic[eType].Add(ac);
            }
            //如果是第一次,就直接添加到字典里面
            else
            {
                List<Delegate> list = new List<Delegate>();
                list.Add(ac);
                m_eventDic.Add(eType, list);
            }
        }

        public void AddListener<T, K, M, K1>(GameEventType eType, UnityAction<T, K, M, K1> ac)
        {
            //如果已经包含了 不是第一次添加
            if (CheckEvent(eType))
            {
                //使用了里市替换
                m_eventDic[eType].Add(ac);
            }
            //如果是第一次,就直接添加到字典里面
            else
            {
                List<Delegate> list = new List<Delegate>();
                list.Add(ac);
                m_eventDic.Add(eType, list);
            }
        }


        /// <summary>
        /// 移除某一个事件中所有的监听者
        /// </summary>
        /// <param name="eType"></param>
        public bool RemoveAllListerner(GameEventType eType)
        {
            bool isSucessed = false;
            if (CheckEvent(eType))
            {
                //字典该Key值,且该key值所对应的所有方法全部干掉
                m_eventDic.Remove(eType);
                isSucessed = true;
            }
            else
            {
                Debug.LogWarningFormat("此事件类型还未添加{0},请仔细检查!", eType.ToString());
            }
            return isSucessed;

        }
        /// <summary>
        /// 移除某一个监听者
        /// </summary>
        /// <param name="eType"></param>
        /// <param name="ac"></param>
        /// <returns></returns>
        public bool RemoveListerner(GameEventType eType, UnityAction ac)
        {
            bool isSucessed = false;
            if (CheckEvent(eType))
            {
                for (int i = 0; i < m_eventDic[eType].Count; i++)
                {
                    //通过Equals去判断是不是 同一个对象
                    if (m_eventDic[eType][i].Equals(ac))
                    {
                        //如果是 就移除掉!
                        m_eventDic[eType].Remove(ac);
                        //Debug.Log("移除成功!");
                    }
                }
                isSucessed = true;
            }
            else
            {
                Debug.LogWarningFormat("此事件类型还未添加{0},请仔细检查!", eType.ToString());
            }
            if (isSucessed == false)
            {
                Debug.LogWarningFormat("要删除的委托对象不存在!{0},{1}", eType.ToString(), ac.ToString());
            }
            return isSucessed;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eType"></param>
        /// <param name="ac"></param>
        /// <returns></returns>
        public bool RemoveListerner<T>(GameEventType eType, UnityAction<T> ac)
        {
            bool isSucessed = false;
            if (CheckEvent(eType))
            {
                for (int i = 0; i < m_eventDic[eType].Count; i++)
                {
                    //通过Equals去判断是不是 同一个对象
                    if (m_eventDic[eType][i].Equals(ac))
                    {
                        //如果是 就移除掉!
                        m_eventDic[eType].Remove(ac);
                        Debug.Log("移除成功!");
                        break;
                    }
                    else
                    {
                        Debug.Log("不是同一个对象!");
                    }
                }
                isSucessed = true;
            }
            else
            {
                Debug.LogWarningFormat("此事件类型还未添加{0},请仔细检查!", eType.ToString());
            }
            if (isSucessed == false)
            {
                Debug.LogWarningFormat("要删除的委托对象不存在!{0},{1}", eType.ToString(), ac.ToString());
            }
            return isSucessed;
        }

        public bool RemoveListerner<T, K>(GameEventType eType, UnityAction<T, K> ac)
        {
            bool isSucessed = false;
            if (CheckEvent(eType))
            {
                for (int i = 0; i < m_eventDic[eType].Count; i++)
                {
                    //通过Equals去判断是不是 同一个对象
                    if (m_eventDic[eType][i].Equals(ac))
                    {
                        //如果是 就移除掉!
                        m_eventDic[eType].Remove(ac);
                    }
                }
                isSucessed = true;
            }
            else
            {
                Debug.LogWarningFormat("此事件类型还未添加{0},请仔细检查!", eType.ToString());
            }
            if (isSucessed == false)
            {
                Debug.LogWarningFormat("要删除的委托对象不存在!{0},{1}", eType.ToString(), ac.ToString());
            }
            return isSucessed;
        }

        public bool RemoveListerner<T, K, M>(GameEventType eType, UnityAction<T, K, M> ac)
        {
            bool isSucessed = false;
            if (CheckEvent(eType))
            {
                for (int i = 0; i < m_eventDic[eType].Count; i++)
                {
                    //通过Equals去判断是不是 同一个对象
                    if (m_eventDic[eType][i].Equals(ac))
                    {
                        //如果是 就移除掉!
                        m_eventDic[eType].Remove(ac);
                    }
                }
                isSucessed = true;
            }
            else
            {
                Debug.LogWarningFormat("此事件类型还未添加{0},请仔细检查!", eType.ToString());
            }
            if (isSucessed == false)
            {
                Debug.LogWarningFormat("要删除的委托对象不存在!{0},{1}", eType.ToString(), ac.ToString());
            }
            return isSucessed;
        }

        public bool RemoveListerner<T, K, M, K1>(GameEventType eType, UnityAction<T, K, M, K1> ac)
        {
            bool isSucessed = false;
            if (CheckEvent(eType))
            {
                for (int i = 0; i < m_eventDic[eType].Count; i++)
                {
                    //通过Equals去判断是不是 同一个对象
                    if (m_eventDic[eType][i].Equals(ac))
                    {
                        //如果是 就移除掉!
                        m_eventDic[eType].Remove(ac);
                    }
                }
                isSucessed = true;
            }
            else
            {
                Debug.LogWarningFormat("此事件类型还未添加{0},请仔细检查!", eType.ToString());
            }
            if (isSucessed == false)
            {
                Debug.LogWarningFormat("要删除的委托对象不存在!{0},{1}", eType.ToString(), ac.ToString());
            }
            return isSucessed;
        }


        private bool CheckDicKey(GameEventType eType)
        {
            return !m_eventDic.ContainsKey(eType);
        }

        public void BroadCastEvent(GameEventType eType)
        {
            if (CheckDicKey(eType))
                return;

            List<Delegate> tempList = m_eventDic[eType];

            if (tempList == null || tempList.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < tempList.Count; i++)
            {
                UnityAction ac = tempList[i] as UnityAction;
                ac?.Invoke();
            }
        }

        public void BroadCastEvent<T>(GameEventType eType, T arg)
        {
            if (CheckDicKey(eType))
                return;

            List<Delegate> tempList = m_eventDic[eType];

            if (tempList == null || tempList.Count <= 0)
            {
                return;
            }
            //遍历集合 执行所有的监听事件
            for (int i = 0; i < tempList.Count; i++)
            {
                UnityAction<T> ac = tempList[i] as UnityAction<T>;
                ac?.Invoke(arg);
            }
        }

        public void BroadCastSingle<T>(GameEventType eType, UnityAction<T> uAc, T arg)
        {
            //不包含此类型 直接开除
            if (CheckDicKey(eType))
                return;

            List<Delegate> tempList = m_eventDic[eType];

            if (tempList == null || tempList.Count <= 0)
            {
                return;
            }
            //遍历集合 执行所有的监听事件
            for (int i = 0; i < tempList.Count; i++)
            {
                UnityAction<T> ac = tempList[i] as UnityAction<T>;
                if (ac.Equals(uAc))
                {
                    ac?.Invoke(arg);
                }
            }
        }

        public void BroadCastEvent<T, M>(GameEventType eType, T arg1, M arg2)
        {
            if (CheckDicKey(eType))
                return;

            List<Delegate> tempList = m_eventDic[eType];

            if (tempList == null || tempList.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < tempList.Count; i++)
            {
                UnityAction<T, M> ac = tempList[i] as UnityAction<T, M>;
                ac?.Invoke(arg1, arg2);
            }
        }

        public void BroadCastEvent<T, M, K>(GameEventType eType, T arg1, M arg2, K arg3)
        {
            if (CheckDicKey(eType))
                return;

            List<Delegate> tempList = m_eventDic[eType];

            if (tempList == null || tempList.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < tempList.Count; i++)
            {
                UnityAction<T, M, K> ac = tempList[i] as UnityAction<T, M, K>;
                ac?.Invoke(arg1, arg2, arg3);
            }
        }

        public void BroadCastEvent<T, M, K, K1>(GameEventType eType, T arg1, M arg2, K arg3, K1 arg4)
        {
            if (CheckDicKey(eType))
                return;

            List<Delegate> tempList = m_eventDic[eType];

            if (tempList == null || tempList.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < tempList.Count; i++)
            {
                UnityAction<T, M, K, K1> ac = tempList[i] as UnityAction<T, M, K, K1>;
                ac?.Invoke(arg1, arg2, arg3, arg4);
            }
        }

    }

}
