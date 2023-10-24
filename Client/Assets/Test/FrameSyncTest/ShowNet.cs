/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/15 23:28:20
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using UnityEngine;

public class ShowNet : MonoBehaviour {

    public UnityEngine.UI.Text text;
    private void Start()
    {


      
    }

    private void Update()
    {
        if (ModuleMgr.GetSession().IsDisconnection()) {
            text.text = "连接不上服务器";
        }
        else {
            text.text = "连接中";
        }

    }

}


