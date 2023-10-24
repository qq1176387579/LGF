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
    public class PlayerManager : HotfixSingletonBase<PlayerManager, PlayerManagerData>
    {
        Dictionary<uint, Player> playerList => _data.playerList;

        /// <summary>
        /// 所有在线的玩家  暂时用不上
        /// </summary>
        public List<Player> OnlinePlayerList2 => _data.OnlinePlayerList2;   //全部玩家      //本来没有这个的  为了兼容原来的写法添加这个

        /// <summary>
        /// 暂时用不上
        /// </summary>
        public List<Player> disconnectionPlayers => _data.disconnectionPlayers;   //断线玩家

        public override void Init()
        {
            base.Init();
        }


        public Dictionary<uint, Player> GetAllPlayer()
        {
            return playerList;
        }

        public Player GetPlayerByID(uint uid)
        {
            if (!playerList.TryGetValue(uid, out Player player))
            {
                return null;
            }
            return player;
        }

        public uint GenerateUniqueUID()
        {
            return _data.CurUid++;
        }

        public Player AddNewPlayer(KcpSession session, string name)
        {
            Player player = new Player();
            player.uid = session.useid = GenerateUniqueUID();
            session.account = name;
            player.name = name;
            player.session = session;
            playerList.Add(player.uid, player);
            return player;
        }


        public void OnOffline(Player player)
        {
            playerList.Remove(player.uid);
            player.room?.LeaveRoom(player);   //离开房间
            player.session.ServerCloseSession();
        }


    }

}
