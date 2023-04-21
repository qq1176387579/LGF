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
    public string serverID;   //获得服务器链接
    //192.168.31.241:55687

    public GameObject nextPanel;

    public Transform contentRoot;
    public TestSelectItem prefabItem;
    public Button refreshBtn, CreateButton;


    float refreshTime = 0;

    //List<EndPoint> serverList = new List<EndPoint>();
    List<TestSelectItem> selectItems = new List<TestSelectItem>();

    // Start is called before the first frame update
    void Start()
    { 
        //EventManager.Instance.AddListener<EndPoint>(GameEventType.Net_GetServersInfo, (point) =>
        //{
        //    serverList.Add(point);
        //    sLog.Debug("Program 获得服务器信息 {0}", point);
        //    Refresh();
        //});

        //获得服务器上全部信息
        refreshBtn.onClick.AddListener(() =>
        {
            if (refreshTime > Time.time)
            {
                sLog.Debug("刷新中, 无法重复点击");
                return;
            }

            refreshTime = Time.time + 3f;
            C_ModuleMgr.GetModule<C_RoomModuble>().GetAllTheRoomsInfo();
        });

        //创建服务器房间房间
        CreateButton.onClick.AddListener(() =>
        {

            C_ModuleMgr.GetModule<C_RoomModuble>().CreateRoom();
            //S_ModuleMgr.Instance.Init();
            //var t = new IPEndPoint(LGF.Net.SocketHelper.GetLocalIPv4_IPAddress(), S_ModuleMgr.Instance.Server.LocalPort);
            //JoinServer(t);
        });


        EventManager.Instance.AddListener(GameEventType.ClientEvent_CreateRoomSucceed, OnJionRoom);

        EventManager.Instance.AddListener<List<CMD_SimpleRoomInfo>>(GameEventType.ClientEvent_GetAllRooms, Refresh);
        EventManager.Instance.AddListener(GameEventType.ClientEvent_JionRoom, OnJionRoom);
    }


    private void OnDestroy()
    {
        EventManager.Instance.RemoveListerner(GameEventType.ClientEvent_CreateRoomSucceed, OnJionRoom);

        EventManager.Instance.RemoveListerner<List<CMD_SimpleRoomInfo>>(GameEventType.ClientEvent_GetAllRooms, Refresh);
        EventManager.Instance.RemoveListerner(GameEventType.ClientEvent_JionRoom, OnJionRoom);
    }


    void Refresh(List<CMD_SimpleRoomInfo> list)
    {
        if (list == null)
        {
            return;
        }

        if (list.Count == 0)
        {
            for (int j = 0; j < selectItems.Count; j++)
            {
                selectItems[j].gameObject.SetActive(false);
            }
            return;
        }

        int i;
        for (i = 0; i < list.Count; i++)
        {
            if (selectItems.Count <= i)
            {
                selectItems.Add(GameObject.Instantiate<TestSelectItem>(prefabItem, contentRoot));
                selectItems[i].itemEvent = SelectEvent;
            }

            TestSelectItem item = selectItems[i];
            item.gameObject.SetActive(true);
            item.Init(list[i].roomID, list[i].roomName);
        }


        for (; i < selectItems.Count; i++)
        {
            selectItems[i].gameObject.SetActive(false);
        }

    }


    void SelectEvent(uint roomid)
    {
        JoinServer(roomid);
    }


    void JoinServer(uint roomid)
    {
        //进入服务器
        sLog.Debug(" SelectEvent roomid: {0}", roomid);
        C_ModuleMgr.GetModule<C_RoomModuble>().JionRoom(roomid);


        //C_ModuleMgr.Instance.Client.TryToConnect(NetTestEntry3.Instance.name, endPoint);
        //C_ModuleMgr.GetModule<C_LoginModule>().ConnectServer(NetTestEntry3.Instance.name, endPoint, () =>
        //{
        //    nextPanel?.gameObject.SetActive(true);
        //    gameObject.SetActive(false);
        //});
    }

    void OnJionRoom()
    {
        nextPanel?.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }



}
