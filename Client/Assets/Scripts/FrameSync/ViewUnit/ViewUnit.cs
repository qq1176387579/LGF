/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/30 18:14:35
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using UnityEngine;

public abstract class ViewUnit 
{

    public ViewUnitData info;   //基础

    public Transform RoationRoot;
    int predictCount;

    protected Vector3 viewTargetPos;
    protected Vector3 viewTargetDir;


    LogicUnit logicUnit = null;
    protected Transform _transform;

    public virtual void Init(LogicUnit logicUnit, Transform transform, ViewUnitConfig cfg)
    {
        info = cfg.data;  //

        _transform = transform;
        this.logicUnit = logicUnit;
        //gameObject.name = logicUnit + "_" + gameObject.name;

        transform.position = logicUnit.LogicPos.ConvertViewVector3();

        if (RoationRoot == null)
        {
            RoationRoot = transform;
        }
        RoationRoot.rotation = CalcRotation(logicUnit.LogicDir.ConvertViewVector3());
    }

    public virtual void OnUpdate()
    {
        if (info.IsSyncDir)
        {
            UpdateDirection();
        }

        if (info.IsSyncPos)
        {
            UpdatePosition();
        }
    }

    void UpdateDirection()
    {
        if (logicUnit.isDirChanged)
        {
            viewTargetDir = GetUnitViewDir();
            logicUnit.isDirChanged = false;
        }
        if (info.SmoothDir)
        {
            float threshold = Time.deltaTime * info.viewDirAccer;
            float angle = Vector3.Angle(RoationRoot.forward, viewTargetDir);
            float angleMult = (angle / 180) * info.AngleMultiplier * Time.deltaTime; //计算角度倍率值 角度越大 值越大  加速的权重值也越大

            if (viewTargetDir != Vector3.zero)
            {
                //角度赔率  角度相差的值乘以这个倍率   效果为赔率越高时 加速度转得越快
                Vector3 interDir = Vector3.Lerp(RoationRoot.forward, viewTargetDir, threshold + angleMult);
                RoationRoot.rotation = CalcRotation(interDir);
            }
        }
        else
        {
            RoationRoot.rotation = CalcRotation(viewTargetDir);
        }
    }

    void UpdatePosition()
    {
        //transform.position = logicUnit.LogicPos.ConvertViewVector3();   //

        if (info.PredictPos) ////是否开启预测
        {
            if (logicUnit.IsPosChanged)    //逻辑帧位置改变
            {
                //逻辑有Tick，目标位置更新到最新
                //更新到目标位置
                viewTargetPos = logicUnit.LogicPos.ConvertViewVector3();
                logicUnit.IsPosChanged = false;
                predictCount = 0;
            }
            else
            {
                if (predictCount > info.PredictMaxCount) //预测最好做最大数量限制。  不然无线预测下去了 这里最大预测15次
                {
                    return;
                }
                //逻辑未Tick，使用预测计算
                float delta = Time.deltaTime;
                //预测位置 = 逻辑速度*逻辑方向
                var predictPos = delta * logicUnit.LogicMoveSpeed.RawFloat * logicUnit.LogicDir.ConvertViewVector3();
                //新的目标位置 = 表现目标位置+预测位置
                viewTargetPos += predictPos;
                ++predictCount;
            }

            //平滑移动
            if (info.SmoothPos)
            {
                //这里平滑加速是viewPosAccer = 10
                _transform.position = Vector3.Lerp(_transform.position, viewTargetPos, Time.deltaTime * info.viewPosAccer);
            }
            else
            {
                _transform.position = viewTargetPos; //没有开启的时候
            }
        }
        else
        {
            //无平滑无预测，强制每帧刷新逻辑层的位置
            _transform.position = logicUnit.LogicPos.ConvertViewVector3();
        }
    }


    /// <summary>
    /// 后续不同对象  override 他
    /// </summary>
    /// <returns></returns>
    protected virtual Vector3 GetUnitViewDir()
    {

        return logicUnit.LogicDir.ConvertViewVector3(); //这个方向是经过物理引擎修改后的方向
        //return  player.InputDir.ConvertViewVector3(); ;   //可以是用原来的方向
    }


    protected Quaternion CalcRotation(Vector3 targetDir)
    {
        return Quaternion.FromToRotation(Vector3.forward, targetDir);   //类似Quaternion.lookat
    }

    public abstract void PlayAni(string aniName);

    public virtual void PlayAudio(string audioName, bool loop = false, int delay = 0)
    {
        // AudioSvc.Instance.PlayEntityAudio(audioName, GetComponent<AudioSource>(), loop, delay);
    }
}
