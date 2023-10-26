using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LGF.Net.KcpServer;
using LGF.Net;


namespace LGF.Server.Hotfix
{
    //public static class BaseHandlerExtend
    //{
    //    public static int ToInt(this string str)
    //    {
    //        if (int.TryParse(str, out int result)) {
    //            return result;
    //        }

    //        return -1;
    //    }
    //}

    public interface IResponseHandler
    {
        public OperationCode OpCode { get; }

        void OnOperationRequest(KcpSession session, string msg);

        void Init();
        void Close();
    }




    public abstract class BaseResponse : IResponseHandler
    {
        public abstract OperationCode OpCode { get;  }

        public virtual void Init()
        {
            
        }

        public virtual void Close()
        {

        }

        public abstract void OnOperationRequest(KcpSession session, string msg);

        protected void Send(KcpSession session, string _data = null, ErrCode errCode = ErrCode.Succeed)
        {
            var tmp = S2C_GameMsg.Get();
            tmp.type = (uint)OpCode;
            if (_data != null) {
                tmp.data = _data;
            }
            tmp.ErrorCode = errCode;
            session.Send(tmp);
        }

        protected void Send(KcpSession session, string _data, int errCode)
        {
            Send(session, _data, (ErrCode)errCode);
        }


        protected void SendEvent(KcpSession session, ErrCode OpCode, string _data = null)
        {
            var tmp = S2C_GameEvent.Get();
            tmp.type = (uint)OpCode;
            if (_data != null) {
                tmp.data = _data;
            }
            session.Send(tmp);
        }
    }
   

}
