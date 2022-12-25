/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/15 1:01:30
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

namespace LGF.Logic
{
    //目前先不实现功能  先搭个架子出来
    using SystemAttr = Dictionary<ATTR_SYS, Attr>;

    //系统属性
    public enum ATTR_SYS
    {
        Test = 1000,      //Test
    };


    public class AvatarAttr
    {
        SystemAttr systemAttr;  //系统属性
        Attr sumAttr;           //所有系统属性的总和
        Attr finalAttr;         //计算最终属性的



    }



}
