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
        public static byte[] bytebuffer = new byte[40960];   //后面写个池子 现在图方便


        public static void Read_UInt32(this LGF.Serializable.LStream stream, ref uint _out)
        {
            _out = stream.read.ReadUInt32();
        }

        public static void Read_Int32(this LGF.Serializable.LStream stream, ref int _out)
        {
            _out = stream.read.ReadInt32();
        }


        public static void Read_Int64(this LGF.Serializable.LStream stream, ref long _out)
        {
            _out = stream.read.ReadInt64();
        }
        public static void Read_UInt64(this LGF.Serializable.LStream stream, ref ulong _out)
        {
            _out = stream.read.ReadUInt64();
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
            if (Length == 0)
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
        public static void Write_UInt64(this LGF.Serializable.LStream stream, in ulong val)
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
                if (val != null && val.Length >= buffer.Length) {
                    val.DebugError($"{val.Length} | {buffer.Length}");
                }
              
                int Length = val == null ? 0 : Encoding.UTF8.GetBytes(val, 0, val.Length, buffer, 0);
                //sLog.Error("Length" + Length);
                stream.writer.Write((ushort)Length);
                stream.writer.Write(buffer, 0, Length);
            }

        }

    }
}
