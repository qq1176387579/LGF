using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using N_Attribute;

namespace N_Avatar
{
    using SystemAttr = Dictionary<ATTR_SYS, Attr>;
    
    //记得属性和目标对象分离

    //角色属性
    public class AvatarAttr
    {
        protected Attr m_attr = new Attr();
        protected SystemAttr m_systemAttr = new SystemAttr();   //系统属性

        Attr changeAttr = new Attr();   //改变的值

        //这里没有开放set  怕外部赋值出问题  不方便查找  如果需要赋值 用SetAttrByType
        public int this[ATTR_TYPE type] { get => GetAttrByType(type); internal set => SetAttrByType(type, value); }  




        /// <summary>
        /// 参数会深拷贝
        /// </summary>
        /// <param name="sysType"></param>
        /// <param name="attr"></param>
        public void ChangeSystemAttr(ATTR_SYS sysType, Dictionary<int,int> tAttr)
        {
            Attr tmp = Attr.Get();
            tmp.AddAttrData(tAttr);
            ChangeSystemAttr(sysType, tmp);
            tmp.Release();
        }

        /// <summary>
        /// 参数会深拷贝
        /// </summary>
        /// <param name="sysType"></param>
        /// <param name="attr"></param>
        public void ChangeSystemAttr(ATTR_SYS sysType, Attr tAttr)
        {
            changeAttr.ClearData();


            if (!m_systemAttr.ContainsKey(sysType))
            {
                Attr attr = new Attr();
                attr.AddAttr(tAttr);  //深拷贝 
                m_systemAttr[sysType] = attr;   //更新系统数据

                changeAttr.AddAttr(tAttr);
            }
            else
            {
                Attr data = m_systemAttr[sysType];

                foreach (var v in tAttr.data)
                    changeAttr.SetAttrByType(v.Key, data.GetChangeValByType(v.Key, v.Value));

                foreach (var v in data.data)
                {
                    if (!tAttr.data.ContainsKey(v.Key))
                    {
                        changeAttr.SetAttrByType(v.Key, data.GetChangeValByType(v.Key, 0));
                    }
                }

                data.ClearData();
                data.AddAttr(tAttr);  //深拷贝  
            }

            //changeAttr    //这里可以写事件回调 foreach (var v in changeAttr) 改变的数据  看需求
            m_attr.AddAttr(changeAttr);     //改变属性

            CalcAddRatio(); //计算加成属性
        }



        /// <summary>
        /// 修改最终属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="val"></param>
        public void SetAttrByType(ATTR_TYPE type, int val)
        {
            m_attr.SetAttrByType(type, val);
        }

        /// <summary>
        /// 添加属性 不推荐修改系统属性 如攻击力 防御等  这些走系统属性改变流程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="val"></param>
        public void AddAttrByType(ATTR_TYPE type, int val)
        {
            SetAttrByType(type, GetAttrByType(type) + val);
        }


        public int GetAttrByType(ATTR_TYPE type)
        {
            return m_attr.GetAttrByType(type);
        }

        public Attr GetAttr()
        {
            return m_attr;
        }


        public void ClearSystemAttrByType(ATTR_SYS type)
        {
            if (m_systemAttr.ContainsKey(type))
            {
                ChangeSystemAttr(type, Attr.Zero);
            }
        }


        public void InitDynamicAttr()
        {
            this[ATTR_TYPE.HP] = this[ATTR_TYPE.MAXHP];
            this[ATTR_TYPE.LOSS_HP] = 0;

        }

        /// <summary>
        /// 计算加成属性
        /// </summary>
        void CalcAddRatio()
        {
            //int maxhp = GetAttrByType(ATTR_TYPE.BASE_MAXHP) * (1 + GetAttrByType(ATTR_TYPE.MAXHP_RATIO) / 100) + GetAttrByType(ATTR_TYPE.MAXHP_AFTER_PRO); //血量
            //SetAttrByType(ATTR_TYPE.MAXHP, maxhp);
            //this[ATTR_TYPE.STR] = this[ATTR_TYPE.BASE_STR];
            //this[ATTR_TYPE.DEX] = this[ATTR_TYPE.BASE_DEX];
            //this[ATTR_TYPE.INT] = this[ATTR_TYPE.BASE_INT];
            //this[ATTR_TYPE.CON] = this[ATTR_TYPE.BASE_CON];

            //this[ATTR_TYPE.STR_DECIMALS] = this[ATTR_TYPE.BASE_STR_DECIMALS];
            //this[ATTR_TYPE.DEX_DECIMALS] = this[ATTR_TYPE.BASE_DEX_DECIMALS];
            //this[ATTR_TYPE.INT_DECIMALS] = this[ATTR_TYPE.BASE_INT_DECIMALS];
            //this[ATTR_TYPE.CON_DECIMALS] = this[ATTR_TYPE.BASE_CON_DECIMALS];



            //this[ATTR_TYPE.MAXHP]   = this[ATTR_TYPE.BASE_MAXHP] + AvatarHelper.CalcBysecondLevelAttr(this, ATTR_TYPE.MAXHP);
            ////Debug.Log("------" + this[ATTR_TYPE.MAXHP]);

            //this[ATTR_TYPE.ATK]     = this[ATTR_TYPE.BASE_ATK]  + AvatarHelper.CalcBysecondLevelAttr(this, ATTR_TYPE.ATK);
            //this[ATTR_TYPE.ATK]     = this[ATTR_TYPE.ATK] * (10000 + this[ATTR_TYPE.ATK_ADDITION]) / 10000; //计算加成比例

            //this[ATTR_TYPE.REG]     = this[ATTR_TYPE.BASE_REG]  + AvatarHelper.CalcBysecondLevelAttr(this, ATTR_TYPE.REG);
            //this[ATTR_TYPE.CRI_D]   = this[ATTR_TYPE.BASE_CRI_D] + Game_Config.Instance.Base_Cri_D;  //基础暴击伤害

            //this[ATTR_TYPE.SPD]     = this[ATTR_TYPE.BASE_SPD]  + AvatarHelper.CalcBysecondLevelAttr(this, ATTR_TYPE.SPD);
            //this[ATTR_TYPE.SPD]     = this[ATTR_TYPE.SPD] * (10000 + this[ATTR_TYPE.SPD_ADDITION]) / 10000;


            //this[ATTR_TYPE.DEF]     = this[ATTR_TYPE.BASE_DEF]  + AvatarHelper.CalcBysecondLevelAttr(this, ATTR_TYPE.DEF);
            //this[ATTR_TYPE.DEF]     = this[ATTR_TYPE.DEF] * (10000 + this[ATTR_TYPE.DEF_ADDITION]) / 10000;
            
            //this[ATTR_TYPE.CRI]     = this[ATTR_TYPE.BASE_CRI] + AvatarHelper.CalcBysecondLevelAttr(this, ATTR_TYPE.CRI);
                                    
            //this[ATTR_TYPE.EVA]     = this[ATTR_TYPE.BASE_EVA] + AvatarHelper.CalcBysecondLevelAttr(this, ATTR_TYPE.EVA);

            //this[ATTR_TYPE.HIT]     = this[ATTR_TYPE.BASE_HIT] + AvatarHelper.CalcBysecondLevelAttr(this, ATTR_TYPE.HIT);

            //this[ATTR_TYPE.CD]      = this[ATTR_TYPE.BASE_CD];


            ////Debug.Log("----ATK-- = " + this[ATTR_TYPE.ATK]);
            ////Debug.Log("----SPD-- = " + this[ATTR_TYPE.SPD]);

            //this[ATTR_TYPE.DEF_Calc_reduce_Pro] = (this[ATTR_TYPE.DEF] * 10000 / (this[ATTR_TYPE.DEF] + 100));  //万分比


            //CalClossHp();

            //if (LHGM.Instance != null && LHGM.Instance.OpenAttrLog)
            //    AvatarHelper.LogError(" new CalcAddRatio >>>>>>>> : " + GetLogInfo());
        }

        /// <summary>
        /// 计算以损失血量
        /// </summary>
        public void CalClossHp()
        {
            //检测血量
            if (this[ATTR_TYPE.HP] > this[ATTR_TYPE.MAXHP])
            {
                this[ATTR_TYPE.HP] = this[ATTR_TYPE.MAXHP];
            }

            //计算已经损失的血量
            this[ATTR_TYPE.LOSS_HP] = this[ATTR_TYPE.MAXHP] - this[ATTR_TYPE.HP];
        }


        public string GetLogInfo()
        {
            return m_attr.GetLogInfo();
        }

        public void Log()
        {
            m_attr.Log();
        }
    }


}