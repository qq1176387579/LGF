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

public class S_PlayerManager : S_SingletonBase<S_PlayerManager>
{

    Dictionary<uint, S_Player> playerList;


    public override void Init()
    {
        playerList = new Dictionary<uint, S_Player>();
    }




    public S_Player GetPlayerByID(uint uid)
    {
        if (!playerList.TryGetValue(uid,out S_Player player))
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
