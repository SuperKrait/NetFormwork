using Common.Log;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.TaskPool
{
    public class TaskPoolHelper
    {
        public readonly static TaskPoolHelper Instance = new TaskPoolHelper();

        private Dictionary<string, TaskItemBase> dic = new Dictionary<string, TaskItemBase>();


        /*给线程赋值一个id*/
        private string GetThreadName()
        {
            return Guid.NewGuid().ToString();
        }

        /*初始化一个线程池中的线程，给线程命名*/
        public void StartNewTask<T>(Action<T> action, T obj = default(T), string threadName = "", Action<string> errCode = null)
        {
            TaskItem<T> item = new TaskItem<T>(GetThreadName());
            item.action = action;
            item.parameter = obj;
            item.cusName = threadName;
            lock (dic)
            {
                dic.Add(item.id, item);
            }

            Action ac = () =>
            {
                try
                {
                    item.action(obj);
                }
                catch (Exception e)
                {
                    lock (dic)
                    {
                        item.SetErrCode(e.ToString());
                    }
                }

                item.Dispose();
                lock (dic)
                    dic.Remove(item.id);
            };


            Task task = new Task(ac);
            task.Start();
        }

        public void StartNewTask(Action action, string threadName = "")
        {
            StartNewTask<object>(delegate (object obj)
            {
                action();
            }, null, threadName);
        }

        /*初始化一个独立的线程*/
        public void StartALongTask<T>(Action<T> action, T obj = default(T), string threadName = "", Action<string> errCode = null)
        {
            TaskItem<T> item = new TaskItem<T>(GetThreadName());
            item.action = action;
            item.parameter = obj;
            item.cusName = threadName;

            lock (dic)
            {
                dic.Add(item.id, item);
            }

            Action ac = () =>
            {
                try
                {
                    item.action(obj);
                }
                catch (Exception e)
                {
                    lock (dic)
                    {
                        item.SetErrCode(e.ToString());
                    }
                }
                item.Dispose();
                lock (dic)
                    dic.Remove(item.id);
            };

            Task.Run(ac);
        }

        public void StartALongTask(Action action, string threadName = "")
        {
            StartALongTask<object>(delegate (object obj)
            {
                action();
            }, null, threadName);
        }

        /*检查当前存活线程数量*/

        public int GetAliveThreadCount()
        {
            return dic.Count;
        }

        public List<KeyValuePair<string, string>> GetAllThreadInfo()
        {
            lock (dic)
            {
                if (dic.Count.Equals(0))
                    return null;

                List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
                foreach (var pair in dic)
                {
                    list.Add(new KeyValuePair<string, string>(pair.Key, pair.Value.cusName));
                }

                return list;
            }            
        }


        class TaskItemBase : IDisposable
        {
            public string cusName;
            public string id;
            public Type type;
            public Action<string> errAction;
            public TaskItemBase(string id)
            {
                this.id = id;
            }

            public void SetErrCode(string err)
            {
                LogAgent.Log(err);
                if (errAction != null)
                {
                    errAction(err);
                }
            }

            public virtual void Dispose()
            {
                cusName = "";
                id = "";
                type = null;
                errAction = null;
            }
        }

        class TaskItem<T>: TaskItemBase
        {
            public Action<T> action;
            public T parameter;
            public TaskItem(string id) : base(id)
            {
                base.type = typeof(T);
            }

            public override void Dispose()
            {
                base.Dispose();
                action = null;
                parameter = default(T);
            }

        }


    }
    
}
