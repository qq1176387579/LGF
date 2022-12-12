/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/11 2:14:38
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;


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

    C2S_ChatMsg,        //文本消息
    S2C_ChatMsg,        //文本消息

    C2S_CreateRoom,     //创建房间
    S2C_CreateRoom,     //创建房间

    C2S_GetAllTheRooms, //获得全部房间信息
    S2C_GetAllTheRooms, //获得全部房间信息


    S2C_JionRoom,   //加入房间

    C2S_InformRoomChange, //通知房间状态改变  如准备 离开房间等
    S2C_InformRoomChange, //通知房间状态改变

    S2C_SyncRoomInfo,   //同步房间信息给玩家  进入房间的时候用


}


/// <summary>
/// 错误编码
/// </summary>
public enum ErrCode
{
    /// <summary>
    /// 成功
    /// </summary>
    Succeed =   0,

    /// <summary>
    /// 失败
    /// </summary>
    Fail =   1,


    /// <summary>
    /// 房间不存在
    /// </summary>
    RoomNotExist = 100001,  //放假内部存在

    /// <summary>
    /// 非法操作
    /// </summary>
    INVALID_OPT,

}

