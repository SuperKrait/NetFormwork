using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Threading;

namespace NetModel.NetMgr.Http
{
    public static class HttpHelper
    {

        /// <summary>
        /// 创建POST方式的HTTP请求
        /// </summary>
        /// <param name="url">Post地址</param>
        /// <param name="parameters">传参</param>
        /// <param name="timeout">超时时间（毫秒整数）</param>
        /// <param name="header">常用头</param>
        /// <param name="headerCustom">自定义头</param>
        /// <param name="dataCustom">自定义数据</param>
        /// <param name="cookies"></param>
        /// <param name="errorHandler">报错回调</param>
        /// <returns></returns>
        public static void CreatePostHttpResponse(string url, 
            IDictionary<string, string> parameters, 
            int timeout, 
            IDictionary<string, string> headerCustom,
            byte[] dataCustom,
            CookieCollection cookies,
            Action<byte[], int> callBack,
            Action<string> errorHandler
            )
        {
            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version11;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            //WebHeaderCollection tmpHeader = new WebHeaderCollection();
            request.Timeout = timeout;

            if (headerCustom != null)
            {
                foreach (var pair in headerCustom)
                {
                    switch (pair.Key)
                    {
                        case "Accept":
                        case "Connection":
                        case "Content-Type":
                        case "Expect":
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Content-Length":
                            //request.ContentLength = long.Parse(tmpHeader[i]);
                            //tmpHeader.Remove("Content-Length");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Date":
                            //request.Date = System.DateTime.Parse(tmpHeader[i]);
                            //tmpHeader.Remove("Date");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Host":
                            request.Proxy = new WebProxy(pair.Value, false);
                            break;
                        case "If-Modified-Since":
                            //request.IfModifiedSince = System.DateTime.Parse(tmpHeader[i]);
                            //tmpHeader.Remove("If-Modified-Since");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Range":
                            //string[] rangeArr = tmpHeader[i].Split('_');
                            //request.AddRange(int.Parse(rangeArr[0]), int.Parse(rangeArr[1]));
                            //tmpHeader.Remove("Range");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Referer":
                            //request.Referer = tmpHeader[i];
                            //tmpHeader.Remove("Referer");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Transfer-Encoding":
                            //request.TransferEncoding = tmpHeader[i];
                            //tmpHeader.Remove("Transfer-Encoding");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "User-Agent":
                            //request.UserAgent = tmpHeader[i];
                            //tmpHeader.Remove("User-Agent");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        default:
                            request.Headers.Add(pair.Key, pair.Value);
                            break;
                    }
                }
            }

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);                
            }

            //发送POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        i++;
                    }
                }
                
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    if(data != null && data.Length > 0)
                        stream.Write(data, 0, data.Length);
                    if(dataCustom != null && dataCustom.Length > 0)
                        stream.Write(dataCustom, 0, dataCustom.Length);
                }
            }

            HttpWebResponse reponse = null;
            try
            {
                reponse = request.GetResponse() as HttpWebResponse;
            }
            catch (System.Net.ProtocolViolationException e)
            {
                errorHandler(e.ToString());
            }
            catch (System.Net.WebException e)
            {
                errorHandler(e.ToString());
            }
            catch (System.InvalidOperationException e)
            {
                errorHandler(e.ToString());
            }
            catch (System.NotSupportedException e)
            {
                errorHandler(e.ToString());
            }
            catch (System.Exception e)
            {
                errorHandler(e.ToString());
            }

            SetCallBackMessage(reponse, callBack);

            if (reponse != null)
                reponse.Close();
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            return false;
        }


        /// <summary>  
        /// 创建GET方式的HTTP请求, callBack完成后.count返回0
        /// </summary>  
        public static void CreateGetHttpResponse(
            string url, 
            int timeout, 
            //IDictionary<HttpRequestHeader, string> header,
            IDictionary<string, string> headerCustom,
            CookieContainer cookies,
            Action<byte[], int> callBack,
            Action<string> errorHandler = null)
        {
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //对服务端证书进行有效性校验（非第三方权威机构颁发的证书，如自己生成的，不进行验证，这里返回true）
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version11;    //http版本，默认是1.1,这里设置为1.0
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "GET";

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            request.Timeout = timeout;

            if (headerCustom != null)
            {
                foreach (var pair in headerCustom)
                {
                    switch (pair.Key)
                    {
                        case "Accept":
                        case "Connection":
                        case "Content-Type":
                        case "Expect":
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Content-Length":
                            //request.ContentLength = long.Parse(tmpHeader[i]);
                            //tmpHeader.Remove("Content-Length");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Date":
                            //request.Date = System.DateTime.Parse(tmpHeader[i]);
                            //tmpHeader.Remove("Date");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Host":
                            request.Proxy = new WebProxy(pair.Value, false);
                            break;
                        case "If-Modified-Since":
                            //request.IfModifiedSince = System.DateTime.Parse(tmpHeader[i]);
                            //tmpHeader.Remove("If-Modified-Since");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Range":
                            //string[] rangeArr = tmpHeader[i].Split('_');
                            //request.AddRange(int.Parse(rangeArr[0]), int.Parse(rangeArr[1]));
                            //tmpHeader.Remove("Range");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Referer":
                            //request.Referer = tmpHeader[i];
                            //tmpHeader.Remove("Referer");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Transfer-Encoding":
                            //request.TransferEncoding = tmpHeader[i];
                            //tmpHeader.Remove("Transfer-Encoding");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "User-Agent":
                            //request.UserAgent = tmpHeader[i];
                            //tmpHeader.Remove("User-Agent");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        default:
                            request.Headers.Add(pair.Key, pair.Value);
                            break;
                    }
                }
            }
            if (cookies != null)
            {
                request.CookieContainer = cookies;
            }

            HttpWebResponse reponse = null;
            try
            {
                reponse = request.GetResponse() as HttpWebResponse;
            }
            catch (System.Net.ProtocolViolationException e)
            {
                errorHandler(e.ToString());
            }
            catch (System.Net.WebException e)
            {
                errorHandler(e.ToString());
            }
            catch (System.InvalidOperationException e)
            {
                errorHandler(e.ToString());
            }
            catch (System.NotSupportedException e)
            {
                errorHandler(e.ToString());
            }
            catch (System.Exception e)
            {
                errorHandler(e.ToString());
            }
            SetCallBackMessage(reponse, callBack);

            //callBack(readStream.ReadToEnd());
            if (reponse != null)
                reponse.Close();
        }

        /// <summary>
        /// 表单提交
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeOut"></param>
        /// <param name="form">表单文件</param>
        /// <returns></returns>
        public static void HttpPostData(string url, 
            int timeOut,
            FormData form,
            //IDictionary<HttpRequestHeader, string> header,
            IDictionary<string, string> headerCustom,
            CookieCollection cookies,
            Action<byte[], int> callBack,
            Action<string> errorHandler
            )
        {
            HttpWebRequest request;

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //对服务端证书进行有效性校验（非第三方权威机构颁发的证书，如自己生成的，不进行验证，这里返回true）
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version11;    //http版本，默认是1.1,这里设置为1.0
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            request.Timeout = timeOut;
            request.Method = "POST";

            if (headerCustom != null)
            {
                foreach (var pair in headerCustom)
                {
                    switch (pair.Key)
                    {
                        case "Accept":
                        case "Connection":
                        case "Content-Type":
                        case "Expect":
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Content-Length":
                            //request.ContentLength = long.Parse(tmpHeader[i]);
                            //tmpHeader.Remove("Content-Length");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Date":
                            //request.Date = System.DateTime.Parse(tmpHeader[i]);
                            //tmpHeader.Remove("Date");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Host":
                            request.Proxy = new WebProxy(pair.Value, false);
                            break;
                        case "If-Modified-Since":
                            //request.IfModifiedSince = System.DateTime.Parse(tmpHeader[i]);
                            //tmpHeader.Remove("If-Modified-Since");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Range":
                            //string[] rangeArr = tmpHeader[i].Split('_');
                            //request.AddRange(int.Parse(rangeArr[0]), int.Parse(rangeArr[1]));
                            //tmpHeader.Remove("Range");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Referer":
                            //request.Referer = tmpHeader[i];
                            //tmpHeader.Remove("Referer");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "Transfer-Encoding":
                            //request.TransferEncoding = tmpHeader[i];
                            //tmpHeader.Remove("Transfer-Encoding");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        case "User-Agent":
                            //request.UserAgent = tmpHeader[i];
                            //tmpHeader.Remove("User-Agent");
                            SetHeaderValue(request.Headers, pair.Key, pair.Value);
                            break;
                        default:
                            request.Headers.Add(pair.Key, pair.Value);
                            break;
                    }
                }
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }

            string boundary = "-----------" + DateTime.Now.Ticks.ToString("x");

            request.ContentType = "multipart/form-data; boundary=" + boundary;

            MemoryStream stream = new MemoryStream();
            form.GetWordsFromData(stream, boundary);
            request.ContentLength = stream.Length;


            using (Stream streamRequest = request.GetRequestStream())
            {
                stream.Seek(0, SeekOrigin.Begin);
                byte[] data = new byte[stream.Length];
                int count = stream.Read(data, 0, data.Length);
                streamRequest.Write(data, 0, count);
            }

            HttpWebResponse reponse = null;

            try
            {
                reponse = request.GetResponse() as HttpWebResponse;
            }
            catch (System.Net.ProtocolViolationException e)
            {
                errorHandler(e.ToString());
            }
            catch (System.Net.WebException e)
            {
                errorHandler(e.ToString());
            }
            catch (System.InvalidOperationException e)
            {
                errorHandler(e.ToString());
            }
            catch (System.NotSupportedException e)
            {
                errorHandler(e.ToString());
            }
            catch (System.Exception e)
            {
                errorHandler(e.ToString());
            }


            SetCallBackMessage(reponse, callBack);


            //callBack(readStream.ReadToEnd());
            if (reponse != null)
                reponse.Close();
        }

        private static void SetCallBackMessage(HttpWebResponse reponse, Action<byte[], int> callBack)
        {
            if (reponse != null)
            {
                try
                {
                    Stream receiveStream = reponse.GetResponseStream();
                    if (!string.IsNullOrEmpty(reponse.Headers.Get("content-length")))
                    {
                        long length = long.Parse(reponse.Headers["content-length"]);
                        byte[] data = new byte[length];
                        byte[] buffer = new byte[1024];
                        long curCount = 0;
                        while (curCount < length)
                        {
                            Thread.Sleep(1);
                            int count = receiveStream.Read(buffer, 0, 1024);
                            if (count > 0 && callBack != null)
                            {
                                callBack(buffer, count);
                            }
                            else
                                break;
                        }
                        Console.WriteLine("获取完毕！");
                        if (callBack != null)
                        {
                            callBack(null, 0);
                        }
                    }
                    else
                    {
                        Encoding cod = Encoding.GetEncoding(reponse.CharacterSet);
                        using (StreamReader reader = new StreamReader(reponse.GetResponseStream(), cod))
                        {
                            byte[] data = cod.GetBytes(reader.ReadToEnd());
                            if (callBack != null)
                            {
                                callBack(data, data.Length);
                                callBack(null, 0);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (callBack != null)
                    {
                        callBack(Encoding.UTF8.GetBytes(e.ToString()), -1);
                    }
                }

            }
        }

        public static void SetHeaderValue(WebHeaderCollection header, string name, string content)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = content;
            }
        }
    }
}
