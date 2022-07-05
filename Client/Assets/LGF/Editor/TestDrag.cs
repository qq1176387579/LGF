/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/3 17:49:32
/// 功能描述:  拖拽获得文件 获得路径
****************************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
 
public class TestDrag : EditorWindow {
 
    string path;
    Rect rect;
 
    [MenuItem("Window/TestDrag")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(TestDrag));
    }
 
    void OnGUI()
    {
        EditorGUILayout.LabelField("路径");
        //获得一个长300的框
        rect = EditorGUILayout.GetControlRect(GUILayout.Width(300));
        //将上面的框作为文本输入框
        path = EditorGUI.TextField(rect, path);
 
        //如果鼠标正在拖拽中或拖拽结束时，并且鼠标所在位置在文本输入框内
        if ((Event.current.type == EventType.DragUpdated
          || Event.current.type == EventType.DragExited)
          && rect.Contains(Event.current.mousePosition))
        {
            //改变鼠标的外表
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (Event.current.type == EventType.DragExited)
            {
                if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                {
                    path = DragAndDrop.paths[0];
                }
            }
           
        }
    }
}