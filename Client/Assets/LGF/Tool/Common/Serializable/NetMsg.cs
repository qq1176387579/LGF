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

/// <summary>
/// 消息定义
/// </summary>
public enum NetMsgDefine
{
    Empty = 0,          //非法操作
    //Customize,  //自定义类型 处理方式  
    N_C2S_GetAllServersInfo,    //获得所有服务器信息
    N_S2C_GetAllServersInfo,    //返回当前服务器信息

    C2S_Connect,
    S2C_Connect,

    C2S_TextMsg,        //文本消息
    S2C_TextMsg,        //文本消息
    
}



public partial class CMD_BASE<T> : Poolable<T>, ISerializer where T : CMD_BASE<T>, new()
{
    [LGF.Serializable.SteamMember(-1000)]
    public NetMsgDefine msgType;   //消息类型

    public CMD_BASE(NetMsgDefine type)
    {
        this.msgType = type;
    }

    public virtual void Deserialize(LStream stream)
    {
        throw new NotImplementedException();
    }

    public virtual void Serialize(LStream stream)
    {
        throw new NotImplementedException();
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
    public S2C_BASE(NetMsgDefine type) : base(type) { }

}

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
/// 广播消息
/// </summary>
[LGF.Serializable.SteamContract]
public partial class C2S_TextMsg : C2S_BASE<C2S_TextMsg>
{
    public C2S_TextMsg() : base(NetMsgDefine.C2S_TextMsg) { }

    [LGF.Serializable.SteamMember(1)]
    public string msg;
}

/// <summary>
/// 广播消息
/// </summary>
[LGF.Serializable.SteamContract]
public partial class S2C_TextMsg : S2C_BASE<S2C_TextMsg>
{
    public S2C_TextMsg() : base(NetMsgDefine.S2C_TextMsg) { }

    [LGF.Serializable.SteamMember(1)]
    public string name;
    [LGF.Serializable.SteamMember(1)]
    public string msg;
}







#endregion





