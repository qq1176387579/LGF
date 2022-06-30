/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/29 20:52:10
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;


namespace LGF.Util
{
    public static class HttpUtils
    {
        public static string ToHttpGetInfo<T>(this List<T> list)
        {
            if (list == null || list.Count < 0)
            {
                return "";
            }
            var tmp = StringPool.GetStringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0)
                    tmp.Append(',');
                if (list[i] is string)
                {
                    tmp.Append(list[i]);
                }
                else
                {
                    tmp.Append(list[i].ToString());
                }
            }

            string str = tmp.ToString();
            tmp.Release();
            return str;
        }

    }

}
