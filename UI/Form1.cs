using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.EventMgr;
using System.Threading;

namespace ExcelMgr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Init();
            EventSystemMgr.RegisteredEvent(EventSystemConst.UpdateUI, SendUICmd);
            EventSystemMgr.SentEvent(EventSystemConst.MainThreadSwitch);

        }
        #region testBox
        private void ShowTestTxt(object obj)
        {
            string str = obj.ToString();
            TestTBX.Text += str + "\r\n";
        }
        #endregion

        #region 通用UI更新模块

        private delegate void UIUseHandle(object obj);
        /// <summary>
        /// UI激活字典
        /// </summary>
        private Dictionary<string, UIUseHandle> uiDic = new Dictionary<string, UIUseHandle>();
        //private Queue<KeyValuePair<string, object>> uiCmdQueue = new Queue<KeyValuePair<string, object>>();
        //private bool isStart = false;
        //private bool isUpdate = true;

        /// <summary>
        /// UI线程锁
        /// </summary>
        private object commonObj = new object();
        private delegate void CallUpdateUIHandle(string uiName, object obj);
        /// <summary>
        /// UI激活方法
        /// </summary>
        private CallUpdateUIHandle updateUIHandler;

        private void Init()
        {
            updateUIHandler = CallUIHandler;
            RegisterUI();
        }
        /// <summary>
        /// 注册UI界面
        /// </summary>
        public void RegisterUI()
        {
            AddUI(UITypeConst.TestTbx, ShowTestTxt);
        }
       
        public void SendUICmd(string eId, params object[] objs)
        {
            string uiName = objs[0].ToString();
            object obj = null;
            if (objs.Length > 1)
            {
                obj = objs[1];
            }
            lock (commonObj)
            {
                KeyValuePair<string, object> pair = new KeyValuePair<string, object>(uiName, obj);
                //uiCmdQueue.Enqueue(pair);
                if (this.InvokeRequired)
                {
                    this.Invoke(updateUIHandler, pair.Key, pair.Value);
                }
                else
                {
                    updateUIHandler(pair.Key, pair.Value);
                }
            }
            
        }

        //private void UpdateUI()
        //{
        //    while (isUpdate)
        //    {
        //        lock (commonObj)
        //        {
        //            while (uiCmdQueue.Count > 0)
        //            {
        //                KeyValuePair<string, object> pair = uiCmdQueue.Dequeue();
        //                CallUIHandler(pair.Key, pair.Value);
        //            }
        //        }
        //        Thread.Sleep(0);
        //    }
        //}

        /// <summary>
        /// 注册UI方法
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="handler"></param>
        private void AddUI(string uiName, UIUseHandle handler)
        {
            uiDic.Add(uiName, handler);
        }
        /// <summary>
        /// 调用UI方法
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="obj"></param>
        private void CallUIHandler(string uiName, object obj)
        {
            UIUseHandle handler;
            if (uiDic.TryGetValue(uiName, out handler))
            {
                if (handler != null)
                {
                    handler(obj);
                    return;
                }
            }
            SetUIError(uiName, obj);
        }
        /// <summary>
        /// UI界面报错方法，使用的时候补全即可
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="obj"></param>
        private void SetUIError(string uiName, object obj)
        {
            //等待添加UI报错提示
        }

        private void Clear()
        {
            lock (commonObj)
            {
                //isUpdate = false;
                //uiCmdQueue.Clear();
                uiDic.Clear();
            }
        }
        #endregion
    }
}
