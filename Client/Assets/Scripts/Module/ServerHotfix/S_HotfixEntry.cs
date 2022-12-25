using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.Net;

namespace LGF.Server.Hotfix
{

    public static class S_HotfixEntry
    {
        public static void Init()
        {
            S_HotfixMoudleMgr.Instance.Init();

            sLog.Debug("- S_HotfixEntry-");
        }


        public static void Close()
        {
            S_HotfixMoudleMgr.Instance.Close();

            sLog.Debug("-S_HotfixEntry Close-");
        }

    }
}