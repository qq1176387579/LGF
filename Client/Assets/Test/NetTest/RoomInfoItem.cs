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


    public void SetReady(bool f)
    {
        ready.text =  f? "准备" : "未准备";
    }

    public void SetHouseOwner(ulong uid)
    {
        houseOwner.gameObject.SetActive(uid == playerID);
    }
}
