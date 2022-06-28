using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LGF;

namespace N_Avatar
{
    using BuffCfg = BuffConfigRow;

    public class BuffInfo : Poolable<BuffInfo>
    {
        public ulong uniqueID;
        public ulong ownerID;
        public Avatar owner;
        public Avatar caster;
        public N_Attribute.Attr attr { get; private set; }   //快照属性
        public int buffid;
        public BuffCfg config;
        public int endTime;   //结束时间  当前是次数的意思
        public int layer;     //层级

        protected override void OnGet()
        {
            base.OnGet();
            ownerID = 0;
            caster = null;
            buffid = 0;
            layer = 1;  //层级默认为1
            config = null;
            owner = null;
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            attr?.Release(); //回收
            attr = null;
            caster = null;
            owner = null;
            config = null;
        }

        internal BuffInfo Init(int buffID, Avatar caster, Avatar owner, BuffCfg buff,int labLayer)
        {
            this.buffid = buffID;
            this.config = buff;
            this.caster = caster;
            this.ownerID = owner.uniqueID;
            this.layer = labLayer != 0 ? labLayer : buff.labLayer; //初始层级
            this.owner = owner;
            this.uniqueID = AvatarHelper.GenBuffUniqueID(); //buff的唯一id
            if (caster != null) //快照属性
            {
                this.attr = N_Attribute.Attr.Get();
                this.attr.AddAttr(caster.GetAttr());
            }
            return this;
        }


        /// <summary>
        /// 减少层级
        /// </summary>
        internal void ReduceLayer(int layer = 1)
        {
            //owner.
            //层数减少
            this.layer -= layer;
            if (this.layer <= 0)
            {
                owner.DelBuffByUniqueID(uniqueID);  //删除后他会自动回收 后面
            }
            else
            {
                owner.buffSystem.BroadcastBuffEvent(this, BUFF_OPT.UPD_NOT);    //广播通知
            }
        }


        internal void DelBuff()
        {
            owner.DelBuffByUniqueID(uniqueID);  //
        }

        /// <summary>
        /// 拷贝信息
        /// </summary>
        public BuffInfo Copy(BuffInfo info)
        {
            this.buffid = info.buffid;
            this.config = info.config;
            this.caster = info.caster;
            this.ownerID = info.ownerID;
            this.layer = info.layer;
            this.owner = info.owner;
            this.uniqueID = info.uniqueID;
            if (info.caster != null) //快照属性
            {
                //回收
                this.attr?.Release();
                this.attr = N_Attribute.Attr.Get();
                this.attr.AddAttr(info.attr);
            }
            return this;
        }



        /// <summary>
        /// 是否是对应的 状态效果
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsStateEffect(BUFF_STATE_EFFECT type)
        {
            return AvatarHelper.HasBuffEffectType(config, BUFF_EFFECT_TYPE.STATE)
                && AvatarHelper.HasBuffStateEffectType(config, type);
        }

    }



    #region BuffCfg

    //public class BuffCfg
    //{

    //    /// <summary>
    //    /// id
    //    /// </summary>
    //    public int id;

    //    /// <summary>
    //    /// 组id(目前用不上)
    //    /// </summary>
    //    public int groupID;

    //    /// <summary>
    //    /// 等级(目前用不上)
    //    /// </summary>
    //    public int grouplv;

    //    /// <summary>
    //    /// 名字
    //    /// </summary>
    //    public string name;

    //    /// <summary>
    //    /// 描述
    //    /// </summary>
    //    public string desc;

    //    /// <summary>
    //    /// 标记,1增益2减益
    //    /// </summary>
    //    public int flag;

    //    /// <summary>
    //    /// 叠加类型:1.不做处理(比如瞬间回血) 2.可叠加   3可覆盖, 4.可以多个同时存在 
    //    /// </summary>
    //    public int labType;

    //    /// <summary>
    //    /// 叠加方式1.时间 2.叠加层数刷新时间
    //    /// </summary>
    //    public int labMode;

    //    /// <summary>
    //    /// buff效果类型
    //    /// </summary>
    //    public int buffEffectType;

    //    /// <summary>
    //    /// 间隔触发
    //    /// </summary>
    //    public int interval;

    //    /// <summary>
    //    /// 属性列表(简单属性)
    //    /// </summary>
    //    public Dictionary<int, int[]> attrList;

    //    /// <summary>
    //    /// 加成参数(属性万分比)
    //    /// </summary>
    //    public Dictionary<int, int[]> additionParam1;

    //    /// <summary>
    //    /// 加成参数2(固定值)
    //    /// </summary>
    //    public int additionParam2;

    //}

    #endregion



}
