using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Theme_Editor
{
    public partial class SaveForm : Form
    {
        public string Name;

        public SaveForm()
        {
            Name = "";
            InitializeComponent();
        }

        private void btn_SaveOK_Click(object sender, EventArgs e)
        {
            Name = txt_SaveName.Text;
        }
    }
}
