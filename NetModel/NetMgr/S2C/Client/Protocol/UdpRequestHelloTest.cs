using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModel.NetMgr.S2C.Client.Protocol
{
    public class UdpRequestHelloTest : UdpRequestBase
    {
        public string Hello
        {
            get;
            set;
        } = "Hello World!\r\n";

        public override void Serialize()
        {
            WriteString(Hello);
        }
    }
}
