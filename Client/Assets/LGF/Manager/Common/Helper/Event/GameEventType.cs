using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGF
{
    public enum GameEventType
    {
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

        #region 三国战斗相关事件标签
        OnSkillCDRefersh,           //技能cd刷新

        #region 攻城TimeLine
        Stpe1_Init,
        Stpe2_Init,
        Stpe3_Init,
        Stpe4_Init,
        Stpe5_Init,
        Stpe1_End,
        Stpe2_End,
        Stpe3_End,
        Stpe4_End,
        Stpe5_End,

        Step2_LiuBeiOrder,
        Stpe4_PlayershibingMove1,
        #endregion
        #endregion



    }

}
