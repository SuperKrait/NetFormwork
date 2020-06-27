using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using NetModel.NetMgr.Http;
using NetModel.NetMgr.S2C.Server;
using Common.TaskPool;

namespace TestDebugModel
{
    class Program
    {
        public int Main(string[] arges)
        {
            
            TaskPoolHelper.Instance.StartALongTask();

            return 0;
        }
    }
}
