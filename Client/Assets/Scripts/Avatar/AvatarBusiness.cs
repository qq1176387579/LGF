using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 *  代理 业务
 * 
 */

namespace N_Avatar
{
    public enum AVATAR_TYPE
    {
        UNKNOW          =   0,      //未知类型
        PLAYER          =   1,      //玩家
        MONSTER         =   2,      //怪物
    }


    //public enum SKILL_CAST_TYPE 
    //{
	   // ACTIVE_SKILL 	    = 1,    // 主动技能
    //    ATTR_SKILL          = 2,    // 属性技能
    //    PASSIVE_SKILL_A     = 3,    // 被动技能(攻击触发)
    //    PASSIVE_SKILL_D     = 4,    // 被动技能(受击触发)
    //}




    //如果有同事存在的标签  流程需要修改
    public enum BUFF_EFFECT_TYPE
    {
        ATTR_CHANGE         = 1, // 属性变更  增加临时属性
        INTERVAL            = 2, // 间隔buff  持续回血 等
        STATE               = 3, // 特殊效果  眩晕等
        INSTANT             = 4, // 立即生效且不持续的buff 与其他几个互斥不能同事存在
        TRIGGER_EFF         = 5, //条件达到 触发效果
    }

    /// <summary>
    /// buff叠加模式
    /// </summary>
    public enum BUFF_LAB_TYPE
    {
        CAN_NOT_LAB     = 0,    //不可叠加
        CAN_LAB         = 1,    //可叠加
        OVERLAY         = 2,    //可覆盖
        ADDED_DIRECTLY  = 3,    //直接添加  //处理可以多个存在的  如炸弹
    }

    /// <summary>
    /// buff叠加模式
    /// </summary>
    public enum BUFF_LAB_MODE
    {
        ADD_TIME    =   1,  //叠加时间
        ADD_LAYER   =   2,  //叠加层数
    }


    public enum BUFF_OPT
    {
        EMPTY   = 0,
        ADD = 1,	    // 添加 播放特效
        DEL = 2,	    // 删除       删除没有特效直接消失
        UPD = 3,	    // 更新 播放特效
        UPD_NOT = 4,	//回合结束更新 不需要特效  只用用于层级跟新
    }

    /// <summary>
    /// BUFF特殊效果
    /// </summary>
    public enum BUFF_STATE_EFFECT
    {
        DIZZY               = 1,            //晕眩 实现
        INVINCIBLE          = 2,            //无敌
        BE_TAUNT            = 3,            //嘲讽
        RESIST_NORMALATK    = 4,            //免疫普通攻击  层数相关 实现 完成
        CHAOS               = 5,            //混乱 完成
        SILENCE             = 6,            //沉默  完成
        BURN                = 7,            //燃烧  完成
        STICK               = 8,            //坚守    //完成  暂时先这样  反伤
        IMMUNE              = 9,            //免疫 控制  完成
    }


    ///// <summary>
    ///// 不同的伤害类型  计算公式不一样
    ///// </summary>
    //public enum DamageType
    //{
    //    ordinary    =   0,  //普通的伤害
    //    real        =   1,  //真实伤害
    //}


    /// <summary>
    /// 技能标签
    /// </summary>
    public enum SKILL_FLAG
    {
        ORDINARY_ATTACK     =   1,      //普通攻击
        ORDINARY_SKILL      =   2,      //普通技能
                                        //3是技能伤害
        STRIKE_BACK         =   4,      //反击  区分普通攻击  普通级有 筛选对象用到了 反击没有 
        PASSIVITY_SKILL     =   5,      //被动  技能
        //5是被动
    }


    ///// <summary>
    ///// 技能参数2
    ///// </summary>
    //public enum SKILL_PARAM2
    //{
    //    DAMAGE_SUCKBLOOD = 1,   //伤害吸血

    //}



    /// <summary>
    /// 被动技能 技能效果
    /// </summary>
    public enum PASSIVE_SKILL_EFFECT_TYPE 
    {
        SKILL               = 1, // 技能
	    BUFF 			    = 2, // buff
      

        // //目前没需求  如果有属性需求 走Attr的流程 走流水计算
        //DAMAGE_REDUCE       = 3, // 伤害 减少
        //DAMAGE_ADD          = 4, // 伤害 加成
        //DAMAGE_SUCKBLOOD    = 5,  //伤害 吸血
    }

    ///// <summary>
    ///// 被动技能触发类型
    ///// </summary>
    //public enum PASSIVE_SKILL_TRIGGER_TYPE
    //{
    //    SELF_ATTACK_BEFORE          = 1,    //自己攻击前
    //    SELF_DAMAGE_TARGET_BEFORE   = 2,    //自己伤害目标之前
    //    TARGET_IS_DAMAGED_BEFORE    = 3,    //目标受到伤害之前
    //    SELF_DAMAGE_TARGET_AFTER    = 4,    //自己伤害目标之后
    //    TARGET_IS_DAMAGED_AFTER     = 5,    //目标受到伤害之后
    //    SELF_USE_SKILL              = 6,    //使用技能
    //}


    ///// <summary>
    ///// 被动技能 触发条件
    ///// </summary>
    //public enum PASSIVE_SKILL_TRIGGER_COND
    //{
    //    PROBABILITY                 = 1,      //概率
    //    HP_SECTION                  = 2,      //血量区间
    //    HP_PERCENTUM_LAYER          = 3,      //血量百分比 层数
    //    SKILL                       = 4,      //使用技能次数

    //    //10    //特定的目标
    //}


    ///// <summary>
    ///// 施法位置类型
    ///// </summary>
    //public enum SKILL_TARGET_POS_TYPE
    //{
    //    GROUP   =   1,              //全组人
    //    FIRST   =   2,              //最高位
    //    LAST    =   3,              //最末位
    //    MIDDLE  =   4,              //中间位
    //    SELF    =   5,              //自己
    //    RANDOM  =   6,              //随机
    //    DEBUFF_MAX  =   7,            //debuff最多的对象
    //    HP_MIN      =   8,             //血量最低的 百分比
    //    EXCEPT_MYSELF   = 10,       //自己之外
    //}


    ///// <summary>
    ///// 工作类型 记得给ID
    ///// </summary>
    //public enum JOB_TYPE    
    //{
    //    //1、工程师
    //    //2、赛博诗人
    //    //3、神射手
    //    //4、突击者
    //    //5、医师
    //    //6、铁壁
    //    IRONCLAD        =   6,           //铁壁
    //    SHOCK_WORKER    =   4,           //突击手    
    //    SHOOTER         =   3,           //射手
    //    MECHANICAL      =   1,           //机械官
    //    PHYSICIAN       =   5,           //医生
    //    POET            =   2,           //诗人
    //}




    ///// <summary>
    ///// 元素属性
    ///// </summary>
    //public enum ELEMENT_ATTR
    //{
    //    GROUND  =   1,          //地
    //    WATER   =   2,          //水
    //    IGNIS   =   3,          //火        
    //    WIND    =   4,          //风        
    //}


    ///// <summary>
    ///// 技能触发参数3效果
    ///// </summary>
    //public enum SKILL_TRIGGER_PARAM3_EFF
    //{
    //    Target_Death    =       1,        //目标死亡
    //    Hit             =       2,        //命中时检测

    //}

    ///// <summary>
    ///// 特殊buff类型
    ///// </summary>
    //public enum SPECIAL_BUFF_ID_TYPE
    //{
    //    COURAGE     =   2,      //勇气id
    //    牛奶四溅    =   15,     //牛奶四溅
    //    //BE_TAUNT    =   16,     //被嘲讽
    //    TAUNT       =   20000,   //嘲讽的id
    //}

}

