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

public class MacAddress : MonoBehaviour
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
        AndroidMsgCenter.Instance.RegisterOnMsg(AndroidMsgDefine.GetUUIDResp, (a) => {
            text.text = a.strParam1;
        });
        AndroidMsgCenter.Instance.SendMsg(AndroidMsgDefine.GetUUID);

        //text.text = GetMacAddress();
#else
        text.text = GetMacAddress();
#endif


        //LHTestServer.Program.Main(null);
        LHTestClient.Program.Main(null);
    }

    private void OnDisable()
    {
        //LHTestServer.Program.kcpServer.Dispose();
        LHTestClient.Program.kcpSock.Dispose();
    }



}



