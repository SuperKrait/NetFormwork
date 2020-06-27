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
            label1.Hide();
        }

        public Action<string> resisnHandler;
        private bool isPress = false;

        private void btnRegisn_Click(object sender, EventArgs e)
        {
            if(!isPress)
                if (resisnHandler != null)
                {
                    isPress = true;
                    btnRegisn.Enabled = false;
                    resisnHandler(textBoxRegisnCode.Text);
                }
        }

        public void GetResultFalse(string content)
        {
            MethodInvoker mi = new MethodInvoker(() =>
            {
                isPress = true;
                btnRegisn.Enabled = true;
                label1.Show();
            });
            this.BeginInvoke(mi);
        }
    }
}
