/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/30 22:51:31
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;

public class TestPlayerssss2 : MonoBehaviour
{
    Vector3 dir;
    // Start is called before the first frame update
    void Start()
    {
        LGF.EventManager.Instance.AddListener<Vector3>(LGF.GameEventType.Test, (_dir) => dir = _dir);
    }


    // Update is called once per frame
    void Update()
    {
        if (dir == Vector3.zero)
        {
            return;
        }
        transform.forward = dir.normalized;
        transform.position = transform.position + transform.forward * 5f * Time.deltaTime;
    }
}
