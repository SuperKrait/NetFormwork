using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    interface IActionTask
    {
        /// <summary>
        /// 退出当前线程
        /// </summary>
        void ExitTask();

        void PlayTask();

        void TaskErr(byte errCode, string errContent);
    }
}
