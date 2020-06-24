using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaPlayerCtl
{
    public partial class BgFrom : Form
    {
        public BgFrom()
        {
            InitializeComponent();
            MainMgr main = new MainMgr();
            if (main.Init())
            {
                main.MainProgress();
            }
        }
    }
}
