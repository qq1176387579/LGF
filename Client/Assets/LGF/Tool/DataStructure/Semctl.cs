//---------------------------------------------------------------------------------------
// Author: lh
// Date: 2021-08-15
// Description: 简单信号量
//---------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGF
{
    public class SemctlBool
    {
        public bool Flag { get => flag; }

        bool flag = false;
        System.Action beginAc, endAc;

        public SemctlBool() { }
        public SemctlBool(System.Action beginAc_, System.Action endAc_) => Register(beginAc_, endAc_);


        public void Register(System.Action beginAc_, System.Action endAc_)
        {
            beginAc = beginAc_;
            endAc = endAc_;
        }

        public void Begin()
        {
            if (!flag)
            {
                beginAc?.Invoke();
                flag = true;
            }
        }

        public void End()
        {
            if (flag)
            {
                endAc?.Invoke();
                flag = false;
            }
        }

    }


    public class SemctlBool<T>
    {
        T val;
        public bool Flag { get => flag; }

        bool flag = false;
        System.Action<T> beginAc, endAc;

        public SemctlBool() { }
        public SemctlBool(T val_, System.Action<T> beginAc_, System.Action<T> endAc_) => Register(val_, beginAc_, endAc_);


        public void Register(T val_,System.Action<T> beginAc_, System.Action<T> endAc_)
        {
            beginAc = beginAc_;
            endAc = endAc_;
            val = val_;
        }

        public void Begin()
        {
            if (!flag)
            {
                beginAc?.Invoke(val);
                flag = true;
            }
        }

        public void End()
        {
            if (flag)
            {
                endAc?.Invoke(val);
                flag = false;
            }
        }

    }
}
