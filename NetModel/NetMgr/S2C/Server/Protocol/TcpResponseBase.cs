﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModel.NetMgr.S2C.Server.Protocol
{
    public class TcpResponseBase : PackageResponse
    {
        public string tcpId;

        public virtual void Serialize()
        {
            ;
        }
    }
}
