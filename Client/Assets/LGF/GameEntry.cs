/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/5 1:10:39
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Util;

namespace LGF
{
    public class GameEntry : SimpleMonoSingleton<GameEntry>
    {
        public override void OnAwake()
        {
            Common.DontDestroyOnLoad(this);   
        }

        private void Start()
        {
            AppEntry.Startup();
        }

        private void Update()
        {
            try
            {
                AppEntry.Update();
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
                AppEntry.LateUpdate();
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
                AppEntry.FixedUpdate();
            }
            catch (System.Exception e)
            {
                e.DebugError();
            }
        }


        private void OnDisable()
        {
            AppEntry.OnDestroy();
        }

    }
}


