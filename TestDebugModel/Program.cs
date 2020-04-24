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
            DateTime createTime = System.DateTime.Now;
            createTime = createTime.AddHours(-8);

            string createDateR = createTime.ToString("R");
            Console.WriteLine(createDateR);
            Console.ReadKey();
        }
        public const string serverBaseUrl = @"http://192.168.1.109:9999";
        public const string heartUrl = serverBaseUrl + "/frtar/hardware/heartBeat";
        private static SynchronizationContext context = new SynchronizationContext();
        public static async Task HttpPost(string url, System.Action<string, string> callBack, Dictionary<string, string> parameters = null)
        {
            await Task.Delay(1);
            try
            {
                string str = HttpSimpleMgr.HttpGetStrContentByPost(url, parameters);
                context.Post(obj =>
                {
                    callBack(str, string.Empty);
                }, str);
            }
            catch (Exception e)
            {
                callBack(string.Empty, e.ToString());
            }
        }

        static async Task GetStart()
        {
            await Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Thread2 id = " + Thread.CurrentThread.ManagedThreadId);
            },
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.Current);

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Thread4 id = " + Thread.CurrentThread.ManagedThreadId);
            },
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.Current);


            Console.WriteLine("Thread3 id = " + Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(3000);
            
        }

        private void GetStart1()
        {
            Task t = new Task(() =>
            { }, CancellationToken.None, TaskCreationOptions.LongRunning);
        }

        static void DisplayTree(Node root)
        {
            var task = Task.Factory.StartNew(() => DisplayNode(root),
                                            CancellationToken.None,
                                            TaskCreationOptions.None,
                                            TaskScheduler.Default);
            task.Wait();
        }

        static void DisplayNode(Node current)
        {

            if (current.Left != null)
                Task.Factory.StartNew(() => DisplayNode(current.Left),
                                            CancellationToken.None,
                                            TaskCreationOptions.AttachedToParent,
                                            TaskScheduler.Default);
            if (current.Right != null)
                Task.Factory.StartNew(() => DisplayNode(current.Right),
                                            CancellationToken.None,
                                            TaskCreationOptions.AttachedToParent,
                                            TaskScheduler.Default);
            Console.WriteLine("当前节点的值为{0};处理的ThreadId={1}", current.Text, Thread.CurrentThread.ManagedThreadId);
        }
    }
}
