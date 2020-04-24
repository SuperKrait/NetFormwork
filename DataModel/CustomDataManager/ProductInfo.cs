using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.CustomDataManager
{
    public class ProductInfo
    {
        public string Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string productType
        {
            get;
            set;
        }
        /// <summary>
        /// 产品介绍路径
        /// </summary>
        public string ProductContentPath
        {
            get;
            set;
        }

        public List<ProductFlagStruct> flag = new List<ProductFlagStruct>();
        /// <summary>
        /// 产品缩略图路径
        /// </summary>
        public string ThumbnailPath
        {
            get;
            set;
        }

        public List<string> relatedSceneNames = new List<string>();

        public List<string> relatedPanoNames = new List<string>();
    }
}
