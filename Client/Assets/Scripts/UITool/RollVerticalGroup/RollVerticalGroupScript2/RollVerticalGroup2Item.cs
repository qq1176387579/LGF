using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollVerticalGroup2Item : MonoBehaviour
{
    public string itemType;

    public int CurID { get => curID; }

    int curID;
    Action<int> onRefreshData;
    private void Awake()
    {
        //EventManager.Instance.BroadCastEvent(GameEventType.NewRollVerticalGroup2Item, this, gameObject);
        //EventManager.Instance.BroadCastEvent(GameEventType.NewRollVerticalGroup2Item, this);    //之前这里报错调用后那边热更代码的 gameobject为空现在又没问题了？
    }


    public void RegisterRefreshData(System.Action<int> _ac)
    {
        onRefreshData = _ac;
    }

    public void RefreshData(int idx)
    {
        curID = idx;
        onRefreshData?.Invoke(idx);
    }

}
