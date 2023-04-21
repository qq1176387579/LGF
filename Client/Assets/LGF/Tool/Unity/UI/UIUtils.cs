using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF.InputSystem;

namespace LGF.Util
{
    public static class UIUtils
    {
        /// <summary>
        /// ��������ת��Ϊ��Ļ����
        /// </summary>
        /// <param name="worldPoint">��Ļ����</param>
        /// <returns></returns>
        public static Vector2 WorldPointToScreenPoint(Camera camera, in Vector3 worldPoint)
        {
            // Camera.main ���������
            Vector2 screenPoint = camera.WorldToScreenPoint(worldPoint);
            return screenPoint;
        }


        /// <summary>
        /// ��Ļ����ת��Ϊ��������
        /// </summary>
        /// <param name="screenPoint">��Ļ����</param>
        /// <param name="planeZ">��������� Z ƽ��ľ���</param>
        /// <returns></returns>
        public static Vector3 ScreenPointToWorldPoint(Camera camera, in Vector2 screenPoint, in float planeZ)
        {
            // Camera.main ���������
            Vector3 position = new Vector3(screenPoint.x, screenPoint.y, planeZ);
            Vector3 worldPoint = camera.ScreenToWorldPoint(position);
            return worldPoint;
        }


        /// <summary>
        /// UI ����ת��Ϊ��Ļ����
        /// </summary>
        /// <param name="uiCamera"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Vector2 UIPointToScreenPoint(Camera uiCamera, RectTransform rect)
        {

            return UIPointToScreenPoint(uiCamera, rect.position);
        }

        /// <summary>
        ///  UI ����ת��Ϊ��Ļ����
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
        /// ��Ļ����ת��Ϊ UGUI ����
        /// </summary>
        /// <param name="uiCamera"></param>
        /// <param name="rt"></param>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        public static Vector3 ScreenPointToUIPoint(Camera uiCamera, RectTransform rt,in Vector2 screenPoint)
        {
            // �� Canvas renderMode Ϊ RenderMode.ScreenSpaceCamera��RenderMode.WorldSpace ʱ uiCamera ����Ϊ��
            // �� Canvas renderMode Ϊ RenderMode.ScreenSpaceOverlay ʱ uiCamera ����Ϊ��
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, screenPoint, uiCamera, out Vector3 globalMousePos);
            // ת����� globalMousePos ʹ�����淽����ֵ
            // target Ϊ��Ҫʹ�õ� UI RectTransform
            // rt ������ target.GetComponent<RectTransform>(), Ҳ������ target.parent.GetComponent<RectTransform>()
            // target.transform.position = globalMousePos;
            return globalMousePos;
        }

        /// <summary>
        /// ��Ļ����ת��Ϊ UGUI RectTransform �� anchoredPosition
        /// </summary>
        /// <param name="uiCamera"></param>
        /// <param name="parentRT"></param>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        public static Vector2 ScreenPointToUILocalPoint(Camera uiCamera, RectTransform parent,in Vector2 screenPoint)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPoint, uiCamera, out Vector2 localPos);
            // ת����� localPos ʹ�����淽����ֵ
            // target Ϊ��Ҫʹ�õ� UI RectTransform
            // parentRT �� target.parent.GetComponent<RectTransform>()
            // ���ֵ target.anchoredPosition = localPos; ���� target.localPosition = localPos;
            return localPos;
        }

        /// <summary>
        /// ���԰汾  ���濴��ô��
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
        /// ��������ת��Ļ����
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
        /// ����תUI����
        /// </summary>
        /// <param name="uiCamera"></param>
        /// <param name="parent"></param>
        /// <returns></returns>

        public static Vector2 MousePositioToUIPoint(Camera uiCamera, RectTransform parent)
        {
            return ScreenPointToUILocalPoint(uiCamera, parent, InputHelper.mousePosition);
        }

        /// <summary>
        /// ���λ��ת��������
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
