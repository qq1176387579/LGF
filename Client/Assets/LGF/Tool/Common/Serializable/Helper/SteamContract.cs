/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/3 11:26:22
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using System;


namespace LGF.Serializable
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SteamContract : Attribute
    {
        public Type type;

    }
}
