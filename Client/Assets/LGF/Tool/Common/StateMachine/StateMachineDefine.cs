using System;
using System.Collections.Generic;
using System.Text;

namespace LGF
{
    public static class StateMachineDefine
    {
        public const int TRANSITION_TO_SELF = 1 << 0x1F;    //过渡到自己 没用上
        public const int MAX_CAPACITY = 0X20;               //最大状态数量 20 * 16 = 320个
        public const int DEFAULT_VALUE = 0x7FFFFFFF;        //当前状态 标签
    }

}
