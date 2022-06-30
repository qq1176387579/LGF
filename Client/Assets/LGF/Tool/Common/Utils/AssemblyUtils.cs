/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/29 20:15:42
/// 功能描述:  AssemblyUtils
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LGF;
using LGF.Log;

namespace LGF.Util
{
    public static partial class AssemblyUtils
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
            str.DebugError(str);
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

