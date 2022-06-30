/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/5/30 11:02:07
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using LGF.DataStruct;


namespace LGF
{

    public interface IEntityIndex<T> : IList2Idx<IEntityIndex<T>>
    {
        void OnEvent(IEventDataBase ev);
    }

    public interface IEventDataBase
    {
        void Release();
    }

    /// <summary>
    /// 游戏事件基类
    /// </summary>
    public abstract class EventDataBase<T> : Poolable<T>, IEventDataBase where T : Poolable<T>, new()
    {

    }

}