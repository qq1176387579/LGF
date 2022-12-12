﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGF
{
    public enum GameEventType
    {
        OnUpdate,

        //Add
        NewRollVerticalGroup2Item,
        Test,
        /// <summary>
        /// SKD消息
        /// </summary>
        AndroidMsg,
        GM,
        OnNetTimeChange,

        NetStatus,


        OnLoadDataEnd,      //加载数据结束
        OnMinute,           //没分钟回调
        OnHour,             //每小时回调
        OnNewDay,           //新的一天

        ///-------------------以上弃用--------------------

        //-------------------下列使用-----------------------------

        KeyCode_Mouse1,




        //------------------------net--------------------


        #region 客户端事件

        c_TextMsg,

        ConnectServerFail,  //连接服务器失败

        ClientEvent_CreateRoomSucceed,  //创建房间成功
        ClientEvent_GetAllRooms,        //获得全部房间事件
        ClientEvent_RoomOptSync,        //房间同步事件
        ClientEvent_JionRoom,           //获得全部房间事件
        #endregion





        #region 服务器
        //------------服务器事件---------------------
        s_TextMsg,

        ServerEvent_GetServersInfo,  //获得服务器信息
        ServerEvent_PlayerConnect,


        ServerEvent_ReConnect,    //退出重进  在连接



        #endregion


    }

}
