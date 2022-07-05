/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/30 22:31:49
/// 功能描述:  Kcp 服务端
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LGF;
using LGF.Log;
using LGF.Serializable;
using UnityEngine;

namespace LGF.Net
{
    /// <summary>
    /// Kcp 服务器
    /// </summary>
    public class KcpServer : KcpCSBase
    {

        public override bool IsClient => false;
        public override bool IsServer => true;

        /// <summary>
        /// 绑定端口号  kcp刷新间隔 帧同步33s每次
        /// </summary>
        /// <param name="port"></param>
        public override void Bing(KcpOnRecv OnRecv, int port = 0, uint interval = 10)  //间隔时间
        {
            base.Bing(OnRecv, port, interval);
            if (m_disposed) return; //启动失败

            this.Debug("服务端已经开启");
        }


     


    }



}


