/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/12 0:43:04
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LGF;

using LGF.Log;


public class S_Room : Poolable<S_Room>
{
    public uint id;                     //房间id
    public string roomName;             //房间名字
    public S_Player houseOwner;         //房主
    public Dictionary<uint, S_Player> playerList = new Dictionary<uint, S_Player>();    //房间玩家
    public int count => playerList.Count;
    public uint curRoomjoinRank;   //当前房间加入排名




    protected override void OnRelease()
    {
        base.OnRelease();
        playerList.Clear();
        houseOwner = null;
    }


}
