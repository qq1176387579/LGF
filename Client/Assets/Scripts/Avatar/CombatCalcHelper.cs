using N_Attribute;
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
    /// 战斗计算帮助器
    /// </summary>
    public static class CombatCalcHelper
    {
        public static bool LOG_SWITCH = false;  //log开关
        


        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="castker"></param>
        /// <param name="skillID"></param>
        /// <param name="targetList"></param>
        public static void CastSkill(Avatar castker, int skillID, List<Avatar> targetList)
        {
            LogError("====>:" + skillID);
            SkillCfg skillConfig = AvatarHelper.GetSkillConfig(skillID);

            for (int i = 0; i < targetList.Count; i++)
            {
             
                //CastSkillEx(castker, skillConfig, targetList[i], param);
            }


            if (skillConfig.nextSkill != null)
            {
                for (int i = 0; i < skillConfig.nextSkill.Length; i++)
                {
                    //castker.skillSystem.CastSkillAutoTarget(skillConfig.nextSkill[i]);
                }
            }

          
        }


        /// <summary>
        /// buff效果 持续伤害 回血等处理
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="skillID"></param>
        public static void BuffEffect(BuffInfo buff, int skillID)
        {
            SkillCfg skillConfig = AvatarHelper.GetSkillConfig(skillID);

            Attr casterAttr = buff.attr;    //获得 施法者属性
            Attr targetAttr = buff.owner.GetAttr();
            //CalcBaseVal(buff.caster, buff.owner, casterAttr, targetAttr, damagePack);

            ////后续计算伤害加成 以及消减
            //CalcOtherDamageAttr(casterAttr, buff.caster, buff.owner, damagePack);

          
        }




        /// <summary>
        /// 计算其他伤害属性   pack 伤害包是共用的 不要使用这个方法
        /// </summary>
        public static void CalcOtherDamageAttr(Attr casterAttr,Avatar avatar, Avatar target, DamageCalcPack pack)
        {

        }



        static void ChangeHP(int addHp,int harm, Avatar avatar, Avatar target, DamageCalcPack pack, bool isRestrained = false)
        {
            //治疗量减去伤害 计算伤害值
            var changeHp = addHp - harm;

            //target.finalAttr[ATTR_TYPE.CUR_BY_SKILLS_HARM_VAL] += harm; //伤害值
            //avatar.finalAttr[ATTR_TYPE.CUR_SKILL_GIVE_HARM_VAL] += harm;    //当前技能造成的伤害
            //LogError("  CUR_SKILL_GIVE_HARM_VAL " + avatar.finalAttr[ATTR_TYPE.CUR_SKILL_GIVE_HARM_VAL]);


            if (changeHp != 0)
                target.ChangeHP(changeHp);    //改变血量


            if (target.IsDeath())         //死亡 
            {
                avatar.OnKill(target, pack);
                target.OnDeath(avatar, pack);

            }
        }





        /// <summary>
        /// 计算基础伤害流程
        /// </summary>
        /// <param name="paramVal"></param>
        /// <param name="casterAttr"></param>
        /// <param name="targetAttr"></param>
        /// <param name="layer">层数</param>
        /// <returns></returns>
        static long CalcBaseVal_ParamVal_1or2(int[] paramVal, Attr casterAttr, Attr targetAttr, DamageCalcPack pack)    
        {
            int layer = pack.layer;
            int attrID = paramVal.GetByID(1);   //属性ID
            bool isCaster = paramVal.GetByID(2) == 1;   //是否是施法者
            int val = paramVal.GetByID(3);
            int layerVal = layer > 1 && paramVal.GetByID(4) != 0 ? (paramVal.GetByID(4) * (layer - 1)) : 0;


            int atlastVal = (val + layerVal) *  + 10000;   //固定伤害 
            //LogError("atlastVal : "+ atlastVal);
            if (attrID != 0)    //attrID == 0 时 表示固定伤害值 
            {
                atlastVal = 0;
                Attr attr = isCaster ? casterAttr : targetAttr;
                if (attr != null)
                {
                    atlastVal = (int)(AvatarHelper.GetAttrVal(attr, (ATTR_TYPE)attrID)  * (val + layerVal));
 
                 	LogError($">>>>>>>>>>>>>>>>>>>>atlastVal : {atlastVal}  ");
                    LogError($"attr[{(ATTR_TYPE)attrID}] = {attr[(ATTR_TYPE)attrID]} ");
                    //LogError($"{val + layerVal} + {pack.add_damageOrAddhpPro } = {(val + layerVal + pack.add_damageOrAddhpPro)}");

                }
            }
            
            return atlastVal;
        }





        static void CalcBaseVal(Avatar caster, Avatar target, Attr casterAttr, Attr targetAttr, DamageCalcPack pack)
        {
            if (pack.skillConfig.Param1 != null)
                CalcBaseValEx(caster, target, casterAttr, targetAttr, pack, pack.skillConfig.Param1);


            var condition = pack.skillConfig.condition;
            var conditionCompletion = pack.skillConfig.conditionCompletion;

           

            if (condition != null)
            {
                //计算条件 条件达成执行对应操作
                System.Func<int[], bool> func = (param) =>
                {
                    switch (param[0])
                    {
                        case 1:     //有buff
                            {
                                int buffID = param.GetByID(1);   //属性ID
                                bool isCaster = param.GetByID(2) == 1;   //是否是施法者
                                Avatar avatar = isCaster ? caster : target;
                                return avatar.buffSystem.GetBuff(buffID) != null;
                            }

                        //break;
                        case 2:    //无buff
                            {
                                int buffID = param.GetByID(1);   //属性ID
                                bool isCaster = param.GetByID(2) == 1;   //是否是施法者
                                Avatar avatar = isCaster ? caster : target;

                                return avatar.buffSystem.GetBuff(buffID) == null;
                            }
                        //break;
                        default:
                            break;
                    }

                    return true;
                };

                for (int i = 0; i < condition.Length; i++)
                {
                    if (func(condition[i]))
                    {
                        CalcBaseValEx(caster, target, casterAttr, targetAttr, pack, conditionCompletion[i]);
                    }
                }
            }


            //后面计算属性加成
            //伤害
            if (pack.harmlong != 0)  //基础伤害值     //外部计算加成值
            {
                pack.harm = Math.Max((int)(pack.harmlong / 10000), 1);
                //UnityEngine.Debug.Log("-----harm----" + pack.harm);

            }

            if (pack.addHplong != 0) //基础回血      //外部计算加成值
            {
                pack.addHp = Math.Max((int)(pack.addHplong / 10000), 1);
                //UnityEngine.Debug.Log("-------addHp--" + pack.addHp);
            }

        }






        /// <summary>
        /// 计算基础值
        /// </summary>
        /// <param name="casterAttr"></param>
        /// <param name="targetAttr"></param>
        /// <param name="skillConfig"></param>
        /// <param name="addHp"></param>
        /// <param name="harm"></param>
        public static void CalcBaseValEx(Avatar caster, Avatar target, Attr casterAttr, Attr targetAttr, DamageCalcPack pack, int[][] Param)
        {
            if (Param == null) return;

            long addHp = pack.addHplong;
            long harm = pack.harmlong;
            //for (int i = 0; i < pack.skillConfig.Param1.Length; i++)
            for (int i = 0; i < Param.Length; i++)
            {
                var paramVal = Param[i];
                if (paramVal.GetByID(0) == 1) //回血
                {
                    addHp += CalcBaseVal_ParamVal_1or2(paramVal, casterAttr, targetAttr, pack);
                    if (addHp <= 0)
                        addHp = 1;
                }
                else if (paramVal.GetByID(0) == 2) //伤害计算
                {
                    harm += CalcBaseVal_ParamVal_1or2(paramVal, casterAttr, targetAttr, pack);
                    if (harm <= 0)
                        harm = 1;
                }
                else if (paramVal.GetByID(0) == 3) //添加buff
                {
                    int buffID = paramVal.GetByID(1);   //属性ID
                    //bool isCaster = paramVal.GetByID(2) == 1;   //是否是施法者
                    //Avatar avatar = isCaster ? caster : target;
                    target.AddBuff(buffID, caster, paramVal.GetByID(2) == 0 ? 0 : pack.count);
                }
                else if (paramVal.GetByID(0) == 4) //添加buff 区间内随机抽取buff
                {
                    bool isCaster = paramVal.GetByID(1) == 1;   //是否是施法者
                    Avatar avatar = isCaster ? caster : target;

                    List<RandomDrawingHelperClass<int>> helperList = ListPool<RandomDrawingHelperClass<int>>.Get();
                    for (int j = 2; j < paramVal.Length; j++)
                    {
                        RandomDrawingHelperClass<int> helperClass = RandomDrawingHelperClass<int>.Get();
                        helperClass.info = paramVal[j++];
                        helperClass.Weight = paramVal[j];
                        helperList.Add(helperClass);
                    }
                    var buffinfo = helperList.RandomDrawing();

                    int buffID = buffinfo.info;  //随机抽取到的值
                    helperList.ReleaseAll();
                    if (buffID != 0)    // buffID 表示为0 表示抽到空的
                    {
                        avatar.AddBuff(buffID, caster);
                    }

                }
                else if (paramVal.GetByID(0) == 5)    //5.召唤怪物 0表示自动补空位
                {
                    int monid = paramVal.GetByID(1);    //怪物id
                    //CombatCheckHelper.SkillCreationMon(monid);
                }
                else if(paramVal.GetByID(0) == 1000)    //1000 表示复制buff  特殊处理
                {
                    List<BuffInfo> bufflist = caster.buffSystem.GetAllDebuffInfo();
                    //复制buff
                    bufflist.ForEach((buffinfo) =>
                    {
                        //复制buff
                        LogError("  复制buff .id " + buffinfo.config.id);
                        target.AddBuff(buffinfo.config.id, buffinfo.caster, buffinfo.layer);
                    });
                    bufflist.Release();
                }
            }

            pack.addHplong = addHp;
            pack.harmlong = harm;

        }



        /// <summary>
        /// 计算伤害加深计算
        /// </summary>
        static int CalcElementRestrain(Attr casterAttr, Attr targetAttr)
        {
            var casterWeaponElement     =   casterAttr[ATTR_TYPE.WEAPON_ELEMENT_ATTR];    //释放者 元素属性
            var targetElement           =   targetAttr[ATTR_TYPE.AVATAR_ELEMENT_ATTR];    //目标  元素属性

            if ((casterWeaponElement + 1) % 4 == targetElement % 4) //克制
            {
                return 12000;
            }
            return 10000;
        }


        //回血操作
        public static void OnTick(Avatar avatar)
        {
         
        }

        public static void LogError(object str)
        {
            if (!LOG_SWITCH) return;
            AvatarHelper.LogError(str);
        }

    }
}
