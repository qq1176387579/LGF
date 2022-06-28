using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Avatar
{
    /// <summary>
    /// 技能系统        这里处理一些技能cd等  加载所有技能的配置处理
    /// 数值方面的处理
    /// 后面显示部分方面需要用另外一套系统
    /// 该系统没做对象的检测    没有验证是否是同阵营的
    /// </summary>
    public class SkillSystem
    {
        public List<SkillInfo> skillInfos { get => _skillInfos;}
        List<SkillInfo> _skillInfos = new List<SkillInfo>();     //所有的技能

        Avatar owner;
        SkillInfo ordinaryTypeSkill;    //普通技能的
        public SkillSystem Init(Avatar owner_)
        {
            owner = owner_;
            return this;
        }


        public void LoadSkill(int[] skillIDs)
        {
            for (int i = 0; i < skillIDs.Length; i++)
            {
                LoadSkill(skillIDs[i], false);
            }

            SortSkillInfos();
        }

        /// <summary>
        /// 加载技能    简单写  后面有其他信息在添加
        /// </summary>
        public void LoadSkill(int skillID,bool isSort = true)
        {
            SkillConfigRow config = SkillConfig.GetByID(skillID);
            if (config == null)
            {
                UnityEngine.Debug.LogError($"Load skill config {skillID} is null");
                return;
            }
            SkillInfo info = SkillInfo.Get();
            info.config = config;
            skillInfos.Add(info);

            if (isSort)
                SortSkillInfos();

            if (config.cd_start != 0)
            {
                info.cdTime = config.cd - config.cd_start;  //初始cd回合
            }
            
        }

        /// <summary>
        /// 排序技能信息
        /// </summary>
        void SortSkillInfos()
        {
            skillInfos.Sort((info, info2) =>
            {
                return AvatarHelper.HasSkillFlag(info.config, SKILL_FLAG.ORDINARY_ATTACK) ? -1 : 1;
            });

            ordinaryTypeSkill = skillInfos.Find((info) =>
            {
                return AvatarHelper.HasSkillFlag(info.config, SKILL_FLAG.ORDINARY_SKILL);
            });
        }


        /// <summary>
        /// 删除技能
        /// </summary>
        /// <param name="skillID"></param>
        public void DelSkill(int skillID)
        {
            skillInfos.RemoveFunc((a) =>
            {
                bool f = a.config.id == skillID;
                if (f)
                {
                    a.Release();
                }
                return f;
            });
        }

        /// <summary>
        /// 释放技能关于手动(能否释放技能 外抛)
        /// </summary>
        public bool CanCastSkillByHand()
        {
            return CanCastSkill(ordinaryTypeSkill, null, true);
        }

        bool ordinaryTypeSkillFire = false;  //
        /// <summary>
        /// 释放技能关于手动()
        /// </summary>
        public bool PlayerSkillByHand()
        {
            if (!CanCastSkillByHand()) return false;

            ordinaryTypeSkillFire = true;   //开启释放技能

            return ordinaryTypeSkillFire;
            //CastSkillAutoTarget(ordinaryTypeSkill);
        }

        ///// <summary>
        ///// 外部接口 用来显示UI
        ///// </summary>
        ///// <returns></returns>
        //public  bool IsSkillCDByUI()
        //{
        //    SkillInfo skillInfo = ordinaryTypeSkill;

        //    if (owner.IsDeath())
        //        return false;

        //    if (skillInfo == null)
        //        return false;

        //    if (skillInfo.cdTime < skillInfo.config.cd)    //cd完成
        //    {
        //        return false;
        //    }

        //    if (!CastSkillCond(skillInfo.config.CastCond))
        //    {
        //        return false;
        //    }

        //    return true;
        //}


        /// <summary>
        /// 能否释放技能
        /// </summary>
        /// <param name="skillInfo"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        bool CanCastSkill(SkillInfo skillInfo, Avatar target = null,bool isHand = false)
        {
            if (owner.IsDeath())
                return false;

            if (skillInfo == null)
                return false;
 
            if (skillInfo.cdTime < skillInfo.config.cd)    //cd完成
            {
                return false;
            }

            //bool isOrdinaryAttack = skillInfo.config.IsOrdinaryAttack();

            //if (owner.buffSystem.GetStateEffect(BUFF_STATE_EFFECT.DIZZY) != null)       //眩晕不能操作
            //    return false;

            //if (!isOrdinaryAttack && !skillInfo.config.HasSkillFlag(SKILL_FLAG.PASSIVITY_SKILL))     //普通和被动技能 处理
            //{
            //    if (owner.buffSystem.GetStateEffect(BUFF_STATE_EFFECT.CHAOS) != null)   //混乱不能丢技能
            //        return false;

            //    if (owner.buffSystem.GetStateEffect(BUFF_STATE_EFFECT.SILENCE) != null) //沉默不能丢技能
            //        return false;

            //    if (owner.buffSystem.GetStateEffect(BUFF_STATE_EFFECT.BE_TAUNT) != null) //嘲讽不能丢技能
            //        return false;
            //}

            //if (!CastSkillCond(skillInfo.config.CastCond))
            //{
            //    return false;
            //}

            //if (skillInfo.config.HasSkillFlag(SKILL_FLAG.ORDINARY_SKILL))   //如果是普通技能
            //{
            //    //如果是非点击 且 熊猫不是自动释放 那么自动施法
            //    if (owner.IsPlayer())  //如果不是自动施法
            //    {
            //        return false;
            //        //return isHand || DuplicateSceneMgr.Instance.GetAuto() || ordinaryTypeSkillFire;
            //    }
            //}

            return true;
        }


        ///// <summary>
        ///// 能否释放技能  cd中 
        ///// </summary>
        ///// <param name="skillID"></param>
        ///// <param name="targetList"></param>
        //public bool CanCastSkill(int skillID, List<Avatar> targetList)
        //{
        //    var skillinfo = skillInfos.Find((a)=> a.config.id == skillID);

        //    if (skillinfo == null)  //能否释放技能
        //    {
        //        return false;
        //    }

        //    skillinfo.cdTime = 0;   //重新记cd时间

        //    return true;
        //}


        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="skillID"></param>
        void CastSkill(int skillID, List<Avatar> targetList) //对象
        {
           
            CombatCalcHelper.CastSkill(owner, skillID, targetList);

            //AvatarHelper.LogError("skillID : " + skillID);
        }

        public void OnTick()
        {
            skillInfos.ForEach((a) =>
            { a.cdTime++; }
         
            );  //cd回合增加

            //EventManager.Instance.BroadCastEvent(GameEventType.OnSkillCDRefersh, owner);
        }

        //目前没用上  如果后续需要回收的话 加上
        public void Clear()
        {
            skillInfos.ForEach((a) => a.Release()); 
            skillInfos.Clear();
        }


        #region 熊猫  释放技能

        internal bool AvatarCanCastSkillByHand()
        {
            return CanCastSkill(ordinaryTypeSkill);
        }

        ///// <summary>
        ///// 是否技能 关于手动
        ///// </summary>
        //internal void AvatarCastSkillByHand()
        //{
        //    if (!AvatarCanCastSkillByHand()) return;

        //      CastSkillAutoTarget(ordinaryTypeSkill);
        //}


        ///// <summary>
        ///// 释放技能  熊猫版本
        ///// </summary>
        ///// <param name="skillID"></param>
        //public void CastSkill()
        //{
        //    if (owner.IsDeath()) return;

        //    for (int i = 0; i < skillInfos.Count; i++)
        //    {
        //        if (CanCastSkill(skillInfos[i])) 
        //        {
        //            CastSkillAutoTarget(skillInfos[i]);
        //        }
        //    }
        //}


        //void CastSkillAutoTarget(SkillInfo info)
        //{
        //    info.cdTime = 0;
        //    if (info.config.HasSkillFlag(SKILL_FLAG.ORDINARY_SKILL))
        //        ordinaryTypeSkillFire = false;

        //    CastSkillAutoTarget(info.config);
        //}

        ///// <summary>
        ///// 内部使用 
        ///// </summary>
        ///// <param name="info"></param>
        //internal void CastSkillAutoTarget(SkillConfigRow config)
        //{
        //    //info
        //    List<Avatar> targets = CombatCheckHelper.ScreenOutSkillTarget(owner, config);
        //    if (targets == null)
        //    {
        //        UnityEngine.Debug.LogError(" error CastSkill  targets is null");
        //        return;
        //    }


        //    CastSkill(config.id, targets);
        //    targets.Release();  //回收
        //}

        ///// <summary>
        ///// 是否技能自动目标
        ///// </summary>
        //internal void CastSkillAutoTarget(int skillid)
        //{
        //    var config = SkillConfig.GetByID(skillid);
        //    if (config == null)
        //    {
        //        AvatarHelper.LogError($"skillid 没有技能配置 {skillid} ");
        //        return;
        //    }

        //    CastSkillAutoTarget(config);
        //}

        #endregion

        #region 减少CD回合

  

        #endregion


        #region 释放的条件

        //public bool CastSkillCond(int[][] param)
        //{
        //    if (param == null) return true; //默认成功

        //    for (int i = 0; i < param.Length; i++)
        //    {
        //        int[] paranVal = param[i];
        //        if (paranVal.GetByID(0) == 1)   //判定血量是否小于到达对应的血量
        //        {
        //            int val = paranVal.GetByID(1);    //血量值
        //            int compareType = paranVal.GetByID(2);    //比较值 -1 小与 0等于 1大于
        //            int hpPro = owner.GetHpPro();    //获得当前血量比例
        //            if (!AvatarHelper.Compare(hpPro, val, compareType))
        //            {
        //                return false;
        //            }
        //        }
        //        else if (paranVal.GetByID(0) == 2)
        //        {
        //            //AvatarHelper.LogError("  GetEnemyHaveDiedPos " + CombatCheckHelper.GetEnemyHaveDiedPos());
        //            if (CombatCheckHelper.GetEnemyHaveDiedPos() == -1)  //不存在空位
        //            {
        //                return false;
        //            }
        //        }
        //    }


        //    return true;
        //}


        #endregion
    }
}
