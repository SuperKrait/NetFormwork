using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerCtl
{
    class UICtrl
    {
        public readonly static UICtrl Instance = new UICtrl();

        public DLForm dlPanel;
        public MainForm mainPanel;
        public MPForm mpPanel;
        public RegisnForm rgPanel;


        public RegisnForm OpenRegisnPanel()
        {
            rgPanel.Show();
            return rgPanel;
        }

        public void CloseRegisnPanel()
        {
            rgPanel.Hide();
        }


        public MainForm OpenMainPanel()
        {
            mainPanel.Show();
            return mainPanel;
        }

        public void CloseMainPanel()
        {
            mainPanel.Hide();
        }


        public DLForm OpenDownloadPanel()
        {
            dlPanel.Show();
            return dlPanel;
        }

        public void CloseDownloadPanel()
        {
            dlPanel.Hide();
        }

        public MPForm OpenMPPanel()
        {
            mpPanel.Show();
            return mpPanel;
        }

        public void CloseMPPanel()
        {
            mpPanel.Hide();
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
