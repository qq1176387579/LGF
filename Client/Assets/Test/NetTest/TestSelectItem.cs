/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/27 18:33:06
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LGF;
using LGF.Log;

public class TestSelectItem : MonoBehaviour
{
    public int id;
    public System.Action<int> itemEvent;
    Text text;
    public void Init(int id_, string str)
    {
        id = id_;
        if (text == null)
            text = transform.GetChild(0).GetComponent<Text>();
        text.text = str;
    }

    public void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            itemEvent?.Invoke(id);  //
        });
    }


}
