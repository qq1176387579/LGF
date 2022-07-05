using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LGF.Log;
using UnityEngine;

namespace LGF.Net
{
    public static class NetConst
    {

        //public const int ServerAnyPort = 45556;

        /// <summary>
        /// 服务器端口   后面写个算法 随机10范围的ip地址
        /// </summary>
        public const int ServerPort = 45557;

        /// <summary>
        /// 客户端端口 随机端口号
        /// </summary>
        public const int ClientPort = 0;

        /// <summary>
        /// 随机端口
        /// </summary>
        public const int RandomPort = 0;

        /// <summary>
        /// kcp 的 conv值
        /// </summary>
        public const int KcpConv = 1022;



        public const int Socket_RecvBufferSize = 4096;   //应该不会大于这个
        public const int Socket_SendBufferSize = 4096;   //应该不会大于这个
    }


    public static class SocketHelper
    {
        public const uint IOC_IN = 0x80000000;
        public const uint IOC_VENDOR = 0x18000000;
        public static readonly uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;

        /// <summary>
        /// 设置低操作 udp连接  处理udp发送失败 与断开连接的问题  不然直接影响整个程序 
        /// </summary>
        /// <param name="socket"></param>
        public static void IOControl_UdpConnreset(this Socket socket)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            //安卓没有这个问题且   安卓用低级操作模式 socket会报错
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                socket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
            }
#endif

        }

        public static Socket UdpBind(IPEndPoint local)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.IOControl_UdpConnreset();
            socket.Bind(local);
            return socket;
        }

        public static Socket UdpBind(IPAddress address, int port)
        {
            return UdpBind(new IPEndPoint(address, port));
        }


        /// <summary>
        /// 设置能广播
        /// </summary>
        public static void SetBroadcast(this Socket socket)
        {
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        }


        public static int ceilpow2(int x)
        {
            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x + 1;
        }


        public static ulong CastIP(string ip)
        {
            IPAddress address = IPAddress.Parse(ip);
            return CastIP(address);
        }


        /// <summary>
        /// 获得ipv4地址  as 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static ulong CastIP(IPAddress address)
        {
            #region 原来写法 有GC
            /*
             //有GC的 内部会new数组
            byte[] addressBytes = address.GetAddressBytes();   
            // This restriction is implicit in your existing code, but
            // it would currently just lose data...
            if (addressBytes.Length != 4)
            {
                throw new ArgumentException("Must be an IPv4 address");
            }
            int networkOrder = BitConverter.ToInt32(addressBytes, 0);
            return (uint)IPAddress.NetworkToHostOrder(networkOrder);
            */
            #endregion

            //只处理ipv4
            if (address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                address.DebugError("Must be an IPv4 address");
                return 0;
            }
#if NOT_UNITY
            byte[] addressBytes = address.GetAddressBytes();    //有GC
            return (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(addressBytes, 0));
#else
            //unity记得看程序集 程序集不一样 这个值也不一样 铁坑
            return (uint)address.GetHashCode(); //源码  ip4时GetHashCode就是ip值  
#endif

        }


        /// <summary>
        /// 获得ipv4地址  as 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static ulong CastIP(EndPoint end)
        {
            //只处理ipv4
            if (end is IPEndPoint)
            {
                return CastIP((end as IPEndPoint).Address); 
            }
            end.DebugError($" {end}  not as IPEndPoint ");
            return 0;
           
        }







        /** 
        * byte数组中取int数值，本方法适用于(低位在前，高位在后)的顺序，和和intToBytes（）配套使用
        *  
        * @param src 
        *            byte数组 
        * @param offset 
        *            从数组的第offset位开始 
        * @return int数值 
        */
        public static int bytesToInt(this byte[] src, int offset = 0)
        {
            int value;
            value = (int)((src[offset] & 0xFF)
                    | ((src[offset + 1] & 0xFF) << 8)
                    | ((src[offset + 2] & 0xFF) << 16)
                    | ((src[offset + 3] & 0xFF) << 24));
            return value;
        }

        /** 
        * byte数组中取int数值，本方法适用于(低位在后，高位在前)的顺序。和intToBytes2（）配套使用
        */
        public static int bytesToInt2(this byte[] src, int offset = 0)
        {
            int value;
            value = (int)(((src[offset] & 0xFF) << 24)
                    | ((src[offset + 1] & 0xFF) << 16)
                    | ((src[offset + 2] & 0xFF) << 8)
                    | (src[offset + 3] & 0xFF));
            return value;
        }



        /**
        * 将int数值转换为占四个字节的byte数组，本方法适用于(低位在前，高位在后)的顺序。 和bytesToInt（）配套使用
        * @param value
        *            要转换的int值
        * @return byte数组
        */
        public static byte[] intToBytes(this int value, byte[] src = null, int offset = 0)
        {
            if (src == null) src = new byte[4];
            src[offset + 3] = (byte)((value >> 24) & 0xFF);
            src[offset + 2] = (byte)((value >> 16) & 0xFF);
            src[offset + 1] = (byte)((value >> 8) & 0xFF);
            src[offset + 0] = (byte)(value & 0xFF);
            return src;
        }

        /**
        * 将int数值转换为占四个字节的byte数组，本方法适用于(高位在前，低位在后)的顺序。  和bytesToInt2（）配套使用
        */
        public static byte[] intToBytes2(this int value, byte[] src = null, int offset = 0)
        {   
            if (src == null) src = new byte[4];
            src[offset + 0] = (byte)((value >> 24) & 0xFF);
            src[offset + 1] = (byte)((value >> 16) & 0xFF);
            src[offset + 2] = (byte)((value >> 8) & 0xFF);
            src[offset + 3] = (byte)(value & 0xFF);
            return src;
        }




    }
}





