/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/3 9:22:02
/// 功能描述:  
****************************************************/

using System.IO;
using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using System;

namespace LGF.Serializable
{
    public class LGFStream
    {
        int stack = 0;

        public MemoryStream stream;
        public BinaryWriter writer;
        public BinaryReader read;
        public int Lenght { get; private set; }


        public LGFStream(MemoryStream stream_)
        {
            stream = stream_;
            writer = new BinaryWriter(stream);
            read = new BinaryReader(stream);
        }

        public LGFStream(int count = 0) : this(new MemoryStream(count)) { }


        public byte[] GetBuffer()
        {
            return stream.GetBuffer();
        }

        public void Clear()
        {
            stream.Seek(0, SeekOrigin.Begin);
            Lenght = 0;
        }

        public int Position { get => (int)stream.Position; set => stream.Position = value; } 

        public void OnStackBegin()
        {
            stack++;
            if (stack >= 10)
            {
                this.DebugError("请检查  是否出现 a b 互相引用的情况");
            }
        }




        public void OnStackEnd()
        {
            stack--;
            if (stack == 0)
            {
                Lenght = Position;
                stream.Seek(0, SeekOrigin.Begin);
            }
                
        }


        public void Reset()
        {
            stream.Seek(0, SeekOrigin.Begin);
        }




        /// <summary>
        /// 获得网络消息类型 并重置位置
        /// </summary>
        /// <returns></returns>
        public NetMsgDefine GetNetMsgType()
        {
            NetMsgDefine val = NetMsgDefine.Empty;
            try
            {
                val = (NetMsgDefine)read.ReadInt32();
            }
            catch (Exception e)
            {
                e.DebugError();
                return NetMsgDefine.Empty;  //非法操作
            }
           
            Reset();
            return val;
        }

    }

}


