using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using UnityEngine;
using System.Reflection;
using LGF;
using LGF.Util;


namespace LGF
{
    public interface IWeight
    {
        int Weight { get; }
    }
}


namespace LGF.Util
{


    public static class Common
    {

        public static void DontDestroyOnLoad(UnityEngine.Object obj)
        {
            UnityEngine.Object.DontDestroyOnLoad(obj);
        }


        #region 时间戳相关

        const int EIGHT_HOURS_TO_SEC = 28800;       //8小时
        const int DAY_TO_SEC = 86400;               //一天

        #region 基于TimeSpan 


        static DateTime _initialDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// 获得明日零点时间戳  12:00:00点整的时候会出问题 记得用12:00:01做判定
        /// </summary>
        /// <returns></returns>
        public static long GetTomorrowZeroTimeStamp()
        {
            TimeSpan ts = DateTime.Now.Date - _initialDateTime;
            return Convert.ToInt64(ts.TotalSeconds) + DAY_TO_SEC - EIGHT_HOURS_TO_SEC;    //28800 8点开始 60 * 60 * 8
        }

        /// <summary>
        /// 获得下次跨天时间戳 
        /// </summary>
        /// <returns></returns>
        public static long GetNextNewDayTimeStamp()
        {
            TimeSpan ts = DateTime.Now.Date - _initialDateTime;//28800 8点开始
            return Convert.ToInt64(ts.TotalSeconds) + DAY_TO_SEC - 7200;     // 6点跨天
        }


        /// <summary>
        /// 将 Unix 时间戳转换为 DateTime
        /// </summary>
        /// <param name="timestamp">Unix 时间戳</param>
        /// <returns>需要转换的时间</returns>
        public static DateTime UnixTimeToDateTime(long timestamp)
        {
            if (timestamp < 0)
                throw new ArgumentOutOfRangeException("timestamp is out of range");

            //return TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0), TimeZoneInfo.Local).AddSeconds(timestamp);
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp + EIGHT_HOURS_TO_SEC);
        }

        /// <summary>
        /// 将 DateTime 转换为 Unix 时间戳
        /// </summary>
        /// <param name="dateTime">需要转换的时间</param>
        /// <returns>Unix 时间戳</returns>
        public static long DateTimeToUnixTime(DateTime dateTime)
        {
            return Convert.ToInt64((dateTime - _initialDateTime).TotalSeconds) - EIGHT_HOURS_TO_SEC;
        }

        #endregion


        #region Unix时间戳 纯数字 


        /// <summary>
        /// 获得零点时间戳
        /// </summary>
        public static long GetZeroTimeStamp(long time)
        {
            time -= ((time + EIGHT_HOURS_TO_SEC) % DAY_TO_SEC);
            return time;
        }

        /// <summary>
        /// 获得下个零点时间戳 明天零点
        /// </summary>
        public static long GetNextZeroTimeStamp(long time)
        {
            return GetZeroTimeStamp(time) + DAY_TO_SEC;
        }


        #endregion




        #endregion

        #region 随机数


        static System.Random random = new System.Random();
        public static int Random(int val = 0)
        {
            if (val == 0) return random.Next();

            //取值范围 0~val -1
            return random.Next(val);
        }


        public static int Random(int min,int max)
        {
            return random.Next(min, max);
        }

        /// <summary>
        /// 随机概率结果
        /// </summary>
        public static bool RandomProResult(int pro, int val = 10000)
        {
            int tmp = Random(val);
            //tmp.ProLog($"RandomProResult >>>>>>>>>>> {pro} >= {tmp} { pro >= tmp}");
            return pro >= tmp;
        }

        /// <summary>
        /// 洗牌算法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(T[] list)
        {
            int count = list.Length;

            for (int i = 0; i < count; i++)
            {
                int idx = random.Next() % (count - i) + i;
                list.Swap(i, idx);
            }
        }

        #endregion


        #region 字符加密        不是自己写的
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="_input">在输入框中需要加密内容</param>
        /// <param name="_keyValue">32位密码</param>
        /// <returns></returns>
        public static string ConductEncryption(string _input, string _keyValue = "95725529957255029957255029543zcb")
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(_keyValue);

            //加密格式
            RijndaelManaged encryption = new RijndaelManaged();
            encryption.Key = keyArray;
            encryption.Mode = CipherMode.ECB;
            encryption.Padding = PaddingMode.PKCS7;

            //生成加密锁
            ICryptoTransform cTransform = encryption.CreateEncryptor();
            byte[] _EncryptArray = UTF8Encoding.UTF8.GetBytes(_input);
            byte[] resultArray = cTransform.TransformFinalBlock(_EncryptArray, 0, _EncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string ConductDecrypt(string _valueDense, string _keyValue = "95725529957255029957255029543zcb")
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(_keyValue);

            RijndaelManaged decipher = new RijndaelManaged();
            decipher.Key = keyArray;
            decipher.Mode = CipherMode.ECB;
            decipher.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = decipher.CreateDecryptor();
            byte[] _EncryptArray = Convert.FromBase64String(_valueDense);
            byte[] resultArray = cTransform.TransformFinalBlock(_EncryptArray, 0, _EncryptArray.Length);
            return UTF8Encoding.UTF8.GetString(resultArray);
        }


        #endregion


        #region 位运算
        public static bool bit_has(int state, int idx)
        {
            if (idx == 0) return false;
            return ((state >> (idx - 1)) & 1) > 0;
        }

        public static int bit_add(int state, int idx)
        {
            return state | (1 << (idx - 1));
        }

        public static int bit_remove(int state, int idx)
        {
            return state ^ (1 << (idx - 1));
        }


        #region 之前写的lua代码  后面有需求可以加上

        //        --标识符
        //local bit_flag = {}
        //for i = 1, 64 do
        //  bit_flag[i] = (bit_flag[i - 1] or 0) | (1 << i - 1)
        //end

        //--状态达标  1~id的bit位全为1
        //function daserver.reach_bit_flag(val, id)
        //	if bit_flag[id] == val then
        //		return true
        //  end
        //  return false
        //end
        #endregion



        #endregion

    }



}



namespace LGF
{
    public static partial class Utility
    {

        /// <summary>
        /// 游戏程序集
        /// </summary>
        private static Assembly sGameAssembly;

        /// <summary>
        /// 程序集列表
        /// </summary>
        private static Assembly[] sAssemblies;

        private static void LogError(string str) 
        {
            Debug.LogError(str);
        }

        /// <summary>
        /// 通过名字创建派生类
        /// </summary>
        /// <typeparam name="TBaseClass"></typeparam>
        /// <param name="derivedClassFullName"></param>
        /// <returns></returns>
        public static TBaseClass CreateDerivedInstance<TBaseClass>(string derivedClassFullName)
        {
            // 类名为空
            if (string.IsNullOrEmpty(derivedClassFullName))
            {
                LogError("Null derivedClassName");
                return default;
            }

            // 从程序集获取类型
            var derivedClassType = Type.GetType(derivedClassFullName);

            // 没有找到类型
            if (null == derivedClassType)
            {
                derivedClassType = GetTypeInAssembies(derivedClassFullName, out Assembly ass);
                if (null == derivedClassType)
                {
                    LogError($"Class {{derivedClassFullName}} not exist");
                    return default;
                }
            }

            // 创建
            return CreateDerivedInstance<TBaseClass>(derivedClassType);
        }

        /// <summary>
        /// 通过类型创建派生类
        /// </summary>
        /// <typeparam name="TBaseClass"></typeparam>
        /// <param name="derivedClassType"></param>
        /// <returns></returns>
        public static TBaseClass CreateDerivedInstance<TBaseClass>(Type derivedClassType)
        {
            var baseClassType = typeof(TBaseClass);

            // 检查继承关系
            if (!baseClassType.IsAssignableFrom(derivedClassType))
            {
                LogError($"Class { derivedClassType.Name} not derived from {baseClassType.Name}");
                return default;
            }

            var inst = Activator.CreateInstance(derivedClassType);
            return (TBaseClass)inst;
        }

        /// <summary>
        /// 通过类型名创建对象
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static object CreateInstance(string typeName)
        {
            var t = Type.GetType(typeName);
            if (null == t)
            {
                LogError($"Type {typeName} not exist");
                return null;
            }
            return Activator.CreateInstance(t);
        }

        /// <summary>
        /// 通过类型创建对象
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object CreateInstance(Type t)
        {
            return Activator.CreateInstance(t);
        }

        ///// <summary>
        ///// 从游戏程序集获取类型
        ///// </summary>
        ///// <param name="className"></param>
        ///// <returns></returns>
        //public static Type GetTypeInGameAssembly(string className)
        //{
        //    // 类名为空
        //    if (string.IsNullOrEmpty(className))
        //    {
        //        Log.Error("Null className");
        //        return null;
        //    }
        //    var fullName = $"{AppEntry.GameNameSpace}.{className}";
        //    return GetTypeInGameAssemblyByFullName(fullName);
        //}

        /// <summary>
        /// 从程序集中搜索类型
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="ass"></param>
        /// <returns></returns>
        private static Type GetTypeInAssembies(string fullName, out Assembly ass)
        {
            ass = null;
            if (null == sAssemblies)
            {
                sAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            }

            // 遍历
            for (int i = 0; i < sAssemblies.Length; ++i)
            {
                var a = sAssemblies[i];
                var t = a.GetType(fullName);
                if (null != t)
                {
                    ass = a;
                    return t;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Type GetTypeInGameAssemblyByFullName(string fullName)
        {
            // 类名为空
            if (string.IsNullOrEmpty(fullName))
            {
                LogError("Null className");
                return null;
            }

            // 搜索Assembly
            if (null == sGameAssembly)
            {
                return GetTypeInAssembies(fullName, out sGameAssembly);
            }
            return sGameAssembly.GetType(fullName);
        }
    }
}





public static class LH_UtilsExten
{
    #region 随机

    public static bool RandomProResult(this int pro) => Common.RandomProResult(pro);

    /// <summary>
    /// 随机抽取
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T RandomDrawing<T>(this T[] list, int count = 0) where T : class, IWeight
    {
        if (list == null || list.Length == 0) return null;

        int weight = 0;
        int _count = count > 0 && count <= list.Length ? count : list.Length;

        for (int i = 0; i < _count; i++) weight += list[i].Weight;

        weight = Common.Random(weight);    //0 ~ weight - 1

        int tmp = 0;
        for (int i = 0; i < _count; i++)
        {
            tmp += list[i].Weight;          // 1 ~ weight
            if (tmp > weight)
            {
                //UnityEngine.Debug.LogError($"weight:{weight} tmp :{tmp}");
                return list[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 随机抽取
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T RandomDrawing<T>(this List<T> list) where T : class, IWeight
    {
        if (list == null || list.Count == 0) return null;

        int weight = 0;
        for (int i = 0; i < list.Count; i++) weight += list[i].Weight;

        weight = Common.Random(weight);    //0 ~ weight - 1
        int tmp = 0;
        for (int i = 0; i < list.Count; i++)
        {
            tmp += list[i].Weight;          // 1 ~ weight
            if (tmp > weight)
            {
                return list[i];
            }
        }
        return null;
    }

    public static T RandomNext<T>(this List<T> list)
    {
        if (list.Count == 0) return default(T);
        return list[Common.Random(list.Count)];
    }


    public static T RandomNext<T>(this T[] arrs)
    {
        if (arrs.Length == 0) return default(T);
        return arrs[Common.Random(arrs.Length)];
    }

    #endregion

    #region 数组相关操作

    public static void Swap<T>(this T[] list, int idx1, int idx2)
    {
        if (idx1 < 0 || idx2 < 0 || idx1 >= list.Length || idx2 >= list.Length) return;
        T tmp = list[idx1];
        list[idx1] = list[idx2];
        list[idx2] = tmp;
    }

    public static void Swap<T>(this List<T> list, int idx1, int idx2)
    {
        if (idx1 < 0 || idx2 < 0 || idx1 >= list.Count || idx2 >= list.Count) return;
        T tmp = list[idx1];
        list[idx1] = list[idx2];
        list[idx2] = tmp;
    }

    /// <summary>
    /// 回收到list缓冲中
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Release<T>(this List<T> list)
    {
        if (list != null) ListPool<T>.Release(list);
    }

    /// <summary>
    /// 回收到list缓冲中
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void ReleaseAll<T>(this List<T> list) where T : Poolable<T>, new()
    {
        if (list == null) return;

        list.ForEach((info) =>
        {
            info.Release();
        });
        ListPool<T>.Release(list);
    }



    public static T GetByID<T>(this T[] arr, int id)
    {
        if (arr.CheckeID(id))
        {
            return arr[id];
        }
        return default(T);
    }

    public static T GetByID<T>(this List<T> arr, int id)
    {
        if (arr.CheckeID(id))
        {
            return arr[id];
        }
        return default(T);
    }



    public static T1 GetByID<T, T1>(this Dictionary<T, T1> dic, T id)
    {
        if (dic.ContainsKey(id))
        {
            return dic[id];
        }
        return default(T1);
    }


    public static bool CheckeID<T>(this List<T> arr, int id)
    {
        if (arr == null || arr.Count <= id || id < 0) return false;
        return true;
    }

    public static bool CheckeID<T>(this T[] arr, int id)
    {
        if (arr == null || arr.Length <= id || id < 0) return false;
        return true;
    }

    public static bool CheckeID<T, T1>(this Dictionary<T, T1> dic, T id)
    {
        if (dic == null || !dic.ContainsKey(id)) return false;
        return true;
    }


    public static bool HasData<T>(this List<T> arr)
    {
        if (arr == null || arr.Count == 0) return false;
        return true;
    }

    public static void CopyTo<T>(this List<T> arr, List<T> list)
    {
        if (arr == null)
        {
            if (list != null)
                list.Clear();
            return;
        }

        list.Clear();
        for (int i = 0; i < arr.Count; i++)
            list.Add(arr[i]);
    }

    public static void ForEach<T>(this T[] list, System.Action<T> func)
    {
        if (list == null)
            return;

        //Debug.LogError("-----12---");
        //Array.ForEach(list, func);
        for (int i = 0; i < list.Length; i++)
        {
            func.Invoke(list[i]);
        }
    }
    //public static void ForEach<T>(this List<T> list, System.Action<T> func)
    //{
    //    if (list == null)
    //        return;
        

    //    for (int i = 0; i < list.Count; i++)
    //    {
    //        func.Invoke(list[i]);
    //    }
    //}



    public static void RemoveFunc<T>(this List<T> list, Func<T, bool> func)
    {
        if (list == null)
            return;

        // 找出第一个空元素 O(n)
        int count = list.Count;
        for (int i = 0; i < count; i++)
            if (func.Invoke(list[i]))
            {
                // 记录当前位置
                int newCount = i++;

                // 对每个非空元素，复制至当前位置 O(n)
                for (; i < count; i++)
                    if (!func.Invoke(list[i]))
                        list[newCount++] = list[i];

                // 移除多余的元素 O(n)
                list.RemoveRange(newCount, count - newCount);
                break;
            }
    }


    public static void RemoveFunc<T>(this List<T> list, Func<T, bool> func, Action<T> OnInvoke)
    {
        if (list == null)
            return;

        // 找出第一个空元素 O(n)
        int count = list.Count;
        for (int i = 0; i < count; i++)
            if (func.Invoke(list[i]))
            {
                OnInvoke.Invoke(list[i]);
                // 记录当前位置
                int newCount = i++;

                // 对每个非空元素，复制至当前位置 O(n)
                for (; i < count; i++)
                    if (!func.Invoke(list[i]))
                    {
                        list[newCount++] = list[i];
                    }
                    else
                    {
                        OnInvoke.Invoke(list[i]);
                    }
               
                // 移除多余的元素 O(n)
                list.RemoveRange(newCount, count - newCount);
                break;
            }
    }

    /// <summary>
    /// 取出最上面的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T Pop<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
            return default;

        T val = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        return val;
    }


    public static T Top<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
            return default;

        T val = list[list.Count - 1];

        return val;
    }

    /// <summary>
    /// 插入到最前面
    /// </summary>
    public static void Enqueue<T>(this List<T> list,T val)
    {
        if (list == null)
            return;

        list.Add(val);
        for (int i = 0; i < list.Count - 1; i++)
        {
            list[i + 1] = list[i];
        }

        list[0] = val;
    }


    #endregion


    #region Color

    public static string ToColor(this object obj, string color)
    {
        return string.Format("<color={0}>{1}</color>", color, obj.ToString());
    }
    public static string ToColor(this string str, string color)
    {
        return string.Format("<color={0}>{1}</color>", color, str);
    }



    public static Color StringToColor(string colorStr)
    {
        if (string.IsNullOrEmpty(colorStr))
        {
            return new Color();
        }

        if (colorStr[0] == '#') colorStr = colorStr.Substring(1, colorStr.Length - 1);
        if (colorStr.Length < 7) colorStr += "FF";
        long colorInt = long.Parse(colorStr, System.Globalization.NumberStyles.AllowHexSpecifier);

        //   Debug.Log("colorInt"+colorInt);
        return IntToColor(colorInt);
    }

    public static Color IntToColor(long colorInt)
    {
        float basenum = 255;
        long a = 0xFF & colorInt;
        long b = 0xFF00 & colorInt;
        b >>= 8;
        long g = 0xFF0000 & colorInt;
        g >>= 16;
        long r = 0xFF000000 & colorInt;
        r >>= 24;
        //   Debug.Log("R:" + r + " G:" + g + " B:" + b + " A:" + a);
        return new Color((float)r / basenum, (float)g / basenum, (float)b / basenum, (float)a / basenum);

    }


    /// <summary>
    /// color 转换hex
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string ColorToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255.0f);
        int g = Mathf.RoundToInt(color.g * 255.0f);
        int b = Mathf.RoundToInt(color.b * 255.0f);
        int a = Mathf.RoundToInt(color.a * 255.0f);
        string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
        return hex;
    }

    /// <summary>
    /// hex转换到color
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static Color HexToColor(string hex)
    {
        if (hex[0] == '#') hex = hex.Substring(1, hex.Length - 1);
        if (hex.Length < 7) hex += "FF";
        byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        float r = br / 255f;
        float g = bg / 255f;
        float b = bb / 255f;
        float a = cc / 255f;
        return new Color(r, g, b, a);
    }
    #endregion


    #region Http

    public static void Release(this StringBuilder  builder)
    {
        StringPool.Release(builder);
    }

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

    #endregion
}



public static class ExtendEnum
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