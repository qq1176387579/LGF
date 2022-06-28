using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Attribute
{
    /// <summary>
    /// 属性类型    
    /// </summary>
    public enum ATTR_TYPE
    {
        #region 系统属性
        BASE_MAXHP          = 1,    //最大血量
        BASE_ATK            = 2,    //攻击力
        BASE_DEF            = 3,    //防御
        BASE_STR            = 4,    //力量                   整数部分
        BASE_DEX            = 5,    //敏捷                   整数部分
        BASE_INT            = 6,    //智力                   整数部分
        BASE_CON            = 7,    //耐力                   整数部分

        BASE_REG            = 8,    //生命值恢复速度          //万分比 非万分比 
        BASE_CRI            = 9,    //暴击率                  万分比 
        BASE_CRI_D          = 10,   //暴击伤害                万分比 
        BASE_EVA            = 11,   //闪避                    万分比 
        BASE_HIT            = 12,   //命中率                  万分比 
        BASE_SPD            = 13,   //速度                    
        BASE_CD             = 14,   //技能冷却


        ATK_ADDITION        = 21,   //攻击力加成比            万分比 
        SPD_ADDITION        = 22,   //速度加成比              万分比 
        DEF_ADDITION        = 23,   //防御的加成比            万分比



        NORMAL_DMG     = 50,   //基础伤害加成
        NORMAL_DEF     = 51,   //基础伤害减免

        GEO_DMG        = 100,  //地属性伤害加成
        HYDRO_DMG      = 101,  //水属性伤害加成
        PYRO_DMG       = 102,  //火属性伤害加成
        ANEMO_DMG      = 103,  //风属性伤害加成

        GEO_DEF        = 150,  //地属性伤害减免
        HYDRO_DEF      = 151,  //水属性伤害减免
        PYRO_DEF       = 152,  //火属性伤害减免
        ANEMO_DEF      = 153,  //风属性伤害减免

        //CHANGE_CUR_HP     = 180,   //改变当前生命 固定值
        //CHANGE_CUR_HP_PER = 181,   //改变当前生命 百分比值

        //199 技能吸血
        SKILL_SUCK_BLOOD    = 199,   //技能吸血
        ORDINARY_SUCK_BLOOD = 200,   //普通攻击 吸血属性


        COMBAT_DAMAGE_REDUCTION         = 201,      //最终伤害减少比例
        COMBAT_DAMAGE_REVERSES_DAMAGE   = 202,      //最终伤害反伤比例  特殊字段 未经过 201 伤害减少计算

        ORDINARY_CRI                    = 203,      //普通攻击暴击概率          //区分技能暴击
        ORDINARY_CRI_D                  = 204,      //普通攻击暴击伤害加成    


        DECIMALS            = 500,          //基础  属性小数位
        BASE_STR_DECIMALS   = DECIMALS + BASE_STR,       //力量        小数位  万分比
        BASE_DEX_DECIMALS   = DECIMALS + BASE_DEX,       //敏捷        小数位  万分比
        BASE_INT_DECIMALS   = DECIMALS + BASE_INT,       //智力        小数位  万分比
        BASE_CON_DECIMALS   = DECIMALS + BASE_CON,       //耐力        小数位  万分比



        BaseMAX = 1000, //基础属性最大值  
        #endregion



        #region 计算最终属性 1000~1999
        /// <summary>
        /// 最大血量
        /// </summary>
        MAXHP           = BaseMAX + BASE_MAXHP,         
        /// <summary>
        /// 攻击力
        /// </summary>
        ATK             = BaseMAX + BASE_ATK,           
        /// <summary>
        /// 防御
        /// </summary>
        DEF             = BaseMAX + BASE_DEF,           
        /// <summary>
        /// 力量
        /// </summary>
        STR             = BaseMAX + BASE_STR,           
        /// <summary>
        /// 敏捷
        /// </summary>
        DEX             = BaseMAX + BASE_DEX,           
        /// <summary>
        /// 智力
        /// </summary>
        INT             = BaseMAX + BASE_INT,           
        /// <summary>
        /// 耐力
        /// </summary>
        CON             = BaseMAX + BASE_CON,           
        /// <summary>
        /// 生命值恢复速度
        /// </summary>
        REG             = BaseMAX + BASE_REG,           
        /// <summary>
        /// 暴击率
        /// </summary>
        CRI             = BaseMAX + BASE_CRI,      
        /// <summary>
        /// 暴击伤害
        /// </summary>
        CRI_D           = BaseMAX + BASE_CRI_D,      
        /// <summary>
        /// 闪避
        /// </summary>
        EVA             = BaseMAX + BASE_EVA,        
        /// <summary>
        /// 命中率
        /// </summary>
        HIT             = BaseMAX + BASE_HIT,   
        /// <summary>
        /// 速度
        /// </summary>
        SPD             = BaseMAX + BASE_SPD,        
        /// <summary>
        /// 技能冷却
        /// </summary>
        CD              = BaseMAX + BASE_CD,

        /// <summary>
        /// 防御力计算后的减少以比例
        /// </summary>
        DEF_Calc_reduce_Pro = BaseMAX + 499,

        Calc_DECIMALS   = 1500,          //属性小数位
        STR_DECIMALS    = STR + DECIMALS,       //力量        小数位  万分比
        DEX_DECIMALS    = DEX + DECIMALS,       //敏捷        小数位  万分比
        INT_DECIMALS    = INT + DECIMALS,       //智力        小数位  万分比
        CON_DECIMALS    = CON + DECIMALS,       //耐力        小数位  万分比


        #endregion

        CalcMAX = 2000, //计算属性最大值  

        #region 其他属性  
        HP              = ATTR_TYPE.CalcMAX + 1,    //hp血量
        LOSS_HP         = ATTR_TYPE.CalcMAX + 2,    //已损失生命
        LAST_LOST_HP    = ATTR_TYPE.CalcMAX + 3,    //上次损失的生命  一些反伤效果需要的字段

        LEVEL           = ATTR_TYPE.CalcMAX + 100,    //等级


        REVERSES_DAMAGE_VAL       = ATTR_TYPE.CalcMAX + 201,    //反伤值  在被攻击前清空
        CUR_BY_SKILLS_HARM_VAL  = ATTR_TYPE.CalcMAX + 202,      //被技能造成的伤害值

        CUR_SKILL_GIVE_HARM_VAL = ATTR_TYPE.CalcMAX + 203,      //当前技能造成的伤害
        CUR_SKILL_HIT_COUNT = ATTR_TYPE.CalcMAX + 204,      //当前技能命中数量


        CUR_ACTION_PROGRESS   = ATTR_TYPE.CalcMAX + 300,  //当前行动进度值

        //----------------------

        AVATAR_TYPE         = ATTR_TYPE.CalcMAX  + 1000,  //种族类型        1 熊猫 
        JOB_TYPE            = ATTR_TYPE.CalcMAX  + 1001,  //职业类型         


        WEAPON_ELEMENT_ATTR = ATTR_TYPE.CalcMAX + 1002,     //当前武器元素属性
        AVATAR_ELEMENT_ATTR = ATTR_TYPE.CalcMAX + 1003,     //当前当前元素属性


        //怪物的配置ID
        MONSTER_ID          = ATTR_TYPE.CalcMAX + 1004,     //当前武器元素属性

        CUR_POS             = ATTR_TYPE.CalcMAX + 2000,     //当前位置

        #endregion



    };




    //系统属性
    public enum ATTR_SYS
    {
        //Weapon          =   1,    //武器属性
        Base = 1,     //基础
        BUFF_SYSTEM,        //buff系统
        SKILL_SYSTEM,       //技能的临时属性

        DUP_LEVEL_SYSTEM,   //副本等级系统

        Test = 1000,      //Test
    };



    

}
