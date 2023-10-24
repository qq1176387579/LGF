using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;

namespace LGF.Server.Hotfix
{

    public static class HotfixEntry
    {
        public static void Init()
        {
            HotfixMoudleMgr.Instance.Init();

            sLog.Debug("- S_HotfixEntry-");
        }


        public static void Close()
        {
            HotfixMoudleMgr.Instance.Close();

            sLog.Debug("-S_HotfixEntry Close-");
        }

    }
}