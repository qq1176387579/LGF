/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/3 20:34:36
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

namespace LGF.Serializable
{
    /// <summary>
    /// 先放这里
    /// </summary>
    public interface ISerializer
    {
        void Serialize(LGF.Serializable.LStream stream);
        void Deserialize(LGF.Serializable.LStream stream);

        //回收
        void Release();
    }

}
