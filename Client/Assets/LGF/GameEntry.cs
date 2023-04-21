/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/5 1:10:39
/// 功能描述:  游戏入口
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Util;

namespace LGF
{
    /// <summary>
    /// 外部可以继承初始化
    /// </summary>
    public class GameEntry : SimpleMonoSingleton<GameEntry>
    {
        public override void OnAwake()
        {
            Common.DontDestroyOnLoad(this);   
        }
        

        protected virtual void Start()
        {
            LGFEntry.Startup();

            OnStart();
        }

        protected virtual void OnStart()
        {

        }

        protected virtual void Update()
        {
            try
            {
                LGFEntry.Update();
            }
            catch (System.Exception e)
            {
                e.DebugError();

            }
        }

        private void LateUpdate()
        {
            try
            {
                LGFEntry.LateUpdate();
            }
            catch (System.Exception e)
            {
                e.DebugError();
            }
        }

        private void FixedUpdate()
        {
            try
            {
                LGFEntry.FixedUpdate();
            }
            catch (System.Exception e)
            {
                e.DebugError();
            }
        }


        protected virtual void OnDisable()
        {
            LGFEntry.OnDestroy();
        }

    }
}


