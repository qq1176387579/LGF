using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using LGF;
using LGF.Util;
using LGF.Log;





namespace LGF.Util
{


    public static partial class Common
    {
        public static void Swap<T>(ref T t1, ref T t2)
        {
            T t = t1;
            t1 = t2;
            t2 = t;
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

#if !NOT_UNITY
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
#endif

        #endregion


        #region 位运算
        public static bool bit_has(this int state, int idx)
        {
            if (idx == 0) return false;
            return ((state >> (idx - 1)) & 1) > 0;
        }

        public static int bit_add(this int state, int idx)
        {
            return state | (1 << (idx - 1));
        }

        public static int bit_remove(this int state, int idx)
        {
            return state ^ (1 << (idx - 1));
        }


        public static bool bit_has(this long state, int idx)
        {
            if (idx == 0) return false;
            return ((state >> (idx - 1)) & 1) > 0;
        }

        public static long bit_add(this long state, int idx)
        {
            return state | ((long)1 << (idx - 1));
        }

        public static long bit_remove(this long state, int idx)
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

