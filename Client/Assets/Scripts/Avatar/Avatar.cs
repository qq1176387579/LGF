
using N_Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LGF;

namespace N_Avatar
{
    /// <summary>
    /// 后面需要添加一个引用池  如果该对象没有引用就应该回收 因为Avatar死亡后 需要处理
    /// 回收后 要进行对应的回收处理
    /// </summary>
    public class Avatar : Poolable<Avatar>
    {
        //public int avatarType { get; protected set; }
        public ulong uniqueID { get; protected set; }
        public AvatarAttr finalAttr { get; protected set; }
        public BuffSystem buffSystem { get; protected set; }
        public SkillSystem skillSystem { get; protected set; }


        public Avatar()
        {
            uniqueID = AvatarHelper.GenUniqueID();
            finalAttr = new AvatarAttr();
            buffSystem = new BuffSystem().Init(this);
            skillSystem = new SkillSystem().Init(this);
        }


        /// <summary>
        /// 修改最终属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="val"></param>
        public void SetAttrByType(ATTR_TYPE type, int val)
        {
            finalAttr.SetAttrByType(type, val);
        }

        public int GetAttrByType(ATTR_TYPE type)
        {
            return finalAttr.GetAttrByType(type);
        }


        /// <summary>
        /// 添加属性 不推荐修改系统属性 如攻击力 防御等  这些走系统属性改变流程
        /// 一般作用于 血量等东西
        /// </summary>
        /// <param name="type"></param>
        /// <param name="val"></param>
        public void AddAttrByType(ATTR_TYPE type, int val)
        {
            SetAttrByType(type, GetAttrByType(type) + val);
        }

        public void ChangeSystemAttr(ATTR_SYS sysType, Attr tAttr)
        {
            finalAttr.ChangeSystemAttr(sysType, tAttr);
        }


        public void ChangeSystemAttr(ATTR_SYS sysType, Dictionary<int, int> tAttr)
        {
            if (tAttr == null)
                return;

            finalAttr.ChangeSystemAttr(sysType, tAttr);
        }


        /// <summary>
        /// 万分比
        /// </summary>
        /// <returns></returns>
        public int GetHpPro()
        {
            return (int)((long)GetAttrByType(ATTR_TYPE.HP) * 10000 / GetAttrByType(ATTR_TYPE.MAXHP));
        }



        ///// <summary>
        /// 改变血量
        /// </summary>
        public void ChangeHP(int val)
        {
            if (val == 0) return;

            if (GetAttrByType(ATTR_TYPE.HP) + val > GetAttrByType(ATTR_TYPE.MAXHP))
            {
                SetAttrByType(ATTR_TYPE.HP, GetAttrByType(ATTR_TYPE.MAXHP));
            }
            else if (GetAttrByType(ATTR_TYPE.HP) + val < 0)
            {
                SetAttrByType(ATTR_TYPE.HP, 0);
            }
            else
            {
                AddAttrByType(ATTR_TYPE.HP, val);
            }
            finalAttr.CalClossHp();
        }



        /// <summary>
        /// 改变属性值 检测最大值最小值
        /// </summary>
        public bool ChangeVal(ATTR_TYPE type, ATTR_TYPE maxType, int val)
        {
            if (GetAttrByType(type) == GetAttrByType(maxType) && val > 0) return false;

            if (GetAttrByType(type) + val > GetAttrByType(maxType))
            {
                SetAttrByType(type, GetAttrByType(maxType));
            }
            else if (GetAttrByType(type) + val < 0)
            {
                SetAttrByType(type, 0);
            }
            else
            {
                AddAttrByType(type, val);
            }
            return true;
        }

        /// <summary>
        /// 是否死亡
        /// </summary>
        /// <returns></returns>
        public bool IsDeath()
        {
            return GetAttrByType(ATTR_TYPE.HP) <= 0;
        }

        /// <summary>
        /// 是否是怪物
        /// </summary>
        /// <returns></returns>
        public virtual bool IsMonster()
        {
            return finalAttr[N_Attribute.ATTR_TYPE.AVATAR_TYPE] == (int)AVATAR_TYPE.MONSTER;     //怪物;
        }

        /// <summary>
        /// 是否是玩家
        /// </summary>
        /// <returns></returns>
        public virtual bool IsPlayer()
        {
            return finalAttr[N_Attribute.ATTR_TYPE.AVATAR_TYPE] == (int)AVATAR_TYPE.PLAYER;     //玩家;;
        }







        ///// <summary>
        ///// 传输技能ID   普通攻击也是以技能方式
        ///// 触发方式放到技能系统里面
        ///// </summary>
        //public void Attack(Avatar target, int skillID)
        //{
        //    OnBeforeAttack(target);
        //    CombatCalcHelper.Attack(this, target, skillID); //处理伤害
        //}


        /// <summary>
        /// 攻击前触发
        /// </summary>
        /// <param name="target"></param>
        public void OnBeforeAttack(Avatar target)
        {

        }


        /// <summary>
        /// 攻击判定成功 后的计算伤害   后面修改流程
        /// </summary>
        /// <param name="target"></param>
        /// <param name="skill">技能的信息</param>
        /// <param name="skillPower">技能的加成比例</param>
        public void OnAttack(Avatar target, AvatarSkill skill)
        {
            
            ////弃用
            //var targetAttr = target.finalAttr;
            //var selfAttr = finalAttr;
            //DamagePack damagePack = DamagePack.GetPooled();

            ////计算命中
            //int hit = 10000 + selfAttr[ATTR_TYPE.HIT] - targetAttr[ATTR_TYPE.EVA];
        
            ////命中目标
            //if (AvatarHelper.CalcPro(hit))
            //{
            //    damagePack.hit = 1;

            //    //基础攻击
            //    var param1 = Math.Max(selfAttr[ATTR_TYPE.ATK] - targetAttr[ATTR_TYPE.DEF], 1);

            //    //技能伤害加成
            //    var baseDamage = (param1 * skill.skillPower[0]) + skill.skillPower[1];


            //    if (AvatarHelper.CalcPro(selfAttr[ATTR_TYPE.CRI])) 
            //    {
            //        baseDamage = param1 * selfAttr[ATTR_TYPE.CRI_D];  //暴击伤害
            //        damagePack.hit = 2;  //暴击
            //    }

            //    damagePack.damage = baseDamage;
            //    HurtTarget(target, skill, damagePack);  //伤害包
            //    TriggerPassiveSkill(SKILL_CAST_TYPE.PASSIVE_SKILL_A, target, TriggerPassiveSkillParam.GetPooled()); //被动 攻击触发
            //}
            //else
            //{
            //    damagePack.hit = 0;
            //    //未命中
            //}

        }


        ///// <summary>
        ///// 伤害目标  直接伤害目标
        ///// </summary>
        //public void HurtTarget(Avatar target, AvatarSkill skill, DamagePack damagePack)
        //{
        //    int beforeHp = target.GetAttrByType(ATTR_TYPE.HP);
        //    //目标受到伤害
        //    target.ChangeHP(-damagePack.damage);    //改变生命

        //    int afterHp = target.GetAttrByType(ATTR_TYPE.HP);

        //    target.TriggerPassiveSkill(SKILL_CAST_TYPE.PASSIVE_SKILL_D, this, TriggerPassiveSkillParam.GetPooled()); //被动防御触发

        //    if (target.IsDeath())
        //    {
        //        OnKill(target, skill);
        //        target.OnDeath(target, skill);
        //    }
        //}


        public void OnBeAttack()
        {

        }

        public virtual bool CanAttack()
        {
            if (IsDeath()) return false;

            return true;
        }

        /// <summary>
        /// 在他的回合中
        /// </summary>
        internal bool InOnTick = false;

        //以场景时间未锚点
        bool lastCanSkill = false; //上一次能否手动释放技能

        //后续后时间处理 中间继续的问题  不然

        /// <summary>
        /// 定时器回调 当前的时间
        /// </summary>
        public virtual void OnTick(int maxSpeed = 1)
        {
            if (IsDeath())  //如果死亡退出
            {
                return;
            }


       
        }

        /// <summary>
        /// 检查是否能释放技能   
        /// </summary>
        public void CheckCanSkill()
        {
            if (IsDeath())  //如果死亡退出
                return;

            if (!lastCanSkill)
                return;

            lastCanSkill = false;

            if (!skillSystem.AvatarCanCastSkillByHand()) return;    //能手动释放就能 并且 点击了释放技能

         
        }

        public void AddBuff(int buffID, Avatar caster, int labLayer = 0)
        {
            buffSystem.AddBuff(buffID, caster, labLayer);
        }

        public void DelBuffByUniqueID(ulong uniqueID)
        {
            buffSystem.DelBuffByUniqueID(uniqueID);
        }


        /// <summary>
        /// 击杀回调
        /// </summary>
        /// <param name="target"></param>
        /// <param name="skill"></param>
        public virtual void OnKill(Avatar target, DamageCalcPack skill)
        {

        }


        

        /// <summary>
        /// 死亡回调
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="skill"></param>
        public virtual void OnDeath(Avatar killer, DamageCalcPack skill)
        {
            buffSystem.OnDeath(killer, skill);

         
            //CombatCheckHelper.ReomveAvatar(this);
        }


        public Attr GetAttr()
        {
            return finalAttr.GetAttr();
        }
        

        //回收用上
        public void Clear()
        {
            //m_finalAttr
        }

    }
}
