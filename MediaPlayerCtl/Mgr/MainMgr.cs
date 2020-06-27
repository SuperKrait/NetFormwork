using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Common.Utils;
using Common.TaskPool;
using Common.Log;
using System.Threading;
using NetModel.NetMgr.Http;
using LitJson;

namespace MediaPlayerCtl
{
    public class MainMgr
    {
        #region 主流程

        public MainMgr(Action<Action> addMainAction)
        {
            this.addMainAction = addMainAction;
        }

        private string mCode;
        /// <summary>
        /// 0为初始化完毕等待开始
        /// 1为开始
        /// 2为暂停
        /// 3为销毁中
        /// </summary>
        private int mgrStatus = 0;

        private Action startAppHandle;
        /// <summary>
        /// 使方法被主线程调用
        /// </summary>
        private Action<Action> addMainAction;


        public void SetStartAppHandle(Action action)
        {
            this.startAppHandle = action;
        }

        public bool Init()
        {
            /*初始化面板*/
            UICtrl.Instance.Init();
            InitWeb();
            UICtrl.Instance.mpPanel.playerHelper.Init(EndPlayMovie);

            this.mCode = GetMechineCode();
            /*判断是否打开注册流程*/
            if (!CheckRegisnFile())
            {
                UICtrl.Instance.rgPanel.resisnHandler = SetRegisnCode;
                UICtrl.Instance.OpenRegisnPanel();
                return false;
            }
            return true;
        }



        public void MainProgress()
        {
            addMainAction(() =>
            {
                UICtrl.Instance.OpenMainPanel();
            });
            mgrStatus = 2;
            StartSendHeart();
            PlayProgress();            
        }

        public void PlayProgress()
        {
            while (true)
            {
                Thread.Sleep(1000);

                switch (mgrStatus)
                {
                    case 0:
                    case 3:
                        goto STOP_PLAY;
                    case 2:
                        continue;
                }

                if (string.IsNullOrEmpty(curPlayPath))
                    continue;
                string tmpPath = curPlayPath;
                curPlayPath = string.Empty;

                addMainAction(() =>
                {
                    UICtrl.Instance.OpenMPPanel();
                    UICtrl.Instance.mpPanel.playerHelper.Play(tmpPath);
                });

                mgrStatus = 2;
            }

        STOP_PLAY:
            LogAgent.Log("销毁播放了");
        }


        private void EndPlayMovie(string path)
        {
            mgrStatus = 1;
            TellServerPlayEnd(path);
            StartSendHeart();
        }

        private string GetMechineCode()
        {
            return MachineCode.MachineCode.GetMachineCodeString();
        }

        public void StopAll()
        {
            StopSendHeart();
            mgrStatus = 3;
        }




        #endregion

        #region 注册相关

        private string regisnTextPath;
        private string regisnCode;

        private bool CheckRegisnFile()
        {
            if (!File.Exists(regisnTextPath))
            {
                return false;
            }

            string[] code = File.ReadAllLines(regisnTextPath);

            if (code.Length < 3)
            {
                return false;
            }
            string regisnCode = code[1];

            string md5 = MD5Code.GetMD5HashFromByte(Encoding.UTF8.GetBytes(mCode + regisnCode));
            if (!string.Compare(md5, code[2]).Equals(0))
            {
                return false;
            }
            this.id = code[0];
            this.regisnCode = regisnCode;
            return true;
        }

        private void SetRegisnCode(string str)
        {
            /*检查注册码是否符合格式*/
            if (str.Length.Equals(22))
            {
                SetRegisnResult(3);
                return;
            }
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] <= 73 && str[i] >= 64)
                {
                    continue;
                }
                if (str[i] <= 81 && str[i] > 106)
                {
                    continue;
                }
                if (str[i] <= 113 && str[i] > 122)
                {
                    continue;
                }
                SetRegisnResult(3);
                return;
            }

            regisnCode = str;
            CheckRegisnFromServerr(str);
        }

        /// <summary>
        /// 反馈注册结果
        /// </summary>
        /// <param name="status">0,为失败,1为成功,2为断网,3为本地输入有误</param>
        public void SetRegisnResult(int status)
        {
            string content = "";
            if (!status.Equals(1))
            {
                switch (status)
                {
                    case 0:
                        content = "注册失败！请重试...";
                        break;
                    case 2:
                        content = "当前网络状态不佳！请重试...";
                        break;
                    case 3:
                        content = "序列号输入有误，请重试...";
                        break;
                }
                addMainAction(()=>
                {
                    UICtrl.Instance.rgPanel.GetResultFalse(content);
                });
                return;
            }


            SetRegisignFile2Local(this.id);
            CheckPublicVedio();
            MainProgress();
        }

        private void SetRegisignFile2Local(string id)
        {
            if (File.Exists(regisnTextPath))
            {
                File.Delete(regisnTextPath);
            }
            string[] code = new string[3];
            code[0] = id;
            code[1] = this.regisnCode;
            code[2] = MD5Code.GetMD5HashFromByte(Encoding.UTF8.GetBytes(this.mCode + this.regisnCode));

            File.WriteAllLines(regisnTextPath, code);
        }


        #endregion

        #region 网络相关
        private string id;
        /// <summary>
        /// 0为初始化完毕,关闭状态
        /// 1为开启状态
        /// 2为暂停状态
        /// 3为正在销毁状态
        /// </summary>
        private int isEnableReceiveCodeStatus = 0;

        private int isDownLoading = 0;

        private void InitWeb()
        {
            isEnableReceiveCodeStatus = 2;
            TaskPoolHelper.Instance.StartALongTask(SendHeart, "MainMgr->SendHeart");
        }

        private void SendHeart()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("eqptCode", regisnCode);
            param.Add("machineId", mCode);
            while (true)
            {
                Thread.Sleep(5000);
                switch (isEnableReceiveCodeStatus)
                {
                    case 0:
                    case 3:
                        goto STOP_Heart;
                    case 2:
                        continue;
                }
                TaskPoolHelper.Instance.StartNewTask(() =>
                {
                    string content = GetMsgFromHttpByPost(httpHeart, param);

                    if (Interlocked.CompareExchange(ref isDownLoading, 1, 0).Equals(0))
                    {
                        string cmdFromS = ;//后台来的命令

                        if (cmdFromS.Equals("GetFile"))
                        {
                            if (!TellServerGetCode())
                            {
                                Interlocked.Exchange(ref isDownLoading, 0);
                                return;
                            }

                            isEnableReceiveCodeStatus = 2;

                            string url = ;//下载文件的url

                            string path = CheckLocalFileExist(url);

                            if (!string.IsNullOrEmpty(path))
                            {
                                this.curPlayPath = path;
                                return;
                            }

                            addMainAction(()=>
                            {
                                UICtrl.Instance.OpenDownloadPanel();
                            });


                            path = DownloadFile(url, );

                            if (!string.IsNullOrEmpty(path))
                            {
                                this.curPlayPath = path;
                                return;
                            }
                            addMainAction(() =>
                            {
                                UICtrl.Instance.OpenMainPanel();
                            });

                        }

                        Interlocked.Exchange(ref isDownLoading, 0);
                    }

                }, "MainMgr->GetHeartMsg");
            }
        STOP_Heart:

            LogAgent.Log("服务器关闭");
        }

        private void StopSendHeart()
        {
            isEnableReceiveCodeStatus = 3;
        }

        private void StartSendHeart()
        {
            isEnableReceiveCodeStatus = 1;
        }



        private bool TellServerGetCode()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add();
            param.Add();
            param.Add();

            string content = GetMsgFromHttpByPost(httpTellServerGetDLUrl, param);

            if (string.IsNullOrEmpty(content))
            {
                return false;
            }
            return true;
        }

        private void TellServerPlayEnd(string filePath)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("eqptId", filePath.Substring(filePath.LastIndexOf('\\') + 1, filePath.LastIndexOf('\\') - filePath.LastIndexOf('.')));

            GetMsgFromHttpByPost(httpTellServerGetDLUrl, param);
        }


        private string DownloadFile(string url, string fileId)
        {
            string path = System.IO.Path.Combine(dirPath, url.Substring(url.LastIndexOf('/') + 1));
            string fileName = path.Substring(path.LastIndexOf('/') + 1);
            string endName = fileName.Substring(fileName.LastIndexOf('.'));
            path = path.Replace(fileName, fileId + endName);

            bool status = false;
            try
            {
                HttpSimpleMgr.DownloadHttp(url, path);
                status = true;
            }
            catch (NullReferenceException e)
            {
                LogAgent.LogError(e.ToString());
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            Interlocked.Exchange(ref isDownLoading, 0);

            if (status)
                return path;
            else
                return string.Empty;
        }

        private void CheckRegisnFromServerr(string regisnCode)
        {
            Dictionary<string, string> tmpDic = new Dictionary<string, string>();
            tmpDic.Add("eqptCode", regisnCode);
            tmpDic.Add("machineId", mCode);
            TaskPoolHelper.Instance.StartNewTask(() =>
            {
                string content = GetMsgFromHttpByPost(httpRegisn, tmpDic);

                if (string.IsNullOrEmpty(content))
                {
                    SetRegisnResult(2);
                    return;
                }

                JsonData json = JsonMapper.ToJson(content);
                if (json["status"].Equals(0))
                {
                    SetRegisnResult(0);
                }
                else
                {
                    this.id = json["data"].ToString();
                    this.regisnCode = regisnCode;

                    if (startAppHandle != null)
                    {
                        startAppHandle();
                    }
                }

            }, "MainMgr->CheckRegisnFromServerr");
        }

        /// <summary>
        /// 当前本地公共视频状态
        /// -1是正在请求，0是没有，1是齐全，2是不全，3是获取列表失败
        /// </summary>
        private bool publicVedioStatus = false;

        /// <summary>
        /// 查看公共视频是否完整
        /// </summary>
        public void CheckPublicVedio()
        {
            string content = "";
            TaskPoolHelper.Instance.StartNewTask(()=>
            {
                Dictionary<string, string> tmpDic = new Dictionary<string, string>();
                content = GetMsgFromHttpByPost(httpGetDefaultVedio, tmpDic);
            });


            while (!publicVedioStatus)
            {
                Thread.Sleep(1);                
            }

            if (string.IsNullOrEmpty(content))
            {
                LogAgent.LogError("获取公共视频列表失败！");
                return;
            }

            string listText = ;

            string[] vedioId = listText.Split(...);
            addMainAction(() =>
            {
                UICtrl.Instance.OpenDownloadPanel();
            });

            /*去检查公共视频是否下载完成*/
            int count = 0;
            for (int i = 0; i < vedioId.Length; i++)
            {
                string path = CheckLocalFileExist(vedioId[i]);
                if (string.IsNullOrEmpty(path))
                {
                    TaskPoolHelper.Instance.StartNewTask(() =>
                    {
                        string[] tmp = vedioId[i].Split(...);
                        //0是url地址，1是id
                        path = DownloadFile(tmp[0], tmp[1]);
                        
                        if (string.IsNullOrEmpty(path))
                        {
                            if (File.Exists(path))
                            {
                                File.Delete(path);
                            }
                        }
                        else
                        {
                            lock (publicVedioList)
                                publicVedioList.Add(path);
                        }
                        Interlocked.Add(ref count, 1);
                    });
                }
                else
                {
                    Interlocked.Add(ref count, 1);
                    publicVedioList.Add(path);
                }                
            }

            /*等待公共视频全部下载完成*/
            while (true)
            {
                if (count.Equals(vedioId.Length))
                {
                    break;
                }
                Thread.Sleep(1000);
            }
            UICtrl.Instance.mainPanel.AddList(publicVedioList);
        }


        private const string httpServer = "http://192.168.1.109:9999";
        private const string httpRegisn = httpServer + "/frtar/hardware/registerMachine";
        private const string httpHeart = httpServer + "";
        private const string httpTellServerGetDLUrl = httpServer + "";
        private const string httpGetQRCode = httpServer + "/frtar/wx/videoCode";
        private const string httpGetVedioUrlByPersonal = httpServer + "frtar/hardware/personalVideo";
        private const string httpGetDefaultVedio = httpServer + "frtar/hardware/personalVideo";
        private const string httpFileDefaultUrl = httpServer + "";


        private string GetMsgFromHttpByPost(string url, Dictionary<string, string> param)
        {
            try
            {
                string content = HttpSimpleMgr.HttpGetStrContentByPost(url, param);
                return content;
            }
            catch (NullReferenceException e)
            {
                LogAgent.LogError(e.ToString());
                return string.Empty;
            }
        }

        #endregion

        #region 文件相关

        private string dirPath = "";
        private string curPlayPath = "";
        private string publicVedioDirName = "";
        /// <summary>
        /// 公共视频列表
        /// </summary>
        private List<string> publicVedioList = new List<string>();
        private int publicVedioIndex = 0;

        private string CheckLocalFileExist(string url)
        {
            string path = System.IO.Path.Combine(dirPath, url.Replace(httpFileDefaultUrl, ""));
            if (File.Exists(path))
            {
                return path;
            }
            return string.Empty;
        }

        #endregion
    }
}
