using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NetModel.NetMgr.S2C.Server
{
    public class PackageResponse : PackageBase
    {
        private MemoryStream stream;
        private BinaryReader reader;
        public long curIndex
        {
            get;
            set;
        } = 0;
        public void Init(byte[] data)
        {
            this.ProtocolData = new byte[data.Length - HeaderCount];
            Array.Copy(data, HeaderCount, this.ProtocolData, 0, this.ProtocolData.Length);
            stream = new MemoryStream(this.ProtocolData);
            reader = new BinaryReader(stream);
        }

        public int ReadInt32()
        {
            int _value = reader.ReadInt32();
            curIndex += 4;
            return _value;
        }

        public long ReadInt64()
        {
            long _value = reader.ReadInt64();
            curIndex += 8;
            return _value;
        }

        public string ReadString()
        {
            int count = ReadInt32();
            curIndex += 4;
            string str = Encoding.UTF8.GetString(ReadBytes(count));
            return str;
        }

        

        public sbyte ReadSbyte()
        {
            sbyte _value = reader.ReadSByte();
            curIndex++;
            return _value;
        }
        public byte ReadByte()
        {
            byte _value = reader.ReadByte();
            curIndex++;
            return _value;
        }
        public bool ReadBoolean()
        {
            byte tmp = reader.ReadByte();
            curIndex++;
            return tmp.Equals(1) ? true : false;
        }

        public byte[] ReadBytes(int count)
        {
            byte[] data = reader.ReadBytes(count);
            curIndex += data.Length;
            return data;
        }

        public override void Dispose()
        {
            base.Dispose();
            //Readr.Flush();
            reader.Close();
            stream.Close();
        }

        ~PackageResponse()
        {
            if (isDestory)
            {
                Dispose();
            }
        }
    }
}
