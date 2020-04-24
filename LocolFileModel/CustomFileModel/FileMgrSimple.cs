using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocolFileModel.CustomFileModel
{
    /// <summary>
    /// 文件读取写入样例类
    /// </summary>
    public static class FileMgrSimple
    {
        /// <summary>
        /// 简单的文件管理器
        /// </summary>
        private static FileMgr fileMgr = new FileMgr();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public static void WriteFile(FileType fileType, string path, byte[] data)
        {
            fileMgr.WriteFile(fileType, path, data);
        }

        public static void LoadFile(FileType fileType, string path, GetDataHandle handler)
        {
            fileMgr.LoadFile(fileType, path, handler);
        }

        public static void WriteFileCycle(string filePath, GetDataCycleHandle handler)
        {
            fileMgr.WriteFileCycle(filePath, handler);
        }
    }
}
