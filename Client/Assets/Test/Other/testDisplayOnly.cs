/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/7 10:04:48
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;

public class testDisplayOnly : MonoBehaviour
{
    [LGF.DisplayOnly]
    [UnityEngine.SerializeField]
    public int t = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
