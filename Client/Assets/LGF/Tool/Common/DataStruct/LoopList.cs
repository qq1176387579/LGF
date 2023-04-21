using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGF.DataStruct
{
    /// <summary>
    /// 循环list 数组大小到使用最大时候 自动清理最先进的数据
    /// 简单实现 后续在优化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LoopList<T>
    {
        T[] values;
        public int Length { get; private set; }
        public int curIdx;
        public bool isLoop = false;
        public LoopList(int length)
        {
            curIdx = 0;
            isLoop = true;
            Length = length;
            values = new T[length];
        }

        public void Add(T val)
        {
            values[curIdx++] = val;
            if (curIdx == Length) {
                curIdx = 0;
                if (!isLoop) {
                    isLoop = true;
                }
            }
        }

        public T[] GetAllData(out int length)
        {

            length = isLoop ? Length : curIdx;
            return values;
        }
    }

}


