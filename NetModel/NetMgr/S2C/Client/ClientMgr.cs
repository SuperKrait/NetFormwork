using Common.Log;
using Common.TaskPool;
using NetModel.NetMgr.S2C.Client.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetModel.NetMgr.S2C.Client
{
    public class ClientMgr
    {
        #region Udp相关
        /*创建Udp监听代理*/
        private UdpAgent udpAgent;
        private bool isUdpStart = true;


        private bool InitUdpServer()
        {
            udpAgent = new UdpAgent();
            if (!udpAgent.Init(serverBoardCostPort))
            {
                udpAgent.DestoryUdpServer();
                return false;
            }
            TaskPoolHelper.Instance.StartALongTask(GetUdpMessage, "ClientMgr->GetUdpMessage");
            return true;
                      
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
        private IPEndPoint serverTcpIpEnd;


        private bool InitTcpServer()
        {
            this.tcpAgent = new TcpAgent();
            IPEndPoint point = null;
            IPAddress[] addrs = GetLocalIp();
            for (int i = 0; i < addrs.Length; i++)
            {
                if (addrs[i].ToString().Equals("127.0.0.1"))
                    continue;
                else
                    point = new IPEndPoint(addrs[i], 0);
                    break;
            }
            if (!this.tcpAgent.Init(point, serverTcpIpEnd))
            {
                this.tcpAgent.DestoryTcpServer();
                return false;
            }
            else
            {
                TaskPoolHelper.Instance.StartALongTask(GetTcpMessage, "ClientMgr->GetTcpMessage");
                return true;
            }
        }

        /*控制Tcp服务器*/

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


        public void GetTcpMessage()
        {
            while (isTcpStart)
            {
                Thread.Sleep(1);
                List<TcpResponseBase> list = tcpAgent.GetAllReponse();
                if (list == null)
                    continue;
                for (int i = 0; i < list.Count; i++)
                {
                    AddMessage(list[i]);   
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
        private int serverBoardCostPort = 0x0faa;

        public void Init()
        {
            if (!InitUdpServer())
            {
                LogAgent.LogError("Udp服务器初始化失败！\r\n");
            }
            if (InitTcpServer())
            {
                LogAgent.LogError("Tcp服务器初始化失败\r\n");
            }
        }


        /*获取本机可用Ip*/       

        private IPAddress[] GetLocalIp()
        {
            IPAddress[] ipAddrs = Dns.GetHostAddresses("localhost");
            return ipAddrs;
        }

        /*
         * 改变服务器状态
         * 1为开始
         * 2为暂停
         */
        private int status = 0;

        public void StartServer()
        {
            if (status.Equals(1))
                return;
            status = 1;
            StartUdpClient();
            StartTcpClient();
        }

        public void PauseServer()
        {
            if (status.Equals(2))
                return;
            status = 2;
            PauseUdpClient();
            PauseTcpClient();
        }



        /*接收数据*/

        private ProtocolMgr protocolMgr;

        private void AddMessage(PackageResponse pack)
        {
            protocolMgr.AddRespPackage2Arr(pack);
        }


        /*服务器通用*/


        public void Update()
        {
            protocolMgr.Update();
        }

        public void Destory()
        {
            if (status.Equals(0))
            {
                return;
            }
            status = 0;
            DestoryTcpClient();
            DestoryUdpClient();
        }

        #endregion

    }
}
