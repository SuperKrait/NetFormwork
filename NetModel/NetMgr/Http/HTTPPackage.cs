using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModel.NetMgr.Http
{
    public class HTTPPackage
    {
        private HTTPPackageLinkNode head;
        private HTTPPackageLinkNode now;

        private int bufferSize = 1024;
        private long bufferCount = 0;

        public HTTPPackage(int bufferSize)
        {
            this.bufferSize = bufferSize;
            head = new HTTPPackageLinkNode(bufferSize);
            now = head;
        }

        public void AddBuffer(byte[] data, long length)
        {
            bufferCount += length;
            now = now.AddBuffer(data, 0, length);
        }

        public long GetPackageCount()
        {
            return bufferCount;
        }

        public byte[] GetData()
        {
            if (bufferCount.Equals(0))
            {
                return null;
            }

            byte[] data = new byte[bufferCount];
            long index = 0;
            head.GetData(data, ref index);
            HTTPPackageLinkNode next = head.nextNode;
            while (next != null)
            {
                next.GetData(data, ref index);
                next = next.nextNode;
            }
            return data;
        }

        public void Clear()
        {
            head.Clear();
            HTTPPackageLinkNode next = head.nextNode;
            while (next != null)
            {
                next.nextNode = null;
                next.Clear();
                next = next.nextNode;
            }
        }
    }
}
