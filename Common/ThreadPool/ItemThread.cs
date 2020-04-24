using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common.ThreadPool
{
    internal class ItemThread
    {
        /// <summary>
        /// 构造一个线程
        /// made by C.S
        /// 2018.03.19
        /// </summary>
        /// <param name="destoryHandler">销毁回调</param>
        /// <param name="endHandler">执行完毕回调</param>
        /// <param name="errorHandler">报错回调</param>
        public ItemThread(MessageHandler destoryHandler, MessageHandler endHandler, MessageHandler errorHandler)
        {
            isUpdate = true;
            threadContoll = new ManualResetEvent(false);
            this.destoryHandler = destoryHandler;
            this.endHandler = endHandler;
            this.errorHandler = errorHandler;
            threadMain = new Thread(ThreadMainLogic);
            threadMain.Start();
        }
        /// <summary>
        /// 内部线程id
        /// </summary>
        private string id;
        /// <summary>
        /// 线程id
        /// </summary>
        public string Id {
            get { return id; }
            private set
            {
                if (string.IsNullOrEmpty(id))
                {
                    id = value;
                }
            }
        }
        /// <summary>
        /// 线程执行委托
        /// </summary>
        public delegate void ThreadMainHandler();
        /// <summary>
        /// 线程回传信息委托
        /// </summary>
        /// <param name="item"></param>
        public delegate void MessageHandler(ItemThread item);
        /// <summary>
        /// 线程errorCode
        /// </summary>
        public string ErrorCode
        {
            get;
            set;
        }
        /// <summary>
        /// 线程执行的委托实例
        /// </summary>
        private ThreadMainHandler mainHandler;
        /// <summary>
        /// 销毁回调
        /// </summary>
        private MessageHandler destoryHandler;
        /// <summary>
        /// 执行完毕回调
        /// </summary>
        private MessageHandler endHandler;
        /// <summary>
        /// 报错回调
        /// </summary>
        private MessageHandler errorHandler;
        /// <summary>
        /// 实际的执行线程
        /// </summary>
        private Thread threadMain;
        /// <summary>
        /// 判定线程是否循环复用
        /// </summary>
        private bool isUpdate = false;
        /// <summary>
        /// 线程开始执行的开关
        /// </summary>
        private ManualResetEvent threadContoll;
        private object threadLock = new object();
        
        /// <summary>
        /// 执行线程
        /// </summary>
        /// <param name="id">线程id</param>
        /// <param name="mainHanlder">线程锁</param>
        public void Start(string id, ThreadMainHandler mainHanlder)
        {
            Id = id;
            this.mainHandler = mainHanlder;
            threadContoll.Set();
        }
        
        /// <summary>
        /// 停止当前线程复用
        /// </summary>
        public void Stop()
        {
            lock (threadLock)
            {
                isUpdate = false;
            }
            threadMain = null;
            threadContoll.Set();
        }
        /// <summary>
        /// 线程的执行逻辑
        /// </summary>
        private void ThreadMainLogic()
        {
            try
            {
                while (true)
                {
                    threadContoll.WaitOne();
                    lock (threadLock)
                    {
                        if (!isUpdate)
                            break;
                    }
                    threadContoll.Reset();
                    //Console.WriteLine("我执行了" + Id);
                    StartThread();
                    //Console.WriteLine("我完毕了" + Id);
                    this.mainHandler = null;
                    End();
                }
            }
            catch (Exception e)
            {
                SetError(e.ToString());
            }
            Destrory();
        }
        /// <summary>
        /// 主方法执行
        /// </summary>
        private void StartThread()
        {
            if (this.mainHandler != null)
            {
                this.mainHandler();
            }
        }
        /// <summary>
        /// 结束执行回调
        /// </summary>
        private void End()
        {
            if (endHandler != null)
                endHandler(this);
        }
        /// <summary>
        /// 销毁回调
        /// </summary>
        private void Destrory()
        {
            if (destoryHandler != null)
                destoryHandler(this);
        }
        /// <summary>
        /// 报错回调
        /// </summary>
        /// <param name="errorCode"></param>
        private void SetError(string errorCode)
        {
            ErrorCode = errorCode;
            if (!string.IsNullOrEmpty(errorCode) && errorHandler != null)
            {
                errorHandler(this);
            }
            Id = string.Empty;
            ErrorCode = string.Empty;
        }        
    }
}
