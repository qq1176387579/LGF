/*************************************************
	作者: Plane
	邮箱: 1785275942@qq.com	
	功能: 基础表现控制

    //=================*=================\\
           教学官网：www.qiqiker.com
           官方微信服务号: qiqikertuts
           Plane老师微信: PlaneZhong
               ~~获取更多教学资讯~~
    \\=================*=================//
*************************************************/

using LGF;
using UnityEngine;

public abstract class ViewUnit : MonoBehaviour {
    //Pos
    public bool IsSyncPos;
    public bool PredictPos; //预测点
    public int PredictMaxCount; //预测最大数量
    public bool SmoothPos;      //平滑
    public float viewPosAccer;  //平滑加速

    //Dir
    public bool IsSyncDir;
    public bool SmoothDir;  
    public float viewDirAccer;      //视图方向加速值   
    public float AngleMultiplier;   //角度赔率  角度相差的值乘以这个倍率 

    public Transform RoationRoot;

    int predictCount;
    protected Vector3 viewTargetPos;
    protected Vector3 viewTargetDir;


    LogicUnit logicUnit = null;

    public virtual void Init(LogicUnit logicUnit) {
        this.logicUnit = logicUnit;
        //gameObject.name = logicUnit + "_" + gameObject.name;

        transform.position = logicUnit.LogicPos.ConvertViewVector3();   

        if (RoationRoot == null)
        {
            RoationRoot = transform;
        }
        RoationRoot.rotation = CalcRotation(logicUnit.LogicDir.ConvertViewVector3());
    }

    protected virtual void Update() {
        if(IsSyncDir) {
            UpdateDirection();
        }

        if(IsSyncPos) {
            UpdatePosition();
        }
    }

    void UpdateDirection() {
        if (logicUnit.isDirChanged)
        {
            viewTargetDir = GetUnitViewDir();
            logicUnit.isDirChanged = false;
        }
        if (SmoothDir)
        {
            float threshold = Time.deltaTime * viewDirAccer;
            float angle = Vector3.Angle(RoationRoot.forward, viewTargetDir);
            float angleMult = (angle / 180) * AngleMultiplier * Time.deltaTime; //计算角度倍率值 角度越大 值越大  加速的权重值也越大

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

        if (PredictPos) ////是否开启预测
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
                if (predictCount > PredictMaxCount) //预测最好做最大数量限制。  不然无线预测下去了 这里最大预测15次
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
            if (SmoothPos)
            {
                //这里平滑加速是viewPosAccer = 10
                transform.position = Vector3.Lerp(transform.position, viewTargetPos, Time.deltaTime * viewPosAccer);
            }
            else
            {
                transform.position = viewTargetPos; //没有开启的时候
            }
        }
        else
        {
            //无平滑无预测，强制每帧刷新逻辑层的位置
            transform.position = logicUnit.LogicPos.ConvertViewVector3();
        }
    }


    /// <summary>
    /// 后续不同对象  override 他
    /// </summary>
    /// <returns></returns>
    protected virtual Vector3 GetUnitViewDir() {

        return logicUnit.LogicDir.ConvertViewVector3(); //这个方向是经过物理引擎修改后的方向
        //return  player.InputDir.ConvertViewVector3(); ;   //可以是用原来的方向
    }


    protected Quaternion CalcRotation(Vector3 targetDir) {
        return Quaternion.FromToRotation(Vector3.forward, targetDir);   //类似Quaternion.lookat
    }

    public abstract void PlayAni(string aniName);

    public virtual void PlayAudio(string audioName, bool loop = false, int delay = 0) {
       // AudioSvc.Instance.PlayEntityAudio(audioName, GetComponent<AudioSource>(), loop, delay);
    }
}
