using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using NetModel.NetMgr.Http;

namespace TestDebugModel
{
    class Node
    {
        public Node Left { get; set; }
        public Node Right { get; set; }
        public string Text { get; set; }
    }

    class Program
    {
        static Node GetNode()
        {
            Node root = new Node
            {
                Left = new Node
                {
                    Left = new Node
                    {
                        Text = "L-L"
                    },
                    Right = new Node
                    {
                        Text = "L-R"
                    },
                    Text = "L"
                },
                Right = new Node
                {
                    Left = new Node
                    {
                        Text = "R-L"
                    },
                    Right = new Node
                    {
                        Text = "R-R"
                    },
                    Text = "R"
                },
                Text = "Root"
            };
            return root;
        }
        
        static void Main(string[] args)
        {
            SByte a = -128;

            short b = a;
            ushort c = (ushort)b;

            Console.WriteLine(c);
            Console.ReadKey();

            //HttpSimpleMgr.ChangeHttpsSecurityProtocol(3);
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            //dic.Add("machineId", "oxo3s4vPUOa7FJYRujbKxf0P0iuM122");
            //dic.Add("devStatus", "0");
            //dic.Add("openId", string.Empty);
            //HttpPost(@"https://store.frtar.com/frtar/hardware/heartBeat", Log, dic);
            //Console.ReadKey();
        }

        public const string serverBaseUrl = @"http://192.168.1.109:9999";
        public const string heartUrl = serverBaseUrl + "/frtar/hardware/heartBeat";

        public static void HttpPost(string url, System.Action<string, string> callBack, Dictionary<string, string> parameters = null)
        {
            try
            {
                string str = HttpSimpleMgr.HttpGetStrContentByPost(url, parameters);

                callBack(str, string.Empty);

            }
            catch (Exception e)
            {
                callBack(string.Empty, e.ToString());
            }
        }

        public static void Log(string msg, string err)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                Console.WriteLine("msg====" + msg);
            }
            else
            {
                Console.WriteLine("error === " + err);
            }
        }

    }
}
