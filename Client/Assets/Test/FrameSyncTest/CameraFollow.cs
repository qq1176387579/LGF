/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2023/10/24 17:59:38
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;

public class CameraFollow : SimpleMonoSingleton<CameraFollow>
{
    public Transform CameraFollowOb;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    private void LateUpdate()
    {
        if (CameraFollowOb == null) {
            return;
        }

        transform.position = Vector3.Lerp(transform.position, CameraFollowOb.transform.position, 0.1f);


    }
}
