﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModel.NetMgr.S2C.Server.Protocol
{
    public abstract class TcpRequestBase : PackageRequest
    {
        public override abstract void Serialize();
    }
}
