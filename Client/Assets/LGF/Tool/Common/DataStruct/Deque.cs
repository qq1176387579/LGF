using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LGF;
using LGF.Log;

/// <summary>
/// 后面改命名空间
/// </summary>
namespace LGF.DataStruct
{
    public interface IOnRefreshData
    {
        void RefreshData();
    }

    public interface IOnRefreshData<T1>
    {
        void RefreshData(T1 t1);
    }

  
    /// <summary>
    /// 双端队列
    /// </summary>
    public class Deque<T> 
    {
        private static readonly ObjectPool<DequeNode<T>> m_nodePool = new ObjectPool<DequeNode<T>>(null, l => l.Clear());

        DequeNode<T> m_root, m_finally;    //根节点  最后节点
        int m_count;
        public int Count { get => m_count; }

        public Deque()
        {
            m_count = 0;
            m_root = m_finally = null;
        }

        public T GetHead()
        {
            if (m_count != 0)
                return m_root.info;
            return default(T);
        }

        public T GetEnd()
        {
            if (m_count != 0)
                return m_finally.info;
            return default(T);
        }

        public T PopHead()
        {
            return Pop(0);
        }

        public T PopEnd()
        {
            return Pop(1);
        }

        public void PushHead(T info)
        {
            Push(0, info);
        }

        public void PushEnd(T info)
        {
            Push(1, info);
        }

        void Push(int type, T info)
        {
            var newObj = m_nodePool.Get();
            newObj.info = info;
            if (m_count == 0)
            {
                m_root = m_finally = newObj;
            }
            else
            {
                if (type == 0)
                {
                    newObj.next = m_root;
                    m_root.last = newObj;
                    m_root = newObj;
                }
                else
                {
                    newObj.last = m_finally;
                    m_finally.next = newObj;
                    m_finally = newObj;
                }
            }
            m_count++;
            //Print();
        }


        T Pop(int type)
        {
            if (m_count <= 0)
            {
                return default(T);
            }

            T tmp;
            if (m_count == 1)
            {
                tmp = m_root.info;
                m_nodePool.Release(m_root);
                m_root = m_finally = null;
            }
            else
            {
                if (type == 0)
                {
                    tmp = m_root.info;
                    m_root = m_root.next;
                    m_nodePool.Release(m_root.last);
                    m_root.last = null;
                }
                else
                {
                    tmp = m_finally.info;
                    m_finally = m_finally.last;
                    m_nodePool.Release(m_finally.next);
                    m_finally.next = null;
                }
            }
          
            m_count--;
            //Print();
            return tmp;
        }

      
        public void Print()
        {
            var node = m_root;
            string str = "";
            while (node != null)
            {
                str += $" {node.info.ToString()} ,";
                node = node.next;
            }
            node.Debug($"count: {m_count}  " + str);
        }



        #region 扩展方法

  
        /// <summary>
        /// 重置所有数据
        /// </summary>
        public void RefreshData()
        {
            RefreshDataEx<IOnRefreshData>((data) => data.RefreshData());
        }

        public void RefreshData<T1>(T1 t1)
        {
            RefreshDataEx<IOnRefreshData<T1>>((data) =>data.RefreshData(t1));
            //List<int> t = new List<int>();
            //t.ForEach
        }

        public void ForEach(System.Action<T> action)
        {
            var node = m_root;
            while (node != null)
            {
                action.Invoke(node.info);
                node = node.next;
            }
        }


        //public void RefreshDataFunc<DataType>(System.Action<DataType> action) where DataType : class
        //{
        //    RefreshDataEx(action);
        //}

        private void RefreshDataEx<DataType>(System.Action<DataType> action) where DataType : class
        {
            var node = m_root;
            if (node == null) return ;
            if (node.info as DataType == null)
            {
                node.DebugError($"未实现{typeof(DataType).ToString()} IOnRefreshData 方法");
                return;
            }

            while (node != null)
            {
                action.Invoke(node.info as DataType);
                node = node.next;
            }
        }

        #endregion

        public List<T> GetAll()
        {
            if (m_root == null)
                return null;

            List<T> list = ListPool<T>.Get();
            var node = m_root;
            list.Add(node.info);
            while (node != null)
            {
                node = node.next;
                if (node != null)
                    list.Add(node.info);
            }
            return list;
        }


    }

    /// <summary>
    /// 链表节点    
    /// </summary>
    internal class DequeNode<T>
    {
        public DequeNode<T> next;
        public DequeNode<T> last;
        public T info;

        public void Clear()
        {
            next = null;
            last = null;
            info = default(T);
        }
    }

}

