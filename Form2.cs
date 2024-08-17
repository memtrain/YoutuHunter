using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoLibrary;

namespace YoutuHunter
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        delegate void DegDataCell(DataGridViewCell obj, string r);
        delegate void DegBtn(Button btn, string content, bool status);

        void DBtn(Button btn, string content, bool status)
        {
            btn.Text = content;
            btn.Enabled = status;
        }
        void DDataCell(DataGridViewCell obj, string r)
        {
            obj.Value = r;
        }
        async Task GetUrlAndVideoName(DataGridViewCell importUrlCell, DataGridViewCell exportNameCell)
        {
            var yt = YouTube.Default;
            video = await yt.GetVideoAsync(importUrlCell.Value.ToString());
            if (InvokeRequired)
            {
                DegDataCell degDataCell = new DegDataCell(DDataCell);
                DegBtn degBtn = new DegBtn(DBtn);
                Invoke(degDataCell, exportNameCell, video.FullName);
                Invoke(degBtn, button5, "送出", true);

                for (int a = 0; a < dataGridView2.Rows.Count; a++)
                {
                    if (video.FullName.Contains($"{dataGridView2.Rows[a].Cells[0].Value}"))
                    {
                        dataGridView2.Rows[a].Cells[0].Style.BackColor = Color.Pink;
                        dataGridView2.Rows[a].Cells[0].Style.SelectionBackColor = Color.Red;
                        importUrlCell.Style.BackColor = Color.Pink;
                        importUrlCell.Style.SelectionBackColor = Color.Red;
                        exportNameCell.Style.BackColor = Color.Pink;
                        exportNameCell.Style.SelectionBackColor = Color.Red;
                        //exportNameCell.Style.BackColor = Color.Pink;
                        //exportNameCell.Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dataGridView2.Rows[a].Cells[0].Style.BackColor = Color.FromArgb(189, 223, 251);
                        dataGridView2.Rows[a].Cells[0].Style.SelectionBackColor = Color.FromArgb(107, 185, 246);
                    }
                }
            }
        }

        internal string urls;
        YouTubeVideo video;

        private void Form2_Load(object sender, EventArgs e)
        {
            this.TopMost = Form1.isTopMost;
        }


        private static readonly Regex sWhitespace = new Regex(@"\s+");
        public static string ReplaceWhitespace(string input, string replacement)
        {
            return sWhitespace.Replace(input, replacement);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int a = 0; a < dataGridView1.Rows.Count ; a++)
            {
                if (textBox1.Text.Contains($"{dataGridView1.Rows[a].Cells[1].Value}"))
                {
                    textBox1.Text = "";
                    MessageBox.Show("連結重複", "連結重複", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                if (!textBox1.Text.Contains("https://www.youtube.com/"))
                {
                    textBox1.Text = "";
                    MessageBox.Show("連結疑似無效", "無效", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                dataGridView1.Rows.Add();
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value = DateTime.Now.ToString();
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1].Value = textBox1.Text;
                button5.Enabled = false;
                button5.Text = "辨識中....";
                //await Task.Run(GetUrlAndVideoName(dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1], dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2]);

                var result = Task.Run(async () =>
                {
                    await GetUrlAndVideoName(dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1], dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2]);
                });

                result.Wait(30);
                //result.ContinueWith(100);
                //guna2Button2.Enabled = true;

                label2.Text = "清單數量: " + dataGridView1.Rows.Count;
                if (checkBox3.Checked)
                {
                    textBox1.Text = "";
                }

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
                return;
            if (dataGridView1.CurrentCell.RowIndex == -1)
                return;
            dataGridView1.Rows.RemoveAt(dataGridView1.CurrentCell.RowIndex);
            label2.Text = "清單數量: " + dataGridView1.Rows.Count;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show($"列表無任何項目", "列表", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            urls = "";
            for (int a = 0; a < dataGridView1.Rows.Count; a++)
            {
                urls += dataGridView1.Rows[a].Cells[1].Value.ToString() + ";";
            }
            string[] sc = urls.Split(';');

            if (checkBox5.Checked)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                if (MessageBox.Show($"確定下載 {urls} 項目，共 {sc.Length - 1} 個項目", "列表", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    DialogResult = DialogResult.OK;
                };
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
                return;
            if (dataGridView2.CurrentCell.RowIndex == -1)
                return;
            dataGridView2.Rows.RemoveAt(dataGridView2.CurrentCell.RowIndex);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "*.mp3 (音訊檔)|*.mp3|*.mp4 (影訊檔)|*.mp4";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (var all in ofd.FileNames)
                {
                    dataGridView2.Rows.Add(Path.GetFileNameWithoutExtension(all));
                }

                label4.Text = $"例外數量: {dataGridView2.Rows.Count}";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            label2.Text = "清單數量: " + dataGridView1.Rows.Count;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string path = Application.StartupPath + "/export.hlt";
            if (!File.Exists(path))
            {
                File.CreateText(path);
                Complete complete = new Complete();
                complete.ShowDialog();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "*.hlt (Hunter List Text)|*.hlt";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    for (int a = 0; a < dataGridView1.Rows.Count; a++)
                    {
                        sw.WriteLine(dataGridView1.Rows[a].Cells[1].Value.ToString() + ";");
                    }
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.hlt (Hunter List Text)|*.hlt";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader sw = new StreamReader(ofd.FileName))
                {
                    var ds = sw.ReadToEnd();
                    string[] sds = ds.Split(';');
                    if (MessageBox.Show($"總共 {sds.Length - 1} 項，確認匯入!! (請自行檢查腳本格式是否完整或正確~~!!)", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        for (int a = 0; a < sds.Length - 1; a++)
                        {
                            sds[a] = Regex.Replace(sds[a], @"\s+", "");
                            textBox1.Text = sds[a];
                            button1_Click(sender, e);
                        }
                    }
                }
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                button1_Click(sender, e);
            }
        }
    }
}
