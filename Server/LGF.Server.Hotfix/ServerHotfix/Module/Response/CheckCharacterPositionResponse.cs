using LGF.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LGF.Log;
using Protocol;



namespace LGF.Server.Hotfix
{

    internal class RoomCheckPos
    {
        public uint roomID;
        public ulong lastFrame = 0;
        public ulong endFrame = 0;  
        public Dictionary<ulong, Dictionary<uint, InformServer_CheckCharacterPosition.playerInfo>> frameInfo = new Dictionary<ulong, Dictionary<uint, InformServer_CheckCharacterPosition.playerInfo>>();

        public void CheckCout()
        {
            //1000帧清楚 500帧
            if (endFrame - lastFrame < 1000) {
                return; //差值小于500帧 不做处理
            }

            ulong len = endFrame - 500;
            for (ulong i = lastFrame; i < len; i++) {
                if (frameInfo.ContainsKey(i)) { //存在清楚
                    frameInfo.Remove(i);
                }
            }
           
        }

    }


    internal class CheckCharacterPositionResponse : BaseResponse
    {

        Dictionary<uint, RoomCheckPos> roomInfoDic = new Dictionary<uint, RoomCheckPos>();

        //测试数据
        public override OperationCode OpCode => OperationCode.InformServer_CheckCharacterPosition;

        public override void Init()
        {
            base.Init();
            //EventManager.Instance.AddListener();
            HotfixEventMgr.Instance.AddListener<uint>(HotfixEventType.DelRoom, OnDelRoom);  //不需要卸载 因为中心加载 的时候这个类直接清空
            LGFEntry.RegisterOnUpdate(OnUpdate);
        }

        private void OnUpdate()
        {
            foreach (var item in roomInfoDic) {
                item.Value.CheckCout();
            }
        }

        private void OnDelRoom(uint roomid)
        {
            roomInfoDic.Remove(roomid);
        }

        //Dictionary<int, string> _characterPositions;
        public override void OnOperationRequest(KcpServer.KcpSession session, string msg)
        {
            var data = msg.ToObjcet<InformServer_CheckCharacterPosition>();
            var player = PlayerManager.Instance.GetPlayer(session);

            if (!roomInfoDic.TryGetValue(player.roomid, out var roomInfo)) {
                roomInfo = new RoomCheckPos();
                roomInfoDic[player.roomid] = roomInfo;
                roomInfo.roomID = player.roomid;
                roomInfo.lastFrame = roomInfo.lastFrame = data.curFrame;
                
            }
            var frameInfo = roomInfo.frameInfo;
           
            //data.curFrame
            var dic = data.playerinfos.GetDic();
            if (frameInfo.TryGetValue(data.curFrame, out var curFrameData)) {
                foreach (var pair in dic) {
                    if (curFrameData.TryGetValue(pair.Key, out var keyPlayerInfo)) {

                        if (!keyPlayerInfo.LogicPos.ValEquals(pair.Value.LogicPos)) {
                            this.Debug($"frame<{data.curFrame}> session.useid<{session.useid}> playeid:<{pair.Value.playerID}> <LogicPos>数据不相等 msg:{pair.Value.LogicPos} server:{keyPlayerInfo.LogicPos}");
                            return;
                        }


                        if (!keyPlayerInfo.InputDir.ValEquals(pair.Value.InputDir)) {
                            this.Debug($"frame<{data.curFrame}> session.useid<{session.useid}> playeid:<{pair.Value.playerID}>  <InputDir>数据不相等 msg:{pair.Value.InputDir} server:{keyPlayerInfo.InputDir}");
                            return;
                        }

                    }
                    else {
                        //string str = curFrameData.Debug("");
                        string str = "";
                        foreach (var item in curFrameData) {
                            str += $"k:<{item.Key}>\t"; 
                        }
                        this.Debug($"frame<{data.curFrame}> session.useid<{session.useid}> 出错 没有该ID<{pair.Key}>的数据 {str}");
                        return;
                    }
                }
            }
            else {
                frameInfo.Add(data.curFrame, dic);
                if (data.curFrame > roomInfo.endFrame) {
                    roomInfo.endFrame = data.curFrame;
                }
               
            }
        }


        public override void Close()
        {
            base.Close();
            LGFEntry.UnRegisterOnUpdate(OnUpdate);
        }
    }
}
