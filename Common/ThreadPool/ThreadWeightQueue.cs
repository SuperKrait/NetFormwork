using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ThreadPool
{
    /// <summary>
    /// 权重队列池
    /// made by C.S
    /// 2018.03.19
    /// </summary>
    internal class ThreadWeightQueue
    {
        //public enum ThreadWeightType : sbyte
        //{
        //    FAST = 0,
        //    QUICK = 1,
        //    NORMOL = 2,
        //    SLOW = 3,
        //    LAST = 4,
        //}
        public ThreadWeightQueue(int maxWeight = 5)
        {
            defaultMaxWeight = maxWeight;
            Init();
        }
        /// <summary>
        /// 线程主委托
        /// </summary>
        public delegate void ThreadMainHandle();
        /// <summary>
        /// 权重字典，权重+线程主委托队列
        /// </summary>
        private Dictionary<int, Queue<ThreadMainHandle>> dic;
        /// <summary>
        /// 给每个方法赋予的ID字典组
        /// </summary>
        private Dictionary<int, Queue<string>> idDic;
        private int pointer;
        /// <summary>
        /// 默认最小优先级的等级
        /// </summary>
        private static int defaultMaxWeight = 5;
        /// <summary>
        /// 默认上次的权重指针索引位置
        /// </summary>
        private int defaultLastIndex;
        private object weightLock;
        private int count;
        public int Count
        {
            get
            {
                lock (weightLock)
                {
                    return count;
                }
            }
            private set
            {
                count = value;
            }
        }

        private void Init()
        {
            weightLock = new object();
            defaultLastIndex = defaultMaxWeight - 1;
            dic = new Dictionary<int, Queue<ThreadMainHandle>>(defaultMaxWeight);
            idDic = new Dictionary<int, Queue<string>>(defaultMaxWeight);
            for (sbyte i = 0; i < defaultMaxWeight; i++)
            {
                Queue<ThreadMainHandle> queue = new Queue<ThreadMainHandle>();
                dic.Add(i, queue);
                Queue<string> idQueue = new Queue<string>();
                idDic.Add(i, idQueue);
            }
            pointer = defaultMaxWeight;
            Count = 0;
        }

        /// <summary>
        /// 压入队列
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="weight">优先级，优先级越小越优先执行</param>
        /// <returns>返回真为权重值正确，返回假为权重值错误</returns>
        public bool EnQueue(ThreadMainHandle handler, string id, int weight = 2)
        {
            if (weight >= defaultMaxWeight)
                return false;
            if (pointer > weight)
                pointer = weight;
            dic[weight].Enqueue(handler);
            idDic[weight].Enqueue(id);
            Count++;

            //Debug.Print("EnQueue====== weight = " + weight + "\tCount = " + Count + "\tpointer = " + pointer);
            
            return true;
        }
        /// <summary>
        /// 取出队列
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ThreadMainHandle DeQueue(out string id)
        {
            id = "";
            if (pointer == defaultMaxWeight)
                return null;
            ThreadMainHandle item = null;
            //Debug.Print("DeQueue ====== dic[pointer].Count = " + dic[pointer].Count + "\tCount = " + Count + "\tpointer = " + pointer);

            item = dic[pointer].Dequeue();
            id = idDic[pointer].Dequeue();
            //检索优先级字典中最优先的方法
            if (dic[pointer].Count == 0)
            {
                if (pointer == defaultLastIndex)
                {
                    pointer = defaultMaxWeight;
                }
                else
                {
                    bool isUpdatePointer = false;
                    for (int i = pointer; i < defaultMaxWeight; i++)
                    {
                        if (dic[i].Count > 0)
                        {
                            pointer = i;
                            isUpdatePointer = true;
                            break;
                        }
                    }
                    if(!isUpdatePointer)
                        pointer = defaultMaxWeight;
                }
            }
            Count--;
            //垃圾方法会被清理掉
            if (item == null)
                item = DeQueue(out id);
            return item;
        }
        /// <summary>
        /// 清空权重队列
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < defaultMaxWeight; i++)
            {
                dic[i].Clear();
            }
            dic.Clear();
            pointer = defaultMaxWeight;
            weightLock = null;
        }
    }
}
