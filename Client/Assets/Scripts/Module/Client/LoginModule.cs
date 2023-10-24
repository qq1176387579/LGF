/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/11 0:48:01
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;






public class LoginModule : ModuleBase
{
    System.Action OnConnect;
    protected override void OnInit()
    {
        base.OnInit();
        RegisterClientMsg<S2C_Login>(OnConnectServerSucceed);

    }

    /// <summary>
    /// 
    /// </summary>
    public void LoginServer(System.Action _OnConnect)
    {
        if (mainPlayer.name.IsNullOrEmpty()) {
            return;
        }

        if (Client.IsDisconnection())
        {
            //尝试连接中
            this.DebugError("断开服务器");
            return;
        }

        OnConnect = _OnConnect;
        C2S_Login data = C2S_Login.Get();
        data.name = player.name;
        Send(data);
    }



    void OnConnectServerSucceed(S2C_Login data)
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
