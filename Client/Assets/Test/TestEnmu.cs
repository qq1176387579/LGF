/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/15 20:58:47
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGF;
using LGF.Log;
using System.Diagnostics;
#pragma warning disable CS0219

namespace Test
{
    public class EventCenter : EventCenterEnum<TestGameEventTypes>
    {

    }

    public enum TestGameEventTypes
    {
        TestExample = LGF.EvtHelper.EventsType.TestExample,
    }

    public class TestEnmu : MonoBehaviour
    {
        public enum s
        {
            fff
        }
        // Start is called before the first frame update
        void Start()
        {


        }

        GameEventType game1, game2;

        // Update is called once per frame
        void Update()
        {
            game1 =  GameEventType.GM;
            game2 =  GameEventType.NetStatus;
            bool f;

            EventCenter.Fire(TestGameEventTypes.TestExample);   //
            double time = 0;
            for (int i = 0; i < 1000000; i++)
            {
                var now = System.DateTime.Now;
                f = game1.GetHashCode()== game2.GetHashCode();
                //int t = s.fff.GetHashCode();
                time += (System.DateTime.Now - now).TotalMilliseconds;
            }

            UnityEngine.Debug.Log(time);
            time = 0;
            for (int i = 0; i < 1000000; i++)
            {
                var now = System.DateTime.Now;
                //int t = (int)s.fff;
                f = game1.CompareTo(game2) == 0;
       
                time += (System.DateTime.Now - now).TotalMilliseconds;
            }
            UnityEngine.Debug.Log(time);
            UnityEngine.Debug.Log("---------");

        }
    }


}

