/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/14 23:26:45
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using PEMath;

namespace LGF.Logic
{

    //using DataType = System.Int32;
    using DataType = PEInt;

  
    public enum ATTR_TYPE
    {
        MAXHP       = 0,    //最大血量
        ATK         = 1,    //攻击
        DEF         = 2,    //防御
        SPD         = 3,    //速度
        HP,
        MAX,  //
    }

    public class Attr  : PoolableNotLock<Attr>
    {
        DataType[] data;
        public Attr()
        {
            data = new DataType[(int)ATTR_TYPE.MAX];
        }

        public DataType this[ATTR_TYPE type] { get => data[(int)type]; set => data[(int)type] = value; }


        public void Add(Attr t)
        {
            for (int i = 0; i < t.data.Length; i++)
            {
                data[i] += t.data[i];
            }
        }

        public void Decrease(Attr t)
        {
            for (int i = 0; i < t.data.Length; i++)
            {
                data[i] -= t.data[i];
            }
        }
    }

}
