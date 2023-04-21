using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF.InputSystem;

namespace LGF.Util
{
    public static class UIUtils
    {
        /// <summary>
        /// 世界坐标转换为屏幕坐标
        /// </summary>
        /// <param name="worldPoint">屏幕坐标</param>
        /// <returns></returns>
        public static Vector2 WorldPointToScreenPoint(Camera camera, in Vector3 worldPoint)
        {
            // Camera.main 世界摄像机
            Vector2 screenPoint = camera.WorldToScreenPoint(worldPoint);
            return screenPoint;
        }


        /// <summary>
        /// 屏幕坐标转换为世界坐标
        /// </summary>
        /// <param name="screenPoint">屏幕坐标</param>
        /// <param name="planeZ">距离摄像机 Z 平面的距离</param>
        /// <returns></returns>
        public static Vector3 ScreenPointToWorldPoint(Camera camera, in Vector2 screenPoint, in float planeZ)
        {
            // Camera.main 世界摄像机
            Vector3 position = new Vector3(screenPoint.x, screenPoint.y, planeZ);
            Vector3 worldPoint = camera.ScreenToWorldPoint(position);
            return worldPoint;
        }


        /// <summary>
        /// UI 坐标转换为屏幕坐标
        /// </summary>
        /// <param name="uiCamera"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Vector2 UIPointToScreenPoint(Camera uiCamera, RectTransform rect)
        {

            return UIPointToScreenPoint(uiCamera, rect.position);
        }

        /// <summary>
        ///  UI 坐标转换为屏幕坐标
        /// </summary>
        /// <param name="uiCamera"></param>
        /// <param name="worldPoint"></param>
        /// <returns></returns>
        public static Vector2 UIPointToScreenPoint(Camera uiCamera,in Vector3 worldPoint)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, worldPoint);
            return screenPoint;
        }



        /// <summary>
        /// 屏幕坐标转换为 UGUI 坐标
        /// </summary>
        /// <param name="uiCamera"></param>
        /// <param name="rt"></param>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        public static Vector3 ScreenPointToUIPoint(Camera uiCamera, RectTransform rt,in Vector2 screenPoint)
        {
            // 当 Canvas renderMode 为 RenderMode.ScreenSpaceCamera、RenderMode.WorldSpace 时 uiCamera 不能为空
            // 当 Canvas renderMode 为 RenderMode.ScreenSpaceOverlay 时 uiCamera 可以为空
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, screenPoint, uiCamera, out Vector3 globalMousePos);
            // 转换后的 globalMousePos 使用下面方法赋值
            // target 为需要使用的 UI RectTransform
            // rt 可以是 target.GetComponent<RectTransform>(), 也可以是 target.parent.GetComponent<RectTransform>()
            // target.transform.position = globalMousePos;
            return globalMousePos;
        }

        /// <summary>
        /// 屏幕坐标转换为 UGUI RectTransform 的 anchoredPosition
        /// </summary>
        /// <param name="uiCamera"></param>
        /// <param name="parentRT"></param>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        public static Vector2 ScreenPointToUILocalPoint(Camera uiCamera, RectTransform parent,in Vector2 screenPoint)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPoint, uiCamera, out Vector2 localPos);
            // 转换后的 localPos 使用下面方法赋值
            // target 为需要使用的 UI RectTransform
            // parentRT 是 target.parent.GetComponent<RectTransform>()
            // 最后赋值 target.anchoredPosition = localPos; 或者 target.localPosition = localPos;
            return localPos;
        }

        /// <summary>
        /// 测试版本  后面看怎么改
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="worldCamera"></param>
        /// <param name="worldPoint"></param>
        /// <param name="uiCamera"></param>
        public static void WorldPointToUIPointTo(RectTransform rect, Camera worldCamera, in Vector3 worldPoint, Camera uiCamera)
        {
            rect.anchoredPosition = WorldPointToUIPoint(worldCamera, worldPoint, uiCamera, rect.parent as RectTransform);
        }



        /// <summary>
        /// 世界坐标转屏幕坐标
        /// </summary>
        /// <param name="worldCamera"></param>
        /// <param name="worldPoint"></param>
        /// <param name="uiCamera"></param>
        /// <param name="parent"></param>
        /// <returns></returns>

        public static Vector2 WorldPointToUIPoint(Camera worldCamera, in Vector3 worldPoint, Camera uiCamera, RectTransform parent)
        {
            return ScreenPointToUILocalPoint(uiCamera, parent, WorldPointToScreenPoint(worldCamera, worldPoint));
        }

        /// <summary>
        /// 鼠标点转UI坐标
        /// </summary>
        /// <param name="uiCamera"></param>
        /// <param name="parent"></param>
        /// <returns></returns>

        public static Vector2 MousePositioToUIPoint(Camera uiCamera, RectTransform parent)
        {
            return ScreenPointToUILocalPoint(uiCamera, parent, InputHelper.mousePosition);
        }

        /// <summary>
        /// 鼠标位置转世界坐标
        /// </summary>
        /// <param name="worldCamera"></param>
        /// <param name="planeZ"></param>
        /// <returns></returns>
        public static Vector2 MousePositioToWorldPoint(Camera worldCamera, in float planeZ = 0.1f)
        {
            return ScreenPointToWorldPoint(worldCamera, InputHelper.mousePosition, planeZ);
        }




    }

}
