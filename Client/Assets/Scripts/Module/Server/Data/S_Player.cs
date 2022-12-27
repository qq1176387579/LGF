/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/11 23:19:33
/// 功能描述:  玩家的数据部分
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using static LGF.Net.KcpServer;

public class S_Player 
{
    public uint uid;
    public string name;
    public KcpSession session;

    #region 房间
    public uint roomid;
    public uint roomjoinRank;
    public S_Room room;

    public bool InRoom => roomid > 0;
    public bool roomReally; //房间是是否是准备状态

    public bool loadFinish; //是否完成加载

    #endregion



}
