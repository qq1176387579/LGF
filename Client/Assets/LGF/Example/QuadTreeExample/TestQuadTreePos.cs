/***************************************************
/// 作者:      tttt
/// 创建日期:  2022/6/9 22:51:48
/// 修改日期:  
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;
using LGF.DataStruct;

namespace LGF.Example
{
    public class TestQuadTreePos : MonoBehaviour, QuadTree.IPos
    {
        static int count = 0;
        int curval;
        //public Vector3 position = new Vector3();
        private void Awake()
        {
            curval = ++count;
        }
        public MeshRenderer meshRenderer;
        public Material[] material = new Material[2];

        float QuadTree.IPos.x => transform.position.x;

        float QuadTree.IPos.y => transform.position.z;

        public QuadTree.Node node { get; set; }
        QuadTree.IPos Deque2<QuadTree.IPos>.INode.next { get; set; }
        QuadTree.IPos Deque2<QuadTree.IPos>.INode.last { get; set; }
        Deque2<QuadTree.IPos> Deque2<QuadTree.IPos>.INode.deque { get; set; }

        private Color _gizmoColor = Color.green;

        public void SetColor()
        {
            _gizmoColor = Color.red;
            meshRenderer.material = material[1];
        }

        public void ResetColor()
        {
            _gizmoColor = Color.green;
            meshRenderer.material = material[0];
        }



        private void OnDrawGizmos()
        {
            if (!QuadTreeHelper.IsOpenOnDrawGizmos) return;

            Gizmos.color = _gizmoColor;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 1.25f);
        }



    }

}
