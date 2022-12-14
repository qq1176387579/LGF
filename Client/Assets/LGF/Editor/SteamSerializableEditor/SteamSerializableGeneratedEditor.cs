/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/3 18:02:14
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using UnityEditor;
using LGF.Serializable;
using System;

using System.IO;
using System.Text;
using UnityEngine;
using System.Reflection;
using LGF.Util;

namespace LGF.Editor
{
    public class SteamSerializableGeneratedEditor : EditorWindow
    {
        [MenuItem("Tools/SteamSerializable/Generated")]
        public static void Generated()
        {
            GeneratedAll();
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("Generated 生成完成");
        }



        #region 生成方法

        //使用实现泛型方法读取  具体实现需要自己写逻辑
        static Dictionary<string, Type> steamMemberList = new Dictionary<string, Type>();
        static Dictionary<Type, string> steamMemberList2 = new Dictionary<Type, string>();

        public static void GeneratedAll()
        {

            steamMemberList.Clear();
            steamMemberList2.Clear();
            List<SteamContract> contracts = new List<SteamContract>();

            Assembly[] assems = new Assembly[1] { Assembly.Load("LGF") }; //AppDomain.CurrentDomain.GetAssemblies();
        
            for (int i = 0; i < assems.Length; i++)
            {
                Type[] types = assems[i].GetTypes();
                foreach (var item in types)
                {
                    //Debug.Log(item);
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
            steamMemberList2.Clear();

            Generated_RegisterNetMsgHandlingMgr();
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
            if (steamMemberList2.ContainsKey(type))
            {
                return steamMemberList2[type];
            }

            string fieldTypeName = type.Name;
            var genericTypeArguments = type.GenericTypeArguments;
            if (genericTypeArguments != null && genericTypeArguments.Length > 0)
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
                    steamMemberList2.Add(type, fieldTypeName);
                }
            }

            if (type.IsEnum)    //如果是枚举处理一下
            {
                if (!steamMemberList.ContainsKey(type.Name))
                {
                    steamMemberList.Add(type.Name, type);
                    steamMemberList2.Add(type, fieldTypeName);
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

        const string blankSpace = "        ";


        public static void GeneratedType(Type type, List<SteamMember> members)
        {
            var writeString = StringPool.GetStringBuilder();
            var readString = StringPool.GetStringBuilder();
            var recycleString = StringPool.GetStringBuilder();
            int recycleCount = 0;
            members.Sort();

            members.ForEach((m) =>
            {
                writeString.Append(blankSpace);
                readString.Append(blankSpace);
                //var FieldTypeName = GetTypeName(m.fieldInfo.FieldType);
                writeString.Append(WriteContext.Replace("<Write_MemberType>", Write_MemberType(m.fieldInfo.FieldType)).Replace("<MemberName>", m.name)).Append("\r\n");
                readString.Append(ReadContext.Replace("<Read_MemberType>", Read_MemberType(m.fieldInfo.FieldType)).Replace("<MemberName>", m.name)).Append("\r\n");
                CheckRecycle(recycleString, m, ref recycleCount);
            });


            string classStr = ClassContext.Replace("<className>", type.Name)
                .Replace("<streamWrite>", writeString.ToString())
                .Replace("<streamRead>", readString.ToString())
                .Replace("<recyclePart>", recycleCount == 0 ? "" : RecycleContext.Replace("<Recycle>", recycleString.ToString()));


            //if (recycleCount == 0)
            //{
            //    classStr.Replace("<recyclePart>","");



            string fileName = FileName.Replace("<className>", type.Name);


            writeString.Release();
            readString.Release();
            recycleString.Release();

            //sLog.Debug(classStr);


            string GeneratedPath = AppConfig.Data.Generated.SteamSerializablePath;   //流序列化地址

            FileUtils.DirectoryCreate(GeneratedPath);   //创建文件夹
            //sLog.Error(GeneratedPath);

            string filePath = GeneratedPath + fileName;
            FileUtils.FileDelete(filePath);

            //sLog.Debug(filePath);

            File.AppendAllText(filePath, classStr);
        }

        static void CheckRecycle(StringBuilder builder, SteamMember steamMember, ref int count)
        {

            Type type = steamMember.fieldInfo.FieldType;
            string name = steamMember.name;
            string keyword = GetTypeName(type).Split('_')[0];
            bool f = false;
            string clearStr = "";
            switch (keyword)
            {
                case "List":
                    {
                        var genericTypeArgument = type.GenericTypeArguments[0];
                        clearStr = $"{name}?.Clear();";
                        if (typeof(ISerializer).IsAssignableFrom(genericTypeArgument))
                            clearStr = $"{name}?.ForEach((a) => a.Release());   {name}?.Clear();";


                        f = true;
                    }

                    break;
                case "Dictionary":
                    {
                        var genericTypeArgument1 = type.GenericTypeArguments[1];

                        clearStr = $"{name}.Clear();";
                        if (typeof(ISerializer).IsAssignableFrom(genericTypeArgument1)) //只做值判断
                            clearStr = $"foreach (var item in dic) item.Value.Release();   {name}.Clear();";


                        f = true;
                        break;
                    }
                default:
                    {
                        //要进行回收  不然有些状态定义的类  也会序列化 但是我只需他为空就行了
                        if (typeof(ISerializer).IsAssignableFrom(type))
                        {
                            clearStr = $"{name}?.Release(); {name} = null;";
                            f = true;
                        }
                    }
                    break;
            }

            if (f)
            {
                builder.Append(blankSpace);
                builder.Append(clearStr);
                builder.Append("\n");
                count++;
            }
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
    <recyclePart>
}

public static class <className>Extend
{
    /// <summary>
    /// 小心死循环 一般没有不要自己引用自己  不然死循环
    /// </summary>
    public static void Read_<className>(this LGF.Serializable.LStream stream, ref <className> val)
    {
        bool s = stream.read.ReadBoolean(); //判断是否有数据
       
        if (!s)
        {
            val?.Release(); //回收
            val = null;
            return;
        }
        if(val == null)   val = <className>.Get();
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
        static string RecycleContext = @"

        
    protected override void OnRelease()
    {
        base.OnRelease();
<Recycle>
    }

";


        #region 生成文件引用 

        /// <summary>
        /// 引导到框架里面 进行依赖
        /// </summary>

        static void GenerateFileReference()
        {
            string filePath = GeneratedPath + "SteamSerializerByLGF.asmref";
            //sLog.Debug("----------4444----");
            FileUtils.FileDelete(filePath);
            File.Copy(AppConfig.Data.Generated.SteamSerializableCpyeAsmrefPath, filePath);

            //File.AppendAllText(filePath, );
        }



        #region 生成泛型方法 或者枚举
        public static void GeneratedGenericMethods()
        {
            //LGF.Util.FileUtil.DirectoryCreate();
            //FileUtils.DirectoryCreate();

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


        #endregion



        #endregion




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




        #region 枚举生成注册 响应脚本


        static void Generated_RegisterNetMsgHandlingMgr()
        {
            HashSet<NetMsgDefine> notRegister = new HashSet<NetMsgDefine>();
            notRegister.Add(NetMsgDefine.Empty);
            notRegister.Add(NetMsgDefine.C2S_Connect);
            notRegister.Add(NetMsgDefine.N_C2S_GetAllServersInfo);
            notRegister.Add(NetMsgDefine.N_S2C_GetAllServersInfo);

            //var sb = StringPool.GetStringBuilder();
            var c2sStr = StringPool.GetStringBuilder();
            var s2cStr = StringPool.GetStringBuilder();

            foreach (var item in Enum.GetValues(typeof(NetMsgDefine)))
            {
                //Debug.Log(item);
                //Debug.Log((NetMsgDefine)item);
                //if ((NetMsgDefine)item == NetMsgDefine.Empty)
                //{
                //    Debug.Log("-----");
                //}

                if (notRegister.Contains((NetMsgDefine)item))
                    continue;

                string name = item.ToString();
                if (name.Contains("C2S_"))
                {
                    c2sStr.AppendLine(C2SMsgString.Replace("<NetMsgName>", name));
                }
                else if (name.Contains("S2C_"))
                {
                    s2cStr.AppendLine(S2CMsgString.Replace("<NetMsgName>", name));
                }
            }

            string ConText = NetMsgHandlingMgrConText
                .Replace("<C2SMsg>", c2sStr.ToString())
                .Replace("<S2CMsg>", s2cStr.ToString())
                .Replace("<DoubleQuotationMarks>", "\"");





            s2cStr.Release();
            c2sStr.Release();

            string filePath = GeneratedPath + "NetMsgHandlingMgr_Register.cs";

            FileUtils.FileDelete(filePath);

            //sLog.Debug(filePath);

            File.AppendAllText(filePath, ConText);
        }


        static string S2CMsgString = @"                case NetMsgDefine.<NetMsgName>: InvokeClientMsg<<NetMsgName>>(type, _stream); break;";
        static string C2SMsgString = @"                case NetMsgDefine.<NetMsgName>: InvokeServerMsg<<NetMsgName>>(type, session, _stream); break;";
        static string NetMsgHandlingMgrConText = @"
using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Serializable;
using UnityEngine;

namespace LGF.Net
{
    //后面自己写脚本自动化处理
    public partial class NetMsgHandlingMgr
    {

        #region 服务器
        protected override void InvokeServerMsgEx(NetMsgDefine type, KcpServer.KcpSession session, LStream _stream)
        {
            switch (type)
            {
<C2SMsg>
                default:
                    sLog.Error(<DoubleQuotationMarks> Server 未注册该事件 或者 流程出错 请检查!! <DoubleQuotationMarks> +type);
                    break;
            }
        }
        #endregion

        #region 客户端 

        protected override void InvokeClientMsgEx(NetMsgDefine type, LStream _stream)
        {
            switch (type)
            {
<S2CMsg>
                default:
                    sLog.Error(<DoubleQuotationMarks> Client 未注册该事件 或者 流程出错 请检查!! <DoubleQuotationMarks> +type);
                    break;
            }
        }
        #endregion
    }
}

";
        #endregion





    }



    #endregion




}
