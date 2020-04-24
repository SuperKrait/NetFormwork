using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.EventMgr
{
    /// <summary>
    /// 事件机制
    /// made by C.S
    /// 2018.03.19
    /// </summary>
    public class EventSystemMgr
    {
        /// <summary>
        /// 委托
        /// </summary>
        /// <param name="eventId">委托id，写在EventSystemConst下</param>
        /// <param name="objs">传参</param>
        public delegate void UIEventHandle(string eventId, params object[] objs);
        /// <summary>
        /// 事件字典
        /// </summary>
        private static Dictionary<string, List<UIEventHandle>> dic = new Dictionary<string, List<UIEventHandle>>();

        /// <summary>
        /// 注册事件，同一个事件可以重复注册，多次注册多次执行，注意注册次数
        /// </summary>
        /// <param name="eventId">时间id</param>
        /// <param name="eventHandler">事件</param>
        public static void RegisteredEvent(string eventId, UIEventHandle eventHandler)
        {
            List<UIEventHandle> handler;
            if (dic.TryGetValue(eventId, out handler))
            {
                handler.Add(eventHandler);
            }
            else
            {
                List<UIEventHandle> uiEventList = new List<UIEventHandle>();
                uiEventList.Add(eventHandler);
                dic.Add(eventId, uiEventList);
            }
        }
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventId">事件Id</param>
        /// <param name="objs">事件参数</param>
        /// <returns>返回真为，有该事件列表，但是并不能保证执行成功，返回假为没有该事件列表</returns>
        public static bool SentEvent(string eventId, params object[] objs)
        {
            List<UIEventHandle> handlerList;
            if (dic.TryGetValue(eventId, out handlerList))
            {
                for (int i = 0; i < handlerList.Count; i++)
                {
                    if(handlerList[i] != null)
                        handlerList[i](eventId, objs);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 销毁事件列表
        /// </summary>
        /// <param name="eventId">事件列表id</param>
        /// <returns>返回真为事件列表被销毁成功，返回假为事件列表销毁失败</returns>
        public static bool UnRegisteredEvent(string eventId)
        {
            if (dic.ContainsKey(eventId))
            {
                dic.Remove(eventId);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 卸载特定事件
        /// </summary>
        /// <param name="eventId">事件列表id</param>
        /// <param name="eventHandler">指定事件</param>
        /// <returns></returns>
        public static bool UnRegisteredEvent(string eventId, UIEventHandle eventHandler)
        {
            List<UIEventHandle> handlerList;
            if (dic.TryGetValue(eventId, out handlerList))
            {
                if (handlerList.Contains(eventHandler))
                {
                    handlerList.Remove(eventHandler);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 检查的当前事件的注册数量
        /// </summary>
        /// <param name="eventId">事件列表id</param>
        /// <param name="eventHandler">事件</param>
        /// <returns></returns>
        public static int GetSameEventCount(string eventId, UIEventHandle eventHandler)
        {
            List<UIEventHandle> handlerList;
            int count = 0;
            if (dic.TryGetValue(eventId, out handlerList))
            {
                for (int i = 0; i < handlerList.Count; i++)
                {
                    if (handlerList[i] == eventHandler)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// 清空会调用GC
        /// </summary>
        public static void Clear()
        {
            foreach (var pair in dic)
            {
                pair.Value.Clear();
            }
            dic.Clear();
            GC.Collect();
        }
    }
}
