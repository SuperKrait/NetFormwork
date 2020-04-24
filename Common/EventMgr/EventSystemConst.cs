using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.EventMgr
{
    /// <summary>
    /// 常用事件注册常量
    /// made by C.S
    /// 2018.03.19
    /// </summary>
    public class EventSystemConst
    {
        #region 主逻辑相关

        #endregion

        #region 数据相关

        #endregion

        #region 网络相关

        #endregion

        #region 本地文件以及文件夹相关
        public const string WriteFile = "WriteFile";
        public const string LoadFile = "LoadFile";
        #endregion

        #region UI相关
        /// <summary>
        /// UI上主逻辑继续执行
        /// </summary>
        public const string MainThreadSwitch = "MainThreadSwitch";
        /// <summary>
        /// 在UI上的测试框中输出
        /// </summary>
        public const string UpdateUI = "UpdateUI";
        #endregion

        #region 测试相关

        #endregion
    }
}
