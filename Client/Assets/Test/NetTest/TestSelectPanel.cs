/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/27 18:26:30
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LGF;
using LGF.Log;
using System.Net;




public class TestSelectPanel : MonoBehaviour
{
    public GameObject nextPanel;

    public Transform contentRoot;
    public TestSelectItem prefabItem;
    public Button refreshBtn;
    float refreshTime = 0;

    List<EndPoint> serverList = new List<EndPoint>();
    List<TestSelectItem> selectItems = new List<TestSelectItem>();

    // Start is called before the first frame update
    void Start()
    { 
        EventManager.Instance.AddListener<EndPoint>(GameEventType.Net_GetServersInfo, (point) =>
        {
            serverList.Add(point);
            sLog.Debug("Program 获得服务器信息 {0}", point);

            Refresh();
        });

        refreshBtn.onClick.AddListener(() =>
        {
            if (refreshTime > Time.time)
            {
                sLog.Debug("刷新中, 无法重复点击");
                return;
            }

            refreshTime = Time.time + 3f;
            serverList.Clear();

            C_ModuleMgr.Instance.Client.GetAllServerInfo(); //获得全部服务器

            Refresh();
        });
    }


    void Refresh()
    {
     
        if (serverList.Count == 0)
        {
            for (int j = 0; j < selectItems.Count; j++)
            {
                selectItems[j].gameObject.SetActive(false);
            }
            return;
        }

        int i;
        for (i = 0; i < serverList.Count; i++)
        {
            if (selectItems.Count <= i)
            {
                selectItems.Add(GameObject.Instantiate<TestSelectItem>(prefabItem, contentRoot));
                selectItems[i].itemEvent = SelectEvent;
            }

            TestSelectItem item = selectItems[i];
            item.gameObject.SetActive(true);
            item.Init(i, serverList[i].ToString());
        }


        for (; i < selectItems.Count; i++)
        {
            selectItems[i].gameObject.SetActive(false);
        }

    }


    public void SelectEvent(int id)
    {
        sLog.Debug(" SelectEvent {0}", serverList[id].ToString());

        C_ModuleMgr.Instance.Client.TryToConnect(NetTestEntry3.Instance.name, serverList[id]);

        nextPanel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }




}
