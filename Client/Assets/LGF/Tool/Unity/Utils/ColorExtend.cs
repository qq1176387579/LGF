/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/29 20:11:41
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using UnityEngine;

public static class ColorExtend 
{


    public static string ToColor(this object obj, string color)
    {
        return string.Format("<color={0}>{1}</color>", color, obj.ToString());
    }
    public static string ToColor(this string str, string color)
    {
        return string.Format("<color={0}>{1}</color>", color, str);
    }


    public static Color StringToColor(string colorStr)
    {
        if (string.IsNullOrEmpty(colorStr))
        {
            return new Color();
        }

        if (colorStr[0] == '#') colorStr = colorStr.Substring(1, colorStr.Length - 1);
        if (colorStr.Length < 7) colorStr += "FF";
        long colorInt = long.Parse(colorStr, System.Globalization.NumberStyles.AllowHexSpecifier);

        //   sLog.Debug("colorInt"+colorInt);
        return IntToColor(colorInt);
    }

    public static Color IntToColor(long colorInt)
    {
        float basenum = 255;
        long a = 0xFF & colorInt;
        long b = 0xFF00 & colorInt;
        b >>= 8;
        long g = 0xFF0000 & colorInt;
        g >>= 16;
        long r = 0xFF000000 & colorInt;
        r >>= 24;
        //   sLog.Debug("R:" + r + " G:" + g + " B:" + b + " A:" + a);
        return new Color((float)r / basenum, (float)g / basenum, (float)b / basenum, (float)a / basenum);

    }


    /// <summary>
    /// color 转换hex
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string ColorToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255.0f);
        int g = Mathf.RoundToInt(color.g * 255.0f);
        int b = Mathf.RoundToInt(color.b * 255.0f);
        int a = Mathf.RoundToInt(color.a * 255.0f);
        string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
        return hex;
    }

    /// <summary>
    /// hex转换到color
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static Color HexToColor(string hex)
    {
        if (hex[0] == '#') hex = hex.Substring(1, hex.Length - 1);
        if (hex.Length < 7) hex += "FF";
        byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        float r = br / 255f;
        float g = bg / 255f;
        float b = bb / 255f;
        float a = cc / 255f;
        return new Color(r, g, b, a);
    }


}
