using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace capture
{
    public partial class frmClose : Form
    {
        public frmClose()
        {
            InitializeComponent();
        }
        RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "lemontree"){

                reg.DeleteValue("capture");
                Application.Exit();
            }
            else
            {
                textBox1.SelectAll();
                textBox1.Focus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void frmClose_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
