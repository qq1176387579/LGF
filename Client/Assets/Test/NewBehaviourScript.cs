/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/2 14:14:38
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;
using System.Text.RegularExpressions;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError(Regex.Replace("<img>abc</img>", @"<img>(?<11111>.*?)</img>", "[图片]${11111}"));

        string input = @"<img src=""file://c:\abc.png""></img>";
        string pattern = @"(?<=<img src="").+?(?=""></img>)";
        string replacement = "guid";
        var val = Regex.Replace(input, pattern, replacement);

        Debug.LogError(val);

        string input2 = @"System.NullReferenceException: Object reference not set to an instance of an object
                          at ArmyUnit.OnUpdate()[0x00045] in D:\Unity project\clzc\clzcDemo\Assets\_Test\Lh2\Sripts\ArmyUnit.cs:248
                          at LGF.MonoBase.OnUpdate_(LGF.IEventDataBase ev)[0x00009] in D:\Unity project\clzc\clzcDemo\Assets\_Test\Lh2\LHLGF\_LGF\Tool\Mono\MonoBase.cs:56
                          at LGF.EventDispatcher`1[T].Fire(LGF.IEventDataBase msg)[0x0001c] in D:\Unity project\clzc\clzcDemo\Assets\_Test\Lh2\LHLGF\_LGF\Manager\Helper\Event\EventDispatcher.cs:113 ";
        //string pattern2 = $" at ([.]+) in [.]+$";
        //string pattern2 = $" at .*(.*).*";

        //\[0x(.*)\] 匹配 [0x0001c] 
        string pattern2 = @" at (.*)\[0x(.*)\] in (.*)\\Assets\\(.*)\.cs:[0-9]+";
        //string regularExpression = @"\.cs:[0-9]";
        //Regex.Replace(input2, pattern2);
        var tmp = Regex.Matches(input2, pattern2, RegexOptions.IgnoreCase | RegexOptions.Multiline);

        foreach (var item in tmp)
        {
            Debug.LogError(item);
        }

        //string pattern3 = @" at (?<str1>.*?)] in (?<str2>.*?)";
        //string replacement3 = "{str1}] () (at {str2})"; 
        
        
        //string pattern3 = @" at (?<str1>.*?)\[0x(.*)\] in (?<str2>.*?)\.cs:(?<str3>([0-9]+)?)";
        //string replacement3 = "${str1} (at ${str2}.cs:${str3})";
        //var val2 = Regex.Replace(input2, pattern3, replacement3);
        //Debug.LogError(val2);

        //string pattern4 = @" at (?<str1>.*?)\[0x(.*)\] in (?<str2>.*?)[\n|$]";
        //string replacement4 = "${str1} (at ${str2})\n";
        //var val3 = Regex.Replace(val2, pattern4, replacement4);
        //Debug.LogError(val3);


        string pattern5 = @" at (?<str1>.*?)\[0x(.*)\] in (.*)\\Assets\\(?<str2>.*?)\.cs:(?<str3>([0-9]+)?)";
        string replacement5 = "() (at Assets\\${str2}.cs:${str3})";
        var val5 = Regex.Replace(input2, pattern5, replacement5);
        Debug.LogError(val5);

        Debug.LogError(Regex.Replace(val5, @"\\", "/"));



        Debug.Log(@"LGF.EventDispatcher`1[LGF.EvtHelper+IOnUpdate]:Fire() (at Assets/_Test/Lh2/LHLGF/_LGF/Manager/Helper/Event/EventDispatcher.cs:117)
                    LGF.EventDispatcher`1[T].Fire (LGF.IEventDataBase msg) (at D:\Unity project\clzc\clzcDemo\Assets\_Test\Lh2\LHLGF\_LGF\Manager\Helper\Event\EventDispatcher.cs:113)");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
