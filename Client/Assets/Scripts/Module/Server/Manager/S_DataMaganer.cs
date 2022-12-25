/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/25 20:59:05
/// 功能描述:  
****************************************************/

using System.Collections;
using System;
using System.Collections.Generic;
using LGF;
using LGF.Log;


/// <summary>
/// 玩家数据管理
/// </summary>
public class S_DataMaganer : S_SingletonBase<S_DataMaganer>
{
    Dictionary<Type, S_ManagerDataBase> dataDic = new Dictionary<Type, S_ManagerDataBase>();
    //public S_PlayerManagerData playerManagerData;
    //public S_RoomManagerData roomManagerData;
    //public S_RoomManagerData roomManagerData;

    public override void Init()
    {
        base.Init();

        InitManagerData<S_PlayerManagerData>();
        InitManagerData<S_RoomManagerData>();
    }

    void InitManagerData<T>() where T : S_ManagerDataBase, new()
    {
        var data = new T();
        data.Init();
        dataDic.Add(typeof(T), data);
    }

    public T GetData<T>() where T : S_ManagerDataBase, new ()
    {
        Type type = typeof(T);
        if (dataDic.TryGetValue(type, out var data))
        {
            return data as T;
        }
        return null;
    }


}
