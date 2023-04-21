/***************************************************
/// 作者:      
/// 创建日期:  
/// 功能描述:  事件系统 线程不安全  请规范使用  需要删除的不要注册到这里
///            该方法还有很多要优化的地方 且没有异常捕获
///            后期这个枚举可以封装出去  外部调用的时候 继承这个枚举就行了 不然会影响项目
****************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using LGF.Log;


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
        public void AddListener(GameEventType eType, System.Action ac)
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

        public void AddListener<T>(GameEventType eType, System.Action<T> ac)
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

        public void AddListener<T, K>(GameEventType eType, System.Action<T, K> ac)
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

        public void AddListener<T, K, M>(GameEventType eType, System.Action<T, K, M> ac)
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

        public void AddListener<T, K, M, K1>(GameEventType eType, System.Action<T, K, M, K1> ac)
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
                sLog.Warning("此事件类型还未添加{0},请仔细检查!", eType.ToString());
            }
            return isSucessed;

        }
        /// <summary>
        /// 移除某一个监听者
        /// </summary>
        /// <param name="eType"></param>
        /// <param name="ac"></param>
        /// <returns></returns>
        public bool RemoveListerner(GameEventType eType, System.Action ac)
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
                        //sLog.Debug("移除成功!");
                    }
                }
                isSucessed = true;
            }
            else
            {
                sLog.Warning("此事件类型还未添加{0},请仔细检查!", eType.ToString());
            }
            if (isSucessed == false)
            {
                sLog.Warning("要删除的委托对象不存在!{0},{1}", eType.ToString(), ac.ToString());
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
        public bool RemoveListerner<T>(GameEventType eType, System.Action<T> ac)
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
                        //sLog.Debug("移除成功!");
                        break;
                    }
                    else
                    {
                        sLog.Debug("不是同一个对象!");
                    }
                }
                isSucessed = true;
            }
            else
            {
                sLog.Warning("此事件类型还未添加{0},请仔细检查!", eType.ToString());
            }
            if (isSucessed == false)
            {
                sLog.Warning("要删除的委托对象不存在!{0},{1}", eType.ToString(), ac.ToString());
            }
            return isSucessed;
        }

        public bool RemoveListerner<T, K>(GameEventType eType, System.Action<T, K> ac)
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
                sLog.Warning("此事件类型还未添加{0},请仔细检查!", eType.ToString());
            }
            if (isSucessed == false)
            {
                sLog.Warning("要删除的委托对象不存在!{0},{1}", eType.ToString(), ac.ToString());
            }
            return isSucessed;
        }

        public bool RemoveListerner<T, K, M>(GameEventType eType, System.Action<T, K, M> ac)
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
                sLog.Warning("此事件类型还未添加{0},请仔细检查!", eType.ToString());
            }
            if (isSucessed == false)
            {
                sLog.Warning("要删除的委托对象不存在!{0},{1}", eType.ToString(), ac.ToString());
            }
            return isSucessed;
        }

        public bool RemoveListerner<T, K, M, K1>(GameEventType eType, System.Action<T, K, M, K1> ac)
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
                sLog.Warning("此事件类型还未添加{0},请仔细检查!", eType.ToString());
            }
            if (isSucessed == false)
            {
                sLog.Warning("要删除的委托对象不存在!{0},{1}", eType.ToString(), ac.ToString());
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
                System.Action ac = tempList[i] as System.Action;
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
                System.Action<T> ac = tempList[i] as System.Action<T>;
                ac?.Invoke(arg);
            }
        }

        public void BroadCastSingle<T>(GameEventType eType, System.Action<T> uAc, T arg)
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
                System.Action<T> ac = tempList[i] as System.Action<T>;
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
                System.Action<T, M> ac = tempList[i] as System.Action<T, M>;
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
                System.Action<T, M, K> ac = tempList[i] as System.Action<T, M, K>;
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
                System.Action<T, M, K, K1> ac = tempList[i] as System.Action<T, M, K, K1>;
                ac?.Invoke(arg1, arg2, arg3, arg4);
            }
        }

    }

}
