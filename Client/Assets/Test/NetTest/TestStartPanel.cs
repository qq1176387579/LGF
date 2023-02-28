/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/27 18:21:58
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LGF;
using LGF.Log;

public class TestStartPanel : MonoBehaviour
{
    public GameObject nextPanel;
    // Start is called before the first frame update
    public Button button;
    public InputField input;
    

    void Start()
    {
        System.Random random = new System.Random();
        //random.get
        //Debug.LogError("GetLocalIPv4_IPAddress" + LGF.Net.SocketHelper.GetLocalIPv4_IPAddress());
        //Debug.LogError("GetLocalIPv4_IPAddress" + LGF.Net.SocketHelper.GetLocalIPv4_IPAddress().ToString());
        if (button == null)
        {
            Debug.LogError("-------sdf-");
        }
        button.onClick.AddListener(() =>
        {
            if (C_ModuleMgr.GetSession().IsTryConnecting)
            {
                Debug.Log("连接过程中");
                return;
            }
            //NetTestEntry3.Instance.name = input.text;
            C_ModuleMgr.GetPlayer().SetName(input.text);    //设置名字

            C_ModuleMgr.GetModule<C_LoginModule>().ConnectServer(() =>
            {
                Debug.Log("----连接成功---");//连接成功
                nextPanel.SetActive(true);
                gameObject.SetActive(false);
            });

          
        });
    }

 
}
