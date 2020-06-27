using Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetModel.NetMgr.S2C.Client
{
    public class ProtocolMgr
    {
        public readonly static ProtocolMgr Instance = new ProtocolMgr();
        #region 协议类型
        /*Udp相关*/
        public const int UDP_Main = 0x100;

        public const int UDP_Hello = UDP_Main + 1;



        /*Tcp相关*/
        public const int Data_Main = 0x200;
        public const int Data_GetClientId = Data_Main + 1;
        /// <summary>
        /// 成功接收Tcp链接
        /// </summary>
        public const int Data_SetTcpId = Data_Main + 2;


        /*游戏相关*/
        public const int GT_Game_Main = 0x1000;
        public const int GT_Game_Content = GT_Game_Main + 1;




        public static void RegisterProtocol(int protocol, Action<PackageResponse> reponse)
        {
            string name = "ProtocolMgr====" + protocol;
            Common.EventMgr.EventSystemMgr.RegisteredEvent(name, (na, objs)=>
            {
                LogAgent.Log("收到协议！协议Id = " + protocol + "\r\n");
                reponse((PackageResponse)objs[0]);
            });
        }

        public static void UnRegisterProtocol(int protocol, Action<PackageResponse> reponse = null)
        {
            string name = "ProtocolMgr====" + protocol;
            Common.EventMgr.EventSystemMgr.UnRegisteredEvent(name);
        }

        private static void SentMsg(int protocol, PackageResponse pack)
        {
            Common.EventMgr.EventSystemMgr.SentEvent("ProtocolMgr====" + protocol, pack);
        }


        #endregion


        #region 处理收到消息
        /// <summary>
        /// 主线程调用的Update
        /// </summary>
        public void Update()
        {
            lock (respList)
            {
                if (respList.Count.Equals(0))
                    return;
                respList.Distinct<PackageResponse>();
                respList.Sort();
                for (int i = 0; i < respList.Count; i++)
                {
                    SentMsg(respList[i].ProtocolId, respList[i]);
                }
                respList.Clear();
            }
        }

        
        private List<PackageResponse> respList = new List<PackageResponse>();

        public void AddRespPackage2Arr(PackageResponse respPack)
        {
            lock(respList)
                respList.Add(respPack);
        }

        #endregion

        /// <summary>
        /// 获取发送数据包
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public static T GetPackageRequest<T>(int clientId) where T : PackageRequest, new()
        {
            T pack = (T)new T().Initialization(T, clientId);
            //T pack = (T)Activator.CreateInstance(typeof(T), clientId);
            switch (typeof(T).Name)
            {
                case "UdpRequestGetClientId":
                    pack.ProtocolId = Data_GetClientId;
                    break;
                default:
                    break;
            }

            return pack;
        }
    }
}
