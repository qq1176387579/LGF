using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


using N_Avatar;


public partial class JsonManager
{

    public string Get_String(string cellString)
    {
        return cellString;
    }

    //获取float
    public float Get_float(string cellString)
    {
        float.TryParse(cellString, out float ret);
        return ret;
    }

    //获取float[]
    public float[] Get_float_array(string cellString)
    {
        if (string.IsNullOrEmpty(cellString))
        {
            return null;
        }
        string[] data = cellString.Split('|');
        float[] ret = new float[data.Length];
        for (int i = 0; i < ret.Length; i++)
        {
            if (!float.TryParse(data[i], out ret[i]))
            {
                Debug.LogError("float解析错误,表格值：" + data[i]);
            }
        }
        return ret;
    }
    //获取int[]
    public int[] Get_int_array(string cellString)
    {
        if (string.IsNullOrEmpty(cellString))
        {
            return null;
        }
        string[] data = cellString.Split('|');
        int[] ret = new int[data.Length];
        for (int i = 0; i < ret.Length; i++)
        {
            if (!Int32.TryParse(data[i], out ret[i]))
            {
                Debug.LogError("int解析错误,表格值：" + data[i]);
            }
            //ret[i] = Int32.Parse(data[i]);
        }
        return ret;
    }

    //获取string[]
    public string[] Get_string_array(string cellString)
    {
        if (string.IsNullOrEmpty(cellString))
        {
            return null;
        }
        string[] ret = cellString.Split('|');
        return ret;
    }
    //获取List<string>
    public List<string> Get_List_string(string cellString)
    {
        if (string.IsNullOrEmpty(cellString))
        {
            return null;
        }
        string[] ret = cellString.Split('|');
        List<string> list = new List<string>();
        for (int i = 0; i < ret.Length; i++)
        {
            list.Add(ret[i]);
        }
        return list;
    }

    //获取Dictionary<string,int>
    public Dictionary<string, int> Get_Dictionary_string_int(string cellString)
    {
        if (string.IsNullOrEmpty(cellString))
        {
            return null;
        }
        Dictionary<string, int> dic = new Dictionary<string, int>();
        string[] ret = cellString.Split('|');

        for (int i = 0; i < ret.Length; i++)
        {
            string[] kv = ret[i].Split(':');
            if (kv.Length < 2)
            {
                Debug.LogError("字典格式不对，表格数据：" + cellString);
                continue;
            }
            Int32.TryParse(kv[1], out int value);
            dic.Add(kv[0], value);
        }
        return dic;
    }

    //获取Dictionary<int,int>
    public Dictionary<int, int> Get_Dictionary_int_int(string cellString)
    {
        if (string.IsNullOrEmpty(cellString))
        {
            return null;
        }
        Dictionary<int, int> dic = new Dictionary<int, int>();
        string[] ret = cellString.Split('|');

        for (int i = 0; i < ret.Length; i++)
        {
            string[] kv = ret[i].Split(':');
            if (kv.Length < 2)
            {
                Debug.LogError("字典格式不对，表格数据：" + cellString);
                continue;
            }
            Int32.TryParse(kv[1].TrimStart('0'), out int value);
            dic.Add(int.Parse(kv[0]), value);
        }
        return dic;
    }

    //获取Dictionary<string,string>
    public Dictionary<string, string> Get_Dictionary_string_string(string cellString)
    {
        if (string.IsNullOrEmpty(cellString))
        {
            return null;
        }
        Dictionary<string, string> dic = new Dictionary<string, string>();
        string[] ret = cellString.Split('|');

        for (int i = 0; i < ret.Length; i++)
        {
            string[] kv = ret[i].Split(':');
            if (kv.Length < 2)
            {
                Debug.LogError("字典格式不对，表格数据：" + cellString);
                continue;
            }
            //Int32.TryParse(kv[1], out int value);
            dic.Add(kv[0], kv[1]);
        }
        return dic;
    }

    //获取Dictionary<string,List<int>>
    public Dictionary<string, List<int>> Get_Dictionary_string_List_int(string cellString)
    {
        if (string.IsNullOrEmpty(cellString))
        {
            return null;
        }
        Dictionary<string, List<int>> dic = new Dictionary<string, List<int>>();
        string[] ret = cellString.Split('|');

        for (int i = 0; i < ret.Length; i++)
        {
            string[] kv = ret[i].Split(':');

            string[] vv = kv[1].Split(',');
            List<int> ins = new List<int>();
            for (int j = 0; j < vv.Length; j++)
            {
                Int32.TryParse(vv[j], out int value);
                ins.Add(value);
            }

            dic.Add(kv[0], ins);
        }

        return dic;
    }

    //获取Dictionary<int,List<int>>
    public Dictionary<int, List<int>> Get_Dictionary_int_List_int(string cellString)
    {
        if (string.IsNullOrEmpty(cellString))
        {
            return null;
        }
        Dictionary<int, List<int>> dic = new Dictionary<int, List<int>>();

        string[] ret = cellString.Split('|');
        for (int i = 0; i < ret.Length; i++)
        {
            string[] kv = ret[i].Split(':');

            string[] vv = kv[1].Split(',');
            List<int> ins = new List<int>();
            for (int j = 0; j < vv.Length; j++)
            {
                Int32.TryParse(vv[j], out int value);
                ins.Add(value);
            }

            dic.Add(int.Parse(kv[0]), ins);
        }

        return dic;
    }

    public Dictionary<int, int[]> Get_Dictionary_int_int_array(string cellString)
    {
        if (string.IsNullOrEmpty(cellString))
        {
            return null;
        }
        Dictionary<int, int[]> dic = new Dictionary<int, int[]>();

        string[] ret = cellString.Split('|');
        for (int i = 0; i < ret.Length; i++)
        {
            string[] kv = ret[i].Split(':');

            string[] vv = kv[1].Split(',');
            int[] ins = new int[vv.Length];
            for (int j = 0; j < vv.Length; j++)
            {
                Int32.TryParse(vv[j], out int value);
                ins[j] = value;
            }

            dic.Add(int.Parse(kv[0]), ins);
        }

        return dic;
    }



    //获取Vector3
    public Vector3 Get_Vector3(string cellString)
    {
        if (string.IsNullOrEmpty(cellString))
        {
            return Vector3.zero;
        }
        string[] ret = cellString.Split('|');
        if (ret.Length < 3)
        {
            Debug.LogError("Vector3格式不对，表格数据：" + cellString);
            return Vector3.zero;
        }

        return new Vector3(float.Parse(ret[0]), float.Parse(ret[1]), float.Parse(ret[2]));
    }





    public Dictionary<int, List<int>>[] Get_Dictionary_int_List_int_array(string cellString)
    {
        //Get_Dictionary_int_List_int
        if (string.IsNullOrEmpty(cellString))
        {
            return null;
        }
        string[] ret = cellString.Split('$');
        Dictionary<int, List<int>>[] arr = new Dictionary<int, List<int>>[ret.Length];

        for (int i = 0; i < ret.Length; i++)
        {
            arr[i] = Get_Dictionary_int_List_int(ret[i]);
        }
        return arr;
    }



    public Dictionary<int, Dictionary<int, List<int>>> Get_Dictionary_int_Dictionary_int_List_int(string cellString)
    {
        if (string.IsNullOrEmpty(cellString))
        {
            return null;
        }
        string[] ret = cellString.Split('$');
        Dictionary<int, Dictionary<int, List<int>>> dic = new Dictionary<int, Dictionary<int, List<int>>>();

        for (int i = 0; i < ret.Length; i++)
        {
            string[] ret2 = ret[i].Split('=');
            int t = 0;
            Int32.TryParse(ret2[0], out t);
            dic[t] = Get_Dictionary_int_List_int(ret2[1]);
        }
        return dic;
    }

    public bool Get_bool(string cellString)
    {
        if (string.IsNullOrEmpty(cellString)) return false;

        int.TryParse(cellString, out int ret);
        return ret != 0;
    }

    public int[] GetIntArray(string cellString)
    {
        if (string.IsNullOrEmpty(cellString)) return null;

        string[] data = cellString.Split(',');
        int[] ret = new int[data.Length];
        for (int i = 0; i < ret.Length; i++)
        {
            if (!Int32.TryParse(data[i], out ret[i]))
            {
                Debug.LogError("int解析错误,表格值：" + data[i]);
            }
            //ret[i] = Int32.Parse(data[i]);
        }
        return ret;
    }

    public int[][] Get_int_array_array(string cellString)
    {
        if (string.IsNullOrEmpty(cellString)) return null;

        string[] ret = cellString.Split('|');
        int[][] arr = new int[ret.Length][];

        for (int i = 0; i < ret.Length; i++)
        {
            arr[i] = GetIntArray(ret[i]);
        }
        return arr;
    }


    public int[][][] Get_int_array_array_array(string cellString)
    {
        if (string.IsNullOrEmpty(cellString)) return null;

        string[] ret = cellString.Split('#');
        int[][][] arr = new int[ret.Length][][];

        for (int i = 0; i < ret.Length; i++)
        {
            arr[i] = Get_int_array_array(ret[i]);
        }
        return arr;
    }

    public Dictionary<int, int[][]>  Get_Dictionary_int_int_array_array(string cellString)
    {
        if (string.IsNullOrEmpty(cellString)) return null;
        string[] ret = cellString.Split('#');

        Dictionary<int, int[][]> dic = new Dictionary<int, int[][]>();

        for (int i = 0; i < ret.Length; i++)
        {
            string[] ret1 = ret[i].Split(':');
            dic.Add(Get_int(ret1[0]), Get_int_array_array(ret1[1]));
        }
        return dic;

    }




    public Flag Get_Flag(string cellString)
    {
        if (string.IsNullOrEmpty(cellString))
        {
            return new Flag();
        }
        string[] ret = cellString.Split(',');
        Flag flag = new Flag();
        for (int i = 0; i < ret.Length; i++)
        {
            int t = Int32.Parse(ret[i]);
            flag.state = bit_add(flag.state, t);
        }
        return flag;
    }

    public bool bit_has(int state, int idx)
    {
        if (idx == 0) return false;
        return ((state >> (idx - 1)) & 1) > 0;
    }

    public int bit_add(int state, int idx)
    {
        if (idx == 0) return state;
        return state | (1 << (idx - 1));
    }

}

//public class WeightClass : LH.IWeight   //权重类
//{
//    public WeightClass() { }
//    public WeightClass(int[] param_, int idx_ = -1)
//    {
//        param = param_;
//        m_idx = idx_;
//    }

//    int m_idx;

//    public int weight { get => param[0]; }
//    public int id
//    {
//        get
//        {
//            if (param.Length > 1) return param[1];
//            return m_idx;
//        }
//    }
//    public int[] param;
//    int LH.IWeight.Weight { get => weight; }
//}








public class Flag
{
    public int state;
}



public static class ConfigExtend
{
    public static bool HasSkillFlag(this SkillConfigRow cfg, N_Avatar.SKILL_FLAG type)
    {
        return N_Avatar.AvatarHelper.HasSkillFlag(cfg, type);
    }


    /// <summary>
    /// 是否是普通攻击
    /// </summary>
    /// <param name="cfg"></param>
    /// <returns></returns>
    public static bool IsOrdinaryAttack(this SkillConfigRow cfg)
    {
        return N_Avatar.AvatarHelper.HasSkillFlag(cfg, N_Avatar.SKILL_FLAG.ORDINARY_ATTACK);
    }


    public static bool HasBuffEffectType(this BuffConfigRow cfg, BUFF_EFFECT_TYPE type)
    {
        return N_Avatar.AvatarHelper.HasBuffEffectType(cfg, type);
    }

    /// <summary>
    /// 是否是对应的 状态效果
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static  bool IsStateEffect(this BuffConfigRow cfg, BUFF_STATE_EFFECT type)
    {
        return AvatarHelper.HasBuffEffectType(cfg, BUFF_EFFECT_TYPE.STATE)
            && AvatarHelper.HasBuffStateEffectType(cfg, type);
    }


    public static bool IsDebuff(this BuffConfigRow cfg)
    {
        return cfg.flag == 2;
    }
}