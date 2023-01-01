/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/11 17:37:07
/// 功能描述:  客户端玩家信息 与操作
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;


/// <summary>
/// 玩家数据管理器
/// </summary>
public class C_MainPlayerManager : C_SingletonBase<C_MainPlayerManager>
{
    public uint uid;
    public string name;
    public uint RoomID;
    public bool InRoom => RoomID > 0;
    C2S_FrameOpKey tmpFrameOpKey;
    public override void Init()
    {
        base.Init();
        tmpFrameOpKey = C2S_FrameOpKey.Get();
    }


    /// <summary>
    /// 设置名字
    /// </summary>
    public void SetName(string _name)
    {
        name = _name;
    }



    public void SendNotRecycle<T>(T data) where T : C2S_BASE<T>, new()
    {
        moduleMgr.SendNotRecycle(data);
    }


    public void Send<T>(T data, bool IsRecycle = true) where T : C2S_BASE<T>, new()
    {
        moduleMgr.Send(data, IsRecycle);
    }


    public CMD_UserInfo CopyUserInfo()
    {
        CMD_UserInfo info = CMD_UserInfo.Get();
        info.uid = uid;
        info.name = name;
        return info;
    }




    public void SendMoveKey(long x, long z, uint Keyid)
    {
        C2S_FrameOpKey key = tmpFrameOpKey;
        key.keytype = Frame_KeyType.Move;
        if (key.moveKey == null) key.moveKey = Frame_MoveKey.Get();
        key.moveKey.x = x;
        key.moveKey.z = z;
        key.moveKey.Keyid = Keyid;

        SendNotRecycle(key);
    }





}
