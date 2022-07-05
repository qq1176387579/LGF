/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/2 20:04:15
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;

public class TestString : MonoBehaviour
{
    char[] chars;
    void Start()
    {
        chars = new char[3] { '你', '号', '啊' };
        string t = new string(chars);
        string t2 = new string(chars, 0, 2);

        Debug.LogError(t);
        Debug.LogError(t2);

        chars[1] = '是';
        Debug.LogError(t);
        Debug.LogError(t2);
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
