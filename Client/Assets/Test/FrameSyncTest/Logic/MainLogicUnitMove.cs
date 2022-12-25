/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/15 1:58:28
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using PEMath;
using PEPhysx;

public class Configs
{
    public const float ClientLogicFrameDeltaSec = 0.066f;
}

public partial class MainLogicUnit
{
    public PECylinderCollider collider;
    List<PEColliderBase> envColliderList;

    /// <summary>
    /// UI输入方向
    /// </summary>
    private PEVector3 inputDir;
    public PEVector3 InputDir { get=>inputDir; protected set => inputDir = value; }




    void InitMove()
    {
        envColliderList = FightMgr.Instance.GetEnvColliders();

        LogicPos = ud.bornPos;
        LogicMoveSpeed = ud.unitCfg.moveSpeed;
        
        collider = new PECylinderCollider(ud.unitCfg.colliCfg);
        collider.mPos = LogicPos;
    }

    void TickMove()
    {
        
        PEVector3 moveDir = InputDir;
        collider.mPos += moveDir * LogicMoveSpeed * (PEInt)Configs.ClientLogicFrameDeltaSec;
        PEVector3 adj = PEVector3.zero;
        try
        {
            collider.CalcCollidersInteraction(envColliderList, ref moveDir, ref adj);
        }
        catch (System.Exception e)
        {
            sLog.Debug("  " + e.ToString());
            throw;
        }
      

        if (LogicDir != moveDir)
        {
            LogicDir = moveDir;
        }
        if (LogicDir != PEVector3.zero)
        {
            LogicPos = collider.mPos + adj; //当前位置加上矫正值
        }
        collider.mPos = LogicPos;
    }


    public void InputMoveKey(PEVector3 dir)
    {
        InputDir = dir;
        sLog.Debug("InputDir:" + dir.ConvertViewVector3());
    }

}
