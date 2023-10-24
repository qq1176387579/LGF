/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2023/10/23 21:05:23
/// 功能描述:  简单实现移动遥感
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Test;
using LGF.Util;

public class TestJoystick : MonoBehaviour
{
    public System.Action<Vector2> MoveEvent;

    [Header("感应范围")]
    public Image imgTouch;//感应范围
    [Header("遥感背景")]
    public Image imgDirBg;//遥感背景
    [Header("遥感方向点")]
    public Image imgDirPoint;//遥感方向点
    [Header("UI摄像机")]
    public Camera UICamera;
    private Vector2 startPos = Vector2.zero;//开始位置
    private Vector2 defaultPos = Vector2.zero;//默认位置
    //外部调整
    public float pointDis = 150f;//最大距离
    public Vector2 differenceValue = Vector2.zero;  //pivot值为 0.5f 0.5f 的时候 在中间
    private void Awake()
    {

        defaultPos = imgDirBg.rectTransform.anchoredPosition;
        if (Mathf.Abs(imgTouch.rectTransform.pivot.x - 0.5f) > 0.01f) { //不在中间
            //自适应
            int f = imgTouch.rectTransform.pivot.x - 0.5f < -0.49 ? -1 : 1;
            differenceValue.x = imgTouch.rectTransform.rect.width / 2 * f;
        }

        if (Mathf.Abs(imgTouch.rectTransform.pivot.y - 0.5f) > 0.01f) { //不在中间
            //自适应
            int f = imgTouch.rectTransform.pivot.y - 0.5f < -0.49 ? -1 : 1;
            differenceValue.y = imgTouch.rectTransform.rect.height / 2 * f;
        }

        //imgDirPoint.gameObject.SetActive(false);
        Init();
    }

    private void Init()
    {
        OnclickDown(imgTouch.gameObject, (PointerEventData evt) => {
            Vector2 vector =  UIUtils.ScreenPointToUILocalPoint(UICamera, imgTouch.rectTransform, evt.position);
            startPos = vector + differenceValue;
            //Debug.Log("vector" + vector + "   differenceValue :" + differenceValue);
            //imgDirPoint.gameObject.SetActive(true);
            imgDirBg.rectTransform.anchoredPosition = startPos;
            imgDirPoint.rectTransform.anchoredPosition = startPos;
        });
        OnclickUp(imgTouch.gameObject, (PointerEventData evt) => {
            imgDirBg.rectTransform.anchoredPosition = defaultPos;
            //imgDirPoint.gameObject.SetActive(false);
            imgDirPoint.rectTransform.anchoredPosition = Vector2.zero;
            MoveEvent?.Invoke(Vector2.zero);
        });
        OnDrag(imgTouch.gameObject, (PointerEventData evt) => {
            Vector2 curPos = UIUtils.ScreenPointToUILocalPoint(UICamera, imgTouch.rectTransform, evt.position) + differenceValue;
            Vector2 dir = curPos - startPos;
            float len = dir.magnitude;
            
            if (len > pointDis) {
                Vector2 clamDir = Vector2.ClampMagnitude(dir, pointDis);
                imgDirPoint.rectTransform.anchoredPosition = startPos + clamDir;
                MoveEvent?.Invoke(dir.normalized);
            }
            else {
                imgDirPoint.rectTransform.anchoredPosition = curPos;
                MoveEvent?.Invoke(dir.normalized * (len / pointDis));
                //Debug.Log(dir.normalized * (len / pointDis));

            }

        });

    }

    #region 点击事件封装
  
    private void OnclickDown(GameObject go, Action<PointerEventData> cb) => go.GetOrAddComponent<TestListener>().onClickDown = cb;
    private void OnclickUp(GameObject go, Action<PointerEventData> cb) => go.GetOrAddComponent<TestListener>().onClickUp = cb;
    private void OnDrag(GameObject go, Action<PointerEventData> cb) => go.GetOrAddComponent<TestListener>().onDrag = cb;

        #endregion

}
