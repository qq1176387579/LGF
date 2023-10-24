/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/14 23:06:22
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Logic;
using PEMath;

public enum UnitTypeEnum
{
    Player,
    Monster,
    Tower,
}

/// <summary>
/// 暂时先这个  后面改结构
/// </summary>

public partial class MainLogicUnit : LogicUnit
{
    public string testuid;  //零时uid

    public MainLogicUnit()
    {
    
    }


    public override void LogicInit()
    {
        InitMove();

    }

    public override void LogicTick()
    {
        TickMove();
    }

    public override void LogicUnInit()
    {

    }


    public void InputKey(C2S_FrameOpKey key)
    {
        if (key.moveKey != null) {
            PEInt x = PEInt.zero;
            x.ScaledValue = key.moveKey.x;
            PEInt z = PEInt.zero;
            z.ScaledValue = key.moveKey.z;
            InputMoveKey(new PEVector3(x, 0, z));

        }
        else if (key.skillKey != null) {

        }

     
    }
}