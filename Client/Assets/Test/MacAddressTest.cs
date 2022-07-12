/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/21 21:55:40
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;
using LGF.Android;
using System.Net.NetworkInformation;
using UnityEngine.UI;
using System.Net;
using System;
using System.Threading.Tasks;
using LGF.Net;

/// <summary>
/// MacAddressTest 初始化
/// </summary>
public class MacAddressTest : MonoBehaviour
{
    public Text text;
    private void Awake()
    {
        AndroidMsgManager.Instance.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError("MacAddress Start");
#if USE_ANDROID
        AndroidMsgCenter.RegisterOnMsg(AndroidMsgDefine.GetUUIDResp, (a) => {
            text.text = a.strParam1;
        }); 
        //获得唯一标识符
        AndroidMsgCenter.SendMsg(AndroidMsgDefine.GetUUID);

        //text.text = GetMacAddress();
#else
        text.text = GetMacAddress();
#endif


    }

    private void OnDisable()
    {
        LHTestClient.Program.kcpSock.Dispose();
    }


 


}



