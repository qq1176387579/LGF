/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/11 18:52:57
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using LGF;
using LGF.Log;

public class ServerConfig : ScriptableObject
{
    IPEndPoint endPoint;
    IPEndPoint localendPoint;
    public string path;
    public string localPath;

    IPAddress GetAddress(string str)
    {

        //return IPAddress.Parse("192.168.31.241");
        //Debug.Log(str);
        //Debug.Log("192.168.31.241");
        return IPAddress.Parse(str);
    }

    public IPEndPoint GetEndPoint()
    {
        if (endPoint != null)
        {
            return endPoint;
        }

        string[] path2 = path.Split(':');
        string[] localPath2 = localPath.Split(':');
        //Debug.Log(path2[0]);
        //Debug.Log("" + path2[0] +"    "+ path2[1]);
        endPoint = new IPEndPoint(GetAddress(path2[0]), int.Parse(path2[1]));
        localendPoint = new IPEndPoint(GetAddress(localPath2[0]), int.Parse(localPath2[1]));
        //Debug.Log("  " + endPoint.ToString());
        return endPoint;
    }


   
}
