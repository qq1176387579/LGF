using LGF.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace LGF.Server
{
    public static class HotfixHelper
    {
        /// <summary>
        /// 重新加载钱 先卸载原来的方法
        /// </summary>
        static MethodInfo OnCloseHotfixMethod;  
        static bool reLoadHotfix = false;
        public static void CheckHotfix()
        {
            if (reLoadHotfix)
            {
                OnCloseHotfixMethod.Invoke(null,null);
                OnCloseHotfixMethod = null;
                //moudleEntry.GetMember("")
                LoadHotfixAssembly();
                reLoadHotfix = false;
                EventManager.Instance.BroadCastEvent(GameEventType.OnReLoadHotfixFinish);

                sLog.Warning("重新加载热更新  加载完成");
            }
        }

        public static void ReLoadHotfix()
        {
            reLoadHotfix = true;    
        }

        public static void LoadHotfixAssembly()
        {
            try
            {
                //byte[] dllBytes = File.ReadAllBytes("../../../../LGF.Server.Hotfix/bin/Debug/net6.0/LGF.Server.Hotfix.dll");
                //byte[] pdbBytes = File.ReadAllBytes("../../../../LGF.Server.Hotfix/bin/Debug/net6.0/LGF.Server.Hotfix.pdb");
                byte[] dllBytes = File.ReadAllBytes("LGF.Server.Hotfix.dll");
                byte[] pdbBytes = File.ReadAllBytes("LGF.Server.Hotfix.pdb");
                Assembly assembly = Assembly.Load(dllBytes, pdbBytes);
                var moudleEntry = assembly.GetType("LGF.Server.Hotfix.HotfixEntry");
                var moudleEntry_Init = moudleEntry.GetMethod("Init");
                moudleEntry_Init.Invoke(null, null);
                OnCloseHotfixMethod = moudleEntry.GetMethod("Close");

                //hotfixAssembly = assembly;
            }
            catch (Exception e)
            {
                e.DebugError();
                sLog.Error("热更新模块 加载失败 请检查配置");
                return;
            }
            //后面改成配置

            //sLog.Warning(" 开始 加载 热更新 模块");
        } 

    }
   
}
