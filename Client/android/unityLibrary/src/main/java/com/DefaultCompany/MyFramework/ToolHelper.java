package com.defaultcompany.myframework;


import android.app.Activity;
import android.content.Context;
import android.content.SharedPreferences;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.nfc.Tag;
import android.os.Bundle;
import android.util.Log;
import java.util.UUID;

public  class ToolHelper {

    private static MainActivity m_Activity = null;
    private static SharedPreferences data = null;
    static String TAG = "unity_ToolHelper";

    public static void Init(MainActivity activity)
    {
        m_Activity = activity;
        data = activity.getSharedPreferences("data", Context.MODE_PRIVATE);
    }


    //显示头部信息 如 电池
    public static void WindowSetFlags()
    {
        m_Activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                m_Activity.getWindow().setFlags(2048,2048);
            }
        });
    }


    public static int GetDataInt(String key)
    {
        return  data.getInt(key, 0);
    }

    public static String GetDataString(String key)
    {
        return data.getString(key, "");
    }

    public static void SetDataInt(String key,int val)
    {
        data.edit().putInt(key, val).commit();
    }

    public static void SetDataString(String key,String val)
    {
        data.edit().putString(key, val).commit();
    }

    public static String GetUUID()
    {
        String key = "UUID";
        String val = GetDataString(key);
        if (val.isEmpty())
        {
            val = UUID.randomUUID().toString();
            Log.e(TAG," Init UUID: " + val);
            SetDataString(key,val);
        }
        Log.e(TAG,"Bree  UUID: " + val);
        return val;
    }




}


