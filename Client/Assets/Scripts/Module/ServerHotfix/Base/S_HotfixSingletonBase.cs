/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/25 21:25:17
/// 功能描述:  对应数据的管理与操作  如所以玩家数据 所以房间数据
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;

//IServerSingletonBase
namespace LGF.Server.Hotfix
{
    public interface IS_HotfixSingletonBase
    {
        void Close();
    }

    public class S_HotfixSingletonBase<T, T1> : LGF.SingletonBase<T>, IS_HotfixSingletonBase where T : S_HotfixSingletonBase<T, T1>, new()
        where T1 : S_ManagerDataBase, new()
    {
        protected T1 _data;

        protected S_DataMaganer dataMgr;
        protected S_HotfixMoudleMgr hotfixMgr;
        protected KcpServer Server => moduleMgr.Server;
        protected S_ModuleMgr moduleMgr;

        public T Init(S_HotfixMoudleMgr s_HotfixMoudle)
        {
            hotfixMgr = s_HotfixMoudle;
            moduleMgr = hotfixMgr.moduleMgr;
            dataMgr = moduleMgr.dataMgr;
            hotfixMgr.AddManager(this);
            _data = dataMgr.GetData<T1>();
            if (_data == null)
            {
                sLog.Error(" type {0} _data Is null", typeof(T1));
            }
            Init();
            return _Instance;
        }

       


        /// <summary>
        /// 关闭服务器
        /// </summary>
        public virtual void Close()
        {
            SingletonClear(); //清除现在的单例
        }

    }



}

