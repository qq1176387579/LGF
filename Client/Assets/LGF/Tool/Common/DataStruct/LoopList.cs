using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGF.DataStruct
{
    /// <summary>
    /// ѭ��list �����С��ʹ�����ʱ�� �Զ��������Ƚ�������
    /// ��ʵ�� �������Ż�
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


