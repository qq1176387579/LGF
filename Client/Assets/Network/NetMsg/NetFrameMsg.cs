/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/24 18:18:19
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Serializable;


//帧消息



#region 操作


/// <summary>
/// 同步进入进度
/// </summary>
[SteamContract]

public partial class S2C_RoomtFinishType : S2C_BASE<S2C_RoomtFinishType>
{
    public S2C_RoomtFinishType() : base(NetMsgDefine.RoomtFinishType) { }
    [SteamMember(1)]
    //type = 1 标识开始场景加载  2房间玩家全部加载完成 
    public int type;
}



/// <summary>
/// 同步进入进度
/// </summary>
[SteamContract]

public partial class S2C_RoomProgress : S2C_BASE<S2C_RoomProgress>
{
    public S2C_RoomProgress() : base(NetMsgDefine.RoomProgress) { }
    [SteamMember(1)]
    public List<C2S_RoomProgress> list;

}




/// <summary>
/// 同步进入进度
/// </summary>
[SteamContract]

public partial class C2S_RoomProgress : C2S_BASE<C2S_RoomProgress>
{
    public C2S_RoomProgress() : base(NetMsgDefine.RoomProgress) { }
    [SteamMember(3)]
    public uint uid;
    [SteamMember(1)]
    public int progress;    //进度值   -1表示加载完成
}




/// <summary>
/// 弃用
/// </summary>
public enum Frame_KeyType
{
    Kill,
    Move
}



/// <summary>
/// 帧操作
/// </summary>
[SteamContract]

public partial class S2C_FrameOpKey : S2C_BASE<S2C_FrameOpKey>
{
    public S2C_FrameOpKey() : base(NetMsgDefine.FrameOpKey) { }

    [SteamMember(1)]
    public ulong curFrame;  //当前是第几帧

    [SteamMember(2)]
    public List<C2S_FrameOpKey> allOpkey;
}




/// <summary>
/// 帧操作
/// </summary>
[SteamContract]

public partial class C2S_FrameOpKey : C2S_BASE<C2S_FrameOpKey>
{
    public C2S_FrameOpKey() : base(NetMsgDefine.FrameOpKey) { }
    [SteamMember(0)]
    public uint uid;

    [SteamMember(1)]
    public Frame_KeyType keytype;   //弃用
    [SteamMember(2)]
    public Frame_SkiilKey skillKey;
    [SteamMember(3)]
    public Frame_MoveKey moveKey;
}



[SteamContract]
public partial class Frame_SkiilKey : CMD_BASE2<Frame_SkiilKey>
{
    [SteamMember(1)]
    public uint skillID;
    [SteamMember(2)]
    public long x_value;
    [SteamMember(3)]
    public long z_value;
}



[SteamContract]
public partial class Frame_MoveKey : CMD_BASE2<Frame_MoveKey>
{
    [SteamMember(1)]
    public uint Keyid;
    [SteamMember(2)]
    public long x;
    [SteamMember(3)]
    public long z;
}




#endregion