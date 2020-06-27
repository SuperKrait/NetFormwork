using System;
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
using NetModel.NetMgr.S2C.Server.Protocol;

namespace NetModel.NetMgr.S2C.Server
{
    class TcpAgent
    {

        public bool Init(IPEndPoint LocalIpEnd)
        {
            if (!status.Equals(0))
            {
                return false;
            }
            ProtocolMgr.RegisterProtocol(ProtocolMgr.Data_SetTcpId, AddNewClient2Dic);
            CreateTcpListener(LocalIpEnd);
            //this.getIpHandler = getIpHandler;
            this.tmpClientDic = new Dictionary<string, ClientWithTcp>();
            this.clientDic = new Dictionary<int, string>();
            PauseClient();
            return true;
            //clientPort = new IPEndPoint(IPAddress.Broadcast, port);
        }


        #region Tcp控制以及服务器
        /*Tcp控制端端相关*/
        /// <summary>
        /// 0：关闭
        /// 1：开启
        /// 2：服务器挂起
        /// -2：关闭中
        /// </summary>
        private int status = 0;
        private object mainLock = new object();

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
            status = -3;
        }

        private void SetDefaultClient()
        {
            status = 0;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void DestoryTcpServer()
        {
            StopClient();
            lock (tmpClientDic)
            {
                foreach (var pairs in tmpClientDic)
                {
                    pairs.Value.DestoryTcpServer();
                }
                tmpClientDic.Clear();
                tmpClientDic = null;
            }

            lock (clientDic)
            {
                clientDic.Clear();
                clientDic = null;
            }

            ProtocolMgr.UnRegisterProtocol(ProtocolMgr.Data_SetTcpId, AddNewClient2Dic);

            SetDefaultClient();
        }


        #endregion

        private TcpListener tcpListener;
        private Dictionary<string, ClientWithTcp> tmpClientDic;
        private Dictionary<int, string> clientDic;


        private void CreateTcpListener(IPEndPoint ipEnd)
        {
            tcpListener = new TcpListener(ipEnd);
            TaskPoolHelper.Instance.StartALongTask(AcceptTcpClient, "TcpAgent->AcceptTcpClient");
        }

        private void AcceptTcpClient()
        {
            while (true)
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
                if (tcpListener.Pending())
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    ClientWithTcp client = new ClientWithTcp();
                    string tcpId = ToolsMgr.GetUUID();
                    client.tcpId = tcpId;
                    if (client.Init(tcpClient))
                    {
                        client.StartClient();
                        client.timeTick = System.DateTime.Now.Ticks;
                        lock(tmpClientDic)
                        {
                            tmpClientDic.Add(tcpId, client);
                        }
                        TcpRequestGetClientId pack = ProtocolMgr.GetPackageRequest<TcpRequestGetClientId>(-2);
                        client.AddReqPackage(pack);
                    }
                    else
                    {
                        LogAgent.LogError("tcpClient客户端初始化失败 + \t" + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString() + "\r\n");
                        client.DestoryTcpServer();
                        tcpClient.Close();
                    }
                }
            }
            StopServer:
            Interlocked.Add(ref status, 1);
            LogAgent.Log("!关闭Udp循环线程");
        }

        /// <summary>
        /// 为客户端id添加Tcp索引列表
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="key"></param>
        public void AddNewClient2Dic(PackageResponse tcpPack)
        {
            TcpResponseBase pack = (TcpResponseBase)tcpPack;

            int clientId = pack.ClientId;
            string key = pack.tcpId;

            string clientKey = null;
            bool isInclude = false;
            lock (clientDic)
            {
                if (clientDic.TryGetValue(clientId, out clientKey))
                {
                    if (clientKey.Equals(key))
                    {
                        return;
                    }
                    isInclude = true;
                    
                }
            }


            if (isInclude)
                lock (tmpClientDic)
                {
                    if (!(tmpClientDic[key].timeTick < tmpClientDic[clientKey].timeTick))
                    {
                        return;
                    }
                    else
                    {
                        tmpClientDic[clientKey].DestoryTcpServer();
                    }
                }


            lock (clientDic)
            {
                clientDic.Remove(clientId);
                clientDic.Add(clientId, clientKey);
            }
        }

        public void AddMessage(TcpRequestBase req)
        {
            string tcpId;
            lock (clientDic)
            {
                if (!clientDic.TryGetValue(req.ClientId, out tcpId))
                {
                    LogAgent.LogWarning("tcp客户端不存在1 clientId = " + req.ClientId + "\r\n");
                    return;
                }
            }
            ClientWithTcp client;
            lock (tmpClientDic)
            {
                if (!tmpClientDic.TryGetValue(tcpId, out client))
                {
                    LogAgent.LogWarning("tcp客户端不存在2 clientId = " + req.ClientId + "\r\n");
                    return;
                }
            }
            client.AddReqPackage(req);
        }

        public Dictionary<string, List<TcpResponseBase>> GetAllReponse()
        {
            Dictionary<string, List<TcpResponseBase>> tmpDic = new Dictionary<string, List<TcpResponseBase>>();
            List<ClientWithTcp> list = new List<ClientWithTcp>();
            lock (tmpClientDic)
            {
                list.AddRange(tmpClientDic.Values);                
            }
            if (list.Count.Equals(0))
            {
                return null;
            }

            int count = 0;
            for (int i = 0; i < list.Count; i++)
            {
                List<TcpResponseBase> tmp = list[i].GetAllReponse();
                if (tmp != null)
                {
                    tmp.Distinct();
                    tmp.Sort();
                    tmpDic.Add(list[i].tcpId, tmp);
                    count++;
                }
            }

            if (count.Equals(0))
                return null;
            
            return tmpDic;
        }

        class ClientWithTcp
        {
            #region 初始化
            /*初始化*/
            public TcpClient client;
            public NetworkStream stream;
            public IPEndPoint ipEnd;
            public string tcpId;
            public long timeTick;
            public int ClientId
            {
                get;
                set;
            }

            public bool Init(TcpClient client)
            {
                if (!this.status.Equals(0))
                {
                    return false;
                }
                this.requestQueue = new List<TcpRequestBase>();
                this.responseQueue = new List<TcpResponseBase>();
                this.client = client;
                this.stream = client.GetStream();
                this.ipEnd = (IPEndPoint)client.Client.RemoteEndPoint;
                PauseClient();
                CreateTcpRequestClients();
                return true;
                //clientPort = new IPEndPoint(IPAddress.Broadcast, port);
            }


            #endregion

            #region Tcp控制以及服务器
            /*Udp控制端端相关*/
            /// <summary>
            /// 0：关闭
            /// 1：开启
            /// 2：服务器挂起
            /// -1：关闭中
            /// </summary>
            private int status = 0;
            private object mainLock = new object();


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
                status = -1;
            }

            private void SetDefaultClient()
            {
                status = 0;
            }

            /// <summary>
            /// 销毁
            /// </summary>
            public void DestoryTcpServer()
            {
                lock (mainLock)
                {
                    StopClient();
                    if (stream != null)
                        stream.Close();
                    if (client != null)
                        client.Close();
                }
                lock (requestQueue)
                {
                    DestorySendQueue(requestQueue);
                }
                lock (responseQueue)
                {
                    DestroyReponseQueue(responseQueue);
                }
            }


            #endregion

            #region 消息模块

            private void CreateTcpRequestClients()
            {
                TaskPoolHelper.Instance.StartNewTask(TcpMessage, "TcpMessage");
            }

            private void TcpMessage()
            {
                byte[] buffer = new byte[0x800];
                List<TcpRequestBase> list = new List<TcpRequestBase>();
                TcpResponseBase reponse = null;

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

                    List<TcpRequestBase> tmplist = GetSendData();
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
                        /*发送消息*/
                        for (int i = 0; i < list.Count; i++)
                        {
                            try
                            {
                                TcpRequestBase req = list[i];
                                if (this.client.Connected && this.stream.CanWrite)
                                    stream.Write(req.getMemData(), 0, (int)req.Count);//udpBug,后期修复 
                            }
                            catch (Exception e)
                            {
                                LogAgent.LogError("Tcp消息发送失败\t" + e.ToString());
                            }                       
                        }
                        DestorySendQueue(list);

                        try
                        {
                            /*接收消息*/
                            if (this.client.Connected && this.client.Available > 0 && this.stream.CanRead)
                            {
                                int count = stream.Read(buffer, 0, buffer.Length);//默认每次只接收一个包
                                byte[] data = CheckPack(buffer, count);
                                if (data == null)
                                {
                                    continue;
                                }
                                reponse = GetReponsePackage(data);
                            }
                        }
                        catch (Exception e)
                        {
                            LogAgent.LogError("Tcp消息接收失败\t" + e.ToString());
                        }
                    }
                    AddReqPackage(reponse);
                }

                StopServer:
                Interlocked.Add(ref status, 1);
                LogAgent.Log("!关闭Udp循环线程");

            }            
            
            private byte[] CheckPack(byte[] data, int count)
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

                Array.Copy(data, finalData, count);

                string md5Check = MD5Code.GetMD5HashFromByte(data);
                string md5Req = Encoding.UTF8.GetString(md5Arr);

                if (!string.Compare(md5Check, md5Req).Equals(0))
                {
                    return null;
                }

                return finalData;
            }

            private TcpResponseBase GetReponsePackage(byte[] data)
            {
                TcpResponseBase package = new TcpResponseBase();
                package.Id = BitConverter.ToInt64(data, 4);
                package.ClientId = BitConverter.ToInt32(data, 20);
                package.ProtocolId = BitConverter.ToInt32(data, 24);
                package.TimeTick = BitConverter.ToInt64(data, 28);
                package.ipEnd = ipEnd;
                package.Init(data);
                package.tcpId = this.tcpId;
                return package;
            }

            #endregion

            #region 发送消息对列
            /*创建发送消息对列*/
            private long reqId = 0;

            public List<TcpRequestBase> requestQueue;
            public void AddReqPackage(TcpRequestBase req)
            {
                SetReqHeader(req);
                lock (requestQueue)
                {
                    requestQueue.Add(req);
                }
            }

            private void SetReqHeader(TcpRequestBase req)
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

            private List<TcpRequestBase> GetSendData()
            {
                try
                {
                    lock (requestQueue)
                    {
                        if (requestQueue.Count > 0)
                        {
                            List<TcpRequestBase> list = new List<TcpRequestBase>();
                            list.AddRange(requestQueue);

                            //Udp数据包没有必要保留，发送了就删除
                            requestQueue.Clear();

                            return list;
                        }
                    }
                }
                catch (NullReferenceException e)
                {
                    LogAgent.LogWarning("tcp发送对列已经销毁" + e.ToString() + "\r\n");
                }
                return null;
            }

            private void DestorySendQueue(List<TcpRequestBase> list)
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
            public List<TcpResponseBase> responseQueue;
            public void AddReqPackage(TcpResponseBase req)
            {
                lock (responseQueue)
                {
                    responseQueue.Add(req);
                }
            }

            public List<TcpResponseBase> GetAllReponse()
            {
                List<TcpResponseBase> list = new List<TcpResponseBase>();

                lock (responseQueue)
                {
                    if (responseQueue.Count.Equals(0))
                        return null;
                    list.AddRange(responseQueue);
                    responseQueue.Clear();
                }

                list.Distinct<TcpResponseBase>();
                list.Sort();
                return list;
            }

            private void DestroyReponseQueue(List<TcpResponseBase> list)
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
}
