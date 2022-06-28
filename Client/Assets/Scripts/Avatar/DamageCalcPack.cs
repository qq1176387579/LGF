using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LGF;


namespace N_Avatar
{



    using SkillCfg = SkillConfigRow;




    /// <summary>
    /// 伤害处理包
    /// </summary>
    public class DamageCalcPack : Poolable<DamageCalcPack>
    {
        public int addHp;  //基础治疗
        public int harm;   //基础伤害
        public long addHplong;
        public long harmlong;
        public SkillCfg skillConfig;
        public ulong sequenceID;    //序列ID
        public bool isParry;    //是否格挡

        public BuffInfo buffInfo;
        public int layer;   //层数
        public int count;   //次数
   

        public CombatCalc_CastSkillParam castSkillParam;
        protected override void OnGet()
        {
            base.OnGet();
            addHp = 0;
            harm = 0;
            skillConfig = null;
            buffInfo = null;
            isParry = false;
            layer = 0;
        
            count = 0;
            harmlong = 0;
            addHplong = 0;
            castSkillParam = null;
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            castSkillParam = null;
        }


    }


    /// <summary>
    /// 战斗计算 释放技能参数
    /// </summary>
    public class CombatCalc_CastSkillParam : Poolable<CombatCalc_CastSkillParam>
    {
        public ulong guid;   //伤害计算包 序列  区分同一阶段伤害包
        public int mul_damageOrAddhpPro; //伤害与血量加成 乘法
        public int add_damageOrAddhpPro; //伤害与血量加成 加法
        public int count;            //次数
        
        protected override void OnGet()
        {
            base.OnGet();
            guid = 0;
            count = 0;
            add_damageOrAddhpPro = 0;
            mul_damageOrAddhpPro = 10000;
        }
    }




}
