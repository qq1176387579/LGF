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



public class S_SingletonBase<T> : LGF.SingletonBase<T> where T : S_SingletonBase<T>, new()
{
    protected KcpServer Server => moduleMgr.Server;
    protected S_ModuleMgr moduleMgr;

    public T Init(S_ModuleMgr s_ModuleMgr)
    {
        moduleMgr = s_ModuleMgr;
     
        Init();
        return _Instance;
    }

    


   
}
