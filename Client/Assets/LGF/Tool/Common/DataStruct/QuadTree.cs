/***************************************************
/// 作者:      liuhuan
/// 创建日期:  #CREATIONDATE#
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using UnityEngine;



/*-------------------------
 * 
 * 有两种实现方式    
 * 一种是在顶部查询
 * 一种在当前层查询
 * 
 * 1.这里实现的是顶部查询 (适合范围查询)
 * 
 * 
 * 
 * 如何搜索当前 格子周围的目标
 * 用当前格子的 rect   大一点的 进行搜索  比如rect(0,0,5,5) => rect(-0.5,-0.5,6,6) 
 * 
 * 圆形搜索范围  判定球形范围内  有多少方块与 圆相交 取出所有的
 * 
 *
 * 这里 先实现点坐标  后面实现圆形和方块
 * 
 * 
 * 因为我只是范围搜索  所有我 压线的放在他的上一层
 * 
 * 碰撞检查 写法是 放在当前层
 * 
 * 
 * 
 * 2.当前层查询查询是 (碰撞检查 查询当前节点的就行)  ps：我这里没去实现  后续看吧
 * 
 * 
 * 
 *-------------------------*/


namespace LGF.DataStruct
{


    public class QuadTree
    {
        public interface IPos : Deque2<IPos>.INode //IList2Idx<IPos>
        {
            float x { get; }
            float y { get; }
            Node node { get; set; }
        }

        public Node root;   //跟节点
        public QuadTreeConfig config;
        /// <summary>
        /// 维护层级  
        /// 
        ///  用空间换时间 来优化
        ///  先删除所有的 需要更新的对象   把他存在的节点中 对于跟新对象删除
        ///  并把对应节点 放入层级list里面的 list[MaxLayer] 中
        ///  然后把所有需要重新更新 在重新插入到节点内 （插入不会影响 节点的相对结构 也不会删除节点 所有改操作没有问题）
        ///  然后在由底向上 维护节点  删除多余的空节点   （最底层一定是叶子节点，如果叶子节点合并 就成新的叶子节点)
        ///  如果节点改变了 且是叶子节点  并且删除的数量 大于新添加的数量
        ///  那么就将其父添加到维护队列进行检测
        ///  如何需要维护的节点不是  叶子节点 检测是否能合并  能合并 就执行合并操作  操作完成后将父节点加入维护队列进行检测
        ///  如果不能合并   那边终止该节点操作
        ///  实际效果一般  不过思路可以用于 burst + job
        /// </summary>
        List2<Node>[] maintainLayer;   //维护层级
        List<IPos> tmplist = new List<IPos>(); //缓存

        public QuadTree(Rect rect_, int maxLayer, int maxObjs)
        {
            this.config = new QuadTreeConfig();
            this.config.maxObjs = maxObjs;
            this.config.maxLayer = maxLayer;
            root = Node.Get(this, rect_);
            maintainLayer = new List2<Node>[maxLayer + 1];
            for (int i = 0; i <= maxLayer; i++)
                maintainLayer[i] = new List2<Node>();
        }

#if !NOT_UNITY
        public void OnDrawGizmos()
        {
            root.OnDrawGizmos();
        }
#endif


        public void Insert(IPos pos)
        {
            root.Insert(pos);
        }


        public void OnMove(IPos pos)
        {
            root.OnMove(pos);
        }


        public void OnMove2<T>(List<T> list) where T : IPos
        {


            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].node == null) continue;
                list[i].node.Remove2(list[i], tmplist);  //移除
            }

            //在添加  插入
            for (int i = tmplist.Count - 1; i >= 0; i--)
            {
                root.Insert(tmplist[i]);
                tmplist.RemoveAt(i);
            }

            for (int i = config.maxLayer; i >= 0; i--)  //拆分
            {
                maintainLayer[i].Clear((a) => a.CheckSplit2());
            }

        }


        public void FindRect(Rect rect_, List<Deque2<IPos>> list)
        {
            root.FindRect(rect_, list);
        }

        public void AddMaintainLayer(Node node)
        {
            if (node.NeedMaintain)
            {
                this.DebugError("非法操作");
                //在维护队列中不操作
                return;
            }

            maintainLayer[node.layer].Add(node);

        }

        /// <summary>
        /// 配置
        /// </summary>
        public class QuadTreeConfig
        {
            //格子的最小值
            //minWidth = maxWidth / 2^maxLayer
            //minHight = maxHight / 2^maxLayer
            public int maxLayer = 9;    //9层就够了
            public int maxObjs = 6;     //临界值
        }


        public partial class Node
        {

            public bool NeedMaintain => ((IList2Idx<Node>)this).idx != 0;   //需要维护

            /// <summary>
            /// 移除自己
            /// </summary>
            public void Remove2(IPos treePos, List<IPos> list)
            {
                if (treePos.node.CanInsert(treePos)) //还在点内
                {
                    return;
                }

                if (treePos.node != this)
                {
                    treePos.node.Remove2(treePos, list);  //移除自己
                    return;
                }

                list.Add(treePos);
                treePos.node = null;
                treePos.remove();   //移除自己

                if (!NeedMaintain)
                    tree.AddMaintainLayer(this);
            }


            /// <summary>
            /// 检测拆分
            /// </summary>
            public void CheckSplit2()
            {
                //if (IsRoot) return;

                if (!IsSplit)   //叶子节点 不做检查 检查父节点
                {
                    if (IsRoot) return;

                    if (!parent.NeedMaintain)
                    {
                        tree.AddMaintainLayer(parent);
                    }

                    return;
                }

                //-----------判断是否要合并  

                //子节点有一个不是叶子节点 不做处理
                if (next[0].IsSplit | next[1].IsSplit | next[2].IsSplit | next[3].IsSplit)
                {
                    return;
                }

                //全是叶子节点 且 数量大于拆分条件
                if (next[0].listCount + next[1].listCount + next[2].listCount + next[3].listCount >= config.maxObjs)
                {
                    return;
                }


                for (int i = 0; i < next.Length; i++)
                    next[i].deque.clear<IAdd<IPos>>((a, _this) => { _this.Add(a); }, this);

                SubSplit();

                if (!IsRoot && !parent.NeedMaintain)
                    tree.AddMaintainLayer(parent);
            }

        }





        /// <summary>
        /// 四叉树节点
        /// </summary>
        public partial class Node : Poolable<Node>, IAdd<IPos>, IList2Idx<Node>, Iinsert<IPos>
        {
            public int layer;  //层级
            Rect rect;
            Node parent;
            Node[] next = new Node[4];
            Deque2<IPos> deque = new Deque2<IPos>();

            //List2<IPos> mlist; //= new List2<IPos>();
            QuadTree tree;
            QuadTreeConfig config => tree.config;


            int listCount => deque.Count;

            /// <summary>
            /// 是否拆分
            /// </summary>
            bool IsSplit => next[0] != null;

            public bool IsRoot => parent == null;

            int IList2Idx<Node>.idx { get; set; }

            /// <summary>
            /// 拆分
            /// </summary>
            void Split()
            {
                if (IsSplit)
                {
                    this.DebugError("非法操作");
                    return;
                }

                float width = rect.width / 2;
                float height = rect.height / 2;

                float posx = rect.x;
                float posy = rect.y;

                next[0] = Get().Init(this, new Rect(posx, posy, width, height));                     //左上
                next[1] = Get().Init(this, new Rect(posx + width, posy, width, height));             //右上
                next[2] = Get().Init(this, new Rect(posx, posy + height, width, height));            //左下
                next[3] = Get().Init(this, new Rect(posx + width, posy + height, width, height));    //右下
            }



            /// <summary>
            /// 卸载拆分
            /// </summary>
            public void SubSplit()
            {
                if (!IsSplit)
                {
                    this.DebugError("非法操作");
                    return;
                }

                next[0].Release();
                next[0] = null;

                next[1].Release();
                next[1] = null;

                next[2].Release();
                next[2] = null;

                next[3].Release();
                next[3] = null;
            }

            public static Node Get(QuadTree tree, Rect rect_)
            {
                var _this = Get();
                _this.rect = rect_;
                _this.tree = tree;
                _this.layer = 0;
                return _this;
            }

            public Node Init(Node parent_, Rect rect_)
            {
                parent = parent_;
                rect = rect_;
                layer = parent.layer + 1;
                tree = parent.tree;
                return this;
            }


            public bool CanInsert(IPos pos)
            {
                //return rect.xMin < pos.x && rect.yMin < pos.y && rect.xMax > pos.x && rect.yMax > pos.y;
                return rect.xMin <= pos.x && rect.yMin <= pos.y && rect.xMax >= pos.x && rect.yMax >= pos.y;
            }



            public void Insert(IPos pos)
            {
                if (!CanInsert(pos))
                    return;

                InsertEx(pos);
            }


            void InsertEx(IPos pos)
            {
                if (IsSplit)
                {
                    for (int i = 0; i < next.Length; i++)
                    {
                        if (next[i].CanInsert(pos))
                        {
                            next[i].InsertEx(pos);
                            return;
                        }
                    }

                    //this.DebugError("------------出错----start----压线了------");
                    //sLog.Debug($" x: {pos.x} y: {pos.y}");
                    #region 注释
                    //for (int i = 0; i < next.Length; i++)
                    //{
                    //    sLog.Debug(next[i].rect.ToString() + "   CanInsert: " + next[i].CanInsert(pos) + $"\n {next[i].rect.xMin} <= {pos.x} && {next[i].rect.yMin} <= {pos.y} && {next[i].rect.xMax} >= {pos.x} && {next[i].rect.yMax} >= {pos.y}  ");
                    //}
                    //sLog.Debug(rect.ToString() + "   CanInsert: " + CanInsert(pos) + $"\n {rect.xMin} <= {pos.x} && {rect.yMin} <= {pos.y} && {rect.xMax} >= {pos.x} && {rect.yMax} >= {pos.y}  ");

                    //var _next = parent.next;
                    //for (int i = 0; i < _next.Length; i++)
                    //{
                    //    sLog.Debug(_next[i].rect.ToString() + "   CanInsert: " + _next[i].CanInsert(pos) + $"\n {_next[i].rect.xMin} <= {pos.x} && {_next[i].rect.yMin} <= {pos.y} && {_next[i].rect.xMax} >= {pos.x} && {_next[i].rect.yMax} >= {pos.y}  ");
                    //}
                    //this.DebugError("------------出错----end----------");
                    #endregion
                    Add(pos);
                    //sLog.Debug("  pos.idx " + pos.idx);
                    return;
                }

                Add(pos);
                if (layer < config.maxLayer && deque.Count >= config.maxObjs)
                {
                    Split();

                    /*
                     *存在逻辑问题  假设树里面的list xy 
                     *如果 tree.OnMove()时候  tree.list[4] 移动一个新的 节点 执行Split(),  然而tree.list[0] 这时候已经不在当前节点内了 
                     *这时候会出现bug  tree.list[0] 无法进入任何4个节点  当前节点也进入不了 
                     */
                    //mlist.Clear(OnListClear);

                    //需要取出所有数据  应该可能插入 如果边插入 边删除 会有问题
                    var curNode = deque.pop_all(out int count);

                    curNode.foreach_root<IPos, Iinsert<IPos>>((node, _this) => { node.clear(); _this.Insert(node); }, this);

                }
            }

            void Add(IPos pos)
            {
                deque.push_back(pos);
                pos.node = this;
            }


            /// <summary>
            /// 搜索
            /// </summary>
            /// <param name="rect"></param>
            /// <returns></returns>
            public void FindRect(Rect rect_, List<Deque2<IPos>> list)
            {
                //return Math.max(curRect.x,tarRect.x) <= Math.min(curRect.right ,tarRect.right) 算法1
                //other.xMax > xMin && other.xMin < xMax 算法2
                if (!rect_.Overlaps(rect)) //重叠
                {
                    return;
                }

                if (IsSplit)
                {
                    for (int i = 0; i < next.Length; i++)
                    {
                        next[i].FindRect(rect_, list);
                    }
                    return;
                }

                if (deque.Count > 0)
                    list.Add(deque);
            }


            /// <summary>
            /// 移动了坐标 检查下是否需要更新
            /// </summary>
            /// <param name="treePos"></param>
            public void OnMove(IPos treePos)
            {
                if (treePos.node == null || !IsRoot)   //非法操作
                {
                    this.DebugError("OnMove 非法操作  treePos 不在树内");
                    return;
                }

                if (treePos.node.CanInsert(treePos)) //还在点内
                {
                    return;
                }

                treePos.node.Remove(treePos);
                Insert(treePos);
            }

            /// <summary>
            /// 移除
            /// </summary>
            public void Remove(IPos treePos)
            {
                if (treePos.node != this)
                {
                    treePos.node.Remove(treePos);
                    return; //移除自己
                }

                deque.remove(treePos);
                treePos.node = null;
                CheckSplit();
            }

            /// <summary>
            /// 检测拆分
            /// </summary>
            void CheckSplit()
            {
                if (!IsSplit && !IsRoot)   //叶子节点 不做检查 检查父节点
                {
                    parent.CheckSplit();
                    return;
                }

                //-----------判断是否要合并  ------------------
                //子节点有一个不是叶子节点 不做处理     
                if (next[0].IsSplit | next[1].IsSplit | next[2].IsSplit | next[3].IsSplit ||
                    next[0].listCount + next[1].listCount + next[2].listCount + next[3].listCount >= config.maxObjs)    //全是叶子节点 且 数量大于拆分条件
                    return;

                for (int i = 0; i < next.Length; i++)
                    next[i].deque.clear<IAdd<IPos>>((a, _this) => { _this.Add(a); }, this);

                SubSplit();

                if (!IsRoot)
                    parent.CheckSplit();    //检查父节点
            }

#if !NOT_UNITY
            public void OnDrawGizmos()
            {
                if (!QuadTreeHelper.IsOpenOnDrawGizmos) return;

                if (IsSplit)
                {
                    for (int i = 0; i < next.Length; i++)
                    {
                        next[i].OnDrawGizmos();
                    }
                }

                Gizmos.color = Color.cyan * ((7f + config.maxLayer - layer) / (7 + config.maxLayer));

                Vector3 p1 = new Vector3(rect.x, 0.1f, rect.y);
                Vector3 p2 = new Vector3(rect.xMax, 0.1f, rect.y);
                Vector3 p3 = new Vector3(rect.xMax, 0.1f, rect.yMax);
                Vector3 p4 = new Vector3(rect.x, 0.1f, rect.yMax);

                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p3, p4);
                Gizmos.DrawLine(p4, p1);

            }
#endif

            void IAdd<IPos>.Add(IPos val)
            {
                this.Add(val);
            }

            void Iinsert<IPos>.Insert(IPos val)
            {
                InsertEx(val);
            }
        }

        
        public interface IAdd<T>
        {
            void Add(T val);
        }


        public interface Iinsert<T>
        {
            void Insert(T val);
        }
    }




    public static class QuadTreeHelper
    {
        public static bool IsOpenOnDrawGizmos = true;

    }

}

