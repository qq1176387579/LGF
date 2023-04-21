using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LGF;
using UnityEngine;

namespace LGF.Util
{
    public static class GameObjcetExtend
    {
        public static void Reset(this UnityEngine.GameObject go)
        {
            go.transform.Reset();
        }
    }
}
