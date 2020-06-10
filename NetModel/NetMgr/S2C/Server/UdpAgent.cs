using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common.Utils;

namespace NetModel.NetMgr.S2C.Server
{
    class UdpAgent
    {
        /*初始化*/        
        
        public void Init(IPEndPoint ip)
        {
            requestQueue = new List<PackageRequest>();
            reponseQueue = new List<PackageReceponse>();
            udpServer = new UdpClient(ip);
        }

        public int flagHeader = 0xff8a;


        /*创建发送消息对列*/
        private long reqId = 0;
        public List<PackageRequest> requestQueue;
        public void AddReqPackage(PackageRequest req)
        {
            SetReqHeader(req);
            lock (requestQueue)
            {
                requestQueue.Add(req);
            }
        }



        private void SetReqHeader(PackageRequest req)
        {
            if (!req.Id.Equals(0xffffffff))
                return;
            req.Id = Interlocked.Add(ref reqId, 1);
            req.SetIndex(0);
            req.WriteInt32(flagHeader);
            req.WriteInt64(req.Id);
            req.SetIndex(20);//跳过包长度
            req.WriteInt32(req.ClientId);
            req.WriteInt32(req.ProtocolId);
            req.TimeTick = System.DateTime.Now.Ticks;
            req.WriteInt64(req.TimeTick);
            string md5 = MD5Code.GetMD5HashFromByte(req.ProtocolData);
            byte[] checkData = Encoding.UTF8.GetBytes(md5);
            req.SetMd5Header(checkData);
            req.SetIndex(12);
            req.Count += 8;//加上自己
            req.WriteInt64(req.Count);
        }

        /*创建接收消息对列*/
        public List<PackageReceponse> reponseQueue;
        public void AddReqPackage(PackageReceponse req)
        {
            lock (reponseQueue)
            {
                reponseQueue.Add(req);
            }
        }


        /*创建一个Udp监听端*/
        private UdpClient udpServer;

        /*关闭Udp监听*/
        /*发送消息给指定ip端口*/
        /*接收消息*/
        /*屏蔽垃圾数据*/
        /*销毁数据*/
    }
}
