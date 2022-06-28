using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LGF;

namespace N_Avatar
{
    using SkillCfg = SkillConfigRow;

    /// <summary>
    /// 技能信息
    /// </summary>
    public class SkillInfo : Poolable<SkillInfo>
    {

        public int cdTime;  //cd时间   暂时先这些  后期可能有等属性
        public SkillCfg config;


        protected override void OnGet()
        {
            base.OnGet();
            cdTime = 0;
            config = null;
        }
    }


}
