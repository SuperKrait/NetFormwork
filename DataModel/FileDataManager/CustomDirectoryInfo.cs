using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.FileDataManager
{
    class CustomDirectoryInfo
    {
        public CustomDirectoryInfo(string dirPath, string id)
        {
            Id = id;
            FullPath = dirPath;
            everyPathArr = dirPath.Split('\\');
            DirName = everyPathArr[everyPathArr.Length - 1];
            FromPath = dirPath.Substring(0, dirPath.LastIndexOf('\\'));

        }
        ~CustomDirectoryInfo()
        {
            Id = null;
            FullPath = null;
            everyPathArr = null;
            DirName = null;
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

        public string DirName
        {
            get;
            set;
        }

        public string FromPath
        {
            get;
            set;
        }
    }
}
