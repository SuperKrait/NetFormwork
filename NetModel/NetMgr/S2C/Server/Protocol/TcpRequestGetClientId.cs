using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModel.NetMgr.S2C.Server.Protocol
{
    public class TcpRequestGetClientId : TcpRequestBase
    {
        public bool ConnectedSucceed
        {
            get;
            set;
        }

        public override void Serialize()
        {
            WriteBoolean(ConnectedSucceed);
        }
    }
}
