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

namespace MediaPlayerCtl
{
    class MainMgr
    {
        #region 主流程

        private string mCode;
        /// <summary>
        /// 0为初始化完毕等待开始
        /// 1为开始
        /// 2为暂停
        /// 3为销毁中
        /// </summary>
        private int mgrStatus = 0;
        private MPHelper player;

        public bool Init()
        {
            mgrStatus = 1;
            player = new MPHelper();
            player.Init(EndPlayMovie);
            this.mCode = GetMechineCode();
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
            
            UICtrl.Instance.OpenMainPanel();
            StartSendHeart();
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

                UICtrl.Instance.OpenMPPanel();
                player.Play(curPlayPath);

            }

            STOP_PLAY:
            LogAgent.Log("销毁播放了");
        }


        private void EndPlayMovie()
        {
            mgrStatus = 1;
            StartSendHeart();
        }

        private string GetMechineCode()
        {
            return MachineCode.MachineCode.GetMachineCodeString();
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
            this.regisnCode = regisnCode;
            return true;
        }

        private void SetRegisnCode(string str)
        {
            if (str.Length.Equals(22))
            {
                SetRegisnResult(3, string.Empty);
                return;
            }
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] <= 73 && str[i] >= 64)
                {
                    continue;
                }
                if(str[i] <= 81 && str[i] > 106)
                {
                    continue;
                }
                if (str[i] <= 113 && str[i] > 122)
                {
                    continue;
                }
                SetRegisnResult(3, string.Empty);
                return;
            }
            regisnCode = str;
            CheckRegisnFromServerr(str);
        }

        /// <summary>
        /// 反馈注册结果
        /// </summary>
        /// <param name="status">0,为失败,1为成功,2为断网,3为本地输入有误</param>
        private void SetRegisnResult(int status, string id)
        {
            if (!status.Equals(1))
            {
                UICtrl.Instance.rgPanel.GetResultFalse();
                return;
            }
            UICtrl.Instance.CloseRegisnPanel();
            SetRegisignFile2Local(id);

            InitWeb();
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
            param.Add();
            param.Add();
            param.Add();
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
                TaskPoolHelper.Instance.StartNewTask(()=>
                {
                    string content = GetMsgFromHttp(httpHeart, param);

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

                            path = DownloadFile(url);

                            if (!string.IsNullOrEmpty(path))
                            {
                                this.curPlayPath = path;
                                return;
                            }                            

                        }
                        
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

            string content = GetMsgFromHttp(httpTellServerGetDLUrl, param);

            if (string.IsNullOrEmpty(content))
            {
                return false;
            }
            return true;
        }
    

        private string DownloadFile(string url)
        {
            string path = System.IO.Path.Combine(dirPath, url.Substring(url.LastIndexOf('/') + 1));
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
            TaskPoolHelper.Instance.StartNewTask(() =>
            {

            }, "MainMgr->CheckRegisnFromServerr");
        }
        private const string httpServer = "";
        private const string httpRegisn = httpServer + "";
        private const string httpHeart = httpServer + "";
        private const string httpTellServerGetDLUrl = httpServer + "";

        private string GetMsgFromHttp(string url, Dictionary<string, string> param)
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

        private string CheckLocalFileExist(string url)
        {
            string path = System.IO.Path.Combine(dirPath, url.Substring(url.LastIndexOf('/') + 1));
            if (File.Exists(path))
            {
                return path;
            }
            return string.Empty;
        }

        #endregion
    }
}
