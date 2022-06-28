using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LGF;
using LGF.Util;


/// <summary>
/// 随机抽取帮助类
/// </summary>
public class RandomDrawingHelperClass<T> : Poolable<RandomDrawingHelperClass<T>>, IWeight
{
    public T info;

    public int Weight { get; set; }

    protected override void OnGet()
    {
        base.OnGet();
        info = default;
        Weight = 0;
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        Weight = 0;
        info = default;
    }
}

