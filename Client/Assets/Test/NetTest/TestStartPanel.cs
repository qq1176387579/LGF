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
        button.onClick.AddListener(() =>
        {
            NetTestEntry3.Instance.name = input.text;
            nextPanel.SetActive(true);
            gameObject.SetActive(false);
        });
    }

 
}