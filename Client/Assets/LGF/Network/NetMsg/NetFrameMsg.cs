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

public enum Frame_KeyType
{
    Kill,
    Move
}



/// <summary>
/// 帧操作
/// </summary>
[SteamContract]

public class S2C_FrameOpKey : S2C_BASE<S2C_FrameOpKey>
{
    public S2C_FrameOpKey() : base(NetMsgDefine.S2C_FrameOpKey) { }

    [SteamMember(1)]
    public List<C2S_FrameOpKey> allOpkey;
}




/// <summary>
/// 帧操作
/// </summary>
[SteamContract]

public class C2S_FrameOpKey : C2S_BASE<C2S_FrameOpKey>
{
    public C2S_FrameOpKey() : base(NetMsgDefine.C2S_FrameOpKey) { }

    [SteamMember(1)]
    public Frame_KeyType keytype;
    [SteamMember(2)]
    public Frame_SkiilKey skillKey;
    [SteamMember(3)]
    public Frame_MoveKey moveKey;
}



[SteamContract]
public class Frame_SkiilKey : CMD_BASE2<Frame_SkiilKey>
{
    [SteamMember(1)]
    public uint skillID;
    [SteamMember(2)]
    public long x_value;
    [SteamMember(3)]
    public long z_value;
}



[SteamContract]
public class Frame_MoveKey : CMD_BASE2<Frame_MoveKey>
{
    [SteamMember(1)]
    public uint Keyid;
    [SteamMember(2)]
    public long x;
    [SteamMember(3)]
    public long z;
}




#endregion