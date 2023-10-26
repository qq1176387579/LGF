/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2023/10/25 13:13:39
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using Protocol;

public class CheckCharacterPositionRequest : BaseRequest<CheckCharacterPositionRequest>
{
    public override OperationCode OpCode => OperationCode.InformServer_CheckCharacterPosition;



    public void DefaultRequest(List<PlayerUnit> players, ulong CurFrame)
    {
        //base.DefaultRequest();
        InformServer_CheckCharacterPosition data = new InformServer_CheckCharacterPosition();
        data.curFrame = CurFrame;

        for (int i = 0; i < players.Count; i++) {

            InformServer_CheckCharacterPosition.playerInfo playerInfo = new InformServer_CheckCharacterPosition.playerInfo();
            data.playerinfos.Add(players[i].playerid, playerInfo);
            playerInfo.playerID = players[i].playerid;
            playerInfo.LogicPos.Copy(players[i].LogicPos.ConvertViewVector3());
            playerInfo.InputDir.Copy(players[i].InputDir.ConvertViewVector3());
        }

        Send(data);

    }


    public override void OnOperationResponse(S2C_GameMsg msg)
    {
        //改协议没有返回值
    }
}
