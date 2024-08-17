using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YoutuHunter
{
    public partial class ExportHistory : Form
    {
        public ExportHistory()
        {
            InitializeComponent();
        }

        //string[] sped;
        //string[] spiL;
        string tempSped;
        string tempSpiL;
        string tempSpes;
        int invalidPathNum;
        int listNum;
        private void ExportHistory_Load(object sender, EventArgs e)
        {
            this.TopMost = Form1.isTopMost;

            var speh = Properties.Settings.Default.ExportHistroy.Split(';');
            var sped = Properties.Settings.Default.ExportDate.Split(';');
            var spes = Properties.Settings.Default.ExportSize.Split(';');
            var spiL = Properties.Settings.Default.ImportURL.Split(';');


            for (int a = 0; a < speh.Length - 1; a++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[a].Cells[2].Value = speh[a];

                try
                {
                    if (string.IsNullOrEmpty(sped[a]))
                    {
                        tempSped += "UNKNOWN;";
                        dataGridView1.Rows[a].Cells[0].Value = "UNKNOWN";
                    }
                    else
                    {
                        dataGridView1.Rows[a].Cells[0].Value = sped[a];
                    }
                }
                catch
                {
                    dataGridView1.Rows[a].Cells[0].Value = "UNKNOWN";
                }
                try
                {
                    if (string.IsNullOrEmpty(spiL[a]))
                    {
                        tempSpiL += "UNKNOWN;";
                        dataGridView1.Rows[a].Cells[1].Value = "UNKNOWN";
                    }
                    else
                    {
                        dataGridView1.Rows[a].Cells[1].Value = spiL[a];
                    }
                }
                catch
                {
                    dataGridView1.Rows[a].Cells[1].Value = "UNKNOWN";
                }
                try
                {
                    if (string.IsNullOrEmpty(spes[a]))
                    {
                        tempSpes += "UNKNOWN;";
                        dataGridView1.Rows[a].Cells[3].Value = "UNKNOWN";
                    }
                    else
                    {
                        dataGridView1.Rows[a].Cells[3].Value = spes[a];
                    }
                }
                catch
                {
                    dataGridView1.Rows[a].Cells[3].Value = "UNKNOWN";
                }
            }

            InvalidPathNumUpdate();
            AllPathNumUpdate();
            /*
            Properties.Settings.Default.ExportDate = tempSped;
            Properties.Settings.Default.ImportURL = tempSpiL;

            Properties.Settings.Default.Save();*/

        }

        void AllPathNumUpdate()
        {
            listNum = 0;
            for (int a = 0; a < dataGridView1.Rows.Count; a++)
            {
                listNum++;
            }
        }

        void InvalidPathNumUpdate()
        {
            invalidPathNum = 0;
            for (int a = 0; a < dataGridView1.Rows.Count - 1; a++)
            {
                if (!string.IsNullOrWhiteSpace(dataGridView1.Rows[a].Cells[2].Value.ToString()) && !File.Exists(dataGridView1.Rows[a].Cells[2].Value.ToString()))
                {
                    invalidPathNum++;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.RowIndex == -1)
                return;
            if (!string.IsNullOrWhiteSpace(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[2].Value.ToString()))
            {
                if (File.Exists(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[2].Value.ToString()))
                {
                    Process.Start("explorer.exe", $"/select, {dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[2].Value.ToString()}");
                }
                else
                {
                    MessageBox.Show("檔案已遺失或重新命名", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.RowIndex == -1)
                return;
            if (!string.IsNullOrWhiteSpace(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[2].Value.ToString()))
            {
                if (File.Exists(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[2].Value.ToString()))
                {
                    Process.Start($"{dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[2].Value.ToString()}");
                }
                else
                {
                    MessageBox.Show("檔案已遺失或重新命名", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.RowIndex == -1)
                return;
            if (!string.IsNullOrWhiteSpace(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value.ToString()))
            {
                Process.Start(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value.ToString());
            }
        }


        int odIndex = 0;
        private void button4_Click(object sender, EventArgs e)
        {
            odIndex = 0;
            Properties.Settings.Default.ExportHistroy = "";
            Properties.Settings.Default.ExportDate = "";
            Properties.Settings.Default.ImportURL = "";
            Properties.Settings.Default.ExportSize = "";

            while (odIndex < dataGridView1.Rows.Count)
            {
                if (!File.Exists(dataGridView1.Rows[odIndex].Cells[2].Value.ToString()))
                {
                    dataGridView1.Rows.RemoveAt(odIndex);
                }
                else
                {
                    odIndex++;
                }
            }

            for (int a = 0; a < dataGridView1.Rows.Count; a++)
            {
                Properties.Settings.Default.ExportHistroy += dataGridView1.Rows[a].Cells[2].Value.ToString() + ";";
                Properties.Settings.Default.ExportDate += dataGridView1.Rows[a].Cells[0].Value.ToString() + ";";
                Properties.Settings.Default.ImportURL += dataGridView1.Rows[a].Cells[1].Value.ToString() + ";";
                Properties.Settings.Default.ExportSize += dataGridView1.Rows[a].Cells[3].Value.ToString() + ";";
            }
            Properties.Settings.Default.Save();

            InvalidPathNumUpdate();
            AllPathNumUpdate();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            Properties.Settings.Default.ExportHistroy = "";
            Properties.Settings.Default.ExportDate = "";
            Properties.Settings.Default.ImportURL = "";
            Properties.Settings.Default.ExportSize = "";
            Properties.Settings.Default.Save();

            InvalidPathNumUpdate();
            AllPathNumUpdate();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
