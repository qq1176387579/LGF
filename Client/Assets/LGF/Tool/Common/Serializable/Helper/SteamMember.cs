/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/3 9:16:17
/// 功能描述:  序列化成员属性
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LGF;
using LGF.Log;
using LGF.Util;

namespace LGF.Serializable
{
   
    //先不实现 Property
    //[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SteamMember : Attribute, IComparable<SteamMember>
    {
        public FieldInfo fieldInfo;
        //public string FieldTypeName;
        public string name;
        public int Tag;
        public SteamMember(int tag)
        {
            this.Tag = tag;
        }

        public int CompareTo(SteamMember other)
        {
            return Tag.CompareTo(other.Tag);
        }
    }


}
