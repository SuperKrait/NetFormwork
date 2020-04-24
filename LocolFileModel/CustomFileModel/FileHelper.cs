using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LocolFileModel.CustomFileModel
{
    /// <summary>
    /// 文件读写类
    /// </summary>
    internal class FileHelper
    {

        ~FileHelper()
        {
            Id = null;
        }
        public string Id
        {
            get;
            set;
        }
        /// <summary>
        /// 循环写入本地文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="handler">获取流资源方法</param>
        public virtual void WriteCycle(string filePath, GetDataCycleHandle handler)
        {
            //循环创建好文件夹
            string tmpFilePath = RecursiveCheckDir(filePath);

            if (string.IsNullOrEmpty(tmpFilePath))
            {
                throw new FileNotFoundException("文件路径有误---->" + filePath);
            }
            if (File.Exists(tmpFilePath))
            {
                File.Delete(tmpFilePath);
            }

            IEnumerator ienum = CycleSaveFile(filePath, handler);

            while (ienum.MoveNext())
            {
                ;
            }
        }
        IEnumerator CycleSaveFile(string filePath, GetDataCycleHandle handler)
        {
            using (FileStream file = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                while (true)
                {
                    if (handler != null)
                    {
                        byte[] data = handler();
                        if (data != null)
                        {
                            file.Write(data, 0, data.Length);
                            file.Flush();
                            yield return 1;
                        }
                        else
                        {
                            break;
                        }                        
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 一次性写入本地
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        public virtual void WriteOnce(string filePath, byte[] data)
        {
            string tmpFilePath = RecursiveCheckDir(filePath);
            if (string.IsNullOrEmpty(tmpFilePath))
            {
                throw new FileNotFoundException("文件路径有误---->" + filePath);
            }
            if (File.Exists(tmpFilePath))
            {
                File.Delete(tmpFilePath);
            }
            File.WriteAllBytes(filePath, data);
        }
        /// <summary>
        /// 遍历文件对应文件夹，如果没有则自动创建文件夹
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        protected string RecursiveCheckDir(string filePath)
        {
            string[] fileDir = filePath.Split('\\');
            if (fileDir.Length < 1)
                return string.Empty;
            string tmpFilePath = fileDir[0] + "\\";
            for (int i = 1; i < fileDir.Length - 1; i++)
            {
                if (!Directory.Exists(tmpFilePath + fileDir[i]))
                {
                    Directory.CreateDirectory(tmpFilePath + fileDir[i]);
                }
                tmpFilePath += fileDir[i] + "\\";
            }
            tmpFilePath += fileDir[fileDir.Length - 1];
            return tmpFilePath;
        }
        /// <summary>
        /// 从本地一次性读取
        /// </summary>
        /// <param name="filePath">读取文件的路径</param>
        /// <param name="hanlder">读取到文件回传的委托</param>
        public virtual void Load(string filePath, GetDataHandle hanlder)
        {
            if (hanlder != null)
            {
                byte[] data = File.ReadAllBytes(filePath);

                hanlder(data);
            }
        }

    }
    /// <summary>
    /// 回传读取到数据的委托
    /// </summary>
    /// <param name="data">读取到的数据</param>
    public delegate void GetDataHandle(byte[] data);

    /// <summary>
    /// 获取存储数据，返回null结束存储
    /// </summary>
    /// <returns>返回需要存储的数据</returns>
    public delegate byte[] GetDataCycleHandle();

}
