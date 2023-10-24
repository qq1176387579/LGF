/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/12 16:19:59
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Net;
using LGF.Log;

namespace LGF.Server
{
    public interface IServerSingletonBase
    {
        void Close();
    }

    public class ModuleSingletonBase<T> : LGF.SingletonBase<T>, IServerSingletonBase where T : ModuleSingletonBase<T>, new()
    {
        protected KcpServer Server => moduleMgr.Server;
        protected ModuleMgr moduleMgr;

        public T Init(ModuleMgr s_ModuleMgr)
        {
            moduleMgr = s_ModuleMgr;
            moduleMgr.AddManager(typeof(T), this);
            Init();
            return _Instance;
        }


        /// <summary>
        /// 关闭服务器
        /// </summary>
        public virtual void Close()
        {
            SingletonClear();
        }

    }
}
