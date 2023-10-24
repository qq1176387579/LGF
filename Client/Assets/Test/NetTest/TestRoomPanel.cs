/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/13 3:24:12
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;
using UnityEngine.UI;

public class TestRoomPanel : MonoBehaviour
{

    public Transform contentRoot;
    public TestChatItemButton prefabItem;

    List<TestChatItemButton> chatItems = new List<TestChatItemButton>();

    public InputField sendMsg;
    public Button sendBtn, ReadyBtn;

    public Transform playersContent;
    public RoomInfoItem roomprefabItem;
    public Text ReadyBtnText;

    ChatModule ChatModule;


    List<RoomInfoItem> roomInfos = new List<RoomInfoItem>();
    public Dictionary<uint, RoomInfoItem> roomDic = new Dictionary<uint, RoomInfoItem>();
    int curUserCount;

    RoomInfoItem houseOwner;

    bool curReady = false;

    // Start is called before the first frame update
    void Start()
    {
        //EventManager.Instance.AddListener()

        ChatModule = ModuleMgr.GetModule<ChatModule>();

    

        sendBtn.onClick.AddListener(() =>
        {
            if (sendMsg.text.IsNullOrEmpty())
            {
                return;
            }

            ChatModule.SendMsgToRoom(sendMsg.text);

            sendMsg.text = "";
        });


        ReadyBtn.onClick.AddListener(() =>
        {
            ModuleMgr.GetModule<RoomModuble>().SetReady(!curReady);
        });

        Refresh();
        //EventManager.Instance.AddListener();
        EventManager.Instance.AddListener<uint, int, string>(GameEventType.ClientEvent_RoomOptSync, OnRoomOptSync);
        EventManager.Instance.AddListener(GameEventType.ClientEvent_StartLoadingScene, OnStartLoadingScene);
        EventManager.Instance.AddListener<S2C_RoomProgress>(GameEventType.ClientEvent_RoomProgress, OnRoomProgress);
        EventManager.Instance.AddListener<string, string>(GameEventType.ClientEvent_RoomChatMsg, OnTextMsg);
        //    EventManager.Instance.BroadCastEvent(GameEventType.ClientEvent_RoomOptSync, msg.playerID, optType);  //有玩家加入房间
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListerner<uint, int, string>(GameEventType.ClientEvent_RoomOptSync, OnRoomOptSync);
        EventManager.Instance.RemoveListerner(GameEventType.ClientEvent_StartLoadingScene, OnStartLoadingScene);
        EventManager.Instance.RemoveListerner<S2C_RoomProgress>(GameEventType.ClientEvent_RoomProgress, OnRoomProgress);
        EventManager.Instance.RemoveListerner<string, string>(GameEventType.ClientEvent_RoomChatMsg, OnTextMsg);
    }

    void OnStartLoadingScene()
    {
        roomDic.Clear();
        foreach (var item in roomInfos)
        {
            roomDic.Add(item.playerID, item);
            item.SetPrg(0);
        }

        

    }

    void OnRoomProgress(S2C_RoomProgress msg)
    {
        for (int i = 0; i < msg.list.Count; i++)
        {
            roomDic.TryGetValue(msg.list[i].uid, out var info);
            if (info == null)
            {
                sLog.Debug("非法操作 uid： {0} ", msg.list[i].uid);
            }
            else
            {
                info.SetPrg(msg.list[i].progress);
            }
        }
        
    }





    // Update is called once per frame

    void OnRoomOptSync(uint playerID, int optType, string name)
    {
        sLog.Debug("=--OnRoomOptSync---playerID: " + playerID + " optType: " + optType + "");
        switch (optType)
        {
            case 1: //加入
            case 2: //离开
                OnTextMsg("系统 :", StringPool.Concat(name, optType == 1 ? "加入房间" : "离开房间"));
                Refresh();
                break;
            case 3: //准备
            case 4: //取消准备
                roomInfos.FirstFunc((item, _playerID, _optType) =>
                {
  
                    bool f = item.playerID == _playerID;
                    Debug.Log($"--> {f} { item.playerID }  {_playerID}  {_optType}");
                    if (f)
                    {
                        item.SetReady(_optType == 3);
                    }
                    return f;
                }, playerID, optType);

                if (playerID == ModuleMgr.Instance.Player.uid)    //如果是自己
                {
                    SetReadyBtn(optType == 3);
                }
                break;
            case 5://交换房主
                houseOwner.SetHouseOwner(playerID);
                houseOwner = roomInfos.FirstFunc((item, _playerID) =>
                {
                    bool f = item.playerID == _playerID;
                    if (f)
                        houseOwner.SetHouseOwner(_playerID);
                    return f;
                }, playerID);

             
                break;
            default:
                break;
        }

    }



    void Refresh()
    {
        var tmp = RoomManager.Instance.GetRoomUsersInfo();
        uint houseOwnerID = RoomManager.Instance.GetHouseOwnerID();
        List<CMD_UserRoomInfo> usersInfos = new List<CMD_UserRoomInfo>();
        foreach (var item in tmp)
        {
            usersInfos.Add(item.Value);
        }

        usersInfos.Sort((a,b) =>
        {
            return a.roomjoinRank.CompareTo(b.roomjoinRank);
        });

        int i;
        for (i = 0; i < usersInfos.Count; i++)
        {
            var info = usersInfos[i];
            if (i >= roomInfos.Count)
            {
                roomInfos.Add(Instantiate<RoomInfoItem>(roomprefabItem, playersContent));
            }
            RoomInfoItem item = roomInfos[i];
            item.name.text = info.useinfo.name;
            item.SetReady(info.ready);
            item.playerID = info.useinfo.uid;
            item.SetHouseOwner(houseOwnerID);
            if (houseOwnerID == item.playerID)
                houseOwner = roomInfos[i]; 
            
            item.gameObject.SetActive(true);
            item.loadParent.SetActive(false);

            if (item.playerID == ModuleMgr.Instance.Player.uid)    //如果是自己
            {
                SetReadyBtn(info.ready);
                //Debug.LogError("-------fff--");
            }
        }

        curUserCount = i;
        for (int j = roomInfos.Count - 1; j >= curUserCount; j--)
        {
            roomInfos[j].gameObject.SetActive(false);   //关闭
        }

    }


    void OnTextMsg(string name, string msg)
    {
        chatItems.Add(GameObject.Instantiate<TestChatItemButton>(prefabItem, contentRoot));
        chatItems[chatItems.Count - 1].Init($"\n {name} : {msg}\n");
        chatItems[chatItems.Count - 1].gameObject.SetActive(true);
    }


    void SetReadyBtn(bool f)
    {
        curReady = f;
        ReadyBtnText.text = !f ? "准备" : "取消准备";
        Debug.Log("  isReady" + curReady);
    }
}
