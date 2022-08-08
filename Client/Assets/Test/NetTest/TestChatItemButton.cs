/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/8/8 17:15:26
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LGF;
using LGF.Log;

public class TestChatItemButton : MonoBehaviour
{
    public Text text;

    public void Init(string str)
    {
        text.text = str;
    }

}
