/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/12 22:18:38
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;

public class C_SingletonBase<T> : LGF.SingletonBase<T> where T : C_SingletonBase<T>, new()
{
    protected KcpClient Client => moduleMgr.Client;
    protected C_MainPlayerManager player => moduleMgr.mainPlayerMgr;
    protected C_ModuleMgr moduleMgr;

    public T Init(C_ModuleMgr c_ModuleMgr)
    {
        moduleMgr = c_ModuleMgr;

        Init();
        return _Instance;
    }

    







}