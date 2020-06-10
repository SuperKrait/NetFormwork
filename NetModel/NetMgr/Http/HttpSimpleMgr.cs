using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModel.NetMgr.Http
{
    public class HttpSimpleMgr
    {
        public static string HttpGetStrContentByGet(string url)
        {
            int bufferSize = 1024;
            HTTPPackage package = new HTTPPackage(bufferSize);
            string content = "";
            NetModel.NetMgr.Http.HttpHelper.CreateGetHttpResponse(
                url,
                10000,
                null,
                null,
                (data, count) =>
                {
                    if (count.Equals(0))
                    {
                        byte[] str = package.GetData();
                        package.Clear();
                        package = null;
                        content = Encoding.UTF8.GetString(str);
                    }
                    else if(count.Equals(-1))
                    {
                        throw new NullReferenceException(Encoding.UTF8.GetString(data));
                    }
                    else
                        package.AddBuffer(data, count);
                },
                err =>
                {
                    throw new NullReferenceException(string.Format("地址:{0}的网络访问出错=====>>{1}", url, err));
                });
            return content;
        }

        /// <summary>
        /// 获取post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="data">这个是用户需要上传的数据，例如json</param>
        /// <returns></returns>
        public static string HttpGetStrContentByPost(string url, Dictionary<string, string> parameters, byte[] data = null)
        {
            int bufferSize = 1024;
            HTTPPackage package = new HTTPPackage(bufferSize);
            string content = "";
            NetModel.NetMgr.Http.HttpHelper.CreatePostHttpResponse(
                url,
                parameters,
                10000,
                null,
                data,
                null,
                (tmpData, count) =>
                {
                    if (count.Equals(0))
                    {
                        byte[] str = package.GetData();
                        package.Clear();
                        package = null;
                        content = Encoding.UTF8.GetString(str);
                    }
                    else if (count.Equals(-1))
                    {
                        throw new NullReferenceException(Encoding.UTF8.GetString(tmpData));
                    }
                    else
                        package.AddBuffer(tmpData, count);
                },
                err =>
                {
                    throw new NullReferenceException(string.Format("地址:{0}的网络访问出错=====>>{1}", url, err));
                });
            return content;
        }

        public static string HttpPostFormMsg(string url, FormData form, int timeout = 10000)
        {
            int bufferSize = 1024;
            HTTPPackage package = new HTTPPackage(bufferSize);
            string content = "";
            NetModel.NetMgr.Http.HttpHelper.HttpPostData(
                url,
                timeout,
                form,
                null,
                null,
                (tmpData, count) =>
                {
                    if (count.Equals(0))
                    {
                        byte[] str = package.GetData();
                        package.Clear();
                        package = null;
                        content = Encoding.UTF8.GetString(str);
                    }
                    else if (count.Equals(-1))
                    {
                        throw new NullReferenceException(Encoding.UTF8.GetString(tmpData));
                    }
                    else
                        package.AddBuffer(tmpData, count);
                },
                err =>
                {
                    throw new NullReferenceException(string.Format("地址:{0}的网络访问出错=====>>{1}", url, err));
                });
            return content;
        }

        public static void DownloadHttp(string url, string filePath, int timeout = 10000)
        {
            FileInfo file = new FileInfo(filePath);
            using (FileStream stream = file.Open(FileMode.Create))
            {
                NetModel.NetMgr.Http.HttpHelper.CreateGetHttpResponse(
                    url,
                    timeout,
                    null,
                    null,
                    (data, count) =>
                    {
                        if (count.Equals(0))
                        {
                            stream.Close();
                        }
                        else if (count.Equals(-1))
                        {
                            stream.Close();
                            file.Delete();
                            throw new NullReferenceException(Encoding.UTF8.GetString(data));
                        }
                        else
                        {
                            stream.Write(data, 0, count);
                            stream.Flush();
                        }
                    },
                    err =>
                    {
                        throw new NullReferenceException(string.Format("地址:{0}的网络访问出错=====>>{1}", url, err));
                    });
            }
            file = null;
        }

        /// <summary>
        /// HTTPS修改加密方式
        /// </summary>
        /// <param name="type">
        /// 0=Ssl3
        /// 1=Tls
        /// 2=Tls11
        /// 3=Tls12
        /// </param>
        public static void ChangeHttpsSecurityProtocol(int type)
        {
            NetModel.NetMgr.Http.HttpHelper.ChangeHttpsSecurityProtocol(type);
        }
    }
}
