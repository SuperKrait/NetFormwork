using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ThreadPool
{
    /// <summary>
    /// 共享的线程控制器
    /// made by C.S
    /// 2018.03.19
    /// </summary>
    public static class ThreadPoolMgrSimple
    {
        private static ThreadPoolMgr poolMgr = new ThreadPoolMgr(10, 100);

        /// <summary>
        /// 开启一个优先级线程
        /// </summary>
        /// <param name="handler">线程方法</param>
        /// <param name="weight">线程优先级，越小越优先</param>
        /// <param name="errorHandler">线程崩溃回调方法</param>
        /// <param name="idHandler">线程id回传方法</param>
        public static void Start(ThreadPoolMgr.ThreadMainHandleWithOutObj handler, int weight = 2, ThreadPoolMgr.ErrorMessageHandle errorHandler = null, ThreadPoolMgr.GetThreadIdHandle idHandler = null)
        {
            poolMgr.Start(handler, weight, errorHandler, idHandler);
        }
        /// <summary>
        /// 开启一个优先级线程
        /// </summary>
        /// <param name="handler">线程方法</param>
        /// <param name="obj">线程所带参数</param>
        /// <param name="weight">线程优先级，越小越优先</param>
        /// <param name="errorHandler">线程崩溃回调方法</param>
        /// <param name="idHandler">线程id回传方法</param>
        public static void Start(ThreadPoolMgr.ThreadMainHandleWithObj handler, object obj, int weight = 2, ThreadPoolMgr.ErrorMessageHandle errorHandler = null, ThreadPoolMgr.GetThreadIdHandle idHandler = null)
        {
            poolMgr.Start(handler, obj, weight, errorHandler, idHandler);
        }

        /// <summary>
        /// 销毁线程池
        /// </summary>
        public static void Destory()
        {
            poolMgr.Destory();
        }

        /// <summary>
        /// 获取线程池中等待队列的数量
        /// </summary>
        /// <returns></returns>
        public static int GetIdleCount()
        {
            return poolMgr.GetIdleCount();
        }
        /// <summary>
        /// 获取已销毁的线程id列表
        /// </summary>
        /// <returns></returns>
        public static string[] GetDestoryThreadNameList()
        {
            return poolMgr.GetDestoryThreadNameList();
        }
        /// <summary>
        /// 获取所有的报错信息
        /// </summary>
        /// <returns>报错信息用数组的方式 id+报错信息</returns>
        public static string[,] GetErrorArr()
        {
            return poolMgr.GetErrorArr();
        }
    }
}
