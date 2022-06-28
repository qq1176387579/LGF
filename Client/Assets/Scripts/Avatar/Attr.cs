using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LGF;

namespace N_Attribute
{
    using AttrData = Dictionary<ATTR_TYPE, int>;
    using AttrData2 = Dictionary<int, int>;
    
    public class Attr : Poolable<Attr>
    {
        private static Attr zero;
        public static Attr Zero
        {
            get
            {
                if (zero == null)
                {
                    zero = new Attr();
                }
                return zero;
            }
        }


        public int this[ATTR_TYPE type] { get => GetAttrByType(type); set => SetAttrByType(type, value); }

        public AttrData data = new AttrData();
        public Attr()
        {
        }

        //skewing偏移量
        public Attr(int[] intList, int skewing = 0)
        {
            for (int i = 0; i < intList.Length; i++)
            {
                if (intList[i] != 0)
                {
                    SetAttrByType((ATTR_TYPE)(i + skewing), intList[i]);
                }
            }
        }

        //int skewing = 0
        //public Attr(int[] intList, int skewing = 0)
        //{
        //    for (int i = 0; i < intList.Length; i++)
        //    {
        //        if (intList[i] != 0)
        //        {
        //            SetAttrByType((ATTR_TYPE)(i + skewing), intList[i]);
        //        }
        //    }
        //}

        public Attr(int[][] intList)
        {
            for (int i = 0; i < intList.Length; i++)
                SetAttrByType((ATTR_TYPE)(int)intList[i][0], intList[i][1]);
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="attr"></param>
        public void AddAttrData(AttrData attrData)
        {
            foreach (var v in attrData)
            {
                AddAttrByType(v.Key, v.Value);
            }
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="attr"></param>
        public void AddAttrData(AttrData2 attrData)
        {
            foreach (var v in attrData)
            {
                AddAttrByType((ATTR_TYPE)v.Key, v.Value);
            }
        }




        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="data"></param>
        public void AddAttr(Attr data)
        {
            AddAttrData(data.GetData());
        }

        /// <summary>
        /// 获得值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetAttrByType(ATTR_TYPE type)
        {
            return data.ContainsKey(type) ? data[type] : 0;
        }


        public int GetAttrByType(int type)
        {
            return GetAttrByType((ATTR_TYPE)type);
        }


        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="val"></param>
        public void SetAttrByType(ATTR_TYPE type, int val)
        {
            data[type] = val;
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="val"></param>
        public void AddAttrByType(ATTR_TYPE type, int val)
        {
            SetAttrByType(type, GetAttrByType(type) + val);
        }



        /// <summary>
        /// 计算当前类型属性变成目标属性的需要的值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="targetVal">目标属性</param>
        public int GetChangeValByType(ATTR_TYPE type, int targetVal)
        {
            return targetVal - GetAttrByType(type);
        }


        /// <summary>
        /// 减少属性
        /// </summary>
        public void DecreaseAttr(Attr data)
        {
            DecreaseAttrData(data.GetData());
        }


        /// <summary>
        /// 减少属性
        /// </summary>
        /// <param name="attr"></param>
        public void DecreaseAttrData(AttrData attrData)
        {
            foreach (var v in attrData)
            {
                AddAttrByType(v.Key, -v.Value);
            }
        }

        public AttrData GetData()
        {
            return data;
        }

        public void ClearData()
        {
            data.Clear();
        }


        protected override void OnGet()
        {
            base.OnGet();
            ClearData();
        }
        protected override void OnRelease()
        {
            base.OnRelease();
            ClearData();
        }

        public string GetLogInfo()
        {
            string str1 = null;
            var str = StringPool.GetStringBuilder();
            foreach (var item in data)
            {
                str.Append(item.Key.ToString());
                str.Append("\t:\t");
                str.Append(item.Value);
                str.Append("\n");
            }
            str1 = str.ToString();
            StringPool.Release(str);
            return str1;
        }

        public void Log()
        {
            UnityEngine.Debug.Log("Attr >> " + GetLogInfo());
        }

    }
}
