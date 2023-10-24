/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/12 0:21:54
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

namespace LGF.Server
{
    public abstract class SystemBase
    {
        protected ModuleMgr ModuleMgr;

        public void Init(ModuleMgr s_ModuleMgr)
        {
            ModuleMgr = s_ModuleMgr;
            OnInit();
        }

        protected abstract void OnInit();

    }
}
