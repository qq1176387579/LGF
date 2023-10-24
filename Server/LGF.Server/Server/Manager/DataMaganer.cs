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


namespace LGF.Server
{
    /// <summary>
    /// 玩家数据管理
    /// </summary>
    public class DataMaganer : ModuleSingletonBase<DataMaganer>
    {
        Dictionary<Type, ManagerDataBase> dataDic = new Dictionary<Type, ManagerDataBase>();
        //public S_PlayerManagerData playerManagerData;
        //public S_RoomManagerData roomManagerData;
        //public S_RoomManagerData roomManagerData;

        public override void Init()
        {
            base.Init();

            InitManagerData<PlayerManagerData>();
            InitManagerData<RoomManagerData>();
        }

        void InitManagerData<T>() where T : ManagerDataBase, new()
        {
            var data = new T();
            data.Init();
            dataDic.Add(typeof(T), data);
        }

        public T GetData<T>() where T : ManagerDataBase, new()
        {
            Type type = typeof(T);
            if (dataDic.TryGetValue(type, out var data)) {
                return data as T;
            }
            return null;
        }


    }

}
