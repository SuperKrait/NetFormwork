using System;
using System.Collections.Generic;
using System.Threading;
using static Common.ThreadPool.ThreadWeightQueue;

namespace Common.ThreadPool
{
    /// <summary>
    /// 线程池
    /// made by C.S
    /// 2018.03.19
    /// </summary>
    public class ThreadPoolMgr
    {
        /// <summary>
        /// 创建线程池
        /// </summary>
        /// <param name="maxThreadCount">最大线程池支持数</param>
        /// <param name="idleCount">待机线程数</param>
        /// <param name="weightQueueMaxWeight">权重队列的权重最多值</param>
        public ThreadPoolMgr(int maxThreadCount = 10, int idleCount = 5, int weightQueueMaxWeight = 5)
        {
            this.maxThreadCount = maxThreadCount;
            if (idleCount > maxThreadCount)
                idleCount = maxThreadCount;
            Init(idleCount, weightQueueMaxWeight);
        }
        /// <summary>
        /// 线程崩溃回调委托
        /// </summary>
        /// <param name="msg"></param>
        public delegate void ErrorMessageHandle(string msg);
        /// <summary>
        /// 获取线程id委托
        /// </summary>
        /// <param name="id"></param>
        public delegate void GetThreadIdHandle(string id);
        /// <summary>
        /// 线程执行委托（带参数）
        /// </summary>
        /// <param name="obj"></param>
        public delegate void ThreadMainHandleWithObj(object obj);
        /// <summary>
        /// 线程执行委托（不带参数）
        /// </summary>
        public delegate void ThreadMainHandleWithOutObj();

        /// <summary>
        /// 线程等待队列
        /// </summary>
        private Queue<ItemThread> idleQueue;
        /// <summary>
        /// 线程运行中的队列
        /// </summary>
        private List<ItemThread> aliveList;
        /// <summary>
        /// 线程崩溃查询字典，id,崩溃回调
        /// </summary>
        private Dictionary<string, ErrorMessageHandle> errorDic;
        /// <summary>
        /// 线程报错信息集合
        /// </summary>
        private List<KeyValuePair<string, string>> errorList;
        /// <summary>
        /// 已经销毁的线程信息
        /// </summary>
        private List<string> threadDestoryIds;
        /// <summary>
        /// 默认初始化线程的数量
        /// </summary>
        private int idleFirstCount = 5;
        /// <summary>
        /// 线程池是否正在销毁
        /// </summary>
        private bool isDestory = false;
        /// <summary>
        /// 当前执行中的线程数量
        /// </summary>
        private int curAliveThreadCount = 0;

        private object curAliveThreadCountLock;
        /// <summary>
        /// 线程池最大值
        /// </summary>
        private int maxThreadCount = 10;
        private object destoryObjLock = new object();
        /// <summary>
        /// 线程池权重队列
        /// </summary>
        private ThreadWeightQueue weightQueue;

        private object genIdLock = new object();

        /// <summary>
        /// 初始化，构造函数自动执行
        /// </summary>
        /// <param name="idleCount"></param>
        private void Init(int idleCount = 5, int weightQueueMaxWeight = 5)
        {
            isDestory = false;
            weightQueue = new ThreadWeightQueue(weightQueueMaxWeight);
            curAliveThreadCount = 0;
            curAliveThreadCountLock = new object();
            this.idleFirstCount = idleCount;
            destoryObjLock = new object();
            //startLock = new object();
            idleQueue = new Queue<ItemThread>(idleCount);
            aliveList = new List<ItemThread>(idleCount);
            errorDic = new Dictionary<string, ErrorMessageHandle>();
            errorList = new List<KeyValuePair<string, string>>();
            threadDestoryIds = new List<string>(idleCount);
            for (int i = 0; i < idleCount; i++)
            {
                idleQueue.Enqueue(GenerateOneItem());
            }
        }
        /// <summary>
        /// 从等待列表中获取一个等待线程线程
        /// </summary>
        /// <returns></returns>
        private ItemThread GetAItemFromIdleQueue()
        {
            lock (idleQueue)
            {
                if (idleQueue.Count > 0)
                    return idleQueue.Dequeue();                
            }
            return null;
        }
        /// <summary>
        /// 获取一个线程
        /// </summary>
        /// <returns></returns>
        private ItemThread GetAItem()
        {
            lock(curAliveThreadCountLock)
            {
                if (curAliveThreadCount > maxThreadCount)
                    return null;
                else
                    curAliveThreadCount++;
            }
            ItemThread item = GetAItemFromIdleQueue();
            if(item == null)
                item = GenerateOneItem();
            return item;
        }
        /// <summary>
        /// 生成一个线程
        /// </summary>
        /// <returns></returns>
        private ItemThread GenerateOneItem()
        {
            ItemThread item = new ItemThread(DestoryThread, EndThread, ErrorThread);
            return item;
        }
        /// <summary>
        /// 线程执行完毕，回归等待队列
        /// </summary>
        /// <param name="item"></param>
        private void EndThread(ItemThread item)
        {
            lock(aliveList)
                aliveList.Remove(item);
            lock (curAliveThreadCountLock)
                curAliveThreadCount--;
            lock (destoryObjLock)
                if (isDestory)
                {
                    item.Stop();
                    return;
                }
            lock (idleQueue)
                idleQueue.Enqueue(item);
            ThreadQueueHandle();
        }
        /// <summary>
        /// 线程执行报错
        /// </summary>
        /// <param name="item"></param>
        private void ErrorThread(ItemThread item)
        {
            KeyValuePair<string, string> pair = new KeyValuePair<string, string>(item.Id, item.ErrorCode);
            lock (errorDic)
            {
                ErrorMessageHandle handler;
                //errorDic.Add(pair);
                if (errorDic.TryGetValue(item.Id, out handler))
                {
                    if (handler != null)
                    {
                        handler(item.ErrorCode);
                    }
                    errorDic.Remove(item.Id);
                }
                errorList.Add(pair);
            }
            lock(curAliveThreadCountLock)
                curAliveThreadCount--;
            lock (destoryObjLock)
                if (isDestory)
                {
                    item.Stop();
                    return;
                }
            ThreadQueueHandle();
        }
        /// <summary>
        /// 销毁一个线程
        /// </summary>
        /// <param name="item"></param>
        private void DestoryThread(ItemThread item)
        {
            lock (threadDestoryIds)
            {
                threadDestoryIds.Add(item.Id);
            }
        }

        /// <summary>
        /// 根据等待执行队列中，事件的权重，获取线程并且执行
        /// </summary>
        private void ThreadQueueHandle()
        {
            lock (curAliveThreadCountLock)
            {
                if (curAliveThreadCount >= maxThreadCount)
                    return;
            }
            ThreadMainHandle handler = null;
            string id = "";
            lock (weightQueue)
            {
                if (weightQueue.Count <= 0)
                    return;                
                handler = weightQueue.DeQueue(out id);
            }
            if (handler != null)
            {
                ItemThread item = GetAItem();
                item.Start(id,
                delegate ()
                {
                    handler();
                });
                lock (aliveList)
                {
                    aliveList.Add(item);
                }
            }
        }

        /// <summary>
        /// 开启一个优先级线程
        /// </summary>
        /// <param name="handler">线程方法</param>
        /// <param name="weight">线程优先级，越小越优先</param>
        /// <param name="errorHandler">线程崩溃回调方法</param>
        /// <param name="idHandler">线程id回传方法</param>
        public void Start(ThreadMainHandleWithOutObj handler, int weight = 2, ErrorMessageHandle errorHandler = null, GetThreadIdHandle idHandler = null)
        {
            lock(destoryObjLock)
                if (isDestory)
                    return;
            GET_ID1://获取id
            string threadId;
            lock (genIdLock)
            {
                threadId = Guid.NewGuid().ToString();
            }
            lock (errorDic)
            {
                if (errorDic.ContainsKey(threadId))
                {
                    Thread.Sleep(0);
                    goto GET_ID1;
                }
                else if(errorHandler != null)
                {
                    errorDic.Add(threadId, errorHandler);
                }
            }
            lock (weightQueue)
            {
                weightQueue.EnQueue(
                delegate ()
                {
                    if (handler != null) handler();
                }
                , threadId, weight);
            }
            ThreadQueueHandle();

            if (idHandler != null)
            {
                idHandler(threadId);
            }
        }
        /// <summary>
        /// 开启一个优先级线程
        /// </summary>
        /// <param name="handler">线程方法</param>
        /// <param name="obj">线程所带参数</param>
        /// <param name="weight">线程优先级，越小越优先</param>
        /// <param name="errorHandler">线程崩溃回调方法</param>
        /// <param name="idHandler">线程id回传方法</param>
        public void Start(ThreadMainHandleWithObj handler, object obj, int weight = 2, ErrorMessageHandle errorHandler = null, GetThreadIdHandle idHandler = null)
        {
            lock (destoryObjLock)
                if (isDestory)
                    return;
            GET_ID2://获取id
            string threadId;
            lock (genIdLock)
            {
                threadId = Guid.NewGuid().ToString();
            }
            lock (errorDic)
            {
                if (errorDic.ContainsKey(threadId))
                {
                    Thread.Sleep(0);
                    goto GET_ID2;
                }
                else if (errorHandler != null)
                {
                    errorDic.Add(threadId, errorHandler);
                }
            }
            lock (weightQueue)
            {                
                weightQueue.EnQueue(
                delegate ()
                {
                    if (handler != null) handler(obj);
                }
                , threadId, weight);
            }
            ThreadQueueHandle();

            if (idHandler != null)
            {
                idHandler(threadId);
            }
        }

        /// <summary>
        /// 销毁线程池
        /// </summary>
        public void Destory()
        {
            lock(destoryObjLock)
                isDestory = true;
            lock (idleQueue)
            {
                for (int i = idleQueue.Count - 1; i >= 0; i--)
                {
                    ItemThread item = idleQueue.Dequeue();
                    item.Stop();
                }
            }
            lock(weightQueue)
                weightQueue.Clear();
        }

        /// <summary>
        /// 获取线程池中等待队列的数量
        /// </summary>
        /// <returns></returns>
        public int GetIdleCount()
        {
            lock (idleQueue)
            {
                return idleQueue.Count;
            }
        }
        /// <summary>
        /// 获取已销毁的线程id列表
        /// </summary>
        /// <returns></returns>
        public string[] GetDestoryThreadNameList()
        {
            lock (threadDestoryIds)
            {
                return threadDestoryIds.ToArray();
            }
        }
        /// <summary>
        /// 获取所有的报错信息
        /// </summary>
        /// <returns>报错信息用数组的方式 id+报错信息</returns>
        public string[,] GetErrorArr()
        {
            string[,] arr = null;
            lock (errorDic)
            {
                arr = new string[errorList.Count, 2];
                for (int i = 0; i < errorList.Count; i++)
                {
                    arr[i, 0] = errorList[i].Key;
                    arr[i, 1] = errorList[i].Value;
                }
            }
            return arr;
        }

        ~ThreadPoolMgr()
        {
            bool tmpIsDestory = false;
            lock (destoryObjLock)
                tmpIsDestory = isDestory;
            if (!tmpIsDestory)
            {
                Destory();
            }
        }
    }
}
