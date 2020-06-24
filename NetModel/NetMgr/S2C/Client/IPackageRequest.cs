using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModel.NetMgr.S2C.Client
{
    public interface IPackageRequest
    {
        IPackageRequest Initialization(Type t, int clientId);
        void Serialize();
    }
}
