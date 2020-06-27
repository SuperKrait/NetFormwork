using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using MediaPlayerCtl;
using System.Diagnostics;
using Common.Log;
using NetModel.NetMgr.S2C.Client;
using NetModel.NetMgr.S2C.Client.Protocol;
using Common.TaskPool;

namespace TestAppWindows
{
    public partial class Form1 : Form
    {
        MPHelper helper;
        public Form1()
        {
            InitializeComponent();

            bool isStart = true;

            ClientMgr client = new ClientMgr();
            client.Init();
            client.StartServer();



            ProtocolMgr.RegisterProtocol(ProtocolMgr.UDP_Hello, (resp) =>
            {
                UdpResponseHelloTest pack = (UdpResponseHelloTest)resp;
                pack.DeSerialize();
                MethodInvoker mi = new MethodInvoker(() =>
                {
                    textBox1.Text = pack.HelloWorld;
                });

                mi.Invoke();
            });


            TaskPoolHelper.Instance.StartALongTask(()=>
            {
                while (isStart)
                {
                    Thread.Sleep(100);
                    client.Update();
                }                
            });

            UdpRequestHelloTest package = ProtocolMgr.GetPackageRequest<UdpRequestHelloTest>(-1);

            client.SendUdpMessage(package);

            this.FormClosed += new FormClosedEventHandler((e, obj) =>
            {
                isStart = false;
            });


            //helper = new MPHelper();
            //helper.player = axWindowsMediaPlayer1;
            //LogAgent.Log("我试试！！！");
            //helper.Init((msg) =>
            //{
            //    try
            //    {
            //        LogAgent.Log("我播放完成啦！！！");
            //        throw new Exception("我是一个错");
            //    }
            //    catch (System.Exception ex)
            //    {
            //        LogAgent.LogError(ex.StackTrace);
            //    }
            //});
            //helper.Play("L:\\ABCD.mkv");
        }
    }
}
