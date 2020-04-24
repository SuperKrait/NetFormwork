using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.FileDataManager
{
    public class CustomFileInfo
    {
        public CustomFileInfo(string filePath, string id)
        {
            Id = id;
            FullPath = filePath;
            everyPathArr = filePath.Split('\\');

            FileName = everyPathArr[everyPathArr.Length - 1];
            int index = FileName.LastIndexOf('.');
            if (index != -1)
            {
                FileName = FileName.Substring(0, index);
            }

            FromPath = filePath.Substring(0, filePath.LastIndexOf('\\'));

            index = FullPath.LastIndexOf('.');
            if (index != -1)
            {
                ExName = FullPath.Substring(index);
            }
        }
        ~CustomFileInfo()
        {
            Id = null;
            FullPath = null;
            everyPathArr = null;
            FileName = null;
            FromPath = null;
        }

        public string Id
        {
            get;
            set;
        }

        public string FullPath
        {
            get;
            set;
        }


        public string[] everyPathArr;

        public string FileName
        {
            get;
            set;
        }

        public string FromPath
        {
            get;
            set;
        }
        /// <summary>
        /// 扩展名
        /// </summary>
        public string ExName
        {
            get;
            set;
        }
    }
}
