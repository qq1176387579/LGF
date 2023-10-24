using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LGF.Util
{
    public static class TransformExtend
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T t = go.GetComponent<T>();
            if (t == null) {
                t = go.AddComponent<T>();
            }
            return t;
        }
        
        public static void SetActive(this Transform tf, bool f)
        {
            tf.gameObject.SetActive(f);
        }

        public static void Reset(this Transform tf)
        {
            tf.localPosition = Vector3.zero;
            tf.localScale = Vector3.one;
            tf.localRotation = Quaternion.identity;
        }
    }
}
