using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetModel.NetMgr.S2C.Server
{
    public class PackageBase : IDisposable, IComparable<PackageBase>, IEqualityComparer<PackageBase>
    {
        public long Id
        {
            get;
            set;
        } = 0xffffffff;

        /// <summary>
        /// 客户端id，
        /// 当Id为-1的时候向全网广播
        /// 当Id位-2的时候为服务器消息
        /// </summary>
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

        public IPEndPoint ipEnd
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

        public int CompareTo(PackageBase other)
        {
            if (other.Id < this.Id)
                return -1;
            else
                return 1;
        }

        public bool Equals(PackageBase x, PackageBase y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(PackageBase obj)
        {
            return obj.Id.ToString().GetHashCode();
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
