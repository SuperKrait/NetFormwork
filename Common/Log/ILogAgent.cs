using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Log
{
    public interface ILogAgent
    {
        void Log(params object[] param);
        void LogWarning(params object[] param);
        void LogError(params object[] param);

        void SaveLocal(string textPath, string text);
    }
}
