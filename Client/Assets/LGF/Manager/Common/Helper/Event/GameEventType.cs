using System.Collections;
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
        #region net

        /// <summary>
        /// 得到服务器 端口信息
        /// </summary>
        Net_GetServersInfo,  //获得服务器信息

        #endregion

        #region c_Module  客户端模块
        c_TextMsg,


        #endregion


        #region s_Module  服务端模块
        s_TextMsg,


        #endregion
    }

}
