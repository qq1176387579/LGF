/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/11 0:48:01
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;






public class C_LoginModule : C_ModuleBase
{
    System.Action OnConnect;
    protected override void OnInit()
    {
        base.OnInit();
        RegisterClientMsg<S2C_Connect>(NetMsgDefine.S2C_Connect, OnConnectServerSucceed);
        
    }

    /// <summary>
    /// 连接线上服务器*
    /// </summary>
    public void ConnectServer(System.Action _OnConnect)
    {
        if (Client.IsTryConnecting)
        {
            //尝试连接中
            return;
        }
          
        ConnectServer(AppConfig.Instance.serverInfo.GetEndPoint(), _OnConnect);
    }


    public void ConnectServer(in System.Net.EndPoint endPoint, System.Action _OnConnect)
    {
        if (mainPlayer.name.IsNullOrEmpty())
        {
            return;
        }
        Client.TryToConnect(player.name, endPoint);
        OnConnect = _OnConnect;
    }

    void OnConnectServerSucceed(S2C_Connect data)
    {
        if (player.uid != 0)
        {
            sLog.Error(" 重复登录了  服务端重复需要做登录验证   也可能是网络波动原因");
            return;
        }

        player.uid = data.uid;
        OnConnect?.Invoke();
    }


}
