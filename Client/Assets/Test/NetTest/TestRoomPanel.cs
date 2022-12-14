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

    C_ChatModule ChatModule;


    List<RoomInfoItem> roomInfos = new List<RoomInfoItem>();
    int curUserCount;

    RoomInfoItem houseOwner;

    bool curReady = false;

    // Start is called before the first frame update
    void Start()
    {
        //EventManager.Instance.AddListener()

        ChatModule = C_ModuleMgr.GetModule<C_ChatModule>();

        EventManager.Instance.AddListener<string, string>(GameEventType.ClientEvent_RoomChatMsg, OnTextMsg);

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
            C_ModuleMgr.GetModule<C_RoomModuble>().SetReady(!curReady);
        });

        Refresh();
        //EventManager.Instance.AddListener();
        EventManager.Instance.AddListener<uint, int, string>(GameEventType.ClientEvent_RoomOptSync, OnRoomOptSync);
        //    EventManager.Instance.BroadCastEvent(GameEventType.ClientEvent_RoomOptSync, msg.playerID, optType);  //有玩家加入房间
    }

    // Update is called once per frame

    void OnRoomOptSync(uint playerID, int optType, string name)
    {
        sLog.Debug("=--OnRoomOptSync---playerID: " + playerID + " optType: " + optType + "");
        switch (optType)
        {
            case 1: //加入
            case 2: //离开
                var info = C_RoomManager.Instance.GetUserInfo(playerID);
                OnTextMsg("系统 :", StringPool.Concat(info.useinfo.name, "加入房间"));
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

                if (playerID == C_ModuleMgr.Instance.Player.uid)    //如果是自己
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
        var tmp = C_RoomManager.Instance.GetRoomUsersInfo();
        uint houseOwnerID = C_RoomManager.Instance.GetHouseOwnerID();
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
            
            roomInfos[i].gameObject.SetActive(true);

            if (item.playerID == C_ModuleMgr.Instance.Player.uid)    //如果是自己
            {
                SetReadyBtn(info.ready);
                Debug.LogError("-------fff--");
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
