/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/11 18:50:53
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
#if !NOT_UNITY
using UnityEngine;

public class AppConfig : SingletonBase<AppConfig>
{
    public ServerConfig serverInfo;

    public override void Init()
    {
        base.Init();
        serverInfo = Load<ServerConfig>("Config2\\ServerConfig");
    }

    T Load<T>(string path) where T : ScriptableObject
    {
        return Resources.Load<T>(path);
    }

}



#endif

