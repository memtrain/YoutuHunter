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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        internal bool isInitShow = true;

        private void Form3_Load(object sender, EventArgs e)
        {
            this.TopMost = Form1.isTopMost;
            if (!isInitShow)
            {
                button1.Visible = false;
                button3.Visible = false;
                button2.Visible = true;
            }
            else
            {
                button1.Visible = true;
                button3.Visible = true;
                button2.Visible = false;
            }
            button1.DialogResult = DialogResult.No;
            button3.DialogResult = DialogResult.Yes;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
