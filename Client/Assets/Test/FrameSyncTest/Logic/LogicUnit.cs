/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/12/14 23:05:54
/// 功能描述:  
****************************************************/

using System.Collections;
using System.Collections.Generic;
using LGF;
using LGF.Log;
using PEMath;

namespace LGF
{
    public abstract class LogicUnit : ILogic
    {
        #region Key Properties
        //逻辑位置
        PEVector3 logicPos;
        public PEVector3 LogicPos { get => logicPos; 
            set { 
                logicPos = value;
                IsPosChanged = true;
            } }

        //逻辑方向
        PEVector3 logicDir;
        public PEVector3 LogicDir { get => logicDir;
            set { 
                logicDir = value;
                isDirChanged = true;
            } }


        public virtual PEInt LogicMoveSpeed { get; set; }



        #endregion

        public bool isDirChanged = false;

        /// <summary>
        /// 逻辑位置改变
        /// </summary>
        public bool IsPosChanged = false;

        public abstract void LogicInit();
        public abstract void LogicTick();
        public abstract void LogicUnInit();

       
    }
}




interface ILogic
{
    void LogicInit();
    void LogicTick();
    void LogicUnInit();


}