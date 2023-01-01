/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/30 18:04:39
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Logic;
using PEMath;

public partial class MainLogicUnit
{
    Attr attr = new Attr();

    public PEInt Hp { get => attr[ATTR_TYPE.HP]; set => attr[ATTR_TYPE.HP] = value; }
    public PEInt Def { get => attr[ATTR_TYPE.DEF]; set => attr[ATTR_TYPE.DEF] = value; }

    public override PEInt LogicMoveSpeed { get => attr[ATTR_TYPE.SPD]; set => attr[ATTR_TYPE.SPD] = value; }

    public Attr Attr { get => attr; }
    public UnitTypeEnum unitType;

    //public LogicUnitData ud;

    public void SetAttr(ATTR_TYPE type, PEInt val)
    {
        attr[type] = val;
    }

    public PEInt GetAttr(ATTR_TYPE type, PEInt val)
    {
        return attr[type];
    }

}