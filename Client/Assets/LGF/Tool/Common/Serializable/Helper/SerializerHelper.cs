/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/3 9:19:56
/// 功能描述:  序列化帮助类
****************************************************/

#define GENERATED

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Reflection;
using LGF.Util;

namespace LGF.Serializable
{




    public static partial class SerializerHelper
    {
        public static byte[] bytebuffer = new byte[1024];   //后面写个池子 现在图方便


        public static void Read_UInt32(this LGF.Serializable.LStream stream, ref uint _out)
        {
            _out = stream.read.ReadUInt32();
        }

        public static void Read_Int32(this LGF.Serializable.LStream stream,ref int _out)
        {
            _out = stream.read.ReadInt32();
        }


        public static void Read_Int64(this LGF.Serializable.LStream stream, ref long _out)
        {
            _out = stream.read.ReadInt64();
        }

        public static void Read_Boolean(this LGF.Serializable.LStream stream, ref bool _out)
        {
            _out = stream.read.ReadBoolean();
        }

        //安卓 下列方法有问题无法调用 会报错
        //public static void Read_Enum<T>(this LGF.Serializable.LStream stream, ref T _out) where T : struct, Enum
        //{
        //    _out = stream.read.ReadInt32().ToEnum<T>();
        //}



        public static void Read_String(this LGF.Serializable.LStream stream, ref string _out)
        {
            //多线程加锁
            int Length = stream.read.ReadUInt16();
            //sLog.Error("Length" + Length);
            if (Length== 0)
            {
                _out = "";
                return;
            }
            lock (bytebuffer)
            {
                var buffer = bytebuffer;
                stream.read.Read(buffer, 0, Length);
                _out = Encoding.UTF8.GetString(buffer, 0, Length);
            }
           
            //sLog.Debug(_out);
        }




        public static void Write_UInt32(this LGF.Serializable.LStream stream, in uint val)
        {
            stream.writer.Write(val);
        }

        public static void Write_Int32(this LGF.Serializable.LStream stream, in int val)
        {
            stream.writer.Write(val);
        }

        public static void Write_Int64(this LGF.Serializable.LStream stream, in long val)
        {
            stream.writer.Write(val);
        }

        public static void Write_Boolean(this LGF.Serializable.LStream stream, in bool val)
        {
            stream.writer.Write(val);
        }

        //public static void Write_Enum<T>(this LGF.Serializable.LStream stream, in T val) where T : struct, Enum
        //{
        //    stream.writer.Write(val.ToInt());
        //}






        public static void Write_String(this LGF.Serializable.LStream stream, in string val)
        {
            lock (bytebuffer)
            {
                var buffer = bytebuffer;
                //多线程加锁
                int Length = val == null ? 0 : Encoding.UTF8.GetBytes(val, 0, val.Length, buffer, 0);
                //sLog.Error("Length" + Length);
                stream.writer.Write((ushort)Length);
                stream.writer.Write(buffer, 0, Length);
            }
          
        }


        //服务器配置没弄  后面看需要去弄个

#if !NO_UNITY

#if UNITY_EDITOR    //只存在于编辑器


        //使用实现泛型方法读取  具体实现需要自己写逻辑
        static Dictionary<string, Type> steamMemberList = new Dictionary<string, Type>();  

        public static void GeneratedAll()
        {
            steamMemberList.Clear();
            List<SteamContract> contracts = new List<SteamContract>();
            Assembly[] assems = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assems.Length; i++)
            {
                Type[] types = assems[i].GetTypes();
                foreach (var item in types)
                {
                    SteamContract s = item.GetCustomAttribute<SteamContract>();
                    if (s != null)
                    {
                        s.type = item;
                        //sLog.Error(item.Name);
                        contracts.Add(s);
                    }
                }
            }

            for (int i = 0; i < contracts.Count; i++)
            {
                GeneratedType(contracts[i].type);
            }

            if (steamMemberList.Count > 0)
            {
                GeneratedGenericMethods();
            }

          
            if (contracts.Count == 0)
            {
                sLog.Debug("没有要生成的目标");
            }
            else
            {
                GenerateFileReference();
            }

            steamMemberList.Clear();
        }


        static void GeneratedType(System.Type type)
        {
            List<SteamMember> members = new List<SteamMember>();
            var fields1 = type.GetFields(BindingFlags.Instance | BindingFlags.Public);  //| BindingFlags.DeclaredOnly

            foreach (var field in fields1)
            {
                //sLog.Error("name :" + field.Name + "   PropertyType : " + field.FieldType.Name );  //打印
                var tmp = field.GetCustomAttribute<SteamMember>();
                if (tmp != null)
                {
                    members.Add(tmp);
                    tmp.name = field.Name;
                    //tmp.FieldTypeName = GetTypeName(field.FieldType);//.Name;
                    tmp.fieldInfo = field;
                    //sLog.Debug(tmp.Tag);  //打印
                }

                //if (field.FieldType.GenericTypeArguments != null && field.FieldType.GenericTypeArguments.Length > 0)
                //{
                //    for (int i = 0; i < field.FieldType.GenericTypeArguments.Length; i++)
                //    {
                //        sLog.Error("name :" + field.FieldType.GenericTypeArguments[i]);  //打印
                //    }
                //}
            }


            GeneratedType(type, members);


        }

        public static string GetTypeName(Type type)
        {
            string fieldTypeName = type.Name;
            var genericTypeArguments = type.GenericTypeArguments;
            if (genericTypeArguments != null &&  genericTypeArguments.Length > 0)
            {
                StringBuilder sb = StringPool.GetStringBuilder();
                
                sb.Append(fieldTypeName.Split('`')[0]);
                foreach (var item in genericTypeArguments)  //不支持复杂结构
                {
                    sb.Append("_").Append(GetTypeName(item));
                }
                fieldTypeName = sb.ToString();
                if (!steamMemberList.ContainsKey(fieldTypeName))
                {
                    steamMemberList.Add(fieldTypeName, type);
                }
            }

            if (type.IsEnum)    //如果是枚举处理一下
            {
                if (!steamMemberList.ContainsKey(type.Name))
                {
                    steamMemberList.Add(type.Name, type);
                }
            }

            return fieldTypeName;
        }


        public static string Write_MemberType(Type type)
        {
            //if (type.IsEnum)
            //    return "Write_Enum";
            return $"Write_{GetTypeName(type)}";
        }


        public static string Read_MemberType(Type type)
        {
            //if (type.IsEnum)
            //    return "Read_Enum";
            return $"Read_{GetTypeName(type)}";
        }

        [System.Diagnostics.Conditional("GENERATED")]
        public static void GeneratedType(Type type, List<SteamMember> members)
        {
            var writeString = StringPool.GetStringBuilder();
            var readString = StringPool.GetStringBuilder();

            members.Sort();
            string blankSpace = "        ";
            members.ForEach((m) =>
            {
                writeString.Append(blankSpace);
                readString.Append(blankSpace);
                //var FieldTypeName = GetTypeName(m.fieldInfo.FieldType);
                writeString.Append(WriteContext.Replace("<Write_MemberType>", Write_MemberType(m.fieldInfo.FieldType)).Replace("<MemberName>", m.name)).Append("\r\n");
                readString.Append(ReadContext.Replace("<Read_MemberType>", Read_MemberType(m.fieldInfo.FieldType)).Replace("<MemberName>", m.name)).Append("\r\n");
            });


            string classStr = ClassContext.Replace("<className>", type.Name)
                .Replace("<streamWrite>", writeString.ToString())
                .Replace("<streamRead>", readString.ToString());

            string fileName = FileName.Replace("<className>", type.Name);

            writeString.Release();
            readString.Release();


            //sLog.Debug(classStr);


            string GeneratedPath = AppConfig.Data.Generated.SteamSerializablePath;   //流序列化地址

            FileUtils.DirectoryCreate(GeneratedPath);   //创建文件夹
            //sLog.Error(GeneratedPath);

            string filePath = GeneratedPath + fileName;
            FileUtils.FileDelete(filePath);

            //sLog.Debug(filePath);

            File.AppendAllText(filePath, classStr);
        }

        static string GeneratedPath => AppConfig.Data.Generated.SteamSerializablePath;
        static string FileName = @"Steam_Serializable_<className>.cs";
        static string WriteContext = @"stream.<Write_MemberType>(<MemberName>);";
        static string ReadContext = @"stream.<Read_MemberType>(ref <MemberName>);";
        static string ClassContext = @"
using LGF.Serializable;

public partial class <className> 
{
    public override void Serialize(LGF.Serializable.LStream stream)
    {
        stream.OnStackBegin();

<streamWrite>
        stream.OnStackEnd();
    }

    public override void Deserialize(LGF.Serializable.LStream stream)
    {
        stream.OnStackBegin();

<streamRead>
        stream.OnStackEnd();
    }

    public override <className> NDeserialize(LStream stream)
    {
        Deserialize(stream);
        return this;
    }

    public override <className> NSerialize(LStream stream)
    {
        Serialize(stream);
        return this;
    }
}

public static class <className>Extend
{
    /// <summary>
    /// 小心死循环 一般没有不要自己引用自己  不然死循环
    /// </summary>
    public static void Read_<className>(this LGF.Serializable.LStream stream, ref <className> val)
    {
        bool s = stream.read.ReadBoolean(); //判断是否有数据
        val?.Release(); //回收
        if (!s)
        {
            val = null;
            return;
        }
        val = <className>.Get();
        val.Deserialize(stream);
    }


    public static void Write_<className>(this LGF.Serializable.LStream stream, in <className> val)
    {
        if (val == null)
        {
            stream.writer.Write(false);
            return;
        }
        stream.writer.Write(true);
        val.Serialize(stream);
    }
}

";



        #region 生成文件引用 

        /// <summary>
        /// 引导到框架里面 进行依赖
        /// </summary>
        [System.Diagnostics.Conditional("GENERATED")]
        static void GenerateFileReference()
        {
            string filePath = GeneratedPath + "SteamSerializerByLGF.asmref";
            //sLog.Debug("----------4444----");
            FileUtils.FileDelete(filePath);
            File.Copy(AppConfig.Data.Generated.SteamSerializableCpyeAsmrefPath, filePath);
            
            //File.AppendAllText(filePath, );
        }



        #region 生成泛型方法 或者枚举
        [System.Diagnostics.Conditional("GENERATED")]
        public static void GeneratedGenericMethods()
        {

            var wsb = StringPool.GetStringBuilder();
            var rsb = StringPool.GetStringBuilder();
            wsb.Clear();
            rsb.Clear();

            foreach (var item in steamMemberList)
            {
                //FileName
                string keyword = item.Key.Split('_')[0];
                switch (keyword)
                {
                    case "List":
                        ListContext(rsb, wsb, item.Value);
                        break;
                   case "Dictionary":
                        DictionaryContext(rsb, wsb, item.Value);
                        break;
                    default:
                        if (item.Value.IsEnum)  //处理枚举
                        {
                            EnmuContext(rsb, wsb, item.Value);
                        }
                        else
                        {
                            sLog.Error($" 没有 keyword: {keyword} 的实现方式 请先实现他");
                        }
                        break;
                }
            }


            string filePath = GeneratedPath + "SerializerHelper.cs";


            string classHelperStr = ClassHelperContext
                .Replace("<GenericStreamReadMethods>", rsb.ToString())
                .Replace("<GenericStreamWriteMethods>", wsb.ToString());

            wsb.Release();
            rsb.Release();
            FileUtils.FileDelete(filePath);

            //sLog.Debug(filePath);

            File.AppendAllText(filePath, classHelperStr);

        }


      
        #endregion



        #endregion


        public static string ClassHelperContext = @"
using System;
using System.Collections.Generic;

namespace LGF.Serializable
{
    public static partial class SerializerHelper
    {
//泛型流读方法
<GenericStreamReadMethods>

//泛型流写方法
<GenericStreamWriteMethods>

    
    }

}";


        #region List<T> list泛型

        public static void ListContext(StringBuilder rsb, StringBuilder wsb, Type m)
        {
            rsb.Append("\r\n        ");
            wsb.Append("\r\n        ");
            var genericTypeArgument = m.GenericTypeArguments[0];
            string memberType = genericTypeArgument.Name;

            string clearStr = "list?.Clear();";
            if (typeof(ISerializer).IsAssignableFrom(genericTypeArgument))
                clearStr = "list?.ForEach((a) => a.Release());   list?.Clear();";

            wsb.Append(WriteListContext.Replace("<MemberType>", memberType)
                .Replace("<Write_MemberType>", Write_MemberType(genericTypeArgument)));

            rsb.Append(ReadListContext.Replace("<MemberType>", memberType)
                .Replace("<Read_MemberType>", Read_MemberType(genericTypeArgument))
                .Replace("<listClear>", clearStr));

        }


        static string WriteListContext = @"
        public static void Write_List_<MemberType>(this LGF.Serializable.LStream stream, in List<<MemberType>> list)
        {
            int Length = list == null ? 0 : list.Count;
            stream.writer.Write(Length);
            for (int i = 0; i < Length; i++)
            {
                stream.<Write_MemberType>(list[i]);
            }
        }";



        static string ReadListContext = @"
        public static void Read_List_<MemberType>(this LGF.Serializable.LStream stream,ref List<<MemberType>> list)
        {
            int Length = stream.read.ReadInt32();
            <listClear>
            if (Length > 0)
            {
                if (list == null)
                    list = new List<<MemberType>>();

                for (int i = 0; i < Length; i++)
                {
                    <MemberType> g = default;
                    stream.<Read_MemberType>(ref g);
                    list.Add(g);
                }
            }
        }";
        #endregion



        #region Dictionary<T,T1> 字典泛型


        public static void DictionaryContext(StringBuilder rsb, StringBuilder wsb, Type m)
        {
            rsb.Append("\r\n        ");
            wsb.Append("\r\n        ");
            var genericTypeArgument = m.GenericTypeArguments[0];
            var genericTypeArgument1 = m.GenericTypeArguments[1];

            string clearStr = "dic.Clear();";
            if (typeof(ISerializer).IsAssignableFrom(genericTypeArgument1)) //只做值判断
                clearStr = "foreach (var item in dic) item.Value.Release();   dic.Clear();";

            wsb.Append(WriteDicContext.Replace("<MemberType>", genericTypeArgument.Name)
                .Replace("<MemberType1>", genericTypeArgument1.Name))
                .Replace("<Write_MemberType>", Write_MemberType(genericTypeArgument))
                .Replace("<Write_MemberType1>", Write_MemberType(genericTypeArgument1));


            rsb.Append(ReadDicContext.Replace("<MemberType>", genericTypeArgument.Name)
                .Replace("<MemberType1>", genericTypeArgument1.Name)
                .Replace("<dicClear>", clearStr))
                .Replace("<Read_MemberType>", Read_MemberType(genericTypeArgument))
                .Replace("<Read_MemberType1>", Read_MemberType(genericTypeArgument1));

        }


        static string WriteDicContext = @"
        public static void Write_Dictionary_<MemberType>_<MemberType1>(this LGF.Serializable.LStream stream, in Dictionary<<MemberType>,<MemberType1>> dic)
        {
            int Length = dic == null ? 0 : dic.Count;
            stream.writer.Write(Length);
            if (Length > 0)
            {
                foreach (var item in dic)
                {
                    stream.<Write_MemberType>(item.Key);
                    stream.<Write_MemberType1>(item.Value);
                }
            }
        }";



        static string ReadDicContext = @"
       public static void Read_Dictionary_<MemberType>_<MemberType1>(this LGF.Serializable.LStream stream, ref Dictionary<<MemberType>, <MemberType1>> dic)
        {
           int Length = stream.read.ReadInt32();

            if (dic != null)
            {
                //foreach (var item in dic) item.Value.Release();   dic.Clear();
                <dicClear>
            }

            if (Length > 0)
            {
                if (dic == null)
                    dic = new Dictionary<<MemberType>, <MemberType1>>();

                for (int i = 0; i < Length; i++)
                {
                    <MemberType> key = default;
                    <MemberType1> value = default; //值
                    stream.<Read_MemberType>(ref key);
                    stream.<Read_MemberType1>(ref value);  //
                    dic.Add(key, value);
                }
            }
        }";
        #endregion

        #region enmu 枚举处理

        //枚举处理
        public static void EnmuContext(StringBuilder rsb, StringBuilder wsb, Type m)
        {
            wsb.Append(WriteEnmuContext.Replace("<MemberType>", m.Name));
            rsb.Append(ReadEnmuContext.Replace("<MemberType>", m.Name));
        }

        static string ReadEnmuContext = @"
        public static void Write_<MemberType>(this LGF.Serializable.LStream stream, in <MemberType> val) 
        {
            stream.writer.Write((int)val);
        }";

        static string WriteEnmuContext = @"
        public static void Read_<MemberType>(this LGF.Serializable.LStream stream, ref <MemberType> _out)
        {
             _out  = (<MemberType>)stream.read.ReadInt32();
        }";

        #endregion

#endif

#endif


    }
}
