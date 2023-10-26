using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LGF.Net.KcpServer;

namespace LGF.Server.Hotfix
{
    internal class GameResponseModule : ModuleBase
    {
        public Dictionary<OperationCode, IResponseHandler> HandlerDict = new Dictionary<OperationCode, IResponseHandler>();

        protected override void OnInit()
        {
            base.OnInit();

            RegisterServerMsg<C2S_GameMsg>(OnResponse);

            InitResponse();
        }


        void InitResponse()
        {
            var baseHandler = typeof(BaseResponse);
            foreach (var type in baseHandler.Assembly.GetTypes()) {
                if (baseHandler.IsAssignableFrom(type) && !type.IsAbstract) {
                    var handle = (IResponseHandler)Activator.CreateInstance(type);
                    HandlerDict.Add(handle.OpCode, handle);
                    handle.Init();
                    LGF.Log.sLog.Debug($"handle.OpCode {handle.OpCode}");
                }
            }
        }

        void OnResponse(KcpSession session, C2S_GameMsg msg)
        {
            //msg
            //LGF.Log.sLog.Debug($" OnResponse : <{(OperationCode)msg.type}>");
            if (HandlerDict.TryGetValue((OperationCode)msg.type, out var val)) {
                val.OnOperationRequest(session, msg.data);
            }
            else {
                LGF.Log.sLog.Debug($" 未注册 : <{(OperationCode)msg.type}>");
            }

        }

        public override void Close()
        {
            base.Close();
            foreach (var item in HandlerDict) {
                item.Value.Close();
            }
        }


    }
}
