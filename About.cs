using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YoutuHunter
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            TopMost = Form1.isTopMost;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/paypalme/81dooms42/1");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
