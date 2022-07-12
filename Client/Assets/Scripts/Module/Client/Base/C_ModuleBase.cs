/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/13 0:21:00
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;

public class C_ModuleBase 
{
    protected C_ModuleMgr s_ModuleMgr = C_ModuleMgr.Instance;
    protected KcpClient Client => s_ModuleMgr.Client;

    public virtual void Init()
    {

    }

}
