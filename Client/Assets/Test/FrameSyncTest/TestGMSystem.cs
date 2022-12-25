/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/15 23:28:20
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

public class TestGMSystem : SimpleMonoSingleton<TestGMSystem>
{
    private void Start()
    {
        DontDestroyOnLoad(this);
    }

}
