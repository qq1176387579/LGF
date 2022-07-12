/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/11 1:02:19
/// 功能描述:  
****************************************************/

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

        
        void InvokeServerMsgEx(NetMsgDefine type, KcpServer.KcpSession session, LStream _stream)
        {
            switch (type)
            {
                case NetMsgDefine.C2S_TextMsg: InvokeServerMsg<C2S_TextMsg>(type, session, _stream); break;
                //case NetMsgDefine.C2S_TextMsg: InvokeServerMsg<C2S_TextMsg>(type, session, _stream); break;   //后续的要在这里注册 添加新的模块或者事件
                default:
                    sLog.Error("Server 未注册该事件 或者 流程出错 请检查!!   " + type);
                    break;
            }
        }

        #endregion



        #region 客户端

 

        void InvokeClientMsgEx(NetMsgDefine type, LStream _stream)
        {
            switch (type)
            {
                case NetMsgDefine.S2C_Connect:  //连接成功
                    InvokeClientMsg<S2C_Connect>(type, _stream);    //这里封装下
                    break;
                case NetMsgDefine.S2C_TextMsg:
                    InvokeClientMsg<S2C_TextMsg>(type, _stream); break;
                default:
                    sLog.Error("Client 未注册该事件 或者 流程出错 请检查!!   " + type);
                    break;
            }
        }

        #endregion

    }

}
