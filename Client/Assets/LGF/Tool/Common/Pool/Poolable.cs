/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/29 20:56:03
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

namespace LGF
{
    public class Poolable<T> : System.IDisposable where T : Poolable<T>, new()
    {
        [System.NonSerialized]
        private static readonly ObjectPool<T> _pool = new ObjectPool<T>(_OnGet, _OnRelease);

        [System.NonSerialized]
        protected int releaseCount;   //回收次数
        public static T GetPooled()
        {
            return _pool.Get();
        }

        public static T Get()
        {
            return _pool.Get();
        }

        public static void ClearPool()
        {
            _pool.Clear();
        }
        static void _OnRelease(Poolable<T> _this)
        {
            _this.OnRelease();
        }
        static void _OnGet(Poolable<T> _this)
        {
            _this.OnGet();
        }

        public virtual void Release()
        {
            if (!IsRelease())
            {
                //lock(_pool)   //后续看情况要不要加锁  多线程
                _pool.Release((T)this);
            }
            else
            {
                this.DebugError($">>>>>>>>>>>> 重复回收了  releaseCount: {releaseCount}  GetType: {GetType().UnderlyingSystemType}");
            }
        }


        protected virtual void OnRelease()
        {
            releaseCount++;
        }

        protected virtual void OnGet()
        {
            releaseCount = 0;
        }

        /// <summary>
        /// 是否时回收状态
        /// </summary>
        /// <returns></returns>
        public bool IsRelease()
        {
            return releaseCount >= 1;
        }

        public virtual void Dispose()
        {
            Release();    //回收过不回收
        }
    }

}

