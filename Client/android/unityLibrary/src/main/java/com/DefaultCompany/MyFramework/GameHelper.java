package com.defaultcompany.myframework;

import android.util.Log;

import com.unity3d.player.UnityPlayer;

import org.json.JSONException;
import org.json.JSONObject;


public class GameHelper
{
    private static MainActivity m_Activity = null;

    private static String m_UnityObject = "AndroidMsgManager";  //Unity用于通信的物体
    private static String m_MethodName = "OnMessage"; //Unity用于接收通信的方法名称
    public  static String TAG = "unity_GameHelper";

    //初始化方法
    public static void Init(MainActivity activity)
    {
        m_Activity = activity;
    }

    //android发送消息到unity
    public static void SendMessageToUnity(int iMsgId, int iParam1,int iParam2,int iParam3,String strParam1,String strParam2,String strParam3)
    {
        Log.d(TAG, "SendUnityMessage: iMsgId：" + iMsgId + " iParam1:"+iParam1+" iParam2:" +iParam2 +" iParam3:"+iParam3 +" strParam1:"+strParam1 +" strParam2:"+strParam2 +" strParam3:"+strParam3 );
        String jsonString = GetJsonStr(iMsgId,iParam1,iParam2,iParam3,strParam1,strParam2,strParam3);
        UnityPlayer.UnitySendMessage(m_UnityObject, m_MethodName, jsonString);
    }

    public static void SendMessageToUnity(int iMsgId,String str)
    {
        Log.d(TAG, "SendUnityMessage: iMsgId：" + iMsgId + " str:" + str);
        String jsonString = GetJsonStr(iMsgId,0,0,0,str,"","");
        UnityPlayer.UnitySendMessage(m_UnityObject, m_MethodName, jsonString);
    }

    public static void SendMessageToUnity(int iMsgId,int param1)
    {
        Log.d(TAG, "SendUnityMessage: iMsgId：" + iMsgId + " param1: " + param1);
        String jsonString = GetJsonStr(iMsgId,param1,0,0,"","","");
        UnityPlayer.UnitySendMessage(m_UnityObject, m_MethodName, jsonString);
    }


    //unity发送消息到android
    public static void SendUnityMessage(int iMsgId, int iParam1,int iParam2,int iParam3,int iParam4,String strParam1,String strParam2,String strParam3,String strParam4)
    {
        Log.d(TAG, "SendUnityMessage: iMsgId：" + iMsgId + " iParam1:"+iParam1+" iParam2:" +iParam2 +" iParam3:"+iParam3 +" iParam4:"+iParam4 +" strParam1:"+strParam1 +" strParam2:"+strParam2 +" strParam3:"+strParam3 +" strParam4:"+strParam4);

        if(m_Activity ==null)
        {
            Log.e(TAG, "m_Activity is null");
        }

        switch (iMsgId)
        {
            case MsgDefine.Test:
                Log.e(TAG, " test 1");
                SendMessageToUnity(MsgDefine.TestResp,1,1,1,"test","","");
                //Log.e(TAG, " start");
                //WxTencent.Login();  //测试代码
                break;

            case  MsgDefine.GetUUID:                    SendUnity_GetUUIDResp();      break;

        }
    }

    //从android获取Int类型
    public static int GetIntFromPlatform(int type)
    {
        switch (type)
        {

        }
        return 0;
    }

    //从android获取String类型
    public static String GetString(int type)
    {

        return "";
    }

    //从android获取Long类型
    public static long GetLong(int type)
    {
        switch (type)
        {

        }
        return 0;
    }

    //从android获取Long类型
    public static long GetLong2(int type, int iParam1,int iParam2,int iParam3,int iParam4,String strParam1,String strParam2,String strParam3,String strParam4)
    {
        switch (type)
        {

        }
        return 0;
    }


    //通过json对象构造字符串
    public static  String GetJsonStr(int iMsgId, int iParam1,int iParam2,int iParam3,String strParam1,String strParam2,String strParam3)
    {
        try
        {
            JSONObject object = new JSONObject();
            object.put("iMsgId",iMsgId);
            object.put("iPararm1",iParam1);
            object.put("iPararm2",iParam2);
            object.put("iPararm3",iParam3);
            object.put("strParam1",strParam1);
            object.put("strParam2",strParam2);
            object.put("strParam3",strParam3);
            return  object.toString();
        }
        catch(JSONException e)
        {
            Log.e(TAG, "Json error" + e.toString() );
            return "";
        }
    }


    public static void SendUnity_GetUUIDResp()
    {
        GameHelper.SendMessageToUnity(MsgDefine.GetUUIDResp, ToolHelper.GetUUID());
    }




}
