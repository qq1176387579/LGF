/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/2 1:15:10
/// 功能描述:  网络消息 暂时先放在这处理
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Serializable;


public static class NetMsgHelper
{
    public static int INT32_SIZE = 4;
}


//这里不加命名空间了 加命名空间需要



#region  CMD base


public class CMD_BASE2<T> : Poolable<T>, ISerializer where T : CMD_BASE2<T>, new()
{

    public virtual void Deserialize(LStream stream)
    {
        throw new NotImplementedException("请先实现化注册该方法 或者 使用unity菜单栏 tools/SteamSerializable/Generated  生成");
    }

    public virtual void Serialize(LStream stream)
    {
        throw new NotImplementedException("请先实现化注册该方法 或者 使用unity菜单栏 tools/SteamSerializable/Generated  生成");
    }

    public static T Get(LStream stream)
    {
        return Get().NDeserialize(stream);
    }


    public virtual T NDeserialize(LStream stream)
    {
        Deserialize(stream);
        return this as T;
    }

    /// <summary>
    /// 序列化 讲数据传给
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public virtual T NSerialize(LStream stream)
    {
        Serialize(stream);
        return this as T;
    }

    protected override void OnGet()
    {
        base.OnGet();
    }
}



public class CMD_BASE<T> : CMD_BASE2<T> where T : CMD_BASE<T>, new()
{
    [LGF.Serializable.SteamMember(-1000)]
    public NetMsgDefine msgType;   //消息类型

    public CMD_BASE(NetMsgDefine type)
    {
        this.msgType = type;
    }
}


public partial class C2S_BASE<T> : CMD_BASE<T> where T : C2S_BASE<T>, new()
{
    [LGF.Serializable.SteamMember(-999)]
    public uint uid;    //用户ID 玩家ID
    public C2S_BASE(NetMsgDefine type, uint uid_ = 0) : base(type)
    {
        this.uid = uid_;
    }

}


public partial class S2C_BASE<T> : CMD_BASE<T> where T : S2C_BASE<T>, new()
{
    [SteamMember(-999)]
    public ErrCode ErrorCode;

    public S2C_BASE(NetMsgDefine type) : base(type) { }

    protected override void OnGet()
    {
        base.OnGet();
        ErrorCode = ErrCode.Succeed;    //默认成功
    }

}

#endregion

///// <summary>
///// 消息定义类型  最基础的消息处理
///// </summary>
//[LGF.Serializable.SteamContract]
//public partial class CMD_MSG_Customize : CMD_BASE<CMD_MSG_Customize>
//{
//    public CMD_MSG_Customize() : base(NetMsgDefine.Customize) { }

//    [LGF.Serializable.SteamMember(2)]
//    public NetMsgDefine CustomizeType;   //消息类型

//    [LGF.Serializable.SteamMember(3)]   //不支持数组  有GC  就算实现该方法   也不好控制数据大小
//    public byte[] data;
//}

#region 非KCP 消息


/// <summary>
/// 获得所有服务器的信息
/// </summary>
[LGF.Serializable.SteamContract]
public partial class N_C2S_GetAllServersInfo : CMD_BASE<N_C2S_GetAllServersInfo>
{
    public N_C2S_GetAllServersInfo() : base(NetMsgDefine.N_C2S_GetAllServersInfo) { }
}


/// <summary>
/// 服务端 返回连接 信息
/// </summary>
[LGF.Serializable.SteamContract]
public partial class N_S2C_GetAllServersInfo : CMD_BASE<N_S2C_GetAllServersInfo>
{
    public N_S2C_GetAllServersInfo() : base(NetMsgDefine.N_S2C_GetAllServersInfo) { }

    [LGF.Serializable.SteamMember(1)]
    public int port;    //端口号
}

#endregion


#region 连接服务器的后的消息通知 消息


/// <summary>
/// 客户端连接  并传递客户端信息
/// </summary>
[LGF.Serializable.SteamContract]
public partial class C2S_Connect : CMD_BASE<C2S_Connect>
{
    public C2S_Connect() : base(NetMsgDefine.C2S_Connect) { }
    [SteamMember(1)]
    public string name; //自己的名称
    [SteamMember(2)]
    public string uuid; //自己的唯一iD
}

/// <summary>
/// 服务端 返回连接 信息  
/// </summary>
[LGF.Serializable.SteamContract]
public partial class S2C_Connect : S2C_BASE<S2C_Connect>
{
    public S2C_Connect() : base(NetMsgDefine.S2C_Connect) { }

    [LGF.Serializable.SteamMember(2)]
    public uint uid;     //返回给客户端一个唯一id
}


/// <summary>
/// 客户端连接  并传递客户端信息
/// </summary>
[LGF.Serializable.SteamContract]
public partial class C2S_HeartBeat : C2S_BASE<C2S_HeartBeat>
{
    public C2S_HeartBeat() : base(NetMsgDefine.C2S_HeartBeat) { }
}

/// <summary>
/// 服务端 返回连接 信息  
/// </summary>
[LGF.Serializable.SteamContract]
public partial class S2C_HeartBeat : S2C_BASE<S2C_HeartBeat>
{
    public S2C_HeartBeat() : base(NetMsgDefine.S2C_HeartBeat) { }
}



//HeartBeat




//Msstype


/// <summary>
/// 广播消息
/// </summary>
[LGF.Serializable.SteamContract]
public partial class C2S_ChatMsg : C2S_BASE<C2S_ChatMsg>
{
    public C2S_ChatMsg() : base(NetMsgDefine.C2S_ChatMsg) { }

    [LGF.Serializable.SteamMember(1)]
    public ChatType type;    //

    [LGF.Serializable.SteamMember(2)]
    public string msg;
}

/// <summary>
/// 广播消息
/// </summary>
[LGF.Serializable.SteamContract]
public partial class S2C_ChatMsg : S2C_BASE<S2C_ChatMsg>
{
    public S2C_ChatMsg() : base(NetMsgDefine.S2C_ChatMsg) { }
    [LGF.Serializable.SteamMember(1)]
    public ChatType type;    //

    [LGF.Serializable.SteamMember(2)]
    public string name;
    [LGF.Serializable.SteamMember(3)]
    public string msg;
}




#endregion



#region Room

/// <summary>
/// 创建房间
/// </summary>
[LGF.Serializable.SteamContract]
public partial class C2S_CreateRoom : C2S_BASE<C2S_CreateRoom>
{
    public C2S_CreateRoom() : base(NetMsgDefine.C2S_CreateRoom) { }

    [LGF.Serializable.SteamMember(1)]
    public string name; //房间名字  暂时没写这个取名

}


/// <summary>
///创建房间
/// </summary>
[LGF.Serializable.SteamContract]
public partial class S2C_CreateRoom : S2C_BASE<S2C_CreateRoom>
{
    public S2C_CreateRoom() : base(NetMsgDefine.S2C_CreateRoom) { }
    [LGF.Serializable.SteamMember(1)]
    public uint roomID;    //房间的ID

    [SteamMember(2)]
    public string roomName;
}


/// <summary>
/// 获得全部房间信息
/// </summary>
[LGF.Serializable.SteamContract]
public partial class C2S_GetAllTheRooms : C2S_BASE<C2S_GetAllTheRooms>
{
    public C2S_GetAllTheRooms() : base(NetMsgDefine.C2S_GetAllTheRooms) { }
}


[LGF.Serializable.SteamContract]
public partial class CMD_SimpleRoomInfo : CMD_BASE2<CMD_SimpleRoomInfo>
{
    /// <summary>
    /// 房间id
    /// </summary>
    [SteamMember(1)]
    public uint roomID;

    /// <summary>
    /// 房间名字
    /// </summary>
    [SteamMember(2)]
    public string roomName;
}


/// <summary>
/// 获得全部房间信息
/// </summary>
[LGF.Serializable.SteamContract]
public partial class S2C_GetAllTheRooms : S2C_BASE<S2C_GetAllTheRooms>
{
    public S2C_GetAllTheRooms() : base(NetMsgDefine.S2C_GetAllTheRooms) { }

    [SteamMember(1)]
    public List<CMD_SimpleRoomInfo> roomList;

}




///// <summary>
///// 进入房间
///// </summary>
//[LGF.Serializable.SteamContract]
//public partial class C2S_JionRoom : C2S_BASE<C2S_JionRoom>
//{
//    public C2S_JionRoom() : base(NetMsgDefine.C2S_JionRoom) { }
//    [LGF.Serializable.SteamMember(1)]
//    public int roomID;    //房间的ID
//    [LGF.Serializable.SteamMember(2)]
//    public string name; //房间名字
//}



/// <summary>
/// 进入房间
/// </summary>
[LGF.Serializable.SteamContract]
public partial class S2C_JionRoom : S2C_BASE<S2C_JionRoom>
{
    public S2C_JionRoom() : base(NetMsgDefine.S2C_JionRoom) { }
    [LGF.Serializable.SteamMember(1)]
    public int roomID;    //房间的ID

    /// <summary>
    /// 房间名字
    /// </summary>
    [SteamMember(2)]
    public string roomName;

    [SteamMember(3)]
    public List<CMD_UserInfo> roomUserList;
}



/// <summary>
/// 告知有玩家进入房间  与玩家准备操作
/// </summary>
[LGF.Serializable.SteamContract]
public partial class C2S_InformRoomChange : C2S_BASE<C2S_InformRoomChange>
{
    public C2S_InformRoomChange() : base(NetMsgDefine.C2S_InformRoomChange) { }

    /// <summary>
    /// 操作
    /// </summary>
    [SteamMember(1)]
    public int opt;    //1.请求加入房间 2表示离开房间   3表示玩家准备  4表示玩家取消准备

    [SteamMember(2)]
    public uint playerID;    //玩家信息

    [SteamMember(3)]
    public uint roomID;     //房间ID 1.请求加入房间
}


//

/// <summary>
/// 告知有玩家进入房间  与玩家准备操作
/// </summary>
[LGF.Serializable.SteamContract]
public partial class S2C_InformRoomChange : S2C_BASE<S2C_InformRoomChange>
{
    public S2C_InformRoomChange() : base(NetMsgDefine.S2C_InformRoomChange) { }

    /// <summary>
    /// 操作
    /// </summary>
    [SteamMember(1)]
    public int opt;    //1 表示有玩家加入  2表示玩家离开  3表示玩家准备  4表示玩家取消准备  5表示更换房主

    [SteamMember(2)]
    public uint playerID;          //玩家信息

    [SteamMember(5)]
    public CMD_UserRoomInfo newUser;    //新用户 操作1的时候用
}


/// <summary>
/// 同步房间信息
/// </summary>
[LGF.Serializable.SteamContract]
public partial class S2C_SyncRoomInfo : S2C_BASE<S2C_SyncRoomInfo>
{
    public S2C_SyncRoomInfo() : base(NetMsgDefine.S2C_SyncRoomInfo) { }

    [SteamMember(1)]
    public uint houseOwnerID;       //房主ID
    [SteamMember(2)]
    public string roomName;           //房主名字

    [SteamMember(3)]
    public List<CMD_UserRoomInfo> infoList;    //新用户 操作1的时候用
}



#endregion



#region 用户信息

/// <summary>
/// 用户房间信息
/// </summary>
[LGF.Serializable.SteamContract]
public partial class CMD_UserRoomInfo : CMD_BASE2<CMD_UserRoomInfo>
{
    [SteamMember(1)]
    public CMD_UserInfo useinfo;   //用户信息

    [SteamMember(2)]
    public bool ready; //是否准备状态

    [SteamMember(3)]
    public uint roomjoinRank;

}

/// <summary>
/// 用户信息
/// </summary>
[LGF.Serializable.SteamContract]
public partial class CMD_UserInfo : CMD_BASE2<CMD_UserInfo>
{

    [SteamMember(1)]
    public uint uid;

    [SteamMember(2)]
    public string name;

}


#endregion