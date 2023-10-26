/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2023/10/26 17:33:32
/// 功能描述:  
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LGF;
using LGF.DataStruct;
using LGF.Log;
using LGF.Net;
using LGF.Serializable;
using LGF.Timers;
using LGF.Util;

/// <summary>
/// 帧数据管理
/// </summary>
public class FrameDataMgr : SingletonBase<FrameDataMgr>
{
    public class GameFrameConfig
    {
        /// <summary>
        /// 服务器逻辑帧间隔
        /// </summary>
        public const int ServerLogicFrameIntervelMs = 66;
   

    }
    bool isDisable = false;


    NBufferingQueue<S2C_FrameOpKey> queue = new NBufferingQueue<S2C_FrameOpKey>();

    string sFileName = "";

    public override void Init()
    {
        //base.Init();
        EventManager.Instance.AddListener(GameEventType.ClientEvent_StartPlay, OnStartPlay); //用于显示
        MonoManager.Instance.AddOnDisableListener(OnDisable);
    }

    /// <summary>
    /// 开始player
    /// </summary>
    void OnStartPlay()
    {
        AddListen();    //开始保存
    }

    /// <summary>
    /// 回放时候的初始化
    /// </summary>
    void PlaybackInit()
    {
        var taskID = TimeManager.Instance.AddTask(OnLogicFrame, GameFrameConfig.ServerLogicFrameIntervelMs, TimeUnit.Millisecond, 0);    //扩展定时器
      
        FrameSyncMgr.Instance.IsPlayback = true;
    }


    /// <summary>
    /// 保存的时候的初始化
    /// </summary>
    public void AddListen()
    {
        EventManager.Instance.AddListener<S2C_FrameOpKey>(GameEventType.ClientEvent_OnServerLogicFrame, OnServerLogicFrame);
        EventManager.Instance.AddListener<GameSceneInitData>(GameEventType.ClientEvent_GameSceneInitData, StartSave);
        //sFileName.
    }

    /// <summary>
    /// 保存时候的初始化
    /// </summary>
    public void RemoveListerner()
    {
        EventManager.Instance.RemoveListerner<S2C_FrameOpKey>(GameEventType.ClientEvent_OnServerLogicFrame, OnServerLogicFrame);
        EventManager.Instance.RemoveListerner<GameSceneInitData>(GameEventType.ClientEvent_GameSceneInitData, StartSave);
    }


    int curIdx = 0;
    List<S2C_FrameOpKey> tmpList;

    /// <summary>
    /// 1s 15帧 广播 
    /// </summary>
    public void OnLogicFrame()
    {
        lock (queue) {
            FrameSyncMgr.Instance.OnFixedUpdate();
        }
    }


    /// <summary>
    /// 帧数据 保存
    /// </summary>
    /// <param name="data"></param>
    public void StartSave(GameSceneInitData data)
    {
        string basePath = $"{ConstValue.FrameDataBasePath}/{data.mainPlayerID}";
        sFileName = $"{ConstValue.FrameDataBasePath}/{data.mainPlayerID}/{DateTime.UtcNow.ToUnixTime()}.byte";  //保存路径
        this.DebugError($"保存地址<{sFileName}>");
        LGF.Util.FileUtils.DirectoryCreate(basePath);
        queue.Get();
        queue.Clear();
        Task.Run(() => {
            try {   
                FileStream fs = new FileStream(sFileName, FileMode.OpenOrCreate);
                byte[] stmp = new byte[4];
                LGF.Serializable.LStream stream = new LGF.Serializable.LStream();
                data.NSerialize(stream);
                Write(fs, stream, stmp);
                fs.Flush();
                queue.OnClear((a) =>{
                    a.NSerialize(stream);
                    Write(fs, stream, stmp);
                    a.SetCanRelease(true);
                    a.Release();
                });

                while (!isDisable) {  //设置完成然后关闭 暂时没有该功能先不做处理
                    if (queue.Count >= 10) {
                        queue.Get();
                        queue.Clear();
                        fs.Flush();
                    }
                    else {
                        Thread.Sleep(100);
                    }
                };

                fs.Close();
                stream.Close();
            }
            catch (Exception e) {
                e.DebugError();
                throw;
            }
        });
    }


    void Write(FileStream fs, LStream stream, byte[] stmp)
    {
        stream.Lenght.intToBytes(stmp);
        fs.Write(stmp, 0, 4);   //传int进去
        fs.Write(stream.GetBuffer(), 0, stream.Lenght); //写入数据
    }

    bool Read(BinaryReader read, LStream stream)
    {
        long dif = read.BaseStream.Length - read.BaseStream.Position;
        if (dif < 4) {
            return false ;
        }
        stream.Clear();
        int size = read.ReadInt32();
        dif -= 4;
        if (dif < size) {
            return false ;
        }

        stream.CheckSize(size); //检查大小
        read.Read(stream.GetBuffer(), 0, size);

        return true ;

    }


    void OnServerLogicFrame(S2C_FrameOpKey msg)
    {
        msg.SetCanRelease(false);   //托管回收
        queue.Add(msg);
    }



    public void StartPath(string path)
    {
        sFileName = path;
        StartRead();
    }


    /// <summary>
    /// 开始读取
    /// </summary>
    void StartRead()
    {
        queue.Get();    //清空数据
        queue.Clear();
        PlaybackInit();

        Task.Run(() => {
            try {
                FileStream fs1 = new FileStream(sFileName, FileMode.Open);
                BinaryReader binReader = new BinaryReader(fs1);
                LGF.Serializable.LStream stream = new LGF.Serializable.LStream(10240);
                Read(binReader, stream);
                GameSceneInitData data = GameSceneInitData.Get(stream);
                //NetMsgMgr.Instance.BroadCastEventByMainThreadt<GameSceneInitData>(GameEventType.NetStatus, data);
                TimeManager.Instance.AddTask(() => {
                    EventManager.Instance.BroadCastEvent(GameEventType.ClientEvent_StartPlayback, data);
                });

                while (!isDisable) {
                    if (FrameSyncMgr.Instance.DicCount >= 2000) {
                        Thread.Sleep(1000); //修眠1s  防止一次性加载太多
                        continue; 
                    }

                    if (!Read(binReader, stream)) {
                        break;
                    }


                    var tmp = S2C_FrameOpKey.Get(stream);
                    lock (queue) {
                        FrameSyncMgr.Instance.AddFrame(tmp);
                    }      

                }

                binReader.Close();
                fs1.Close();
                stream.Close();
            }
            catch (System.Exception e) {
                e.DebugError();
                throw;
            }
          
        });
        

     
        
     

    }

    void OnDisable()
    {
        isDisable = true;
    }



}
