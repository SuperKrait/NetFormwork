﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common.Utils;
using Common.TaskPool;
using Common.Log;
using NetModel.NetMgr.S2C.Client.Protocol;

namespace NetModel.NetMgr.S2C.Client
{
    class UdpAgent
    {
        #region 初始化
        /*初始化*/

        public bool Init(int serverPort)
        {
            if (!status.Equals(0))
            {
                return false;
            }
            requestQueue = new List<UdpRequestBase>();
            responseQueue = new List<UdpResponseBase>();
            boardIpEnd = new IPEndPoint(IPAddress.Broadcast, serverPort);
            PauseClient();
            CreateUdpRequestClients();
            return true;
            //clientPort = new IPEndPoint(IPAddress.Broadcast, port);
        }
        #endregion

        #region Udp控制以及服务器
        /*Udp控制端端相关*/
        /// <summary>
        /// 0：关闭
        /// 1：开启
        /// 2：服务器挂起
        /// -2：关闭中
        /// </summary>
        private int status = 0;
        private object mainLock = new object();
        private IPEndPoint boardIpEnd;
        //private Func<int, IPEndPoint> /*getIpHandler*/ = null;

        public int flagHeader = 0xff8a;

        //private IPEndPoint clientPort;


        public void StartClient()
        {
            status = 1;
        }

        public void PauseClient()
        {
            status = 2;
        }

        private void StopClient()
        {
            status = -2;
        }

        private void SetDefaultClient()
        {
            status = 0;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void DestoryUdpServer()
        {
            lock (mainLock)
            {
                StopClient();
                if (udp2AnyRequest != null)
                    udp2AnyRequest.Close();
                if (udp2ClientResponse != null)
                    udp2ClientResponse.Close();
            }
            lock (requestQueue)
            {
                DestorySendQueue(requestQueue);
            }
            lock (responseQueue)
            {
                DestroyReponseQueue(responseQueue);
            }
            SetDefaultClient();
        }

        
        #endregion

        #region 发送消息模块
        /*发送消息*/
        private UdpClient udp2AnyRequest;

        private void CreateUdpRequestClients()
        {
            lock (mainLock)
            {
                udp2AnyRequest = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            }
            TaskPoolHelper.Instance.StartALongTask(SendMessage, "UdpSendMessage");
            TaskPoolHelper.Instance.StartALongTask(ReceiveMessage, "UdpReceiveMessage");
        }

        private void SendMessage()
        {
            List<UdpRequestBase> list = new List<UdpRequestBase>();
            while (true)
            {
                Thread.Sleep(1);

                switch (status)
                {
                    case 1:
                        break;
                    case 2:
                        continue;
                    default:
                        goto StopServer;
                }

                List<UdpRequestBase> tmplist = GetSendData();
                if (tmplist == null)
                {
                    continue;
                }

                list.AddRange(tmplist);

                lock (mainLock)
                {
                    switch (status)
                    {
                        case 1:
                            break;
                        case 2:
                            DestorySendQueue(list);
                            continue;
                        default:
                            DestorySendQueue(list);
                            goto StopServer;
                    }
                    try
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            UdpRequestBase req = list[i];

                            udp2AnyRequest.Send(req.getMemData(), (int)req.Count, boardIpEnd);//udpBug,后期修复                        
                        }
                    }
                    catch (Exception e)
                    {
                        LogAgent.LogError("Udp消息发送失败\t" + e.ToString());
                    }
                    DestorySendQueue(list);
                }
            }

        StopServer:
            Interlocked.Add(ref status, 1);
            LogAgent.Log("!关闭Udp循环线程");

        }
        #endregion

        #region 接收消息模块
        /*接收消息*/
        private UdpClient udp2ClientResponse;
        IPEndPoint receiveClientIpend = new IPEndPoint(IPAddress.Any, 0);
        private void ReceiveMessage()
        {
            while (true)
            {
                Thread.Sleep(1);
                try
                {
                    UdpResponseBase reponse = null;
                    lock (mainLock)
                    {
                        switch (status)
                        {
                            case 1:
                                break;
                            case 2:
                                continue;
                            default:
                                goto StopServer;
                        }

                        byte[] data = udp2ClientResponse.Receive(ref receiveClientIpend);//默认每次只接收一个包
                        data = CheckPack(data);
                        if (data == null)
                        {
                            continue;
                        }

                        reponse = GetReponsePackage(data);
                    }
                    AddReqPackage(reponse);
                }
                catch (Exception e)
                {
                    LogAgent.LogError("Udp消息接收失败\t" + e.ToString());
                }

            }


        StopServer:
            Interlocked.Add(ref status, 1);
            LogAgent.Log("!关闭Udp循环线程");
        }

        private byte[] CheckPack(byte[] data)
        {
            if (data.Length < 4 || !BitConverter.ToInt32(data, 0).Equals(flagHeader))
            {
                return null;
            }
            byte[] countArr = new byte[8];
            byte[] md5Arr = new byte[32];

            for (int i = 4; i < 12; i++)
            {
                countArr[i - 4] = data[i];
                data[i] = 0;
            }

            for (int i = 36; i < 68; i++)
            {
                md5Arr[i - 36] = data[i];
                data[i] = 0;
            }

            byte[] finalData = new byte[BitConverter.ToInt32(countArr, 0)];         

            Array.Copy(data, finalData, finalData.Length);

            string md5Check = MD5Code.GetMD5HashFromByte(data);
            string md5Req = Encoding.UTF8.GetString(md5Arr);

            if (!string.Compare(md5Check, md5Req).Equals(0))
            {
                return null;
            }

            return finalData;
        }

        private UdpResponseBase GetReponsePackage(byte[] data)
        {
            UdpResponseBase package = new UdpResponseBase();
            package.Id = BitConverter.ToInt64(data, 4);
            package.ClientId = BitConverter.ToInt32(data, 20);
            package.ProtocolId = BitConverter.ToInt32(data, 24);
            package.TimeTick = BitConverter.ToInt64(data, 28);
            package.ipEnd = receiveClientIpend;
            package.Init(data);
            return package;
        }

        #endregion

        #region 发送消息对列
        /*创建发送消息对列*/
        private long reqId = 0;

        public List<UdpRequestBase> requestQueue;
        public void AddReqPackage(UdpRequestBase req)
        {
            SetReqHeader(req);
            lock (requestQueue)
            {
                requestQueue.Add(req);
            }
        }

        private void SetReqHeader(UdpRequestBase req)
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
            string md5 = MD5Code.GetMD5HashFromByte(req.getMemData());
            byte[] checkData = Encoding.UTF8.GetBytes(md5);
            req.SetIndex(36);
            req.SetMd5Header(checkData);
            req.SetIndex(12);
            req.Count += 8;//加上自己
            req.WriteInt64(req.Count);
        }

        private List<UdpRequestBase> GetSendData()
        {
            try
            {
                lock (requestQueue)
                {
                    if (requestQueue.Count > 0)
                    {
                        List<UdpRequestBase> list = new List<UdpRequestBase>();
                        list.AddRange(requestQueue);

                        //Udp数据包没有必要保留，发送了就删除
                        requestQueue.Clear();

                        return list;
                    }
                }
            }
            catch(NullReferenceException e)
            {
                LogAgent.LogWarning ("udp发送对列已经销毁" + e.ToString() + "\r\n");
            }
            return null;
        }

        private void DestorySendQueue(List<UdpRequestBase> list)
        {
            if (list == null)
                return;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                {
                    list[i].Dispose();
                }
            }
            list.Clear();
        }
        #endregion

        #region 接收消息对列
        /*创建接收消息对列*/
        public List<UdpResponseBase> responseQueue;
        public void AddReqPackage(UdpResponseBase req)
        {
            lock (responseQueue)
            {
                responseQueue.Add(req);
            }
        }

        public List<UdpResponseBase> GetAllReponse()
        {
            List<UdpResponseBase> list = new List<UdpResponseBase>();

            lock (responseQueue)
            {
                if (responseQueue.Count.Equals(0))
                    return null;
                list.AddRange(responseQueue);
                responseQueue.Clear();
            }
            list.Distinct<UdpResponseBase>();
            list.Sort();
            return list;
        }

        private void DestroyReponseQueue(List<UdpResponseBase> list)
        {
            if (list == null)
                return;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                {
                    list[i].Dispose();
                }
            }
            list.Clear();
        }

        #endregion


    }
}
