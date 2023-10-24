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
    //坑如果ServerConfig 在热更脚本中 那么热更资源加载不了该代码
    //在热更新中弃用 后续换json 或者txt来读取
    //public ServerConfig serverInfo;

    //public override void Init()
    //{
    //    base.Init();
    //    new ServerConfig();
    //    Debug.Log("new ServerConfig -- testConfig");
    //    //serverInfo = AddressableManager.Instance.LoadAsset2<ServerConfig>("testConfig"); //加载配置表
    //    //serverInfo = Load<ServerConfig>("ServerConfig");
    //    if (serverInfo == null) {
    //        Debug.LogError("配置文件为空");
    //    }
    //}

    //T Load<T>(string path) where T : ScriptableObject
    //{
    //    //return Resources.Load<T>(path);
    //    return AddressableManager.Instance.LoadAsset2<T>(path);
    //}

}



#endif

