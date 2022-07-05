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
    public static int INT32_SIZE = 32;
}


//这里不加命名空间了 加命名空间需要

/// <summary>
/// 消息定义
/// </summary>
public enum NetMsgDefine
{
    Empty,          //非法操作
    //Customize,  //自定义类型 处理方式  
    C2S_Connect,
    S2C_Connect,
    COM_TextMsg,        //文本消息
}



public partial class CMD_BASE<T> : Poolable<T>, ISerializer where T : CMD_BASE<T>, new()
{
    [LGF.Serializable.SteamMember(0)]
    public NetMsgDefine msgType;   //消息类型
    public CMD_BASE(NetMsgDefine type)
    {
        this.msgType = type;
    }

    public virtual void Deserialize(LGFStream stream)
    {
        throw new NotImplementedException();
    }

    public virtual void Serialize(LGFStream stream)
    {
        throw new NotImplementedException();
    }
    
    public static T Get(LGFStream stream)
    {
        return Get().NDeserialize(stream);
    }


    public T NDeserialize(LGFStream stream)
    {
        Deserialize(stream);
        return this as T;
    }

    public T NSerialize(LGFStream stream)
    {
        Serialize(stream);
        return this as T;
    }
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



/// <summary>
/// 客户端连接
/// </summary>
[LGF.Serializable.SteamContract]
public partial class C2S_Connect : CMD_BASE<C2S_Connect>
{
    public C2S_Connect() : base(NetMsgDefine.C2S_Connect) { }
}


/// <summary>
/// 服务端连接
/// </summary>
[LGF.Serializable.SteamContract]
public partial class S2C_Connect :  CMD_BASE<S2C_Connect>
{
    public S2C_Connect() : base(NetMsgDefine.S2C_Connect) { }

    [LGF.Serializable.SteamMember(1)]
    public int port;    //端口号
}


/// <summary>
/// 服务端连接
/// </summary>
[LGF.Serializable.SteamContract]
public partial class COM_TextMsg : CMD_BASE<S2C_Connect>
{
    public COM_TextMsg() : base(NetMsgDefine.S2C_Connect) { }

    [LGF.Serializable.SteamMember(1)]
    public string msg;
}







