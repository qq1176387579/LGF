using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ExcelToJosn : EditorWindow
{
    private static string AllJsonDataPath = "/Resources/AllJsonDatas";

#if UNITY_EDITOR


    //#region 针对玩家功能
    //[UnityEditor.MenuItem("数据工具/打开玩家数据文件夹")]
    //public static void OpenLocalDataFolder()
    //{
    //    UnityEditor.EditorUtility.RevealInFinder(Paths.Json_PlayerData);
    //}

    //[UnityEditor.MenuItem("数据工具/删除玩家数据")]
    //public static void DeleteLocalData()
    //{
    //    try
    //    {
    //        //if (File.Exists(Paths.Json_PlayerData))
    //        //    File.Delete(Paths.Json_PlayerData);
    //    }
    //    catch (System.Exception error) 
    //    { 
    //        UnityEngine.Debug.LogError("删除失败 Error:" + error); 
    //    }
    //    EditorUtility.DisplayDialog("删除玩家数据", "删除成功！", "确认");
    //}

    //[UnityEditor.MenuItem("数据工具/清除PlayerPrefs")]
    //public static void ClearPlayerPrefs()
    //{
    //    PlayerPrefs.DeleteAll();
    //}
    //#endregion

    #region 转json

    //[UnityEditor.MenuItem("数据工具/打开策划数据文件夹")]
    //public static void OpenExcelFolder()
    //{
    //    UnityEditor.EditorUtility.RevealInFinder(Paths.Json_ExcelData);
    //}


    [MenuItem("Tools/ExcToJson/刷新AllJsonData")]
    public static int GenExcelToJson()
    {
        Process process = new Process();

        string workDirectory = Application.dataPath.Replace("Assets", "策划数据表/");
        process.StartInfo.WorkingDirectory = workDirectory;
        process.StartInfo.FileName = "00一键生成json.bat";

        process.Start();
        process.WaitForExit();
        UnityEngine.Debug.Log("刷新json..");
        return process.ExitCode;
    }
    #endregion

    [MenuItem("Tools/ExcToJson/不刷新AllJsonData 生成读取Json的代码")]
    private static void CreateClassEx()
    {
        string directory = Application.dataPath + AllJsonDataPath;
        if (Directory.Exists(directory))
        {
            if (string.IsNullOrEmpty(JsonManagePath))
            {
                JsonManagePath = Application.dataPath + "/Scritps/JsonManager.cs";
            }

            if (File.Exists(JsonManagePath))
            {
                File.Delete(JsonManagePath);
            }
            FileStream temp = File.Create(JsonManagePath);
            temp.Close();
            File.WriteAllText(JsonManagePath, baseClass.Replace("<quatation>", "\""));

            string[] paths = Directory.GetFiles(directory);

            List<string> configPath = new List<string>();
            //生成普通表
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].EndsWith(".json"))
                {
                    if (paths[i].EndsWith("_Config.json"))
                    {
                        configPath.Add(paths[i]);
                    }
                    else
                    {
                        StartCreate(paths[i]);
                    }
                }
            }
            //生成配置表
            for (int i = 0; i < configPath.Count; i++)
            {
                StartCreateConfig(configPath[i]);
            }

            EditorUtility.DisplayDialog("生成Json类", "生成Json类完成", "确认");
            AssetDatabase.Refresh();
        }
        else
        {
            UnityEngine.Debug.LogError("没有目录：" + directory);
        }
    }



    #region 生成解析json类
    [MenuItem("Tools/ExcToJson/生成读取Json的代码")]
    private static void CreateClass()
    {
        //先刷新表
        if (GenExcelToJson() != 0)
        {
            EditorUtility.DisplayDialog("生成json", "生成json文件出错，请检查表格", "确认");
            return;
        }

        CreateClassEx();
    }
 


    private static void StartCreate(string path)
    {
        JObject json = ReadJson(path);
        if (json == null || !json.HasValues)
        {
            return;
        }
        List<string> sheetNames = new List<string>();          //sheet名，类名
        List<string> rowClassName = new List<string>();        //一行，将作为一个数据块

        Dictionary<string, List<string>> sheetColNames = new Dictionary<string, List<string>>();             //列名，字段名
        Dictionary<string, List<string>> sheetColDescription = new Dictionary<string, List<string>>();      //备注，注释
        Dictionary<string, List<string>> sheetColTypes = new Dictionary<string, List<string>>();            //类型，变量类型
        string jsonName = Path.GetFileNameWithoutExtension(path);

        foreach (var j in json)
        {
            sheetNames.Add(j.Key);
            if (sheetColNames.ContainsKey(j.Key))
            {
                UnityEngine.Debug.LogError("重复的key：" + j.Key + ";\n请检查表：" + jsonName);
                return;
            }
            sheetColNames.Add(j.Key, new List<string>());
            sheetColTypes.Add(j.Key, new List<string>());
            sheetColDescription.Add(j.Key, new List<string>());


            int index = 0;

            if (j.Value.HasValues)
            {
                foreach (var child in j.Value.Children())
                {
                    if (index == 0)
                    {
                        foreach (var row in child)
                        {
                            string colName = GetJTokenKeyByPath(row.Path);
                            if (string.IsNullOrEmpty(colName) || colName.StartsWith("#"))
                            {
                                //跳过空列和注释列
                                continue;
                            }

                            sheetColNames[j.Key].Add(colName);
                            sheetColDescription[j.Key].Add(row.First.ToString());//添加注释
                        }
                    }
                    else if (index == 1)
                    {
                        foreach (var row in child)
                        {
                            string colName = GetJTokenKeyByPath(row.Path);
                            if (string.IsNullOrEmpty(colName) || colName.StartsWith("#"))
                            {
                                //跳过空列和注释列
                                continue;
                            }
                            sheetColTypes[j.Key].Add(row.First.ToString());//添加变量类型
                        }
                    }
                    else
                    {
                        break;
                    }

                    index++;
                }
            }

        }

        for (int i = 0; i < sheetNames.Count; i++)
        {
            rowClassName.Add(sheetNames[i] + "Row");
        }

        CreateCSJsonClass(jsonName, sheetNames, sheetColNames, sheetColTypes, sheetColDescription, rowClassName);

    }
    //配置表固定四列
    private static List<string> sheetConfigColName = new List<string>()
    {
        "FieldName",    //字段名
        "Des",          //注释，描述
        "Type",         //类型
        "Value"         //值
    };
    //生成配置
    private static void StartCreateConfig(string path)
    {
        JObject json = ReadJson(path);
        if (json == null || !json.HasValues)
        {
            return;
        }
        string sheetName = GetJTokenKeyByPath(json.First.Path);          //sheet名，类名
        List<string> sheetRowNames = new List<string>();            //行名，字段名
        List<string> sheetRowDescription = new List<string>();      //备注，注释
        List<string> sheetRowTypes = new List<string>();            //类型，变量类型
        List<string> sheetRowValues = new List<string>();           //值

        int head = 0;
        foreach (var row in json[sheetName])//只考虑sheet1
        {
            if (head < 2)//跳过前两行，因为配置模式的表格头没有意义
            {
                head++;
                continue;
            }

            string fieldName = (string)row[sheetConfigColName[0]];
            string des = (string)row[sheetConfigColName[1]];
            string type = (string)row[sheetConfigColName[2]];
            string value = (string)row[sheetConfigColName[3]];

            sheetRowNames.Add(fieldName);
            sheetRowDescription.Add(des);
            sheetRowTypes.Add(type);
            sheetRowValues.Add(value);
        }

        string config = tempConfigClass.Replace("<classname>", sheetName);
        string getValues = string.Empty;
        string propertys = string.Empty;
        for (int i = 0; i < sheetRowNames.Count; i++)
        {
            string funcName = sheetRowTypes[i];
            funcName = funcName.Replace("[]", "_array");//替换数组
            funcName = funcName.Trim('>');
            funcName = funcName.Replace(",", "_");
            funcName = funcName.Replace("<", "_");//替换<>，以及<,>.针对List<T>,及Dictionary<T1,T2>
            funcName = $"jm.Get_{funcName}(\"{sheetRowValues[i]}\")";
            getValues += $"{sheetRowNames[i]} = {funcName};\n\t\t";

            UnityEngine.Debug.Log(funcName);
            propertys += fieldSummary.Replace("<description>", sheetRowDescription[i]) + "\n\t";
            propertys += $"public {sheetRowTypes[i]} {sheetRowNames[i]}" + " {get;}\n\t";
        }

        config = config.Replace("<getvalue>", getValues);
        config = config.Replace("<propertys>", propertys);

        File.AppendAllText(JsonManagePath, config, Encoding.UTF8);
    }
    //将asset path转为win path
    private static string GetWinPathByAssetPath(string assetPath)
    {
        string winPath = Application.dataPath;
        return winPath.Replace("Assets", assetPath);
    }
    //读取json
    private static JObject ReadJson(string path)
    {
        if (!File.Exists(path))
        {
            UnityEngine.Debug.LogError("没有找到文件：" + path);
            return null;
        }
        StreamReader streamreader = new StreamReader(path);//读取数据，转换成数据流 //
        JsonTextReader reader = new JsonTextReader(streamreader);
        JObject jObject = (JObject)JToken.ReadFrom(reader);
        reader.Close();
        streamreader.Close();
        return jObject;
    }

    private static string GetJTokenKeyByPath(string path)
    {

        int index = path.LastIndexOf('.');

        string ret = path.Substring(index + 1);

        return ret;
    }

    private static string baseClass = @"
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
    private string jsonDirectory = <quatation>AllJsonDatas/<quatation> ;
    private Dictionary<string,IJsonTable> allTableDic = new Dictionary<string, IJsonTable>();
    public JObject ReadJson(string jsonName)
    {
        jsonName = jsonDirectory + jsonName;
        TextAsset textAsset = Resources.Load<TextAsset>(jsonName);
        if (textAsset == null)
        {
            Debug.LogError(<quatation>没有找到文件：<quatation> + jsonName);
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
";


    private static string fieldSummary = @"
    /// <summary>
    /// <description>
    /// </summary>";

    private static string tempRowClass = @"
public class <rowclass> <interfaceName>
{
    <rowclassfiled>
}
";

    private static string tempConfigClass = @"
public class <classname> 
{
    private static <classname> instance;
    public static <classname> Instance => instance ?? (instance = new <classname>());

    private <classname>()
    {
        JsonManager jm = JsonManager.Instance;
        <getvalue>
    }
    <propertys>
}
";

    private static string tempSheetClass = @"
public class <classname> : IJsonTable 
{
    private Dictionary<<keytype>, <rowclass>> rows;
    private static <classname> instance;
    public static <classname> Instance => instance ?? (instance = new <classname>());
    private <classname>()
    {
        rows = new Dictionary<<keytype>, <rowclass>>();
        Init();
    }

    private string jsonName = <jsonnamestring>;
    private void Init()
    {
        JsonManager jm = JsonManager.Instance;
        JObject json = jm.ReadJson(jsonName);
        if (json == null || !json.HasValues)
        {
            return;
        }
        int index = 0;
        foreach (var row in json[<classnamestring>])
        {
            if (index < 2)
            {
                index++;
                continue;
            }
            <rowclass> r = new <rowclass>();
            
            <jsondatatorow>

            if (!rows.ContainsKey(r.<ID>))
            {
                rows.Add(r.<ID>, r);
            }
            else
            {
                <hint1>
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
        instance = new <classname>();
    }

    public <rowclass> this[<keytype> key]
    {
        get
        {
            if (rows.ContainsKey(key))
            {
                return rows[key];
            }
            else
            {
                <hint2>
                return null;
            }
        }
    }
    public Dictionary<<keytype>, <rowclass>> GetAllRows()
    {
        return rows;
    }

    public static Dictionary<<keytype>, <rowclass>> GetAll()
    {
        return Instance.rows;
    }

    public static <rowclass> GetByID(<keytype> id)
    {
        var rows_ = Instance.rows;
        if (!rows_.ContainsKey(id)) return null;
        return rows_[id];
    }
}
";

    private static string hint1 = "Debug.LogError(\"重复的key：\" + r.<ID> + \";请检查表：\" + jsonName);";
    private static string hint2 = "Debug.LogError(\"没有这个key：\" + key+ \";请检查表：\" + jsonName);";


    private static string JsonManagePath;
    private static void CreateCSJsonClass(string jsonName, List<string> className, Dictionary<string, List<string>> colNameDic, Dictionary<string, List<string>> colTypes, Dictionary<string, List<string>> colDesDic, List<string> rowDataName)
    {
        int i = 0;
        if (i < className.Count)
        {
            List<string> colName = colNameDic[className[i]];
            List<string> colType = colTypes[className[i]];
            List<string> colDes = colDesDic[className[i]];
            string script = tempSheetClass.Replace("<hint1>", hint1);
            script = script.Replace("<hint2>", hint2);
            WriteSheetClassString(script, colName, colType, colDes, jsonName, className[i], rowDataName[i]);
            //2020-05-12 只考虑sheet1的内容
        }

        //for (int i = 0; i < className.Count; i++)
        //    break;
    }

    private static void WriteSheetClassString(string script, List<string> colName, List<string> colType, List<string> colDes, string jsonName, string className, string rowClassName)
    {
        string giveData = string.Empty;
        string rowClassFiled = string.Empty;
        string interfaceName = string.Empty;
        for (int i = 0; i < colName.Count; i++)
        {
            if (string.IsNullOrEmpty(colType[i]))
            {
                continue;
            }

            string funcName = colType[i];
            funcName = funcName.Replace(">", string.Empty);
            funcName = funcName.Replace("[]", "_array");//替换数组
            funcName = funcName.Replace(",", "_");
            funcName = funcName.Replace("<", "_");//替换<>，以及<,>.针对List<T>,及Dictionary<T1,T2>

            //解析数据类型的方法名
            giveData += "r." + colName[i] + " = " + "jm.Get_" + funcName + "((string)row[\"" + colName[i] + "\"]);\n\t\t\t";

            //添加注释
            rowClassFiled += fieldSummary.Replace("<description>", colDes[i]) + "\n\t";
            //添加字段
            rowClassFiled += "public " + colType[i] + " " + colName[i] + ";\n\t";

            CheckInterfaceName(colName[i], ref interfaceName, ref rowClassFiled);
        }


        script = script.Replace("<jsondatatorow>", giveData);
        script = script.Replace("<classname>", className);
        script = script.Replace("<rowclass>", rowClassName);
        script = script.Replace("<jsonnamestring>", "\"" + jsonName + "\"");
        script = script.Replace("<classnamestring>", "\"" + className + "\"");
        script = script.Replace("<ID>", colName[0]);
        script = script.Replace("<IDstring>", "\"" + colName[0] + "\"");
        script = script.Replace("<annotationflag>", "\"#\"");
        script = script.Replace("<keytype>", colType[0]);

        string rowClassScript = tempRowClass.Replace("<rowclass>", rowClassName);
        rowClassScript = rowClassScript.Replace("<interfaceName>", interfaceName);
        rowClassScript = rowClassScript.Replace("<rowclassfiled>", rowClassFiled);

        File.AppendAllText(JsonManagePath, rowClassScript, Encoding.UTF8);
        File.AppendAllText(JsonManagePath, script, Encoding.UTF8);
    }


    /// <summary>
    /// 获得接口名
    /// </summary>
    static void CheckInterfaceName(string name,ref string interfaceName, ref string rowClassFiled)
    {
        switch (name)
        {
            case "weight":
                AddInterfaceName(ref interfaceName, "LH.IWeight");
                rowClassFiled += "int LH.IWeight.Weight { get => weight; }\n\t";
                break;
            default:
                break;
        }
    }

    static void AddInterfaceName(ref string interfaceName, string name)
    {
        if (string.IsNullOrEmpty(interfaceName))
        {
            interfaceName = ":" + name;
        }
        else
        {
            interfaceName += "," + interfaceName;
        }
    }


    #endregion



    #region 数值工具
    //[UnityEditor.MenuItem("游戏中数值设置/添加金币")]
    //public static void AddMoneyTool()
    //{
    //    //MoneyModelManager.AddMoney(100000);
    //}
    //[UnityEditor.MenuItem("游戏中数值设置/生命心情填满")]
    //public static void AddLifeHappyTool()
    //{
    //    //CommonDataModelManager.AddLifeValue(100);
    //    //CommonDataModelManager.AddHappyValue(100);
    //}
    #endregion

#endif

}
