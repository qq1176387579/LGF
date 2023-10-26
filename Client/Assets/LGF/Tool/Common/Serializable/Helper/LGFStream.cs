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
    public class LStream
    {
        int stack = 0;

        public MemoryStream stream;
        public BinaryWriter writer;
        public BinaryReader read;
        public int Lenght { get; private set; }

        const int tmpBytsCount = 1024;
        static byte[] tmpByts = new byte[tmpBytsCount]; //用来填充数据的 不然 MemoryStream的ID位置有问题

        public LStream(MemoryStream stream_)
        {
            stream = stream_;
            writer = new BinaryWriter(stream);
            read = new BinaryReader(stream);
        }

        public LStream(int count = 0) : this(new MemoryStream(count)) {
            int count1 = count;
            while (count1 > 0)
            {
                stream.Write(tmpByts, 0, count1 > tmpBytsCount ? tmpBytsCount : count1);
                count1 -= tmpBytsCount;
            }
            Clear();
        }

        /// <summary>
        /// 检查大小 如果不够扩容
        /// </summary>
        /// <param name="size"></param>
        public void CheckSize(int size)
        {
            if (stream.GetBuffer().Length >= size) {
                return;
            }

            long tmpPos = Position;
            Position = Lenght;
            stream.Seek(Position, SeekOrigin.Begin);
            int count1 = size;
            while (count1 > 0) {
                stream.Write(tmpByts, 0, count1 > tmpBytsCount ? tmpBytsCount : count1);
                count1 -= tmpBytsCount;
            }
            Position = tmpPos;
            stream.Seek(Position, SeekOrigin.Begin);    //还原
        }


        public byte[] GetBuffer()
        {
            return stream.GetBuffer();
        }

        public void Clear()
        {
            stream.Seek(0, SeekOrigin.Begin);
            Lenght = 0;
        }

        public long Position { get => stream.Position; set => stream.Position = value; } 

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
                Lenght = (int)Position;
                stream.Seek(0, SeekOrigin.Begin);
            }
                
        }


        public void Reset()
        {
            stream.Seek(0, SeekOrigin.Begin);
        }




        ///// <summary>
        ///// 获得网络消息类型 并重置位置
        ///// 后面看情况改成扩展方法
        ///// </summary>
        ///// <returns></returns>
        //public NetMsgDefine GetNetMsgType()
        //{
        //    NetMsgDefine val = NetMsgDefine.Empty;
        //    try
        //    {
        //        val = (NetMsgDefine)read.ReadInt32();
        //    }
        //    catch (Exception e)
        //    {
        //        e.DebugError();
        //        return NetMsgDefine.Empty;  //非法操作
        //    }
        //    Reset();
        //    return val;
        //}


        public uint GetUid()
        {
            uint val = 0;
            stream.Position = 4;
            val = read.ReadUInt32();
            //try
            //{
            //    val = read.ReadUInt32();
            //}
            //catch (Exception e)
            //{
            //    e.DebugError();
            //    return 0;  //非法操作
            //}
            Reset();
            return val;
        }

        public void Close()
        {
            stream.Close();
            writer.Close();
            read.Close();
        }

    }

}


