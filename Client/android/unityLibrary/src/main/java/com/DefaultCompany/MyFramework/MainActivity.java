package com.defaultcompany.myframework;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageManager;
import android.nfc.Tag;
import android.os.Bundle;
import android.util.Log;


import com.unity3d.player.UnityPlayerActivity;
//import static com.bytedance.ad.sdk.mediation.SplashMainActivity.SPLASH_KEY;

public class MainActivity extends UnityPlayerActivity {
    static MainActivity _this;

    public String GetChannelID()
    {
        return "dtyybao";
    }
    static String TAG = "unity_MainAct";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        _this = this;
        Log.e(TAG,"onCreate");

        ToolHelper.Init(this);
        GameHelper.Init(this);

        //顶部信息
        //ToolHelper.WindowSetFlags();

    }

    @Override
    protected void onStart() {
        GameHelper.SendUnity_GetUUIDResp(); //通知UUID
        super.onStart();
    }





}
