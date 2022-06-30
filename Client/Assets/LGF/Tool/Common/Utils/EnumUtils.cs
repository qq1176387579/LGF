/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/29 21:02:49
/// 功能描述:  
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

namespace LGF
{
    public static class EnumUtils
    {
        /*  e.GetHashCode() 测试效率代码
    *  double time = 0;
       for (int i = 0; i < 1000000; i++)
       {
           var now = System.DateTime.Now;
           int t = s.fff.GetHashCode();
           time += (System.DateTime.Now - now).TotalMilliseconds;
       }

       UnityEngine.Debug.Log(time);
       time = 0;
       for (int i = 0; i < 1000000; i++)
       {
           var now = System.DateTime.Now;
           int t = (int)s.fff;
           time += (System.DateTime.Now - now).TotalMilliseconds;
       }
    */
        /// <summary>
        ///  为了避免装箱强制转换    消耗与强转是一样的  测试代码在上面
        ///  详细看该文章 https://blog.csdn.net/lzdidiv/article/details/71170528  
        ///  有GC
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static int ToInt(this System.Enum e)
        {
            return e.GetHashCode();
        }


        /// <summary>
        /// 无GC 效率与(int)(enumVal)也无差别
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static int ToInt<T>(this T e) where T : System.Enum
        {
            return e.GetHashCode();
        }


        public static T ToEnum<T>(this string str) where T : struct, Enum
        {
            System.Enum.TryParse(str, out T val);
            return val;
        }

    }

}
