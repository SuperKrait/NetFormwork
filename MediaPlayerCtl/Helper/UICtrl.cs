using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaPlayerCtl
{
    class UICtrl
    {
        public readonly static UICtrl Instance = new UICtrl();

        public DLForm dlPanel;
        public MainForm mainPanel;
        public MPForm mpPanel;
        public RegisnForm rgPanel;

        public Form curForm = null;

        public void Init()
        {
            dlPanel = new DLForm();
            mainPanel = new MainForm();
            mpPanel = new MPForm();
            rgPanel = new RegisnForm();
        }


        public RegisnForm OpenRegisnPanel()
        {
            if (curForm != null)
                curForm.Hide();
            curForm = rgPanel;

            rgPanel.Show();
            return rgPanel;
        }



        public MainForm OpenMainPanel()
        {
            if (curForm != null)
                curForm.Hide();
            curForm = mainPanel;

            mainPanel.Show();
            mainPanel.PlayVedio();
            return mainPanel;
        }


        public DLForm OpenDownloadPanel()
        {
            if (curForm != null)
                curForm.Hide();
            curForm = dlPanel;

            dlPanel.Show();
            return dlPanel;
        }

        public MPForm OpenMPPanel()
        {
            if (curForm != null)
                curForm.Hide();
            curForm = mpPanel;

            mpPanel.Show();
            return mpPanel;
        }

        public void Destory()
        {
            mpPanel.Close();
            dlPanel.Close();
            rgPanel.Close();
            mainPanel.Close();
        }
    }
}
