/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/11 23:22:39
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using static LGF.Net.KcpServer;

namespace LGF.Server.Hotfix
{
    public class S_PlayerManager : S_HotfixSingletonBase<S_PlayerManager, S_PlayerManagerData>
    {
        Dictionary<uint, S_Player> playerList => _data.playerList;
        public override void Init()
        {
            base.Init();
        }


        public Dictionary<uint, S_Player> GetAllPlayer()
        {
            return playerList;
        }

        public S_Player GetPlayerByID(uint uid)
        {
            if (!playerList.TryGetValue(uid, out S_Player player))
            {
                return null;
            }
            return player;
        }


        public void AddNewPlayer(KcpSession session)
        {
            S_Player player = new S_Player();

            player.uid = session.playerID;
            player.name = session.name;
            player.session = session;
            playerList.Add(player.uid, player);
        }






    }

}
