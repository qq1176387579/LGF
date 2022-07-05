/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/4 23:27:00
/// 功能描述:  消息处理管理器
****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Net;
using LGF;
using LGF.Log;
using LGF.Serializable;
using LGF.DataStruct;

namespace LGF.Net
{
    /// <summary>
    /// 消息处理管理器  这里可以分流 处理客户端 给客户端处理  服务端给服务端 处理 我这里简单放一起处理了
    /// 如果要分流的话 给协议定大小 大于或者小于多少是客户端协议   其他是服务端协议 
    /// </summary>
    public class NetMsgHandlingMgr : SingletonBase<NetMsgHandlingMgr> , INetMsgHandling
    {

        /// <summary>
        /// kcp消息处理 
        /// </summary>
        public void OnKcpNetMsg(KcpSocket.KcpAgent agent, LGFStream stream)
        {

        }


        /// <summary>
        /// 消息处理cs处理  接收数据
        /// </summary>
        public void OnCSNetMsg(KcpCSBase csBase, in EndPoint point, LGFStream stream)
        {
            NetMsgDefine type = stream.GetNetMsgType();//
            //消息处理
            switch (type)
            {

                case NetMsgDefine.C2S_Connect:
                    {
                        this.Debug(StringPool.Concat(point.ToString(), "  连接成功")); //后面可能乱码 记得加线程
                        S2C_Connect _Connect = S2C_Connect.Get();
                        _Connect.port = csBase.LocalPort;
                        csBase.Send(_Connect, point);
                    }
                    break;
                case NetMsgDefine.S2C_Connect:
                    {
                        var tmp = S2C_Connect.Get(stream);   //获得服务器端口
                        this.Debug($"获得服务器端口号 :", tmp.port);
                        //也是房间号
                        EndPoint endPoint = new IPEndPoint((point as IPEndPoint).Address, tmp.port);  //获得尝试连接对象的广播号
                        tmp.Release();
                        EventManager.Instance.BroadCastEvent(GameEventType.Net_S2C_Connect, endPoint);
                    }
                    break;
                default:
                    this.Debug("type {0} 非法消息" , type);
                    stream.Clear(); ;
                    break;
            }
        }

    }

}

