/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/2 2:02:31
/// 功能描述:  消息处理
****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Net;
using LGF;
using LGF.Log;
using LGF.Serializable;
using LGF.DataStruct;

namespace LGF.Net
{

    public interface INetMsgHandling
    {
        public void OnCSNetMsg(KcpCSBase csBase, in EndPoint point, LStream stream);
        //public void OnKcpNetMsg(ISession agent, LStream stream);

        /// <summary>
        /// 流会被序列化 并偏移，  所有不要将使 已经反序列过的LStream在丢进来
        /// </summary>
        /// <param name="server"></param>
        /// <param name="type"></param>
        /// <param name="sessionID"></param>
        /// <param name="_stream"></param>
        public void OnServerMsg(KcpServer server, NetMsgDefine type, uint sessionID, LStream _stream);

        public void OnClientMsg(KcpClient client, NetMsgDefine type, LStream _stream);

    }

    /// <summary>
    /// 网络消息管理器
    /// 绑定AppEntry
    /// 
    /// QueueOnMainThread 主线程函数
    /// </summary>
    public class NetMsgMgr : SingletonBase<NetMsgMgr>
    {
       
        NBufferingQueue<IDelegateBase> queue = new NBufferingQueue<IDelegateBase>();
        protected override void OnNew()
        {
            LGFEntry.RegisterOnUpdate(OnUpdate);

            queue.OnClear((a) => a.Release());

            //无意义 也执行不了
            //EventManager.Instance.AddListener(GameEventType.OnReLoadHotfixFinish, OnReLoadHotfixFinish);
        }

        //bool isReLoadHotfixFinish = false;
        //void OnReLoadHotfixFinish()
        //{
        //    isReLoadHotfixFinish = true;    //执行了热修复
        //}


        /// <summary>
        /// 主线程update
        /// </summary>
        void OnUpdate()
        {
            if (!queue.CanGet()) return;

            var list = queue.Get();
            foreach (var item in list)
            {
                item.Invoke2();
            }

            queue.Clear();
        }


        ///// <summary>
        ///// kcp消息处理 
        ///// </summary>
        //public void OnKcpNetMsg(ISession agent, LStream stream)
        //{
        //    m_MsgHandling.OnKcpNetMsg(agent, stream);
        //}

        ///// <summary>
        ///// 消息处理cs处理  接收数据
        ///// </summary>
        //public void OnCSNetMsg(KcpCSBase csBase,in EndPoint point, LStream stream)
        //{
        //    m_MsgHandling.OnCSNetMsg(csBase, point, stream);
        //}


        #region QueueOnMainThread  主线程处理数据

  
        /// <summary>
        /// 添加到主线程
        /// </summary>
        public static void QueueOnMainThread(IDelegateBase evt)
        {
            Instance.queue.Add(evt);
        }


        /// <summary>
        /// 添加到主线程
        /// </summary>
        public static void QueueOnMainThread(System.Action evt)
        {
            Instance.queue.Add(NDelegate.Get(evt));
        }

        /// <summary>
        ///静态方法 添加到主线程
        /// </summary>
        public static void QueueOnMainThread<T1>(System.Action<T1> evt, in T1 param2)
        {
            Instance.queue.Add(NDelegate.Get(evt, param2));
        }

        /// <summary>
        ///静态方法 添加到主线程
        /// </summary>
        public static void QueueOnMainThread<T1, T2>(System.Action<T1, T2> evt, in T1 param2, in T2 param3)
        {
            Instance.queue.Add(NDelegate.Get(evt, param2, param3));
        }


        /// <summary>
        ///静态方法 添加到主线程
        /// </summary>
        public static void QueueOnMainThread<T1, T2, T3>(System.Action<T1, T2, T3> evt, in T1 param2, in T2 param3, in T3 param4)
        {
            Instance.queue.Add(NDelegate.Get(evt, param2, param3, param4));
        }


        /// <summary>
        ///静态方法 添加到主线程
        /// </summary>
        public static void QueueOnMainThread<T1, T2, T3, T4>(System.Action<T1, T2, T3, T4> evt, in T1 param2, in T2 param3, in T3 param4, in T4 param5)
        {
            Instance.queue.Add(NDelegate.Get(evt, param2, param3, param4, param5));
        }


        /// <summary>
        /// 静态方法 添加到主线程
        /// </summary>
        public static void QueueOnMainThreadt(IDelegateBase evt)
        {
            Instance.queue.Add(evt);
        }




        /// <summary>
        /// 添加到主线程
        /// </summary>
        public void QueueOnMainThreadt(System.Action evt)
        {
            Instance.queue.Add(NDelegate.Get(evt));
        }

        /// <summary>
        /// 添加到主线程
        /// </summary>
        public void QueueOnMainThreadt<T1>(System.Action<T1> evt, in T1 param2)
        {
            Instance.queue.Add(NDelegate.Get(evt, param2));
        }

        /// <summary>
        /// 添加到主线程
        /// </summary>
        public void QueueOnMainThreadt<T1, T2>(System.Action<T1, T2> evt, in T1 param2, in T2 param3)
        {
            Instance.queue.Add(NDelegate.Get(evt, param2, param3));
        }


        /// <summary>
        /// 添加到主线程
        /// </summary>
        public void QueueOnMainThreadt<T1, T2, T3>(System.Action<T1, T2, T3> evt, in T1 param2, in T2 param3, in T3 param4)
        {
            queue.Add(NDelegate.Get(evt, param2, param3, param4));
        }


        /// <summary>
        /// 添加到主线程
        /// </summary>
        public void QueueOnMainThreadt<T1, T2, T3,T4>(System.Action<T1, T2, T3,T4> evt, in T1 param2, in T2 param3, in T3 param4, in T4 param5)
        {
            queue.Add(NDelegate.Get(evt, param2, param3, param4, param5));
        }


        public void BroadCastEventByMainThreadt<T1, T2>(GameEventType type, in T1 param2, in T2 param3)
        {
            QueueOnMainThreadt((type, _param2, _param3) =>
            {
                EventManager.Instance.BroadCastEvent(type, _param2, _param3);
            }, type, param2, param3);
        }

        public void BroadCastEventByMainThreadt<T1>(GameEventType type, in T1 param2)
        {
            QueueOnMainThreadt((type, _param2) =>
            {
                EventManager.Instance.BroadCastEvent(type, _param2);
            }, type, param2);
        }

        public void BroadCastEventByMainThreadt(GameEventType type)
        {
            QueueOnMainThreadt((type) =>
            {
                EventManager.Instance.BroadCastEvent(type);
            }, type);
        }


        #endregion
    }

}
