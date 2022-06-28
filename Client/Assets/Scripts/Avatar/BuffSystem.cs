using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N_Attribute;
using LGF;


namespace N_Avatar
{
    using BuffCfg = BuffConfigRow;




    /// <summary>
    /// buff系统
    /// 层数 我这里没做啥统一流程
    /// </summary>
    public class BuffSystem
    {
        /// <summary>
        /// 系统属性 最终的
        /// </summary>
        Attr systemAttr = new Attr();

        Avatar owner;
        //后期换链表 因为不清楚LinkedList的移除规则 目前不敢用
        List<BuffInfo> buffList = new List<BuffInfo>();


        public BuffSystem Init(Avatar owner_)
        {
            owner = owner_;
            return this;
        }

     

        /// <summary>
        /// 设置或者添加buff层数 设置层数的buffID和创建对象组成key 只能存在一个
        /// 给一些被动技能特殊用的
        /// </summary>
        /// <param name="buffID"></param>
        /// <param name="caster"></param>
        /// <param name="labLayer"></param>
        public ulong SetOrAddBuffLayer(int buffID, Avatar caster, int labLayer = 0)
        {
            BuffCfg config = AvatarHelper.GetBuff(buffID); //BuffConfig.GetByID(buffID);
            BuffInfo buff = GetBuff(config, caster.uniqueID);
            if (buff == null)
            {
                AddBuff(buffID, caster, labLayer);
                buff = GetBuff(config, caster.uniqueID);
                return buff.uniqueID;
            }
            else
            {
                if (buff.layer == labLayer)
                {
                    return buff.uniqueID;
                }
                else
                {
                    buff.layer = labLayer;
                    //属性改变
                    if (IsAttrChange(buff))
                    {
                        CalcAttr();
                    }
                }
            }

            return buff.uniqueID;
        }
        

       //public void AddBuff(SPECIAL_BUFF_ID_TYPE buffID, Avatar caster, int labLayer = 0)
       // {
       //     AddBuff((int)buffID, caster, labLayer);
       // }

        /// <summary>
        /// 添加buff   0表示默认配置层数
        /// </summary>
        /// <param name="buffID">buffid</param>
        /// <param name="casterID">创建者ID</param>
        /// <param name="labLayer">默认层级</param>
        public void AddBuff(int buffID, Avatar caster,int labLayer = 0) 
        {
            if (buffID == 0)
            {
                AvatarHelper.LogError("--------AddBuff  buffID == 0");
                return;
            }
            //AvatarHelper.LogError($" uniqueID: {owner.uniqueID} AddBuff {buffID}  labLayer: {labLayer}");
            BuffCfg config = AvatarHelper.GetBuff(buffID); //BuffConfig.GetByID(buffID);

            //立即生效  有些立即生效状态
            //如果有那种第一次 直接与后续的效果不一样的在去看
            if (AvatarHelper.HasBuffEffectType(config, BUFF_EFFECT_TYPE.INSTANT))
            {
                BuffInfo buffinfo = BuffInfo.GetPooled().Init(buffID, caster, owner, config, labLayer);
                DoAttrChangeEffect(buffinfo, BUFF_EFFECT_TYPE.INSTANT);
                BroadcastBuffEvent(buffinfo, BUFF_OPT.ADD);

                if (buffinfo.IsStateEffect(BUFF_STATE_EFFECT.IMMUNE)) //如果是免疫
                {
                    ClearAllDebuff();   //清除所有debuff
                }

                buffinfo.Release();
                return;
            }

            ////如果能被免疫  那么免疫他
            //if (IsStateEffect(BUFF_STATE_EFFECT.IMMUNE) && CanByImmune(config))
            //{
            //    //用来播放效果
            //    BuffInfo buffinfo = BuffInfo.GetPooled().Init(buffID, caster, owner, config, labLayer);
            //    //免疫效果
            //    buffinfo.Release(); 
            //    return;
            //}



            if (config.labType == (int)BUFF_LAB_TYPE.ADDED_DIRECTLY)   //直接添加新buff
            {
                AddNewBuff(buffID, caster, config, labLayer);
                return;
            }

            BuffInfo buff = GetBuff(config, caster.uniqueID);

            if (buff != null)
            {
                if (buff.config.labType == (int)BUFF_LAB_TYPE.OVERLAY)  //覆盖
                {
                    buff.caster = caster;
                    // Global.CurSceneTimeMsec + config.time  以时间为单位
                    //当前游戏是次数  
                    buff.endTime = config.time;
                }
                else if (buff.config.labType == (int)BUFF_LAB_TYPE.CAN_LAB) //叠加层级
                {
                    switch ((BUFF_LAB_MODE)buff.config.labMode)
                    {
                        case BUFF_LAB_MODE.ADD_TIME:
                            buff.caster = caster;
                            buff.endTime += config.time;    //增加时间
                            break;
                        case BUFF_LAB_MODE.ADD_LAYER:
                            buff.caster = caster;
                            buff.layer += labLayer != 0 ? labLayer : config.labLayer;    //添加层级
                            buff.layer = buff.layer > config.labLayerMax ? config.labLayerMax : buff.layer;
                            
                            buff.endTime = config.time;     //时间重置
                            break;
                        default:
                            //出问题
                            UnityEngine.Debug.LogError(StringPool.Concat(">>>>> buff.config is error!!!  buffid = ", buff.config.id.ToString()));
                            break;
                    }

                }
                BroadcastBuffEvent(buff, BUFF_OPT.UPD);
                return;
            }
            else
            {
                AddNewBuff(buffID, caster, config, labLayer);
            }
        }


        //internal BuffInfo GetBuff(SPECIAL_BUFF_ID_TYPE buffid)
        //{
        //    return GetBuff((int)buffid);
        //}

        /// <summary>
        /// 查询第一个为这个的buffid
        /// </summary>
        /// <param name="buffid"></param>
        /// <returns></returns>
        internal BuffInfo GetBuff(int buffid)
        {
            for (int i = 0; i < buffList.Count; i++)
            {
                if (buffList[i].buffid == buffid)
                {
                    return buffList[i];
                }
            }
            return null;
        }


        bool CheckBuffConfigCasterID(BuffInfo info,BuffCfg config, ulong casterID)
        {
            //检测buffid
            if (info.buffid != config.id)
            {
                return false;
            }
            //区分释放者
            if (config.differentiateCaster == 1 && (info.caster == null || info.caster.uniqueID != casterID))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获得buff
        /// </summary>
        /// <param name="config"></param>
        /// <param name="casterID"></param>
        /// <returns></returns>
        public BuffInfo GetBuff(BuffCfg config, ulong casterID)
        {
            for (int i = 0; i < buffList.Count; i++)
            {
                if (CheckBuffConfigCasterID(buffList[i], config, casterID))
                {
                    return buffList[i]; //获得buff
                }
            }
            return null;
        }

        void AddNewBuff(int buffID, Avatar caster, BuffCfg cfg, int labLayer)
        {
            var buff = BuffInfo.GetPooled().Init(buffID, caster, owner, cfg, labLayer);
            buff.endTime = cfg.time;
            buffList.Add(buff);

            if (IsAttrChange(buff))     //属性改变
            {
                DoAttrChangeEffect(buff, BUFF_EFFECT_TYPE.INSTANT); //添加buff后不立即生效
            }
            BroadcastBuffEvent(buff, BUFF_OPT.ADD);

            //如果有免疫buff  那么免疫debuff
            if (buff.IsStateEffect(BUFF_STATE_EFFECT.IMMUNE)) 
            {
                ClearAllDebuff();
            }

            buffList.Sort((info1,info2) =>
            {
                return info1.config.flag.CompareTo(info2.config.flag);  //
            });
        }

        /// <summary>
        /// 清除所有debuff
        /// </summary>
        public void ClearAllDebuff()
        {
            //删除所有控制状态
            DelBuff((buffinfo) =>
            {
                return CanByImmune(buffinfo.config);
            });
        }

        /// <summary>
        /// 清除所有buff
        /// </summary>
        public void ClearAll()
        {
            //删除所有控制状态
            DelBuff((buffinfo) =>
            {
                return true;
            });
        }


        /// <summary>
        /// 计算属性加成
        /// 当前只做了简单增益减益处理  1表示能叠加属性  0表示取 最大 最小值 进行处理
        /// </summary>
        void CalcAttr()
        {
            systemAttr.ClearData();
            // 系统属性  取最大值  最小值
            Attr systemAttrMax = Attr.Get();
            Attr systemDebuffAttrMin = Attr.Get();

            buffList.ForEach((buff) =>
            {
                var attrList = buff.config.attrList;
                if (attrList == null) return;

                foreach (var item in attrList)
                {
                    ATTR_TYPE type = (ATTR_TYPE)item.Key;
                    var val = item.Value[0] + item.Value.GetByID(2) * (buff.layer - 1); //加成层级
                    //不能叠加 取最大最小值
                    if (item.Value.GetByID(1) == 1)
                    {
                        if (val < 0) //减益效果
                        {
                            var curVal = systemDebuffAttrMin.GetAttrByType(type);
                            if (curVal > val)
                                systemDebuffAttrMin.SetAttrByType(type, val);
                        }
                        else //增益效果
                        {
                            var curVal = systemAttrMax.GetAttrByType(type);
                            if (curVal < val)
                                systemAttrMax.SetAttrByType(type, val);
                        }
                    }
                    else
                    {   //能叠加
                        systemAttr.AddAttrByType(type, val);
                    }
                }
            });

            systemAttr.AddAttr(systemAttrMax); //添加属性
            systemAttr.AddAttr(systemDebuffAttrMin); //添加属性



            owner.ChangeSystemAttr(ATTR_SYS.BUFF_SYSTEM, systemAttr);   //改变系统属性
        }


        /// <summary>
        ///触发效果  处理被动触发效果
        /// </summary>
        /// <param name="buffInfo"></param>
        public void TriggerSKillEffect(BuffInfo buffInfo, Avatar target)
        {
            DoAttrChangeEffect(buffInfo, BUFF_EFFECT_TYPE.TRIGGER_EFF, false, target);
            if (buffInfo.config.labType == (int)BUFF_LAB_TYPE.CAN_LAB)  //层数判定才做减少层数
            {
                buffInfo.ReduceLayer();
            }
        }

        /// <summary>
        /// 执行buff效果  
        /// </summary>
        /// <param name="buffInfo"></param>
        void DoAttrChangeEffect(BuffInfo buffInfo, BUFF_EFFECT_TYPE type , bool isCalcAttr = true, Avatar target = null)
        {
            //属性改变
            if (isCalcAttr && IsAttrChange(buffInfo))
            {
                CalcAttr();
            }

            //不做伤害计算 交给其他系统
            //回收 立即触发
            if (AvatarHelper.HasBuffEffectType(buffInfo, type) && buffInfo.config.Param1 != null)
            {
                for (int i = 0; i < buffInfo.config.Param1.Length; i++)
                {
                    if (owner.IsDeath()) return;

                    var param = buffInfo.config.Param1[i];
                    if (param[0] == 1)  //如果技能ID  伤害效果  或者治疗效果 
                    {
                        if (type == BUFF_EFFECT_TYPE.INTERVAL)
                        {
                            //BroadcastBuffEvent(buffInfo, BUFF_OPT.UPD); //播放效果
                        }
                        CombatCalcHelper.BuffEffect(buffInfo, param[1]);    //换流程计算伤害 或者治疗效果
                    }
                 

                }
            }

        }


        internal void BroadcastBuffEvent(BuffInfo buff, BUFF_OPT opt)
        {
            //通知buff时间
            //可以通知到对应的玩家
            //如果需要广播也在这里操作

            //BattlePack.Get().Init_BuffInfoPack(buff, opt).Send();
        }


        /// <summary>
        /// 是否属性改变
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        bool IsAttrChange(BuffInfo buff)
        {
            return AvatarHelper.HasBuffEffectType(buff, BUFF_EFFECT_TYPE.ATTR_CHANGE);
        }

        /// <summary>
        /// 是否间隔触发
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        bool IsInterval(BuffInfo buff)
        {
            return AvatarHelper.HasBuffEffectType(buff, BUFF_EFFECT_TYPE.INTERVAL);
        }



        /// <summary>
        /// 定时器回调   当前游戏是不需要时间 以回合触发  这里的
        /// </summary>
        public void OnTick(bool before = true)
        {

            if (before)
            {
                //触发buff
                buffList.ForEach((buff) =>
                {
                    buff.endTime--;

                    if (IsInterval(buff))
                    {
                        DoAttrChangeEffect(buff,  BUFF_EFFECT_TYPE.INTERVAL,false);   //触发buff
                    }
                });
            }
            else
            {

                //删除buff
                DelBuff((buff) =>
                {
                    return buff.endTime <= 0;
                });

                buffList.ForEach((buff) =>
                {
                    BroadcastBuffEvent(buff, BUFF_OPT.UPD_NOT);
                });
            }

            //if (owner.IsDeath())    //死亡清楚bufff
            //{
            //    DelBuff((buff) =>
            //    {
            //        return true;
            //    });
            //}
         

        }

        internal void OnDeath(Avatar killer, DamageCalcPack skill)
        {
            if (!owner.InOnTick)    //不在自己回合死亡
            {
                ClearAll(); //删除所有buff
            }

        }


        /// <summary>
        /// 删除buff时的回调
        /// </summary>
        /// <param name="buffInfo"></param>
        void OnDelBuff(BuffInfo buffInfo)
        {
            BroadcastBuffEvent(buffInfo, BUFF_OPT.DEL);

            //GetBuff(,);
        }

        /// <summary>
        /// 删除buff  先清空 在广播  不然BroadcastBuffEvent 判断buff状态可能存在问题
        /// </summary>
        /// <param name="action"></param>
        void DelBuff(Func<BuffInfo,bool> action)
        {
            bool changeAttr = false;
            List<BuffInfo> clearList = null;

            //删除buff
            buffList.RemoveFunc(action,
            (buff) =>
            {
                if (clearList == null) clearList = ListPool<BuffInfo>.Get();
                clearList.Add(buff);    //缓存需要清理的buff
            });

            //清除buff
            //此时buff并没有在buffList中删除  可以缓存下  延迟广播
            //记得回收buff
            if (clearList != null)
            {
                clearList.ForEach((buff) =>
                {
                    if (!changeAttr && IsAttrChange(buff))
                        changeAttr = true;
                    //AvatarHelper.LogError(buff.uniqueID);

                    OnDelBuff(buff);
                    buff.Release(); //回收
                });

                clearList.Clear();
                clearList.Release();
            }

            if (changeAttr)
                CalcAttr();

        }

        public bool DelBuffByUniqueID(ulong uniqueID)
        {
            bool f = false;
            DelBuff((buff) =>
            {
                if (!f) f = buff.uniqueID == uniqueID;
                return buff.uniqueID == uniqueID;
            });
            return f;
        }


        
        //后续有时间在做优化  对不同 BUFF_EFFECT_TYPE 类型进行分类存存字典
        //--------------------------特殊效果检测 ------------------------------

        /// <summary>
        /// 是否眩晕
        /// </summary>
        /// <returns></returns>
        public bool IsDizzy()
        {
            return IsStateEffect(BUFF_STATE_EFFECT.DIZZY);
        }

        /// <summary>
        /// 无敌
        /// </summary>
        /// <returns></returns>
        public bool IsInvincible()
        {
            return IsStateEffect(BUFF_STATE_EFFECT.INVINCIBLE);
        }

        /// <summary>
        /// 是否是对应的 状态效果
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsStateEffect(BUFF_STATE_EFFECT type)
        {
            return GetStateEffect(type) != null;
        }


        /// <summary>
        /// 获得对应buff效果处理
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public BuffInfo GetStateEffect(BUFF_STATE_EFFECT type)
        {
            BuffInfo t = buffList.Find((buff) =>
            {
                return buff.IsStateEffect(type);
            });
            return t;
        }

        /// <summary>
        /// 获得对应buff效果处理
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<BuffInfo>  GetStateEffectList(BUFF_STATE_EFFECT type)
        {
            List<BuffInfo> list = ListPool<BuffInfo>.Get();
            buffList.ForEach((buff) =>
            {
                if (buff.IsStateEffect(type))
                {
                    list.Add(buff);
                }
            });

            if (list.Count < 0)
            {
                return null;
            }
            return list;
        }


     

        //------------------------------其他api------------------------------------



        /// <summary>
        /// 获得debuff的数量
        /// </summary>
        /// <returns></returns>
        public int GetDebuffCount()
        {
            int count = 0;
            buffList.ForEach((info)=> {
                if (info.config.IsDebuff())
                {
                    count++;
                }
            });
            return count;
        }


        public List<BuffInfo> GetAllDebuffInfo()
        {
            List<BuffInfo> list = ListPool<BuffInfo>.Get();
            buffList.ForEach((info) =>
            {
                if (info.config.IsDebuff())
                {
                    list.Add(info);
                }
            });
            return list;
        }

        #region 免疫效果

        /// <summary>
        /// 能免疫的    buff    效果
        /// </summary>
        bool CanByImmune(BuffCfg config)
        {
            return config.IsDebuff();
        }

        #endregion


   


    }

}
