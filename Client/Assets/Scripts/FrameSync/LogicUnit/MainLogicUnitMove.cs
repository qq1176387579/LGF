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
        envColliderList = GameSceneMgr.Instance.GetEnvColliders();

        //LogicPos = ud.bornPos;    初始位置
        //LogicMoveSpeed = ud.unitCfg.moveSpeed;    初始速度

        //暂时先放这里  后面工厂模式 或者 策略方法
        collider = new PECylinderCollider(new ColliderConfig
        {
            mType = ColliderType.Cylinder,
            mRadius = (PEInt)0.5f
        });

        collider.mPos = LogicPos;
    }

    void TickMove()
    {
        
        PEVector3 moveDir = InputDir;
        //暂时先这样  后面去看资料  我记得客户端要比服务端 快1倍的帧率
        collider.mPos += moveDir * LogicMoveSpeed * (PEInt)Configs.ClientLogicFrameDeltaSec;
        //sLog.Debug("collider.mPos " + collider.mPos);
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

        //sLog.Debug("{0} ", GameSceneMgr.Instance.CurFrame);
        //sLog.Debug("{0} ", testuid);
        //sLog.Debug("{0} ", LogicPos);
        //sLog.Debug("{0} ", moveDir);

        //sLog.DebugAndSave("{0} LogicPos playerid:{1} :  pos: {2}  moveDir: {3}", GameSceneMgr.Instance.CurFrame, testuid, LogicPos, moveDir);
    }


    public void InputMoveKey(PEVector3 dir)
    {
        InputDir = dir;
        //sLog.Debug("InputDir:" + dir.ConvertViewVector3());
    }

}
