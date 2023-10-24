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


namespace LGF.Server
{
    public class PlayerManagerData : ManagerDataBase
    {
        public Dictionary<uint, Player> playerList;    //全部玩家

        /// <summary>
        /// 所有在线的玩家
        /// </summary>
        public List<Player> OnlinePlayerList2;   //全部玩家      //本来没有这个的  为了兼容原来的写法添加这个

        public List<Player> disconnectionPlayers;   //断线玩家

        public uint CurUid = 1;  //当前唯一ID

        public override void Init()
        {
            playerList = new Dictionary<uint, Player>();
            disconnectionPlayers = new List<Player>();
            OnlinePlayerList2 = new List<Player>();

        }
    }
}

