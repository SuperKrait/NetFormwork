using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NetModel.NetMgr.S2C.Server
{
    public class PackageRequest : PackageBase, IPackageRequest
    {
        private MemoryStream stream;
        private BinaryWriter writer;
        public long Count
        {
            get;
            set;
        } = 0;

        public virtual IPackageRequest Initialization(Type t, int clientId)
        {
            this.ClientId = clientId;
            this.ProtocolData = new byte[System.Runtime.InteropServices.Marshal.SizeOf(t) + HeaderCount];
            stream = new MemoryStream(this.ProtocolData);
            SetIndex(HeaderCount);
            //this.Count = HeaderCount;
            writer = new BinaryWriter(stream);
            return this;
        }

        public void WriteInt32(int _value)
        {
            writer.Write(_value);
            Count += 4;
        }

        public void WriteInt64(long _value)
        {
            writer.Write(_value);
            Count += 8;
        }

        public void WriteString(string _value)
        {
            byte[] data = Encoding.UTF8.GetBytes(_value);
            WriteBytes(data);
        }

        public void WriteBytes(byte[] data)
        {
            writer.Write(data.Length);
            Count += 4;
            writer.Write(data);
            Count += data.Length;
        }

        public void WriteSbyte(sbyte _value)
        {
            writer.Write(_value);
            Count++;
        }
        public void WriteByte(byte _value)
        {
            writer.Write(_value);
            Count++;
        }
        public void WriteBoolean(bool _value)
        {
            byte tmp = 0;
            if (_value)
                tmp = 1;  
            writer.Write(tmp);
            Count++;
        }

        public void SetIndex(int index)
        {
            writer.Seek(index, SeekOrigin.Begin);
        }

        public void SetMd5Header(byte[] md5)
        {
            writer.Write(md5);
            Count += md5.Length;
        }

        public override void Dispose()
        {
            base.Dispose();
            //writer.Flush();
            writer.Close();
            stream.Close();
        }

        public virtual void Serialize()
        {
            ;
        }

        ~PackageRequest()
        {
            if (isDestory)
            {
                Dispose();
            }
        }
    }
}
