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
    public partial class RegisnForm : Form
    {
        public RegisnForm()
        {
            InitializeComponent();
        }

        public Action<string> resisnHandler;
        private bool isPress = false;

        private void btnRegisn_Click(object sender, EventArgs e)
        {
            if(!isPress)
                if (resisnHandler != null)
                {
                    isPress = true;
                    resisnHandler(textBoxRegisnCode.Text);
                }
        }

        public void GetResultFalse()
        {
            isPress = true;
        }
    }
}
