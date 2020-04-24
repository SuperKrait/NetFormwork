using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModel.NetMgr.Http
{
    internal class HTTPPackageLinkNode
    {
        public HTTPPackageLinkNode nextNode = null;
        private byte[] buffer;
        private long index = 0;

        public HTTPPackageLinkNode(int bufferSize)
        {
            buffer = new byte[bufferSize];
        }

        public HTTPPackageLinkNode AddBuffer(byte[] data, long sourcesIndex, long length)
        {
            long totolLength = index + length;
            if (totolLength <= buffer.Length)
            {
                Array.Copy(data, sourcesIndex, buffer, index, length);
                index += length;
                return this;
            }
            else
            {
                long tmpLength = buffer.Length - index;
                Array.Copy(data, sourcesIndex, buffer, index, tmpLength);
                index += tmpLength;
                nextNode = CreateNewNode();
                return nextNode.AddBuffer(data, sourcesIndex + tmpLength, length - tmpLength);
            }            
        }

        private HTTPPackageLinkNode CreateNewNode()
        {
            return new HTTPPackageLinkNode(buffer.Length);
        }

        public void GetData(byte[] data, ref long curIndex)
        {
            long curLength = curIndex + index;
            if (data.Length < curLength)
            {
                throw new IndexOutOfRangeException("Memery out of range from httpNetwork, Giveing size smaller than real data");
            }
            Array.Copy(buffer, 0, data, curIndex, index);
            curIndex += index;
        }

        public void Clear()
        {
            buffer = null;
            index = 0;
        }
    }
}
