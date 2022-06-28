
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public interface IJsonTable
{
    void ReLoad();
}

public partial class JsonManager
{
    private static JsonManager jm = new JsonManager();
    public static JsonManager Instance => jm;
    private JsonManager() { }
    private string jsonDirectory = "AllJsonDatas/" ;
    private Dictionary<string,IJsonTable> allTableDic = new Dictionary<string, IJsonTable>();
    public JObject ReadJson(string jsonName)
    {
        jsonName = jsonDirectory + jsonName;
        TextAsset textAsset = Resources.Load<TextAsset>(jsonName);
        if (textAsset == null)
        {
            Debug.LogError("没有找到文件：" + jsonName);
            return null;
        }
        TextReader tr = new StringReader(textAsset.text);
        JsonReader jr = new JsonTextReader(tr);
        JObject jObject = (JObject)JToken.ReadFrom(jr);
        tr.Close();
        jr.Close();
        return jObject;

    }

    // 添加一张表
    public void AddTable(string jsonName,IJsonTable jsonTable)
    {
        if (allTableDic.ContainsKey(jsonName))
        {
            allTableDic[jsonName] = jsonTable;
        }
        else
        {
            allTableDic.Add(jsonName, jsonTable);
        }
    }
    /// <summary>
    /// 重新加载所有表格,除非用于服务器不停服更新表格，一般用不到这个
    /// </summary>
    public void ReLoadAllJsonTable()
    {
        var ie = allTableDic.GetEnumerator();
        while (ie.MoveNext())
        {
            ie.Current.Value.ReLoad();
        }
        ie.Dispose();
    } 

    public int Get_int(string cellString)
    {
        int ret = 0;
        Int32.TryParse(cellString, out ret);
        return ret;
    }
    public string Get_string(string cellString)
    {
        return cellString;
    }

}

public class BuffConfigRow 
{
    
    /// <summary>
    /// id
    /// </summary>
	public int id;
	
    /// <summary>
    /// 图标
    /// </summary>
	public string icon;
	
    /// <summary>
    /// 组id
    /// </summary>
	public int groupID;
	
    /// <summary>
    /// 组等级
    /// </summary>
	public int grouplv;
	
    /// <summary>
    /// 名字
    /// </summary>
	public string name;
	
    /// <summary>
    /// 描述
    /// </summary>
	public string desc;
	
    /// <summary>
    /// 标记,1增益2减益
    /// </summary>
	public int flag;
	
    /// <summary>
    /// 区分施法者1.表示区分 0表示不区分
    /// </summary>
	public int differentiateCaster;
	
    /// <summary>
    /// 叠加类型:0.不做处理(比如瞬间回血) 1.可叠加   2可覆盖, 3.可以多个同时存在(目前没有需求)
    /// </summary>
	public int labType;
	
    /// <summary>
    /// 叠加方式1.时间 2.叠加层数
    /// </summary>
	public int labMode;
	
    /// <summary>
    /// 叠加的最大值
    /// </summary>
	public int labLayerMax;
	
    /// <summary>
    /// 添加层级
    /// </summary>
	public int labLayer;
	
    /// <summary>
    /// buff效果类型1.属性变更 2.间隔buff 3.特殊效果 4.立即生效且不持续的buff 
    /// </summary>
	public Flag buffEffectType;
	
    /// <summary>
    /// buff类型1.DOT  2.昏迷  3.减速 (没用上) 弃用
    /// </summary>
	public Flag buffTypeFlag;
	
    /// <summary>
    /// 持续时间(回合数)
    /// </summary>
	public int time;
	
    /// <summary>
    /// 间隔触发(没用上,后面有多回合的时候使用)
    /// </summary>
	public int interval;
	
    /// <summary>
    /// 属性列表(简单属性) 属性ID：属性值,(能否叠加0表示能叠加1表示取最大值),(层数加的数值 * [层级 -1])
    /// </summary>
	public Dictionary<int,int[]> attrList;
	
    /// <summary>
    /// 参数 (类型,参数1，参数2.....)   类型 1.技能,技能id   

    /// </summary>
	public int[][] Param1;
	
    /// <summary>
    /// 特殊效果参数(后面复合需求在改)
    /// </summary>
	public int Param2;
	
    /// <summary>
    /// 效果文本
    /// </summary>
	public string NameText;
	
    /// <summary>
    /// 特效路径
    /// </summary>
	public string EffectPath;
	
    /// <summary>
    /// 特效周期时间(填入-1时是特效自身周期)
    /// </summary>
	public float EffectDelay;
	
    /// <summary>
    /// 偏移值
    /// </summary>
	public float OffsetY;
	
    /// <summary>
    /// 音效编号
    /// </summary>
	public int MuiscId;
	
}

public class BuffConfig : IJsonTable 
{
    private Dictionary<int, BuffConfigRow> rows;
    private static BuffConfig instance;
    public static BuffConfig Instance => instance ?? (instance = new BuffConfig());
    private BuffConfig()
    {
        rows = new Dictionary<int, BuffConfigRow>();
        Init();
    }

    private string jsonName = "BuffConfig";
    private void Init()
    {
        JsonManager jm = JsonManager.Instance;
        JObject json = jm.ReadJson(jsonName);
        if (json == null || !json.HasValues)
        {
            return;
        }
        int index = 0;
        foreach (var row in json["BuffConfig"])
        {
            if (index < 2)
            {
                index++;
                continue;
            }
            BuffConfigRow r = new BuffConfigRow();
            
            r.id = jm.Get_int((string)row["id"]);
			r.icon = jm.Get_string((string)row["icon"]);
			r.groupID = jm.Get_int((string)row["groupID"]);
			r.grouplv = jm.Get_int((string)row["grouplv"]);
			r.name = jm.Get_string((string)row["name"]);
			r.desc = jm.Get_string((string)row["desc"]);
			r.flag = jm.Get_int((string)row["flag"]);
			r.differentiateCaster = jm.Get_int((string)row["differentiateCaster"]);
			r.labType = jm.Get_int((string)row["labType"]);
			r.labMode = jm.Get_int((string)row["labMode"]);
			r.labLayerMax = jm.Get_int((string)row["labLayerMax"]);
			r.labLayer = jm.Get_int((string)row["labLayer"]);
			r.buffEffectType = jm.Get_Flag((string)row["buffEffectType"]);
			r.buffTypeFlag = jm.Get_Flag((string)row["buffTypeFlag"]);
			r.time = jm.Get_int((string)row["time"]);
			r.interval = jm.Get_int((string)row["interval"]);
			r.attrList = jm.Get_Dictionary_int_int_array((string)row["attrList"]);
			r.Param1 = jm.Get_int_array_array((string)row["Param1"]);
			r.Param2 = jm.Get_int((string)row["Param2"]);
			r.NameText = jm.Get_string((string)row["NameText"]);
			r.EffectPath = jm.Get_string((string)row["EffectPath"]);
			r.EffectDelay = jm.Get_float((string)row["EffectDelay"]);
			r.OffsetY = jm.Get_float((string)row["OffsetY"]);
			r.MuiscId = jm.Get_int((string)row["MuiscId"]);
			

            if (!rows.ContainsKey(r.id))
            {
                rows.Add(r.id, r);
            }
            else
            {
                Debug.LogError("重复的key：" + r.id + ";请检查表：" + jsonName);
            }
        }

        jm.AddTable(jsonName,this);
    }

    //什么也不做，需要提前加载时调用,然后构造方法会加载数据
    public void PreLoad()
    {

    }

    /// <summary>
    /// 重新加载，除非用于服务器不停服更新表格，一般用不到这个
    /// </summary>
    public void ReLoad()
    {
        instance = new BuffConfig();
    }

    public BuffConfigRow this[int key]
    {
        get
        {
            if (rows.ContainsKey(key))
            {
                return rows[key];
            }
            else
            {
                Debug.LogError("没有这个key：" + key+ ";请检查表：" + jsonName);
                return null;
            }
        }
    }
    public Dictionary<int, BuffConfigRow> GetAllRows()
    {
        return rows;
    }

    public static Dictionary<int, BuffConfigRow> GetAll()
    {
        return Instance.rows;
    }

    public static BuffConfigRow GetByID(int id)
    {
        var rows_ = Instance.rows;
        if (!rows_.ContainsKey(id)) return null;
        return rows_[id];
    }
}

public class CarRow 
{
    
    /// <summary>
    /// 索引名称
    /// </summary>
	public int Index;
	
    /// <summary>
    /// 交通工具名称
    /// </summary>
	public string CarName;
	
    /// <summary>
    /// 交通工具图片地址
    /// </summary>
	public string CarIcon;
	
    /// <summary>
    /// 交通工具每月消耗
    /// </summary>
	public int CarExpend;
	
    /// <summary>
    /// 交通工具使用费
    /// </summary>
	public int CarNeedMoney;
	
    /// <summary>
    /// 交通工具解锁条件(学业)
    /// </summary>
	public string[] CarTerm;
	
    /// <summary>
    /// 交通工具信息说明
    /// </summary>
	public Dictionary<string,int> CarMessage;
	
}

public class Car : IJsonTable 
{
    private Dictionary<int, CarRow> rows;
    private static Car instance;
    public static Car Instance => instance ?? (instance = new Car());
    private Car()
    {
        rows = new Dictionary<int, CarRow>();
        Init();
    }

    private string jsonName = "Car";
    private void Init()
    {
        JsonManager jm = JsonManager.Instance;
        JObject json = jm.ReadJson(jsonName);
        if (json == null || !json.HasValues)
        {
            return;
        }
        int index = 0;
        foreach (var row in json["Car"])
        {
            if (index < 2)
            {
                index++;
                continue;
            }
            CarRow r = new CarRow();
            
            r.Index = jm.Get_int((string)row["Index"]);
			r.CarName = jm.Get_string((string)row["CarName"]);
			r.CarIcon = jm.Get_string((string)row["CarIcon"]);
			r.CarExpend = jm.Get_int((string)row["CarExpend"]);
			r.CarNeedMoney = jm.Get_int((string)row["CarNeedMoney"]);
			r.CarTerm = jm.Get_string_array((string)row["CarTerm"]);
			r.CarMessage = jm.Get_Dictionary_string_int((string)row["CarMessage"]);
			

            if (!rows.ContainsKey(r.Index))
            {
                rows.Add(r.Index, r);
            }
            else
            {
                Debug.LogError("重复的key：" + r.Index + ";请检查表：" + jsonName);
            }
        }

        jm.AddTable(jsonName,this);
    }

    //什么也不做，需要提前加载时调用,然后构造方法会加载数据
    public void PreLoad()
    {

    }

    /// <summary>
    /// 重新加载，除非用于服务器不停服更新表格，一般用不到这个
    /// </summary>
    public void ReLoad()
    {
        instance = new Car();
    }

    public CarRow this[int key]
    {
        get
        {
            if (rows.ContainsKey(key))
            {
                return rows[key];
            }
            else
            {
                Debug.LogError("没有这个key：" + key+ ";请检查表：" + jsonName);
                return null;
            }
        }
    }
    public Dictionary<int, CarRow> GetAllRows()
    {
        return rows;
    }

    public static Dictionary<int, CarRow> GetAll()
    {
        return Instance.rows;
    }

    public static CarRow GetByID(int id)
    {
        var rows_ = Instance.rows;
        if (!rows_.ContainsKey(id)) return null;
        return rows_[id];
    }
}

public class ClothesRow 
{
    
    /// <summary>
    /// 索引名称
    /// </summary>
	public int Index;
	
    /// <summary>
    /// 服装名称
    /// </summary>
	public string ClothesName;
	
    /// <summary>
    /// 服装说明
    /// </summary>
	public string ClothesMessage;
	
    /// <summary>
    /// 服装图片地址:半身像|全是像|图片
    /// </summary>
	public string[] ClothesIcon;
	
    /// <summary>
    /// 服装每月消耗金币
    /// </summary>
	public int ClothesExpend;
	
    /// <summary>
    /// 点击服饰角色的对话
    /// </summary>
	public Dictionary<string,int> ClothesSay;
	
    /// <summary>
    /// 购买服饰消耗金币
    /// </summary>
	public int ClothesBuy;
	
    /// <summary>
    /// 每月增加心情值
    /// </summary>
	public int AddHappyValue;
	
    /// <summary>
    /// 需要观看视频的次数
    /// </summary>
	public int ClothesVideo;
	
}

public class Clothes : IJsonTable 
{
    private Dictionary<int, ClothesRow> rows;
    private static Clothes instance;
    public static Clothes Instance => instance ?? (instance = new Clothes());
    private Clothes()
    {
        rows = new Dictionary<int, ClothesRow>();
        Init();
    }

    private string jsonName = "Clothes";
    private void Init()
    {
        JsonManager jm = JsonManager.Instance;
        JObject json = jm.ReadJson(jsonName);
        if (json == null || !json.HasValues)
        {
            return;
        }
        int index = 0;
        foreach (var row in json["Clothes"])
        {
            if (index < 2)
            {
                index++;
                continue;
            }
            ClothesRow r = new ClothesRow();
            
            r.Index = jm.Get_int((string)row["Index"]);
			r.ClothesName = jm.Get_string((string)row["ClothesName"]);
			r.ClothesMessage = jm.Get_string((string)row["ClothesMessage"]);
			r.ClothesIcon = jm.Get_string_array((string)row["ClothesIcon"]);
			r.ClothesExpend = jm.Get_int((string)row["ClothesExpend"]);
			r.ClothesSay = jm.Get_Dictionary_string_int((string)row["ClothesSay"]);
			r.ClothesBuy = jm.Get_int((string)row["ClothesBuy"]);
			r.AddHappyValue = jm.Get_int((string)row["AddHappyValue"]);
			r.ClothesVideo = jm.Get_int((string)row["ClothesVideo"]);
			

            if (!rows.ContainsKey(r.Index))
            {
                rows.Add(r.Index, r);
            }
            else
            {
                Debug.LogError("重复的key：" + r.Index + ";请检查表：" + jsonName);
            }
        }

        jm.AddTable(jsonName,this);
    }

    //什么也不做，需要提前加载时调用,然后构造方法会加载数据
    public void PreLoad()
    {

    }

    /// <summary>
    /// 重新加载，除非用于服务器不停服更新表格，一般用不到这个
    /// </summary>
    public void ReLoad()
    {
        instance = new Clothes();
    }

    public ClothesRow this[int key]
    {
        get
        {
            if (rows.ContainsKey(key))
            {
                return rows[key];
            }
            else
            {
                Debug.LogError("没有这个key：" + key+ ";请检查表：" + jsonName);
                return null;
            }
        }
    }
    public Dictionary<int, ClothesRow> GetAllRows()
    {
        return rows;
    }

    public static Dictionary<int, ClothesRow> GetAll()
    {
        return Instance.rows;
    }

    public static ClothesRow GetByID(int id)
    {
        var rows_ = Instance.rows;
        if (!rows_.ContainsKey(id)) return null;
        return rows_[id];
    }
}

public class SkillConfigRow 
{
    
    /// <summary>
    /// 标识
    /// </summary>
	public int id;
	
    /// <summary>
    /// 技能名称
    /// </summary>
	public string Name;
	
    /// <summary>
    /// 描述
    /// </summary>
	public string describe;
	
    /// <summary>
    /// 目标位置类型
    /// </summary>
	public int targetPosType;
	
    /// <summary>
    /// 作用于
    /// </summary>
	public bool actOnEnemy;
	
    /// <summary>
    /// 冷却回合
    /// </summary>
	public int cd;
	
    /// <summary>
    /// 初始回合数
    /// </summary>
	public int cd_start;
	
    /// <summary>
    /// 效果类型
    /// </summary>
	public Flag effType;
	
    /// <summary>
    /// 参数
    /// </summary>
	public int[][] Param1;
	
    /// <summary>
    /// 释放技能的条件
    /// </summary>
	public int[][] CastCond;
	
    /// <summary>
    /// 释放技能后-条件分支检测
    /// </summary>
	public int[][] condition;
	
    /// <summary>
    /// 条件触发(检测完成才会加成)
    /// </summary>
	public int[][][] conditionCompletion;
	
    /// <summary>
    /// 触发技能时加成属性
    /// </summary>
	public Dictionary<int,int> skillAttr;
	
    /// <summary>
    /// 伤害类型
    /// </summary>
	public int DamageType;
	
    /// <summary>
    /// 连续释放次数（随机释放区间）
    /// </summary>
	public int[] count;
	
    /// <summary>
    /// 特殊效果参数(1:技能吸血百分比 后看看情况添加实现)
    /// </summary>
	public int[] param2;
	
    /// <summary>
    /// 后续技能ID(有时候会有复合型技能,复合型技能在这里处理,比 如给对面造成伤害 并给自己增加回血 buff等)
    /// </summary>
	public int[] nextSkill;
	
    /// <summary>
    /// 条件触发(a,...) a表示触发类型 后面表示参数 
    /// </summary>
	public Dictionary<int,int[][]> param3;
	
    /// <summary>
    /// 站位前后偏移
    /// </summary>
	public float Distance;
	
    /// <summary>
    /// 站位偏移时间
    /// </summary>
	public float DelayTime;
	
    /// <summary>
    /// 技能动画高帧特效适配
    /// </summary>
	public float AnimHeightLine;
	
}

public class SkillConfig : IJsonTable 
{
    private Dictionary<int, SkillConfigRow> rows;
    private static SkillConfig instance;
    public static SkillConfig Instance => instance ?? (instance = new SkillConfig());
    private SkillConfig()
    {
        rows = new Dictionary<int, SkillConfigRow>();
        Init();
    }

    private string jsonName = "SkillConfig";
    private void Init()
    {
        JsonManager jm = JsonManager.Instance;
        JObject json = jm.ReadJson(jsonName);
        if (json == null || !json.HasValues)
        {
            return;
        }
        int index = 0;
        foreach (var row in json["SkillConfig"])
        {
            if (index < 2)
            {
                index++;
                continue;
            }
            SkillConfigRow r = new SkillConfigRow();
            
            r.id = jm.Get_int((string)row["id"]);
			r.Name = jm.Get_string((string)row["Name"]);
			r.describe = jm.Get_string((string)row["describe"]);
			r.targetPosType = jm.Get_int((string)row["targetPosType"]);
			r.actOnEnemy = jm.Get_bool((string)row["actOnEnemy"]);
			r.cd = jm.Get_int((string)row["cd"]);
			r.cd_start = jm.Get_int((string)row["cd_start"]);
			r.effType = jm.Get_Flag((string)row["effType"]);
			r.Param1 = jm.Get_int_array_array((string)row["Param1"]);
			r.CastCond = jm.Get_int_array_array((string)row["CastCond"]);
			r.condition = jm.Get_int_array_array((string)row["condition"]);
			r.conditionCompletion = jm.Get_int_array_array_array((string)row["conditionCompletion"]);
			r.skillAttr = jm.Get_Dictionary_int_int((string)row["skillAttr"]);
			r.DamageType = jm.Get_int((string)row["DamageType"]);
			r.count = jm.Get_int_array((string)row["count"]);
			r.param2 = jm.Get_int_array((string)row["param2"]);
			r.nextSkill = jm.Get_int_array((string)row["nextSkill"]);
			r.param3 = jm.Get_Dictionary_int_int_array_array((string)row["param3"]);
			r.Distance = jm.Get_float((string)row["Distance"]);
			r.DelayTime = jm.Get_float((string)row["DelayTime"]);
			r.AnimHeightLine = jm.Get_float((string)row["AnimHeightLine"]);
			

            if (!rows.ContainsKey(r.id))
            {
                rows.Add(r.id, r);
            }
            else
            {
                Debug.LogError("重复的key：" + r.id + ";请检查表：" + jsonName);
            }
        }

        jm.AddTable(jsonName,this);
    }

    //什么也不做，需要提前加载时调用,然后构造方法会加载数据
    public void PreLoad()
    {

    }

    /// <summary>
    /// 重新加载，除非用于服务器不停服更新表格，一般用不到这个
    /// </summary>
    public void ReLoad()
    {
        instance = new SkillConfig();
    }

    public SkillConfigRow this[int key]
    {
        get
        {
            if (rows.ContainsKey(key))
            {
                return rows[key];
            }
            else
            {
                Debug.LogError("没有这个key：" + key+ ";请检查表：" + jsonName);
                return null;
            }
        }
    }
    public Dictionary<int, SkillConfigRow> GetAllRows()
    {
        return rows;
    }

    public static Dictionary<int, SkillConfigRow> GetAll()
    {
        return Instance.rows;
    }

    public static SkillConfigRow GetByID(int id)
    {
        var rows_ = Instance.rows;
        if (!rows_.ContainsKey(id)) return null;
        return rows_[id];
    }
}

public class SoldierRow 
{
    
    /// <summary>
    /// 士兵编号
    /// </summary>
	public int SoldierId;
	
    /// <summary>
    /// 模型Id
    /// </summary>
	public int ModleId;
	
    /// <summary>
    /// 名称
    /// </summary>
	public String Name;
	
    /// <summary>
    /// 士兵类型
    /// </summary>
	public int Type;
	
    /// <summary>
    /// 属性
    /// </summary>
	public Dictionary<int,int> Attr;
	
}

public class Soldier : IJsonTable 
{
    private Dictionary<int, SoldierRow> rows;
    private static Soldier instance;
    public static Soldier Instance => instance ?? (instance = new Soldier());
    private Soldier()
    {
        rows = new Dictionary<int, SoldierRow>();
        Init();
    }

    private string jsonName = "Soldier";
    private void Init()
    {
        JsonManager jm = JsonManager.Instance;
        JObject json = jm.ReadJson(jsonName);
        if (json == null || !json.HasValues)
        {
            return;
        }
        int index = 0;
        foreach (var row in json["Soldier"])
        {
            if (index < 2)
            {
                index++;
                continue;
            }
            SoldierRow r = new SoldierRow();
            
            r.SoldierId = jm.Get_int((string)row["SoldierId"]);
			r.ModleId = jm.Get_int((string)row["ModleId"]);
			r.Name = jm.Get_String((string)row["Name"]);
			r.Type = jm.Get_int((string)row["Type"]);
			r.Attr = jm.Get_Dictionary_int_int((string)row["Attr"]);
			

            if (!rows.ContainsKey(r.SoldierId))
            {
                rows.Add(r.SoldierId, r);
            }
            else
            {
                Debug.LogError("重复的key：" + r.SoldierId + ";请检查表：" + jsonName);
            }
        }

        jm.AddTable(jsonName,this);
    }

    //什么也不做，需要提前加载时调用,然后构造方法会加载数据
    public void PreLoad()
    {

    }

    /// <summary>
    /// 重新加载，除非用于服务器不停服更新表格，一般用不到这个
    /// </summary>
    public void ReLoad()
    {
        instance = new Soldier();
    }

    public SoldierRow this[int key]
    {
        get
        {
            if (rows.ContainsKey(key))
            {
                return rows[key];
            }
            else
            {
                Debug.LogError("没有这个key：" + key+ ";请检查表：" + jsonName);
                return null;
            }
        }
    }
    public Dictionary<int, SoldierRow> GetAllRows()
    {
        return rows;
    }

    public static Dictionary<int, SoldierRow> GetAll()
    {
        return Instance.rows;
    }

    public static SoldierRow GetByID(int id)
    {
        var rows_ = Instance.rows;
        if (!rows_.ContainsKey(id)) return null;
        return rows_[id];
    }
}

public class UIArmyInfosRow 
{
    
    /// <summary>
    /// 部队ID
    /// </summary>
	public int armyId;
	
    /// <summary>
    /// 部队图集名
    /// </summary>
	public string armyAtlasName;
	
    /// <summary>
    /// 头像图片路径
    /// </summary>
	public string armyTouXiang;
	
    /// <summary>
    /// 部队等级
    /// </summary>
	public string armyLevel;
	
    /// <summary>
    /// 部队兵种图片
    /// </summary>
	public string armyBingType;
	
    /// <summary>
    /// 部队阵法图标
    /// </summary>
	public string armyZhenType;
	
    /// <summary>
    /// 部队名称
    /// </summary>
	public string armyName;
	
}

public class UIArmyInfos : IJsonTable 
{
    private Dictionary<int, UIArmyInfosRow> rows;
    private static UIArmyInfos instance;
    public static UIArmyInfos Instance => instance ?? (instance = new UIArmyInfos());
    private UIArmyInfos()
    {
        rows = new Dictionary<int, UIArmyInfosRow>();
        Init();
    }

    private string jsonName = "UIArmyInfos";
    private void Init()
    {
        JsonManager jm = JsonManager.Instance;
        JObject json = jm.ReadJson(jsonName);
        if (json == null || !json.HasValues)
        {
            return;
        }
        int index = 0;
        foreach (var row in json["UIArmyInfos"])
        {
            if (index < 2)
            {
                index++;
                continue;
            }
            UIArmyInfosRow r = new UIArmyInfosRow();
            
            r.armyId = jm.Get_int((string)row["armyId"]);
			r.armyAtlasName = jm.Get_string((string)row["armyAtlasName"]);
			r.armyTouXiang = jm.Get_string((string)row["armyTouXiang"]);
			r.armyLevel = jm.Get_string((string)row["armyLevel"]);
			r.armyBingType = jm.Get_string((string)row["armyBingType"]);
			r.armyZhenType = jm.Get_string((string)row["armyZhenType"]);
			r.armyName = jm.Get_string((string)row["armyName"]);
			

            if (!rows.ContainsKey(r.armyId))
            {
                rows.Add(r.armyId, r);
            }
            else
            {
                Debug.LogError("重复的key：" + r.armyId + ";请检查表：" + jsonName);
            }
        }

        jm.AddTable(jsonName,this);
    }

    //什么也不做，需要提前加载时调用,然后构造方法会加载数据
    public void PreLoad()
    {

    }

    /// <summary>
    /// 重新加载，除非用于服务器不停服更新表格，一般用不到这个
    /// </summary>
    public void ReLoad()
    {
        instance = new UIArmyInfos();
    }

    public UIArmyInfosRow this[int key]
    {
        get
        {
            if (rows.ContainsKey(key))
            {
                return rows[key];
            }
            else
            {
                Debug.LogError("没有这个key：" + key+ ";请检查表：" + jsonName);
                return null;
            }
        }
    }
    public Dictionary<int, UIArmyInfosRow> GetAllRows()
    {
        return rows;
    }

    public static Dictionary<int, UIArmyInfosRow> GetAll()
    {
        return Instance.rows;
    }

    public static UIArmyInfosRow GetByID(int id)
    {
        var rows_ = Instance.rows;
        if (!rows_.ContainsKey(id)) return null;
        return rows_[id];
    }
}
