/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/13 3:25:46
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;
using UnityEngine.UI;

public class RoomInfoItem : MonoBehaviour
{
    public uint playerID;
    public Text name, ready;
    public Transform houseOwner;

    public Text loadPrg;     //加载进度值
    public GameObject loadParent;   //加载的父节点


    public void SetReady(bool f)
    {
        ready.text =  f? "准备" : "未准备";
    }

    public void SetHouseOwner(ulong uid)
    {
        houseOwner.gameObject.SetActive(uid == playerID);
    }


    public void SetPrg(int val)
    {
        Debug.Log("--SetPrg-" + val);
        loadParent.gameObject.SetActive(true);
        if (val == -1)
        {
            val = 100;
        }
        loadPrg.text = StringPool.Concat("加载", val.ToString(), "%");

    }
}
