/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/25 20:28:03
/// 功能描述:  Protobuf 帮助类
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using System.IO;
using ProtoBuf;
using ProtoBuf.Meta;
using System.Text;

namespace LGF.Serializable
{
    public class ProtobufHelper
    {
        static byte[] m_buffer;

        static void CheckBuffer()
        {
            m_buffer = new byte[1024];
        }

        public static T Clone<T>(T data)
        {
            CheckBuffer();
            m_buffer = ToByte<T>(data);
            return ToObjcet<T>(m_buffer);
        }


        /// <summary>
        /// 序列化  产生内存垃圾
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static byte[] ToByte<T>(T t)
        {
            byte[] buffer = null;
            using (MemoryStream m = new MemoryStream()) //有GC
            {
                ProtoBuf.Serializer.Serialize<T>(m, t);
                m.Position = 0;
                int length = (int)m.Length;
                buffer = new byte[length];
                m.Read(buffer, 0, length);
            }
            return buffer;
        }


        public static int ToByte<T>(T t, out byte[] buffer)
        {
            buffer = m_buffer;
            return ToByte(t, m_buffer);
        }

        
        public static int ToByte<T>(T t, byte[] buffer)
        {
            using (MemoryStream m = new MemoryStream()) //有GC
            {
                ProtoBuf.Serializer.Serialize<T>(m, t);
                m.Position = 0;
                int length = (int)m.Length;
                m.Read(buffer, 0, length);
                return length;
            }
            //return 0;
        }


        /// <summary>
        /// 反序列化pb数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T ToObjcet<T>(byte[] buffer)
        {
            T t; 
            using (MemoryStream m = new MemoryStream(buffer))   //后面换成写入池
            {
                t = ProtoBuf.Serializer.Deserialize<T>(m);
            }
            return t;
        }

        #region obj

        //public static object NDeserialize(byte[] buffer, System.Type type)
        //{
        //    object t = null;
        //    using (MemoryStream m = new MemoryStream(buffer))
        //    {
        //        t = RuntimeTypeModel.Default.Deserialize(m, null, type);
        //    }
        //    return t;
        //}

        //public static object NDeserialize(byte[] buffer, int len, System.Type type)
        //{
        //    object t = null;
        //    using (MemoryStream m = new MemoryStream(buffer))
        //    {
        //        t = RuntimeTypeModel.Default.Deserialize(m, null, type, len);
        //    }
        //    return t;
        //}

        //public static T NDeserialize<T>(Stream stream)
        //{
        //    T t = default(T);
        //    t = Serializer.Deserialize<T>(stream);
        //    return t;
        //}



        //public static byte[] ToByte(object t)
        //{
        //    using (MemoryStream m = new MemoryStream())
        //    {
        //        if (t != null)
        //        {
        //            RuntimeTypeModel.Default.Serialize(m, t);
        //        }

        //        m.Position = 0;
        //        int length = (int)m.Length;
        //        buffer = new byte[length];
        //        m.Read(buffer, 0, length);
        //    }
        //    return buffer;
        //}
        #endregion
    }

}


