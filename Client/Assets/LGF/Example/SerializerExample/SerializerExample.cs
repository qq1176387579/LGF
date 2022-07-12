/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/2 20:39:10
/// 功能描述:  测试序列化的正确性   string类型 gc没有解决方案
****************************************************/

//#define TestSerializer


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using LGF;
using LGF.Log;
using UnityEngine;
using LGF.Util;
using LGF.Serializable;
using UnityEditor;


#region TestSerializer

//打开菜单栏的 
//数据工具/SteamSerializable/Generated
//自动生成下列程序

#if TestSerializer

//序列化成员
public class stttBase<T> : Poolable<T>, ISerializer where T : stttBase<T>, new()
{
    [LGF.Serializable.SteamMember(0)]
    public int tfff;

    public virtual void Deserialize(LGFStream stream)
    {

    }

    public virtual void Serialize(LGFStream stream)
    {

    }
}


[SteamContract]
public partial class sttt : stttBase<sttt>
{
    [LGF.Serializable.SteamMember(1)]    //形成流的顺序
    public long t;
    [LGF.Serializable.SteamMember(4)]    //形成流的顺序
    public bool bo;
    [LGF.Serializable.SteamMember(2)]
    public List<int> list;
    [LGF.Serializable.SteamMember(2)]
    public List<string> list2;


    [LGF.Serializable.SteamMember(2)]
    public List<sttt> list4;

    [LGF.Serializable.SteamMember(10)]
    public sttt f;
    [LGF.Serializable.SteamMember(3)]
    public Dictionary<int, string> dict = new Dictionary<int, string>();

    [LGF.Serializable.SteamMember(11)]
    public Dictionary<string, sttt> dict1 = new Dictionary<string, sttt>();

    [LGF.Serializable.SteamMember(3)]
    public string s;


    public override string ToString()
    {
        string str = "";
        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                str += list[i] + "  ";
            }
        }

        string str1 = "";
        if (list4 != null)
        {
            for (int i = 0; i < list4.Count; i++)
            {
                str1 += list4[i].ToString() + "  ";
            }
        }


        string st2 = "";
        foreach (var item in dict)
        {
            st2 += $" [key: {item.Key}-val {item.Value}]";
        }

        string st3 = "";
        foreach (var item in dict1)
        {
            st2 += $" [key: {item.Key}-val {item.Value}]";
        }

        //return String.Format($" t: {t} | bo: {bo} | s : {s} | tfff : {tfff} | list{str} f:[ {f} ]   list4 [ {str1} ]  dic: [ {st2} ]   dic1: [ {st3} ]");
        return String.Format($" t: {t} | bo: {bo} | s : {s} | tfff : {tfff} | dic: [ {st2} ]   dic1: [ \n{st3} ]");
        //return String.Format($" t: {t} | bo: {bo} | s : {s} | tfff : {tfff} | list{str} ");
    }


}


#endif

#endregion



namespace LGF.Serializable
{
    public static class SerializerTestClass
    {

        public static void PP()
        {

#if TestSerializer

            sttt s = sttt.Get();
            s.tfff = 1;
            s.bo = false;
            s.s = "你好";
            s.t = 2;
            s.list = new List<int>();
            s.list.Add(1);
            s.list.Add(5);
            s.f = sttt.Get();
            s.f.s = " 我的";
            //s.list4 = new List<sttt>();
            //s.list4.Add(sttt.Get());
            //s.list4.Add(sttt.Get());
            //s.list4[0].s = " list1 ";
            //s.list4[1].s = " list2 ";

            s.dict = new Dictionary<int, string>();
            s.dict.Add(1, " dic_1");
            s.dict.Add(3, " dic_3");

            s.dict1 = new Dictionary<string, sttt>();
            sttt s7 = sttt.Get();
            sttt s8 = sttt.Get();
            sttt s9 = sttt.Get();
            s7.s = "s7";
            s8.s = "s8";
            s9.s = "s9";


            s8.dict1 = new Dictionary<string, sttt>();
            s8.dict1.Add("s8_44", s9);


            s.dict1.Add("33", s7);
            s.dict1.Add("44", s8);

            sLog.Debug(s);


            LGF.Serializable.LGFStream stream = new LGF.Serializable.LGFStream();
            s.Serialize(stream);
            sttt s1 = sttt.Get();
            sLog.Debug(s1);
            s1.Deserialize(stream);
            sLog.Debug(s1);

#endif


        }

    }



    public class SerializerExample : MonoBehaviour
    {
        private void Start()
        {
            SerializerTestClass.PP();
        }
    }

}








