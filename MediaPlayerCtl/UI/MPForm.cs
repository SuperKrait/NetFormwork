﻿using System;
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
        public MPHelper playerHelper;
        public MPForm()
        {
            InitializeComponent();
            playerHelper = new MPHelper();
            playerHelper.player = axWindowsMediaPlayer1;
        }
    }
}
