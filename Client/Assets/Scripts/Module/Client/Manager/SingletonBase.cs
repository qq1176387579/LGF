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

public class ModuleSingletonBase<T> : LGF.SingletonBase<T> where T : ModuleSingletonBase<T>, new()
{
    protected KcpClient Client => moduleMgr.Client;
    protected MainPlayerManager player => moduleMgr.mainPlayerMgr;
    protected ModuleMgr moduleMgr;

    public T Init(ModuleMgr _moduleMgr)
    {
        moduleMgr = _moduleMgr;

        Init();
        return _Instance;
    }

    







}