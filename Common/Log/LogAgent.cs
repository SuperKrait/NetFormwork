using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Log
{
    public class LogAgent
    {
        private static ILogAgent agent = new DefaultLogAgent();

        public static void SetAgent(ILogAgent newAgent)
        {
            if(newAgent != null)
                LogAgent.agent = newAgent;
        }

        public static void Log(params object[] objs)
        {
            agent.Log(objs);
        }

        public static void LogWarning(params object[] objs)
        {
            agent.LogWarning(objs);
        }

        public static void LogError(params object[] objs)
        {
            agent.LogError(objs);
        }
    }
}
