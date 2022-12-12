/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/8/8 16:27:38
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LGF;
using LGF.Log;

public class TestChatPanel : MonoBehaviour
{
    public Transform contentRoot;
    public TestChatItemButton prefabItem;


    List<TestChatItemButton> chatItems = new List<TestChatItemButton>();

    public InputField sendMsg;
    public Button sendBtn;
    C_ChatModule ChatModule;

    void Start()
    {
        ChatModule = C_ModuleMgr.GetModule<C_ChatModule>();

        EventManager.Instance.AddListener<string, string>(GameEventType.c_TextMsg, OnTextMsg);

        sendBtn.onClick.AddListener(() =>
        {
            if (sendMsg.text.IsNullOrEmpty())
            {
                return;
            }

            ChatModule.SendMsg(sendMsg.text);

            sendMsg.text = "";
        });
    }



    void OnTextMsg(string name, string msg)
    {
        chatItems.Add(GameObject.Instantiate<TestChatItemButton>(prefabItem, contentRoot));
        chatItems[chatItems.Count - 1].Init($"\n {name} : {msg}\n");
        chatItems[chatItems.Count - 1].gameObject.SetActive(true);
    }


   
}
