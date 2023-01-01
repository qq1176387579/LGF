/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/30 18:51:31
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using UnityEngine;

[System.Serializable]
public class PlayerViewUnit : MainViewUnit
{
    public override void Init(LogicUnit logicUnit, Transform transform, ViewUnitConfig cfg)
    {
        base.Init(logicUnit, transform, cfg);
    }

    protected override Vector3 GetUnitViewDir()
    {
        //玩家朝向使用UI输入位置朝向，不使用物理引擎运算修正方向
        return mainLogicUnit.InputDir.ConvertViewVector3();
    }

}
