using NetModel.NetMgr.S2C.Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.TaskPool;

namespace NetModel.NetMgr.S2C.Server
{
    class ServerMgr
    {
        #region Udp相关
        /*创建Udp监听代理*/
        private UdpAgent udpAgent;
        private bool isUdpStart = true;

        private bool InitUdpServer()
        {
            udpAgent = new UdpAgent();
            if (!udpAgent.Init(GetClientIpById))
            {
                udpAgent.DestoryUdpServer();
                return false;
            }
            else
            {
                TaskPoolHelper.Instance.StartALongTask(GetUdpMessage, "ServerMgr->GetUdpMessage");
                return true;
            }
        }

        /*控制UDP服务器*/

        private void StartUdpClient()
        {
            udpAgent.StartClient();
        }

        private void PauseUdpClient()
        {
            udpAgent.PauseClient();
        }

        private void DestoryUdpClient()
        {
            udpAgent.DestoryUdpServer();
        }

        /*接收到客户端消息*/

        private void GetUdpMessage()
        {
            while (isUdpStart)
            {
                Thread.Sleep(1);
                List<UdpReponseBase> list = udpAgent.GetAllReponse();
                if (list == null)
                    continue;
                for (int i = 0; i < list.Count; i++)
                {
                    AddClient(list[i].ClientId, list[i].ipEnd);
                    AddMessage(list[i]);
                }
            }
            
        }


        /*发送消息到指定ip端口*/

        public void SendUdpMessage(UdpRequestBase request)
        {
            if (!isUdpStart)
            {
                return;
            }
            udpAgent.AddReqPackage(request);
        }

        #endregion

        #region Tcp相关
        /*创建Tcp监听代理*/
        private TcpAgent tcpAgent;
        private bool isTcpStart = true;
        private int tcpListenerPort = 0xff61;


        private bool InitTcpServer()
        {
            this.tcpAgent = new TcpAgent();
            if (this.tcpAgent.Init(GetLocalTcpServerIp()))
            {
                TaskPoolHelper.Instance.StartALongTask(GetTcpMessage, "ServerMgr->GetTcpMessage");
                return true;
            }
            else
            {
                this.tcpAgent.DestoryTcpServer();
                return false;
            }
        }
        /// <summary>
        /// 获取当前Ip地址
        /// </summary>
        /// <returns></returns>
        private IPEndPoint GetLocalTcpServerIp()
        {
            IPAddress[] addrs = GetLocalIp();
            IPEndPoint point = null;
            for (int i = 0; i < addrs.Length; i++)
            {
                if (addrs[i].ToString().IndexOf("192").Equals(0))
                {
                    point = new IPEndPoint(addrs[i], this.tcpListenerPort);
                    break;
                }
            }
            return point;
        }

        /*控制TCP服务器*/

        private void StartTcpClient()
        {
            tcpAgent.StartClient();
        }

        private void PauseTcpClient()
        {
            tcpAgent.PauseClient();
        }

        private void DestoryTcpClient()
        {
            tcpAgent.DestoryTcpServer();
        }

        /*接收到客户端消息*/

        private void GetTcpMessage()
        {
            while (isTcpStart)
            {
                Thread.Sleep(1);
                Dictionary<string, List<TcpResponseBase>> dic = tcpAgent.GetAllReponse();
                if (dic == null)
                    continue;
                foreach (var pairs in dic)
                {
                    List<TcpResponseBase> tmpList = pairs.Value;
                    for (int i = 0; i < tmpList.Count; i++)
                        AddClient(tmpList[i].ClientId, tmpList[i].ipEnd);
                }
            }

        }


        /*发送消息到指定ip端口*/

        public void SendTcpMessage(TcpRequestBase request)
        {
            if (!isTcpStart)
            {
                return;
            }
            tcpAgent.AddMessage(request);
        }

        #endregion

        #region 服务器相关

        /*管理客户端*/
        private Dictionary<int, IPEndPoint> clientDic;
        private void InitClientMgr()
        {
            clientDic = new Dictionary<int, IPEndPoint>();
            clientDic.Add(-1, new IPEndPoint(IPAddress.Broadcast, tcpListenerPort));
        }

        private IPEndPoint GetClientIpById(int id)
        {
            lock (clientDic)
            {
                IPEndPoint ipend = null;
                clientDic.TryGetValue(id, out ipend);
                return ipend;
            }
        }

        private void AddClient(int id, IPEndPoint ipend)
        {
            lock (clientDic)
            {
                if (!clientDic.ContainsKey(id))
                {
                    clientDic.Add(id, ipend);
                }
                else
                {
                    clientDic[id] = ipend;
                }
            }
        }

        private void RemoveClient(int id)
        {
            lock (clientDic)
            {
                if (!clientDic.ContainsKey(id))
                {
                    clientDic.Remove(id);
                }
            }
        }

        private void ClearAll()
        {
            lock(clientDic)
                clientDic.Clear();
        }

        /*获取本机可用Ip*/
        private IPAddress[] GetLocalIp()
        {
            IPAddress[] ipAddrs = Dns.GetHostAddresses("localhost");
            return ipAddrs;
        }

        /*改变服务器状态*/


        private int status = 0;

        public void SetServerStatus(int status)
        {
            this.status = status;
        }


        /*接收数据*/

        private ProtocolMgr protocolMgr;

        public void AddMessage(PackageResponse pack)
        {
            protocolMgr.AddRespPackage2Arr(pack);
        }


        /*服务器通用*/


        public void Update()
        {
            protocolMgr.Update();
        }

        #endregion

    }
}
