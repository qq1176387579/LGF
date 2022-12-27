/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/27 17:33:26
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

public enum RoomStateEnum
{
    Create,     //创建中允许玩家加入
    Loading,    //进入游戏前 加载中  用于加载资源等
    Playing,    //游戏中
}
