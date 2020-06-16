using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            return udpAgent.Init(GetClientIpById);            
        }

        /*控制UDP服务器*/

        public void StartClient()
        {
            udpAgent.StartClient();
        }

        public void PauseClient()
        {
            udpAgent.PauseClient();
        }

        private void DestoryClient()
        {
            udpAgent.DestoryUdpServer();
        }

        /*接收到客户端消息*/

        private void GetUdpMessage()
        {
            while (isUdpStart)
            {
                Thread.Sleep(1);
                List<PackageResponse> list = udpAgent.GetAllReponse();
                if (list == null)
                    continue;
                for (int i = 0; i < list.Count; i++)
                {
                    AddClient(list[i].ClientId, list[i].ipEnd);

                }
            }
            
        }


        /*发送消息到指定ip端口*/

        public void SendMessage(PackageRequest request)
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

        /*关闭Tcp监听代理*/

        /*接收到客户端连接*/

        /*断开客户端连接*/

        /*接收到客户端消息*/

        /*发送消息给客户端*/

        #endregion

        #region 服务器相关

        /*管理客户端*/
        private int clientBoardCostPort = 0x0faa;
        private Dictionary<int, IPEndPoint> clientDic;
        private void InitClientMgr()
        {
            clientDic = new Dictionary<int, IPEndPoint>();
            clientDic.Add(-1, new IPEndPoint(IPAddress.Broadcast, clientBoardCostPort));
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

        /*启动服务器*/

        /*关闭服务器*/

        /*回收数据*/


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
