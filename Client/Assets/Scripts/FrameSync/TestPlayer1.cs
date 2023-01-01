/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/30 21:31:55
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;
using LGF.Logic;

public class TestPlayer1 : Mono2Base , ILogic
{
    protected override MonoType monoType => MonoType.OnUpdate;
    public PlayerUnit logicUnit = new PlayerUnit();
    public PlayerViewUnit viewUnit = new PlayerViewUnit();  //视图单位



    public void Init()
    {
        logicUnit.LogicPos = new PEMath.PEVector3(0, 0, 0); //初始位置
        logicUnit.SetAttr(ATTR_TYPE.SPD, 5);    //基础速度

        viewUnit.Init(logicUnit, transform, Resources.Load<ViewUnitConfig>("UnitDataConfig/ViewUnitConfig"));
    }

    /// <summary>
    /// 设置位置
    /// </summary>
    public void SetPos(Vector3 vector3)
    {
        logicUnit.LogicPos = new PEMath.PEVector3(vector3);
        transform.position = vector3;
    }


    protected override void OnUpdate()
    {
        //Debug.LogError("--TestPlayer1---OnUpdate-");
        viewUnit.OnUpdate();
    }

    public void LogicInit()
    {
        logicUnit.LogicInit();
    }

    public void LogicTick()
    {
        logicUnit.LogicTick();
    }

    public void LogicUnInit()
    {
        logicUnit.LogicUnInit();
    }
}
