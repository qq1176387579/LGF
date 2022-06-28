package com.defaultcompany.myframework;

public interface MsgDefine {
    int     Test                            = 1;             //测试
    int     GetUUID                         = 2;              //获得UUID;


    int     Resp                            = 100000;
    int     TestResp                        = Resp + Test ;         //Test相应
    int     GetUUIDResp                     = Resp + GetUUID;       //获得UUID


}
