/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/12 23:31:26
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;

public class S_ModuleBase
{
    protected S_ModuleMgr s_ModuleMgr = S_ModuleMgr.Instance;
    protected KcpServer Server => s_ModuleMgr.Server;

    public virtual void Init()
    {

    }


}
