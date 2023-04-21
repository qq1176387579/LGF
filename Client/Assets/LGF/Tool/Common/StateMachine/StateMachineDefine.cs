using System;
using System.Collections.Generic;
using System.Text;

namespace LGF
{
    public static class StateMachineDefine
    {
        //public const int TRANSITION_TO_SELF = 1 << 0x1F;    //过渡到自己  不懂为啥有这个
        public const int MAX_CAPACITY = 0X3F;               //最大状态数量 4 * 16 = 64 - 1个  最高位用来判断 
        public const ulong DEFAULT_VALUE = ulong.MaxValue;  //0xFFFFFFFF FFFFFFFF;        //当前状态 标签
        public const ulong TRANSITION_OPEN = 0x8000000000000000;   //最高位用来记录 变化关闭
    }

}
