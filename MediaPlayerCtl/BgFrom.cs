using Common.TaskPool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace MediaPlayerCtl
{
    public partial class BgFrom : Form
    {
        private MainMgr main;
        private bool isMainStart = true;

        public BgFrom()
        {
            InitializeComponent();
            main = new MainMgr(AddMainAction);

            TaskPoolHelper.Instance.StartALongTask(()=>
            {
                while (isMainStart)
                {
                    Thread.Sleep(1);
                    ExcuteMainAction();
                }
            });

            if (main.Init())
            {
                main.CheckPublicVedio();
                main.MainProgress();
            }
            else
            {
                main.SetStartAppHandle(StartApp);
            }
        }

        private List<Action> actionList = new List<Action>();
        private void AddMainAction(Action action)
        {
            lock (actionList)
            {
                actionList.Add(action);
            }
        }

        private void ExcuteMainAction()
        {
            lock(actionList)
                while(actionList.Count > 0)
                {
                    if (actionList[0] != null)
                    {
                        actionList[0]();
                    }
                    actionList.RemoveAt(0);
                }
        }


        private void StartApp()
        {
            MethodInvoker mi = new MethodInvoker(() =>
            {
                main.SetRegisnResult(1);
            });
            this.BeginInvoke(mi);
        }

    }
}
