using N_Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LGF.Util;

namespace N_Avatar
{
    using BuffCfg = BuffConfigRow;
    using SkillCfg = SkillConfigRow;

    public static class AvatarHelper
    {
        static ulong uniqueID = 10000000000;
        public static ulong GenUniqueID()
        {
            return uniqueID++;
        }


        public static Avatar GetAvatarByID(ulong id)
        {
            //暂时用这个
            return null;
        }



        #region 概率


        static System.Random random = new Random();
        public static int Random(int val = 0)
        {
            if (val == 0) return random.Next();

            return random.Next(val);
        }


        /// <summary>
        /// 计算概率
        /// </summary>
        public static bool CalcPro(int pro)
        {
            if (pro >= 10000) return true;


            var val = Common.Random(10000);

            return val <= pro;
        }


        /// <summary>
        /// 计算概率
        /// </summary>
        public static bool CalcPro(float pro)
        {
            if (pro >= 1) return true;


            var val = Common.Random(10000);

            return val <= pro * 10000;
        }


        /// <summary>
        /// 计算概率
        /// </summary>
        public static bool CalcPro(float pro, out int val)
        {
            val = 0;
            if (pro >= 1) return true;
            val = Common.Random(10000);
            return val <= pro * 10000;
        }

        #endregion

        #region buff
        //在这里get  解耦

        public static BuffCfg GetBuff(int buffid)
        {
            return BuffConfig.GetByID(buffid);
        }

        
        static ulong buffUniqueID = 1;
        /// <summary>
        /// 获得buff的唯一id
        /// </summary>
        /// <returns></returns>
        public static ulong GenBuffUniqueID()
        {
            return buffUniqueID++;
        }


        public static bool HasBuffEffectType(BuffInfo buff, BUFF_EFFECT_TYPE type)
        {
            return HasBuffEffectType(buff.config, type);
        }

        public static bool HasBuffEffectType(BuffCfg buffCfg, BUFF_EFFECT_TYPE type)
        {
            if (buffCfg.buffEffectType == null)
                return false;

            return Common.bit_has(buffCfg.buffEffectType.state, (int)type);
        }

     


        /// <summary>
        /// buff 特殊效果 类型
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasBuffStateEffectType(BuffInfo buff, BUFF_STATE_EFFECT type)
        {
            return HasBuffStateEffectType(buff.config, type);
        }

        /// <summary>
        /// buff 特殊效果 类型
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasBuffStateEffectType(BuffCfg buffCfg, BUFF_STATE_EFFECT type)
        {
            return buffCfg.Param2 == (int)type;
        }

        #endregion

        #region 技能


        public static SkillCfg GetSkillConfig(int id)
        {
            //暂时用这个
            return SkillConfig.GetByID(id);
        }


        public static bool HasSkillFlag(SkillCfg skillCfg, SKILL_FLAG type)
        {
            if (skillCfg == null)
            {
                return false;
            }
            return Common.bit_has(skillCfg.effType.state, (int)type);
        }





        #endregion


        #region 被动技能




        #endregion


        #region 战斗帮助方法


        static ulong curBattleSequenceUniqueID = 0;
        public static ulong GenBattleSequenceID() => ++curBattleSequenceUniqueID;

        static ulong curBattleOrderID = 0;
        public static ulong GenBattleOrderID() => ++curBattleOrderID;


        /// <summary>
        /// 初始 单回合 战斗 
        /// </summary>
        public static void InitialBattleBySingleLeg()
        {
            curBattleOrderID = 0;
        }
        #endregion


        #region 属性计算帮助类

        public static int CalcBysecondLevelAttr(AvatarAttr attr, ATTR_TYPE type)
        {
            return (int)CalcBysecondLevelAttrEx(attr, type);
        }

        /// <summary>
        ///  AvatarAttr 计算属性  计算二级属性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static float CalcBysecondLevelAttrEx(AvatarAttr attr, ATTR_TYPE type)
        {
            //Log("------CalcBysecondLevelAttr-------123---");
            //目前只有熊猫有换算属性
            if (attr[ATTR_TYPE.AVATAR_TYPE] != (int)AVATAR_TYPE.PLAYER)
            {
                return 0;
            }

  

            return 0;
        }




        private static float GetCalcAttr(AvatarAttr attr, ATTR_TYPE type,float val)
        {
            return (GetAttrVal(attr.GetAttr(), type) * val);
        }

        /// <summary>
        /// 过滤值
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static float GetAttrVal(Attr attr, ATTR_TYPE type)
        {
            if (type <= ATTR_TYPE.BaseMAX || type >= ATTR_TYPE.CalcMAX) 
            {
                return attr[type];
            }
            float t = attr[type] + (float)attr[type + (int)ATTR_TYPE.DECIMALS] / 10000;
            return t;
        }


        #endregion

        public static void  LogError(object str)
        {
            UnityEngine.Debug.LogError(str);
        }

        public static void Log(object str)
        {
            UnityEngine.Debug.Log(str);
        }


        public static bool Compare(int param1,int param2,int CompareType)
        {
            if (CompareType == -2)
            {
                return param1 <= param2;
            }
            else if(CompareType == -1)
            {
                return param1 < param2;
            }
            else if (CompareType == 0)
            {
                return param1 == param2;
            }
            else if (CompareType == 1)
            {
                return param1 > param2;
            }
            else if (CompareType == 2)
            {
                return param1 >= param2;
            }

            return false;
        }
    }


}
