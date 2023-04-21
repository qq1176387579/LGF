using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
#endif
//

namespace LGF.InputSystem
{
    public class InputHelper
    {
        #region Êó±ê Mouse
        public static Vector3 mousePosition => GetMousePos();

        public static Vector3 GetMousePos()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current.position.ReadValue();
#else
            return Input.mousePosition;
#endif

        }

#if ENABLE_INPUT_SYSTEM

        static UnityEngine.InputSystem.Controls.ButtonControl GetMouse(int i)
        {
            switch (i) {
                case 0:
                    return UnityEngine.InputSystem.Mouse.current.leftButton;
                case 1:
                    return UnityEngine.InputSystem.Mouse.current.rightButton;
                case 2:
                    return UnityEngine.InputSystem.Mouse.current.middleButton;
                default:
                    return null;
            }
        }

        public enum MouseButton
        {
            Down,
            Pressed,
            Up
        }

        public static bool GetMouseButton(int i, MouseButton button)
        {
            var bt = GetMouse(i);
            if (bt == null) {
                return false;
            }
           
            switch (button) {
                case MouseButton.Down:
                    return bt.wasPressedThisFrame;
                case MouseButton.Up:
                    return bt.wasReleasedThisFrame;
                case MouseButton.Pressed:
                    return bt.isPressed;
                default:
                    break;
            }
            return false;
        }
#endif

        public static bool GetMouseButtonDown(int i)
        {
#if ENABLE_INPUT_SYSTEM
            return GetMouseButton(i, MouseButton.Down);
#else
            return Input.GetMouseButtonDown(i);
#endif
        }


        public static bool GetMouseButtonUp(int i)
        {
#if ENABLE_INPUT_SYSTEM
            return GetMouseButton(i, MouseButton.Up);
#else
            return Input.GetMouseButtonUp(i);
#endif
        }


        public static bool GetMouseButton(int i)
        {
#if ENABLE_INPUT_SYSTEM
            return GetMouseButton(i, MouseButton.Pressed);
#else
            return Input.GetMouseButton(i);
#endif
        }

        #endregion



        #region touches ´¥Ãþ°å


        public class TouchesHelper
        {
            public int Length => GetLength();
            int GetLength()
            {
#if ENABLE_INPUT_SYSTEM
                Touchscreen ts = Touchscreen.current;
                return ts != null ? ts.touches.Count : 0;
#else
                return Input.touches.Length;
#endif
            }

#if ENABLE_INPUT_SYSTEM
            public UnityEngine.InputSystem.Controls.TouchControl this[int id] { get => Touchscreen.current.touches[id]; }
#else
            public Touch this[int id] { get => Input.touches[id]; }
#endif

            public TouchPhase Phase(int idx)
            {
#if ENABLE_INPUT_SYSTEM
                return this[idx].phase.ReadValue();
#else
                return this[idx].phase;
#endif
            }

            public Vector2 Position(int idx)
            {
#if ENABLE_INPUT_SYSTEM
                return this[idx].position.ReadValue();
#else
                return this[idx].position;
#endif
            }
        }



        public static TouchesHelper touches = new TouchesHelper();



        #endregion



    }
}

