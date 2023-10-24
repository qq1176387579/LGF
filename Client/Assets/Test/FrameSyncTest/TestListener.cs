/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2023/10/23 21:03:30
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
    public class TestListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public Action<PointerEventData> onClickDown;
        public Action<PointerEventData> onClickUp;
        public Action<PointerEventData> onDrag;
        /// <summary>
        /// 拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            if (onDrag != null) {
                onDrag(eventData);
            }
        }
        /// <summary>
        /// 按下
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (onClickDown != null) {
                onClickDown(eventData);
            }
        }
        /// <summary>
        /// 抬起
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (onClickUp != null) {
                onClickUp(eventData);
            }
        }
    }
}

