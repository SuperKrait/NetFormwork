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
    public partial class MPForm : Form
    {
        public MPForm()
        {
            InitializeComponent();
        }

        public AxWMPLib.AxWindowsMediaPlayer GetPlayer()
        {
            return axWindowsMediaPlayer1;
        }
    }
}
