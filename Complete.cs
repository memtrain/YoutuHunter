using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YoutuHunter
{
    public partial class Complete : Form
    {
        public Complete()
        {
            InitializeComponent();
        }

        private void Complete_Load(object sender, EventArgs e)
        {
            TopMost = Form1.isTopMost;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
