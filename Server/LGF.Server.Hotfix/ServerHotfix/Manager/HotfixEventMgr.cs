using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGF.Server.Hotfix
{
    internal class HotfixEventMgr : EventManagerBase<HotfixEventMgr,HotfixEventType>, IHotfixSingletonBase
    {
        public HotfixEventMgr Init(HotfixMoudleMgr s_HotfixMoudle)
        {
            s_HotfixMoudle.AddManager(this);
            return this;
        }

        public virtual void Close()
        {
            SingletonClear();
        }
    }
}
