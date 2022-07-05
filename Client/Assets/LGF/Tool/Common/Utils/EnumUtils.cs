/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/29 21:02:49
/// 功能描述:  
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        public static T ToEnum<T>(this int param) where T : struct, Enum
        {

            
#if UNITY_ANDROID || !NOT_UNITY
            return (T)(object)param;    //装箱拆箱 不推荐用
#else
            //处理初始化的时候有GC 其他情况都没有GC
            //安卓平台用不了 报错
            return EnumConverter<T>.Convert(param); 
#endif

            //安卓用不了
        }


        /// <summary>
        /// 参考链接    https://cloud.tencent.com/developer/ask/sof/98760 
        /// 效率对比
        /// Cast (reference): Value
        /// EnumConverter: Value
        /// Enum.ToObject: Value
        /// Cast (reference): 00:00:00.3175615
        /// EnumConverter: 00:00:00.4335949
        /// Enum.ToObject: 00:00:14.3396366
        /// 
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        static class EnumConverter<TEnum> where TEnum : struct, IConvertible
        {
            public static readonly Func<long, TEnum> Convert = GenerateConverter();

            static Func<long, TEnum> GenerateConverter()
            {
                //Expression.ConvertChecked
                var parameter = Expression.Parameter(typeof(long));
                var dynamicMethod = Expression.Lambda<Func<long, TEnum>>(
                    Expression.Convert(parameter, typeof(TEnum)),
                    parameter);
                return dynamicMethod.Compile();
            }
        }

    }

}
