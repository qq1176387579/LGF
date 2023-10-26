using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
//json协议  前后端通用解析方式 图方便

public enum OperationCode
{
    InformServer_CheckCharacterPosition, //检测位置
}

//区分服务器向客户端发送的事件的类型
public enum EventCode 
{
    
}

public static class BaseHandlerExtend
{
    public static int ToInt(this string str)
    {
        if (int.TryParse(str, out int result)) {
            return result;
        }

        return -1;
    }
}


namespace Protocol
{


    public class Vector3Int
    {
        public int x;
        public int y;
        public int z;

        public void Copy(UnityEngine.Vector3 vector)
        {
            vector *= 10000;
            x = (int)vector.x;
            y = (int)vector.y;
            z = (int)vector.z;
        }

        /// <summary>
        /// 值相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ValEquals(Vector3Int obj)
        {
            return x == obj.x && y == obj.y && z == obj.z;
        }



        public override string ToString()
        {
            return $"<{x}-{y}-{z}>";
        }
    }



    public interface IProtocol
    {

    }

    /// <summary>
    /// 通知服务器 检查位置
    /// </summary>
    public class InformServer_CheckCharacterPosition : IProtocol
    {
        public class playerInfo
        {
            public uint playerID;
            public Vector3Int LogicPos = new Vector3Int();
            public Vector3Int InputDir = new Vector3Int();
        }


        public ulong curFrame;
        //public List<playerInfo> playerinfos = new List<playerInfo>();
        public DictionaryToJsonHelper<uint, playerInfo> playerinfos = new DictionaryToJsonHelper<uint, playerInfo>();
    }

    /// <summary>
    /// 请求
    /// </summary>
    public class Request_DailySignin : IProtocol
    {
        public int type;    //1表示签到 如果今天签到了那么补签 2表示领取下方累计签到奖励
        public int param;   //类型为2的时候 表示领取第几个
    }

    /// <summary>
    /// 响应
    /// </summary>
    public class Response_DailySignin : IProtocol
    {
        public int type;    //3类型 表示同步  1表示签到响应  2表示累计签到响应
        public int SingInCount; //签到数量
        public int TotalSingInState;    //总签到状态
        public int OffsetSignIn;     //补签次数
        public int LastTimeSignin;    //上次签到时间
    }


    /// <summary>
    /// 同步SyncUserBaseInfo 基本信息
    /// </summary>
    public class SyncUserBaseInfo : IProtocol
    {
        public int WZQNum;
        public int GBNum;
    }

    /// <summary>
    /// 同步SyncUserBaseInfo 基本信息
    /// </summary>
    public class DictionaryToJsonHelper<T,T2> : IProtocol
    {
        public List<T> keys = new List<T>();
        public List<T2> vals = new List<T2>();
        public void Add(T key, T2 val)
        {
            keys.Add(key);
            vals.Add(val);
        }
        
        public void CopyTo(Dictionary<T,T2> dic)
        {
            for (int i = 0; i < keys.Count; i++) {
                dic.Add(keys[i], vals[i]);
            }
        }

        public Dictionary<T, T2> GetDic()
        {
            Dictionary<T,T2> pairs = new Dictionary<T,T2>();
            CopyTo(pairs);
            return pairs;
        }

        public void InitData(Dictionary<T, T2> dic)
        {
            foreach (var item in dic) {
                Add(item.Key, item.Value);
            }
        }

        public void Clear()
        {
            keys.Clear();
            vals.Clear();
        }

    }

    public static class ProtocolHelper
    {
        public static string ToJson<T>(this T protocol) 
        {
            return JsonMapper.ToJson(protocol); ;
        }

        public static T ToObjcet<T>(this string json) 
        {
            return JsonMapper.ToObject<T>(json); 
        }
    }


}



