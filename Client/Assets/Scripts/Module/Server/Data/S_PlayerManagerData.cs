///***************************************************
///// 作者:      liuhuan
///// 创建日期:  2022/12/11 23:22:39
///// 功能描述:  
//****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using static LGF.Net.KcpServer;




public class S_PlayerManagerData : S_ManagerDataBase
{
    public Dictionary<uint, S_Player> playerList;    //全部玩家

    public override void Init()
    {
        playerList = new Dictionary<uint, S_Player>();
    }
}