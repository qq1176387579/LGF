#if HAS_PROTOBUF

using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using LGF;
using LGF.Log;
using System;
using ProtoBuf;
using LGF.Android;

namespace ProtoTest_1
{
    [ProtoContract]
    public class MyClass
    {
        [ProtoBuf.ProtoMember(1)]
        public int _nNumber;
        [ProtoBuf.ProtoMember(2)]
        public string _strName;
        [ProtoBuf.ProtoMember(3)]
        public List<string> _lstInfo;
        [ProtoBuf.ProtoMember(4)]
        public Dictionary<int, string> _dictInfo;
    }

    public class TestProtobuf : MonoBehaviour
    {
        private void Awake()
        {
            AndroidMsgManager.Instance.Init(); 
        }

        public Text text;
        public static int NSerialize<T>(T t, byte[] buffer)
        {
            int length = 0;
            using (MemoryStream m = new MemoryStream())
            {
                if (t != null)
                {
                    ProtoBuf.Serializer.Serialize<T>(m, t);
                }

                m.Position = 0;
                length = (int)m.Length;
                m.Read(buffer, 0, length);
            }
            return length;
        }


        /// <summary>
        /// 反序列化pb数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T NDeserialize<T>(byte[] buffer)
        {
            T t = default(T);

            using (MemoryStream m = new MemoryStream(buffer))
            {
                t = ProtoBuf.Serializer.Deserialize<T>(m);
            }
            return t;
        }


        private void Start()
        {
            MyClass my = new MyClass();
            my._nNumber = 0;
            my._strName = "test";
            my._lstInfo = new List<string>();
            my._lstInfo.Add("a");
            my._lstInfo.Add("b");
            my._lstInfo.Add("c");
            my._dictInfo = new Dictionary<int, string>();
            my._dictInfo.Add(1, "a");
            my._dictInfo.Add(2, "b");
            my._dictInfo.Add(3, "c");

            //using (FileStream stream = File.OpenWrite("test.dat"))
            //{
            //    //序列化后的数据存入文件
            //    ProtoBuf.Serializer.Serialize<MyClass>(stream, my);
            //}
            //ProtoBuf.Serializer.Serialize<MyClass>(stream, my);

            MyClass mt;
            using (MemoryStream m1 = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(m1, my);
                m1.Position = 0;
                //mt = (MyClass)ProtoBuf.Serializer.Deserialize(typeof(MyClass), m1);
                mt = ProtoBuf.Serializer.Deserialize<MyClass>(m1);
            }





            text.text = mt._strName;

            //NSerialize();

            //MyClass my2 = null;
            //using (FileStream stream = File.OpenRead("test.dat"))
            //{
            //    //从文件中读取数据并反序列化
            //    my2 = ProtoBuf.Serializer.Deserialize<MyClass>(stream);
            //}
        }

    }

}

#endif