/*************************************************
	作者: Plane
	邮箱: 1785275942@qq.com	
	功能: 英雄表现控制

    //=================*=================\\
           教学官网：www.qiqiker.com
           官方微信服务号: qiqikertuts
           Plane老师微信: PlaneZhong
               ~~获取更多教学资讯~~
    \\=================*=================//
*************************************************/

using UnityEngine;
using LGF;
/// <summary>
/// 技能释放显示，旁白（击杀）
/// </summary>
public class HeroView : MainViewUnit {
    //public Transform sk1;
    //public Transform sk2;
    //public Transform sk3;

    PlayerUnit player;

    public override void Init(LogicUnit logicUnit) {
        base.Init(logicUnit);

        player = logicUnit as PlayerUnit;

        //skillRange.gameObject.SetActive(false);
        //if(sk1 != null) {
        //    sk1.gameObject.SetActive(false);
        //}
        //if(sk2 != null) {
        //    sk2.gameObject.SetActive(false);
        //}
        //if(sk3 != null) {
        //    sk3.gameObject.SetActive(false);
        //}
    }

    //public void DisableSkillGuide(int skillIndex) {
    //    switch(skillIndex) {
    //        case 1:
    //            if(sk1 != null) {
    //                sk1.gameObject.SetActive(false);
    //            }
    //            break;
    //        case 2:
    //            if(sk2 != null) {
    //                sk2.gameObject.SetActive(false);
    //            }
    //            break;
    //        case 3:
    //            if(sk3 != null) {
    //                sk3.gameObject.SetActive(false);
    //            }
    //            break;
    //        default:
    //            break;
    //    }
    //}

    protected override Vector3 GetUnitViewDir()
    {
        //玩家朝向使用UI输入位置朝向，不使用物理引擎运算修正方向
        return player.InputDir.ConvertViewVector3();
    }

    //public void SetSkillGuide(int skillIndex, bool state, ReleaseModeEnum mode, Vector3 vector)
    //{
    //    switch (skillIndex)
    //    {
    //        case 1:
    //            sk1.gameObject.SetActive(state);
    //            if (state)
    //            {
    //                UpdateSkillGuide(sk1, mode, vector);
    //            }
    //            break;
    //        case 2:
    //            sk2.gameObject.SetActive(state);
    //            if (state)
    //            {
    //                UpdateSkillGuide(sk2, mode, vector);
    //            }
    //            break;
    //        case 3:
    //            sk3.gameObject.SetActive(state);
    //            if (state)
    //            {
    //                UpdateSkillGuide(sk3, mode, vector);
    //            }
    //            break;
    //        default:
    //            break;
    //    }
    //}

    //void UpdateSkillGuide(Transform sk, ReleaseModeEnum mode, Vector3 vector)
    //{
    //    if (mode == ReleaseModeEnum.Postion)
    //    {
    //        sk.localPosition = vector;
    //    }
    //    else
    //    {
    //        float angle = Vector2.SignedAngle(new Vector2(vector.x, vector.z), new Vector2(0, 1));
    //        sk.localEulerAngles = new Vector3(0, angle, 0);
    //    }
    //}
}
