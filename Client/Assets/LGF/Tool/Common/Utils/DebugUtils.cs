using System.Collections;
using System.Collections.Generic;
using LGF;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System;

#if !NOT_UNITY
using UnityEngine;
#endif

namespace LGF.Log
{
#if NOT_UNITY
    public static class Debug
    {
        [System.Diagnostics.Conditional("DEBUG")]
        public static void Log(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(string.Format(format, args));
        }


        [System.Diagnostics.Conditional("DEBUG")]
        public static void Log(object message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message.ToString());
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogWarning(object message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message.ToString());
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogWarningFormat(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Format(format, args));
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogError(object message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message.ToString());
        }


    }
#endif

    public static class DebugExtension
    {
        //static int testCount = 0;

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Debug(this object obj, object str)
        {
#if NOT_UNITY

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(str);
#else
            UnityEngine.Debug.Log(str);
#endif
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Debug(this object obj,string str,params object[] param)
        {

#if NOT_UNITY
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(string.Format(str, param));
#else
            UnityEngine.Debug.LogFormat(str, param);
#endif
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void DebugError(this object obj, object str)
        {
#if NOT_UNITY
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(str);
#else
            UnityEngine.Debug.LogError( str );
#endif
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void DebugError(this System.Exception e)
        {

#if NOT_UNITY
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.ToString());
#else
            UnityEngine.Debug.LogError(DebugUtils.GetExceptionStackInfoToUnity(e));
#endif
        }




        public static void ThreadDebugInfo(this object obj, string str)
        {
#if NOT_UNITY
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(str);
#else
            //ThreadDebug.Log(str);
            UnityEngine.Debug.Log(str);
#endif
        }

        public static void ThreadDebugInfo2(this object obj, string str, bool isAsync = true)
        {
#if NOT_UNITY
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(str);
#else
            UnityEngine.Debug.LogError(str);    //现在已经是线程安全了
            //ThreadDebug.Log2(str, isAsync);
#endif

        }


#if !NOT_UNITY
        static long count = 0;
        public static void debugInfo(this Transform tr, string msg)
        {
            count++;
            UnityEngine.Debug.Log($"count:{count}  Time:{Time.time}  name: {tr.name} | {msg}");
        }

        public static void debugError(this Transform tr, string msg)
        {
            count++;
            UnityEngine.Debug.LogError($"count:{count}  Time:{Time.time}  name: {tr.name} | {msg}");
        }



        /// <summary>
        /// 在多线程之前需要先初始化TimeManager
        /// </summary>
        public static class ThreadDebug
        {
            private static System.Diagnostics.Stopwatch m_stopWatch = new System.Diagnostics.Stopwatch();
            public static void Log(string str)
            {
                TimeManager.Instance.AddTask(() => { UnityEngine.Debug.Log(str); });
            }

            public static void Log2(string str, bool isAsync = false)
            {
                if (!m_stopWatch.IsRunning)
                    m_stopWatch.Start();

                float curTime = m_stopWatch.ElapsedMilliseconds;
                var stackInfo = new System.Diagnostics.StackTrace(1, true);//.ToString();

                
                if (isAsync)
                {
                    TimeManager.GetSingleton();//未初始化初始化一下
                    Task.Run(() =>
                    {
                        string stackTrace = DebugUtils.GetFormedStacksInfo(stackInfo);    //可以阻塞那样线程顺序正确
                        TimeManager.Instance.AddTask(() => { UnityEngine.Debug.LogFormat("time:{0} : info{1}\n\n{2}", curTime, str, stackTrace); });
                    });
                }
                else
                {
                    TimeManager.Instance.AddTask(() => { UnityEngine.Debug.LogFormat("time:{0} : info{1}\n\n{2}", curTime, str, DebugUtils.GetFormedStacksInfo(stackInfo)); });
                }

            }
        }

#endif
        public static class DebugUtils
        {
            //-------------------------------------------------https://yanchezuo.com/csharp-3.html  参考日志
            /// <summary>
            /// 获取像Unity一样方式输出的堆栈信息
            /// </summary>
            /// <param name="st"></param>
            /// <returns></returns>
            public static string GetFormedStacksInfo(System.Diagnostics.StackTrace st, string str = null)
            {
                System.Text.StringBuilder sb = StringPool.GetStringBuilder();
                if (str != null)
                    sb.Append(str);

                //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);
                for (int i = 1, n = st.FrameCount; i < n; ++i)
                {
                    System.Diagnostics.StackFrame frame = st.GetFrame(i);

                    // fileName从Assets目录开始打印
                    var fullFileName = frame.GetFileName();
                    if (fullFileName == null)
                    {
                        sb.Release();
                        return st.ToString();
                    }

                    int sIndex = fullFileName.IndexOf("Assets");
                    string fileName = fullFileName.Substring(sIndex, fullFileName.Length - sIndex);
                    fileName = fileName.Replace('\\', '/');         // 替换下目录符

                    var method = frame.GetMethod();

                    string info = string.Format("{0}:{1}() (at {2}:{3})"
                        , method.DeclaringType, method.Name, fileName, frame.GetFileLineNumber());
                    sb.AppendLine(info);
                }
                string tmp = sb.ToString();
                sb.Release();
                return tmp;
            }


            /// <summary>
            /// 获取像Unity一样方式输出的堆栈信息
            /// 打印最上层的信息
            /// </summary>
            /// <param name="st"></param>
            /// <returns></returns>
            public static string Get1FormedStackInfo(System.Diagnostics.StackTrace st)
            {
                // System.Text.StringBuilder sb = new System.Text.StringBuilder();
                // System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);
                if (st.FrameCount > 0)
                {
                    System.Diagnostics.StackFrame frame = st.GetFrame(1);
                    var fullFileName = frame.GetFileName();
                    int sIndex = fullFileName.IndexOf("Assets");
                    string fileName = fullFileName.Substring(sIndex, fullFileName.Length - sIndex);
                    fileName = fileName.Replace('\\', '/');

                    var method = frame.GetMethod();

                    string info = string.Format("{0}:{1}() (at {2}:{3})"
                        , method.DeclaringType, method.Name, fileName, frame.GetFileLineNumber());
                    return info;
                }
                return "";
            }

            static class RegexHelper
            {
                //public const string MatchesPattern2 = @" at (.*)\[0x(.*)\] in (.*)\.cs:[0-9]+";   Regex.Matches字段 匹配字段
                public const string pattern = @"  at (?<str1>.*?) \[0x(.*)\] in (?<str2>.*?)\.cs:(?<str3>([0-9]+)?)";
                public const string replacement = "${str1} (at ${str2}.cs:${str3})";
            }


            /// <summary>
            /// () (at {path}) 这种会高亮  所以用通配符替换
            /// </summary>
            /// <param name="e"></param>
            /// <returns></returns>
            public static string GetExceptionStackInfoToUnity(System.Exception e)
            {
                return Regex.Replace(e.ToString(), RegexHelper.pattern, RegexHelper.replacement) + "\n\n";
            }

        }




    }

}
