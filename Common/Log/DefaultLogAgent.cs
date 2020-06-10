using System.Diagnostics;
using System.Text;

namespace Common.Log
{
    class DefaultLogAgent : ILogAgent
    {
        public static readonly DefaultLogAgent Instance = new DefaultLogAgent();

        public void Log(params object[] param)
        {
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i < param.Length; i++)
            {
                builder.Append(param[i].ToString());
            }
            Debug.Print("======This is a log======\r\n" + builder.ToString() + "\r\n" + System.DateTime.Now);
        }

        public void LogWarning(params object[] param)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < param.Length; i++)
            {
                builder.Append(param[i].ToString());
            }
            Debug.Print("######This is a warning######\r\n" + builder.ToString() + "\r\n" + System.DateTime.Now);
        }

        public void LogError(params object[] param)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < param.Length; i++)
            {
                builder.Append(param[i].ToString());
            }
            Debug.Print("*****************This is an Error!!!!!!!!****************\r\n" + builder.ToString() + "\r\n" + System.DateTime.Now);
        }

        public void SaveLocal(string textPath, string text)
        {
            ;
        }
    }
}
