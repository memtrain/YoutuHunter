using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoLibrary;
using MediaToolkit;
using YoutubeExplode;

using YoutubeExplode;
using YoutubeExplode.Converter;

namespace YoutuHunter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            timer1.Stop();

            Form3 form3 = new Form3();
            form3.ShowDialog();

            /*
            ValidationParameters validationParameters = new ValidationParameters();
            validationParameters.setProductNumber("PZSACR34E");
            validationParameters.put("M3SWV64HD", "Pay", "man");

            Context context = new Context();
            context.securityMode = SecurityMode.APIKEY_IDENTIFICATION;
            context.apiKey = "78f41426-4a8f-4e4c-a549-ef2f92802673";
            ValidationResult validationResult = LicenseeService.validate(context, "I44KQ3HEF", validationParameters);
            */
        }

        //PerformanceCounterCategory pcg;
        private void Form1_Load(object sender, EventArgs e)
        {
            label15.Text = $"(Wine_配適版) V{Application.ProductVersion}";
            index = 0;
            textBox3.Text = Application.StartupPath;

            checkBox1.Checked = Properties.Settings.Default.Remember;
            if (Properties.Settings.Default.Remember)
            {
                checkBox2.Checked = Properties.Settings.Default.TopMost;
                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.SavePath))
                    textBox3.Text = Properties.Settings.Default.SavePath;
                textBox4.Text = Properties.Settings.Default.YoutuURL;
                comboBox1.SelectedIndex = Properties.Settings.Default.SaveFormatIndex;
            }
            else
            {
                comboBox1.SelectedIndex = index;
            }

            label3.Text = "準備就緒!!";
            label8.Text = "準備就緒!!";
        }

        private delegate void DelShowMessage(Control control, string info);
        private delegate void ProgressShowMessage(int max, int cur);

        float vds;
        bool isMultiDownload = false;
        CancellationTokenSource sc;
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (InvokeRequired) // 若非同執行緒
            {
                DelShowMessage del = new DelShowMessage(Shows); //利用委派執行
                Invoke(del, label3, "計算中....");
                Invoke(del, label8, "等候中....");
            }
            backgroundWorker1.ReportProgress(1);
            downloadIndex = 0;
            sc = new CancellationTokenSource();
            if (isMultiDownload)
            {
                var result = Task.Run(async () =>
                {
                    await RequestToGetInputReport(urls);
                    if (sc.IsCancellationRequested)
                    {
                        MessageBox.Show("用戶已終止下載", "中止");
                    }
                }, sc.Token);
                try
                {
                    result.Wait(sc.Token);
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException)
                    {
                        MessageBox.Show("用戶已終止下載", "中止");
                    }
                }
            }
            else
            {
                var result = Task.Run(async () =>
                {
                    await RequestToGetInputReport("");
                    if (sc.IsCancellationRequested)
                    {
                        MessageBox.Show("用戶已終止下載", "中止");
                    }
                }, sc.Token);
                try
                {
                    result.Wait(sc.Token);
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException)
                    {
                        MessageBox.Show("用戶已終止下載", "中止");
                    }
                }
            }
        }

        public void Shows(Control control, string info)
        {
            control.Text = "" + info;
        }
        public void Shows(int maxValue, int value)
        {
            progressBar2.Maximum = maxValue;
            progressBar2.Value = value;
        }

        string curExportPath;
        int downloadIndex = 0;
        int downloadMaxIndex = 0;

        private async Task RequestToGetInputReport(string multiUrls = "")
        {
            var youtube = new YoutubeClient();
            //var yt = YouTube.Default;
            if (multiUrls != "")
            {
                if (InvokeRequired) // 若非同執行緒
                {
                    if (!sc.IsCancellationRequested)
                    {
                        DelShowMessage del = new DelShowMessage(Shows); //利用委派執行
                        ProgressShowMessage pros = new ProgressShowMessage(Shows); //利用委派執行

                        Invoke(del, label3, "分割中....");
                        Invoke(del, label8, "等待處裡中....");
                        backgroundWorker1.ReportProgress(20);
                    }
                    else
                    {
                        return;  //若收到"取消"消息，則回傳return
                    }
                }
                var d = multiUrls.Split(';');
                downloadMaxIndex = d.Length - 1;

                await youtube.Videos.DownloadAsync("https://youtube.com/watch?v=u_yIGGhubZs", "video.mp4", o => o
                    .SetFormat("mp3") // override format
                    .SetPreset(ConversionPreset.UltraFast) // change preset
                );
            }
            else
            {
                await youtube.Videos.DownloadAsync("https://youtube.com/watch?v=u_yIGGhubZs", "video.mp4", o => o
                    .SetFormat("mp3") // override format
                    .SetPreset(ConversionPreset.UltraFast) // change preset
                );
            }
            //vds = video.GetBytes().Length / 1024f / 1024f;
            if (InvokeRequired) // 若非同執行緒
            {
                if (!sc.IsCancellationRequested)
                {
                    DelShowMessage del = new DelShowMessage(Shows); //利用委派執行
                    ProgressShowMessage pros = new ProgressShowMessage(Shows); //利用委派執行

                    Invoke(pros, downloadMaxIndex, downloadIndex);

                    Invoke(del, label3, string.Format("{0:0.00}", vds) + " MB");
                    Invoke(del, label8, "下載中....");
                    backgroundWorker1.ReportProgress(50);
                }
                else
                {
                    return;  //若收到"取消"消息，則回傳return
                }
            }
            //File.WriteAllBytes(textBox3.Text + @"\" + video.FullName, await video.GetBytesAsync());
            //curExportPath = textBox3.Text + @"\" + video.FullName;

            if (index == 1 || index == 2)
            {
                if (InvokeRequired) // 若非同執行緒
                {
                    if (!sc.IsCancellationRequested)
                    {
                        DelShowMessage del = new DelShowMessage(Shows); //利用委派執行
                        Invoke(del, label3, "預計大小為" + string.Format("{0:0.0}", vds / 3) + " MB");
                        Invoke(del, label8, "轉檔中....");
                        backgroundWorker1.ReportProgress(70);
                    }
                    else
                    {
                            return;  //若收到"取消"消息，則回傳return
                    }
                }

                // var inputfile = new MediaToolkit.Model.MediaFile { Filename = textBox3.Text + @"\" + video.FullName };
                //var outputfile = new MediaToolkit.Model.MediaFile { Filename = textBox3.Text + @"\" + video.FullName + ".mp3" };
                //Mp3Convert(inputfile, outputfile); //轉為MP3

                //if (index == 2)
                //File.Delete(textBox3.Text + @"\" + video.FullName);
            }

            if (multiUrls != "")
            {
                if (index == 0 || index == 1)
                {
                    Properties.Settings.Default.ExportHistroy += curExportPath + ";";
                    Properties.Settings.Default.ImportURL += $"{multiUrls.Split(';')[downloadIndex]}" + ";";
                    Properties.Settings.Default.ExportDate += $"{DateTime.Now}" + ";";
                    Properties.Settings.Default.ExportSize += $"約 {string.Format("{0:0.00}", vds)} MB" + ";";
                }
                if (index == 1 || index == 2)
                {
                    Properties.Settings.Default.ExportHistroy += curExportPath + ".mp3;";
                    Properties.Settings.Default.ImportURL += $"{multiUrls.Split(';')[downloadIndex]}" + ";";
                    Properties.Settings.Default.ExportDate += $"{DateTime.Now}" + ";";
                    Properties.Settings.Default.ExportSize += $"約 {string.Format("{0:0.00}", vds / 3)} MB" + ";";
                }
                Properties.Settings.Default.Save();
                if (sc.IsCancellationRequested)
                {
                    return;
                }
                downloadIndex++;
                if (downloadIndex < downloadMaxIndex)
                {
                    await Task.Run(async () => await RequestToGetInputReport(multiUrls));
                }
            }
            else
            {
                if (index == 0 || index == 1)
                {
                    Properties.Settings.Default.ExportHistroy += curExportPath + ";";
                    Properties.Settings.Default.ImportURL += $"{textBox4.Text}" + ";";
                    Properties.Settings.Default.ExportDate += $"{DateTime.Now}" + ";";
                    Properties.Settings.Default.ExportSize += $"約 {string.Format("{0:0.00}", vds)} MB" + ";";
                }
                if (index == 1 || index == 2)
                {
                    Properties.Settings.Default.ExportHistroy += curExportPath + ".mp3;";
                    Properties.Settings.Default.ImportURL += $"{textBox4.Text}" + ";";
                    Properties.Settings.Default.ExportDate += $"{DateTime.Now}" + ";";
                    Properties.Settings.Default.ExportSize += $"約 {string.Format("{0:0.00}", vds / 3)} MB" + ";";
                }
                Properties.Settings.Default.Save();
            }

        }

        void Mp3Convert(MediaToolkit.Model.MediaFile inputfile, MediaToolkit.Model.MediaFile outputfile)
        {
            using (var enging = new Engine())
            {
                enging.GetMetadata(inputfile);
                enging.Convert(inputfile, outputfile);
            }
        }

        void Mp3Convert(string inputPath, string outputPath)
        {
            var inputfile = new MediaToolkit.Model.MediaFile { Filename = inputPath };
            var outputfile = new MediaToolkit.Model.MediaFile { Filename = outputPath };
            using (var enging = new Engine())
            {
                enging.GetMetadata(inputfile);
                enging.Convert(inputfile, outputfile);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button4.Text = backgroundWorker1.IsBusy ? "中止下載" : "開始下載";

            notifyIcon1.ShowBalloonTip(5000);
            timer1.Stop();
            el_time_sec = 0;
            el_time_min = 0;
            el_time_hour = 0;
            label3.Text = "準備就緒!!";
            label8.Text = "準備就緒!!";
            progressBar1.Value = progressBar1.Maximum;
            progressBar2.Value = progressBar2.Maximum;
            button4.Enabled = true;
            button5.Enabled = true;
            textBox4.Enabled = true;
            Complete complete = new Complete();
            complete.ShowDialog();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
        }
        
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        string urls;
        private void guna2Button4_Click(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!textBox4.Text.Contains("https://www.youtube.com/"))
            {
                MessageBox.Show("連結疑似無效", "無效", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox4.Text = "";
                return;
            }
            Properties.Settings.Default.YoutuURL = textBox4.Text;
            Properties.Settings.Default.Save();
        }

        internal static bool isTopMost;
        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Properties.Settings.Default.TopMost = checkBox2.Checked;
                Properties.Settings.Default.Save();
            }
            isTopMost = checkBox2.Checked;
            this.TopMost = isTopMost;
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {

        }

        private void guna2CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Remember = checkBox1.Checked;
            Properties.Settings.Default.Save();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {

        }

       // private PerformanceCounter mCounter;//當前計數器
        void StartNetM()
        {
            //初始化Counter
            //PerformanceCounterCategory pcCategory = new PerformanceCounterCategory("Network Interface");
            //string[] iNames = pcCategory.GetInstanceNames();//此函數第一次執行特別耗時（不知道爲什麼）
            //PerformanceCounter[] pCounters = pcCategory.GetCounters(iNames[1]);//iNames[0]爲"ASIX AX88772C USB2.0 to Fast Ethernet Adapter"；iNames[1]爲"Intel[R] Ethernet Connection [7] I219-V"
            //mCounter = pCounters[0];
            //mCounter.NextValue();//初始值 
        }

        int el_time_sec;
        int el_time_min;
        int el_time_hour;

        private void timer1_Tick(object sender, EventArgs e)
        {
            /*
            double SpeedKbps = mCounter.NextValue() * 8 / 1000;
            if ((SpeedKbps / 1000) > 1)
            {
                label15.Text = String.Format("{0:f1} Mbps", SpeedKbps / 1000); //得到該適配器的上傳速度
            }
            else
            {
                label15.Text = String.Format("{0:f1} Kbps", SpeedKbps); //得到該適配器的上傳速度
            }*/

            el_time_sec++;
            if (el_time_sec >= 60)
            {
                el_time_min++;
                el_time_sec = 0;
            }
            if (el_time_min >= 60)
            {
                el_time_hour++;
                el_time_min = 0;
                el_time_sec = 0;
            }
            label14.Text = $"經過時間: {string.Format("{0:00}", el_time_hour)}:{string.Format("{0:00}", el_time_min)}:{string.Format("{0:00}", el_time_sec)}";
            //Console.WriteLine("Bytes Received: {0}", pcreceived.NextValue() / 1024);
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fdb = new FolderBrowserDialog();
            if (fdb.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = fdb.SelectedPath;
                Properties.Settings.Default.SavePath = textBox3.Text;
                Properties.Settings.Default.Save();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                sc.Cancel();

                backgroundWorker1.CancelAsync();
                button4.Text = backgroundWorker1.IsBusy ? "中止下載" : "開始下載";
                return;
            }
            isMultiDownload = false;
            if (!string.IsNullOrWhiteSpace(textBox3.Text) && !string.IsNullOrWhiteSpace(textBox4.Text))
            {
                if (!backgroundWorker1.IsBusy)
                {
                    StartNetM();
                    timer1.Start();

                    button5.Enabled = false;
                    textBox4.Enabled = false;
                    backgroundWorker1.RunWorkerAsync();
                }
                button4.Text = backgroundWorker1.IsBusy ? "中止下載" : "開始下載";
            }
            else
            {
                MessageBox.Show("所有欄位必填", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            if (form2.ShowDialog() == DialogResult.OK)
            {
                isMultiDownload = true;
                urls = form2.urls;

                if (!backgroundWorker1.IsBusy)
                {
                    StartNetM();
                    timer1.Start();

                    button5.Enabled = false;
                    textBox4.Enabled = false;

                    backgroundWorker1.RunWorkerAsync();
                }

                button4.Text = backgroundWorker1.IsBusy ? "中止下載" : "開始下載";
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.isInitShow = false;
            form3.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ExportHistory exportHistory = new ExportHistory();
            exportHistory.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        int index = 0;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = comboBox1.SelectedIndex;
            Properties.Settings.Default.SaveFormatIndex = comboBox1.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text))
                return;
            if (File.Exists(textBox3.Text))
            {
                Mp3Convert(textBox3.Text, $"{textBox3.Text}.mp3");
            }
            else
            {
                return;
            }
        }
    }
}
