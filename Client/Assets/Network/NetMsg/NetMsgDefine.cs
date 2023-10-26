/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/11 2:14:38
/// 功能描述:  
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

//GetNetMsgType()

namespace LGF.Serializable
{
    public static class LStream_Extend
    {
        ///// <summary>
        ///// 获得网络消息类型 并重置位置
        ///// 后面看情况改成扩展方法
        ///// </summary>
        ///// <returns></returns>
        public static NetMsgDefine GetNetMsgType(this LStream _this)
        {
            NetMsgDefine val = NetMsgDefine.Empty;
            try {
                val = (NetMsgDefine)_this.read.ReadInt32();
            }
            catch (Exception e) {
                e.DebugError();
                return NetMsgDefine.Empty;  //非法操作
            }
            _this.Reset();
            return val;
        }
    }
}


/// <summary>
/// 消息定义
/// </summary>
public enum NetMsgDefine
{
    Empty = 0,          //非法操作
    //Customize,  //自定义类型 处理方式  
    //N_C2S_GetAllServersInfo,    //获得所有服务器信息
    //N_S2C_GetAllServersInfo,    //返回当前服务器信息
    //GGSYmsg,        //故国神游消息处理
    //GGSYEvent,        //故国神游消息处理
    Login,            //登录
    HeartBeat,      //心跳
    ChatMsg,        //文本消息


    CreateRoom,     //创建房间
    GetAllTheRooms, //获得全部房间信息


    JionRoom,   //加入房间

    InformRoomChange, //通知房间状态改变  如准备 离开房间等

    SyncRoomInfo,   //同步房间信息给玩家  进入房间的时候用

    FrameOpKey,


    RoomtFinishType,  //type = 1 标识开始场景加载  2房间玩家全部加载完成 
    RoomProgress,

    GameCInfo,        //游戏中客户端发送给的信息.
    GameMsg,          //游戏消息处理 客户端请求并响应
    GameEvent,        //游戏服务器通知客户端的事件 
}


/// <summary>
/// 错误编码
/// </summary>
public enum ErrCode
{
    /// <summary>
    /// 成功
    /// </summary>
    Success = 0,
    /// <summary>
    /// 成功
    /// </summary>
    Succeed =   0,
    /// <summary>
    /// 失败
    /// </summary>
    Failed = 1,
    /// <summary>
    /// 失败
    /// </summary>
    Fail =   1,


    NOT_ENOUGH_GB           = 2,   //没有足够的古币
    MaxOffsetSignIn         = 3,    //最大补签数量
    INVALID_OPT             = 4,        //非法操作



    /// <summary>
    /// 房间不存在
    /// </summary>
    RoomNotExist = 100001,  //放假内部存在


}


/// <summary>
/// 通话类型
/// </summary>
public enum ChatType
{
    Room = 1,   //房间

}


