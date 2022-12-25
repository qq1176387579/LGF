/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/15 1:38:20
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

public class PlayerUnit : MainLogicUnit
{
    public int heroID;
    public int posIndex;
    public string userName;//玩家名字
    public uint playerid;


    public PlayerUnit(HeroData hd) : base(hd)
    {
        heroID = hd.heroID;
        posIndex = hd.posIndex;
        userName = hd.userName;

        unitType = UnitTypeEnum.Player;
        //unitName = ud.unitCfg.unitName + "_" + userName;
    }




}
