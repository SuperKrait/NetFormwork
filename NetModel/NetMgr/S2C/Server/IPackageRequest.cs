using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModel.NetMgr.S2C.Server
{
    public interface IPackageRequest
    {
        IPackageRequest Initialization(int clientId);
        void Serialize();
    }
}
