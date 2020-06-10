using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModel.NetMgr.S2C.Server
{
    class PackageBase : IDisposable
    {
        public long Id
        {
            get;
            set;
        } = 0xffffffff;

        public int ClientId
        {
            get;
            set;
        }

        public int ProtocolId
        {
            get;
            set;
        }        

        public byte[] ProtocolData
        {
            get;
            set;
        }

        public long TimeTick
        {
            get;
            set;
        }

        /*
         * 标识位|包id|包括包头在内的包大小|客户端id|协议id|时间戳|MD5密文
         *   4   |  8 |         8          |    4   |   4  |   8  |  32 
         */
        public int HeaderCount
        {
            get;
            set;
        } = 68;

        protected bool isDestory = false;

        public virtual void Dispose()
        {
            Id = 0;
            ClientId = 0;
            ProtocolId = 0;
            ProtocolData = null;
            isDestory = true;
        }

        ~PackageBase()
        {
            if (!isDestory)
            {
                Dispose();
            }
        }
    }
}
