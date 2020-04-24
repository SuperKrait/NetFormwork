using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocolFileModel.CustomFileModel
{
    /// <summary>
    /// 文件管理器
    /// </summary>
    public class FileMgr
    {
        public FileMgr()
        {

        }
        ~FileMgr()
        {
            lock (dic)
            {
                dic.Clear();
            }
        }
        /// <summary>
        /// 文件读写执行字典
        /// </summary>
        private Dictionary<string, FileHelper> dic = new Dictionary<string, FileHelper>();
        /// <summary>
        /// 循环写入文件
        /// </summary>
        /// <param name="filePath">文件写入路径</param>
        /// <param name="handler">可以持续获取字节流的委托</param>
        public void WriteFileCycle(string filePath, GetDataCycleHandle handler)
        {
            FileHelper fileHelper = new FileHelper();
            lock (dic)
            {
                GET_ID:
                fileHelper.Id = Guid.NewGuid().ToString();
                if (dic.ContainsKey(fileHelper.Id))
                {
                    goto GET_ID;
                }
                dic.Add(fileHelper.Id, fileHelper);
            }
            fileHelper.WriteCycle(filePath, handler);
            lock (dic)
            {
                if (dic.ContainsKey(fileHelper.Id))
                {
                    dic.Remove(fileHelper.Id);
                }
            }
        }

        /// <summary>
        /// 资源写入本地
        /// </summary>
        /// <param name="fileType">文件类型</param>
        /// <param name="path">文件路径</param>
        /// <param name="data">资源byte[]</param>
        public void WriteFile(FileType fileType, string path, byte[] data)
        {
            switch (fileType)
            {
                case FileType.Custom:
                    WriteCustom(path, data);
                    break;
                case FileType.Txt:
                    WriteTxt(path, data);
                    break;
                case FileType.Texture:
                    WriteTexture(path, data);
                    break;
                case FileType.Vedio:
                    WriteVedio(path, data);
                    break;
            }

        }
        /// <summary>
        /// 写入文本文档
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        private void WriteTxt(string path, byte[] data)
        {
            TxtFileHelper fileHelper = new TxtFileHelper();
            lock (dic)
            {
                GET_ID:
                fileHelper.Id = Guid.NewGuid().ToString();
                if (dic.ContainsKey(fileHelper.Id))
                {
                    goto GET_ID;
                }
                dic.Add(fileHelper.Id, fileHelper);
            }
            fileHelper.WriteOnce(path, data);
            lock (dic)
            {
                if (dic.ContainsKey(fileHelper.Id))
                {
                    dic.Remove(fileHelper.Id);
                }
            }
        }
        /// <summary>
        /// 写入图片
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        private void WriteTexture(string path, byte[] data)
        {
            TextureFileHelper fileHelper = new TextureFileHelper();

            lock (dic)
            {
                GET_ID:
                fileHelper.Id = Guid.NewGuid().ToString();
                if (dic.ContainsKey(fileHelper.Id))
                {
                    goto GET_ID;
                }
                dic.Add(fileHelper.Id, fileHelper);
            }
            fileHelper.WriteOnce(path, data);
            lock (dic)
            {
                if (dic.ContainsKey(fileHelper.Id))
                {
                    dic.Remove(fileHelper.Id);
                }
            }

        }
        /// <summary>
        /// 写入媒体文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        private void WriteVedio(string path, byte[] data)
        {
            VedioFileHelper fileHelper = new VedioFileHelper();
            lock (dic)
            {
                GET_ID:
                fileHelper.Id = Guid.NewGuid().ToString();
                if (dic.ContainsKey(fileHelper.Id))
                {
                    goto GET_ID;
                }
                dic.Add(fileHelper.Id, fileHelper);
            }
            fileHelper.WriteOnce(path, data);
            lock (dic)
            {
                if (dic.ContainsKey(fileHelper.Id))
                {
                    dic.Remove(fileHelper.Id);
                }
            }
        }
        /// <summary>
        /// 写入自定义文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        private void WriteCustom(string path, byte[] data)
        {
            FileHelper fileHelper = new FileHelper();
            lock (dic)
            {
                GET_ID:
                fileHelper.Id = Guid.NewGuid().ToString();
                if (dic.ContainsKey(fileHelper.Id))
                {
                    goto GET_ID;
                }
                dic.Add(fileHelper.Id, fileHelper);
            }
            fileHelper.WriteOnce(path, data);
            lock (dic)
            {
                if (dic.ContainsKey(fileHelper.Id))
                {
                    dic.Remove(fileHelper.Id);
                }
            }
        }
        /// <summary>
        /// 读取本地资源
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="path">文件路径</param>
        /// <param name="handler">获取资源回调</param>
        public void LoadFile(FileType fileType, string path, GetDataHandle handler)
        {
            switch (fileType)
            {
                case FileType.Custom:
                    LoadCustom(path, handler);
                    break;
                case FileType.Txt:
                    LoadTxt(path, handler);
                    break;
                case FileType.Texture:
                    LoadTexture(path, handler);
                    break;
                case FileType.Vedio:
                    LoadVedio(path, handler);
                    break;
            }
        }

        private void LoadTxt(string path, GetDataHandle handler)
        {
            TxtFileHelper fileHelper = new TxtFileHelper();
            lock (dic)
            {
                GET_ID:
                fileHelper.Id = Guid.NewGuid().ToString();
                if (dic.ContainsKey(fileHelper.Id))
                {
                    goto GET_ID;
                }
                dic.Add(fileHelper.Id, fileHelper);
            }
            fileHelper.Load(path, handler);
            lock (dic)
            {
                if (dic.ContainsKey(fileHelper.Id))
                {
                    dic.Remove(fileHelper.Id);
                }
            }
        }
        private void LoadTexture(string path, GetDataHandle handler)
        {
            TextureFileHelper fileHelper = new TextureFileHelper();

            lock (dic)
            {
                GET_ID:
                fileHelper.Id = Guid.NewGuid().ToString();
                if (dic.ContainsKey(fileHelper.Id))
                {
                    goto GET_ID;
                }
                dic.Add(fileHelper.Id, fileHelper);
            }
            fileHelper.Load(path, handler);
            lock (dic)
            {
                if (dic.ContainsKey(fileHelper.Id))
                {
                    dic.Remove(fileHelper.Id);
                }
            }

        }
        private void LoadVedio(string path, GetDataHandle handler)
        {
            VedioFileHelper fileHelper = new VedioFileHelper();
            lock (dic)
            {
                GET_ID:
                fileHelper.Id = Guid.NewGuid().ToString();
                if (dic.ContainsKey(fileHelper.Id))
                {
                    goto GET_ID;
                }
                dic.Add(fileHelper.Id, fileHelper);
            }
            fileHelper.Load(path, handler);
            lock (dic)
            {
                if (dic.ContainsKey(fileHelper.Id))
                {
                    dic.Remove(fileHelper.Id);
                }
            }
        }
        private void LoadCustom(string path, GetDataHandle handler)
        {
            FileHelper fileHelper = new FileHelper();
            lock (dic)
            {
                GET_ID:
                fileHelper.Id = Guid.NewGuid().ToString();
                if (dic.ContainsKey(fileHelper.Id))
                {
                    goto GET_ID;
                }
                dic.Add(fileHelper.Id, fileHelper);
            }
            fileHelper.Load(path, handler);
            lock (dic)
            {
                if (dic.ContainsKey(fileHelper.Id))
                {
                    dic.Remove(fileHelper.Id);
                }
            }
        }        
    }
}
