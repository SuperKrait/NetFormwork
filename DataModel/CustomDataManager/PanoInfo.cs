using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.CustomDataManager
{
    /// <summary>
    /// 全景图类
    /// </summary>
    public class PanoInfo
    {
        public string Id
        {
            get;
            set;
        }
        public string Name
        {
            set;
            get;
        }

        /// <summary>
        /// 是全景图还是平面图
        /// </summary>
        public string PanoType
        {
            get;
            set;
        }
        public string PanoTexturePath
        {
            get;
            set;
        }
    }
}
