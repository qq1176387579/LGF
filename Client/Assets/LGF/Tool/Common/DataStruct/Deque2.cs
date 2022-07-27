/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/10 23:54:57
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;

namespace LGF.DataStruct
{
    public partial class Deque2<T> 
    {

        public interface INode
        {
            T next { get; set; }
            T last { get; set; }
            Deque2<T> deque { get; set; }
        }
    }
      

    /// <summary>
    /// 接口双端队列
    /// </summary>
    public partial class Deque2<T> where T : Deque2<T>.INode 
    {
        enum Opt { Front, Back }
        public int Count { get; private set; }
        T _front = default;
        T _back = default;
        public Deque2() 
        {
            Count = 0;
        }
           
        public void push_back(T info)
        {
            add(Opt.Back, info);
        }

        public void push_front(T info)
        {
            add(Opt.Front, info);
        }

        public T pop_back()
        {
            return get(Opt.Back);
        }

        public T pop_front()
        {
            return get(Opt.Front);
        }

        public T front()
        {
            return _front;
        }

        public T back()
        {
            return _back;
        }

        void add(Opt type, T info)
        {
            if (info.last != null || info.next != null || info.deque != null)
            {
                this.DebugError("非法操作 检查下状态");
                return;
            }

            if (Count == 0)
                _front = _back = info;
            else
            {
                if (type == Opt.Front)
                {
                    info.next = _front;
                    _front.last = info;
                    _front = info;
                }
                else
                {
                    info.last = _back;
                    _back.next = info;
                    _back = info;
                }
            }
            info.deque = this;
            Count++;

        }

        /// <summary>
        /// 还没有 测试过
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        T get(Opt type)
        {
            if (Count <= 0)
                return default(T);

            T info;
            if (Count == 1)
            {
                info = _front;
                _front = _back = default(T);
            }
            else
            {
                if (type == Opt.Front)
                {
                    info = _front;
                    _front = _front.next;
                    info.next = _front.last = default(T);
                }
                else
                {
                    info = _back;
                    _back = _back.last;
                    info.last = _back.next = default(T);
                }
            }

               info .deque = null;

            Count--;
            return info;
        }



        //没测试过
        //public void move_to(Deque2<T> deque)
        //{
        //    if (Count == 0) //没有不操作
        //        return;

        //    if (deque.Count == 0)
        //        deque._front = _front;
        //    var tmp_back = _back;

        //    var front = pop_all(out int count_, true, deque);
        //    if (deque.Count != 0)
        //    {
        //        deque._back.next = front;
        //        front.last = deque._back;
        //    }
        //    deque._back = tmp_back;
        //    deque.Count += count_;

        //}


        /// <summary>
        /// 不安全使用 需要自己去处理数据
        /// </summary>
        /// <param name="count_"></param>
        /// <param name="init"></param>
        /// <param name="deque"></param>
        /// <returns></returns>
        public T pop_all(out int count_)
        {
            T tmp = _front;
            count_ = Count;
            Count = 0;
            _front = _back = default(T);
            return tmp;
        }


        public void clear<T1>(System.Action<T,T1> action,T1 obj)
        {
            if (Count < 0)
                this.DebugError("clear 非法操作");
            if (Count <= 0) return;

            T curNode = _front;
            while (curNode != null)
            {
                T lastNode = curNode.next;
                remove(curNode);
                action.Invoke(curNode, obj);
                curNode = lastNode;
            }

            if (Count != 0)
                this.Debug("数据出错");
            if (_front != null || _back != null)
                this.DebugError(" _front or _back not null");
        }

      
        public void remove(T node)
        {
            if (node.deque != this)
            {
                this.DebugError(" remove 非法操作");
                return;
            }

            if (_front.Equals(node)) 
                _front = node.next;
            if (_back.Equals(node))
                _back = node.last;

            node.clear();

            Count--;
            if (Count == 0)
                _front = _back = default(T);
        }





        public void foreach_root(System.Action<T> action)
        {
            _front.foreach_root(action);
        }

        public void foreach_root<T1>(System.Action<T, T1> action, T1 param)
        {
            _front.foreach_root(action, param);
        }
        public void foreach_root<T1, T2>(System.Action<T, T1, T2> action, in T1 param, in T2 param1)
        {
            _front.foreach_root(action, param, param1);
        }

        public void foreach_root<T1, T2, T3>(System.Action<T, T1, T2, T3> action, in T1 param, in T2 param1, in T3 param2)
        {
            _front.foreach_root(action, param, param1, param2);
        }

    }


    //public class Deque2Mgr : SingletonBase<Deque2Mgr>
    //{
    //    /// <summary>
    //    /// 父类是INode2 判定
    //    /// </summary>
    //    Dictionary<System.Type, bool> IsSubClassOfByINode2 = new Dictionary<System.Type, bool>();

    //    /// <summary>
    //    /// https://www.zhihu.com/question/405871850 
    //    /// 注意 缓存效率一样 这个可能更慢  不要用这个代替is 
    //    /// 测试的时候 这个脚本达不到预期 还是有问题
    //    /// 
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <returns></returns>
    //    public bool IsINode2<T>() where T : Deque2<T>.INode
    //    {
    //        var type = typeof(T);
    //        if (!IsSubClassOfByINode2.TryGetValue(type, out bool f))
    //        {
    //            IsSubClassOfByINode2.Add(type, type.IsSubclassOf(typeof(Deque2<T>.INode2)));
    //        }
    //        return f;
    //    }
    //}


    public static class Deque2Helper
    {

        /// <summary>
        /// 不安全使用 请确认安全后使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ss"></param>
        public static void remove<T>(this T node) where T : Deque2<T>.INode
        {
            node.deque.remove(node);
        }


        
        public static void clear<T>(this T node) where T : Deque2<T>.INode
        {
            if (node.last != null)
                node.last.next = node.next;  //删除操作
            if (node.next != null)
                node.next.last = node.last;

            node.next = node.last = default(T);
            node.deque = null;
        }



        /// <summary>
        /// foreach_root 根节点遍历  注意用lambda表达式达成无GC
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        public static void foreach_root<T>(this T rootNode, System.Action<T> action) where T : Deque2<T>.INode
        {
            if (!FrontCheck(rootNode)) return;

            T curNode = rootNode;
            while (curNode != null)
            {
                var next = curNode.next;    //防止 action 调用 clear
                action.Invoke(curNode);
                curNode = next;
            }
        }

        /// <summary>
        /// foreach_root 根节点遍历  注意用lambda表达式达成无GC
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        public static void foreach_root<T,T1>(this T rootNode, System.Action<T,T1> action, T1 param) where T : Deque2<T>.INode
        {
            if (!FrontCheck(rootNode)) return;

            T curNode = rootNode;
            while (curNode != null)
            {
                var next = curNode.next;
                action.Invoke(curNode, param);  //防止 action 调用 clear
                curNode = next;
            }
        }

        /// <summary>
        /// foreach_root 根节点遍历  注意用lambda表达式达成无GC
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        public static void foreach_root<T, T1, T2>(this T rootNode, System.Action<T, T1, T2> action, T1 param, T2 param1) where T : Deque2<T>.INode
        {
            if (!FrontCheck(rootNode)) return;

            T curNode = rootNode;
            while (curNode != null)
            {
                var next = curNode.next;    //防止 action 调用 clear
                action.Invoke(curNode, param, param1);
                curNode = next;
            }
        }

        /// <summary>
        /// foreach_root 根节点遍历  注意用lambda表达式达成无GC
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        public static void foreach_root<T, T1, T2, T3>(this T rootNode, System.Action<T, T1, T2, T3> action, in T1 param, in T2 param1, T3 param2) where T : Deque2<T>.INode
        {
            if (!FrontCheck(rootNode)) return;

            T curNode = rootNode;
            while (curNode != null)
            {
                var next = curNode.next;    //防止 action 调用 clear
                action.Invoke(curNode, param, param1, param2);
                curNode = next;
            }
        }


        static bool FrontCheck<T>(T rootNode) where T : Deque2<T>.INode
        {
            if (rootNode.last != null)
            {
                rootNode.DebugError("非法操作");
                return false;
            }
            return true;
        }

    }


}
