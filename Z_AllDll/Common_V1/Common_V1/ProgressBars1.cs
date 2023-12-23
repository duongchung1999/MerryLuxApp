using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using NAudio.CoreAudioApi;

namespace MerryTestFramework.testitem.Forms
{
    internal class ProgressBars1 : Form
    {
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);//设置此窗体为活动窗体
        private int i = 0;
        private IContainer components = null;
        internal ProgressBar progressBar1;

        internal System.Windows.Forms.Timer timer1;

        private Label label1;

        public ProgressBars1(string name)
        {
            InitializeComponent();
            Text = name;
            label1.Text = name;
        }

        private void ProgressBars_Load(object sender, EventArgs e)
        {
            i = 0;
            timer1.Interval = 200;
            SetForegroundWindow(Handle);
            this.TopMost = true;
            timer1.Enabled = true;
        }

        private void SetSysVolume(int volumeLevel)
        {
            try
            {
                MMDeviceEnumerator mMDeviceEnumerator = new MMDeviceEnumerator();
                MMDevice defaultAudioEndpoint = mMDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                defaultAudioEndpoint.AudioEndpointVolume.MasterVolumeLevelScalar = (float)volumeLevel / 100f;
            }
            catch (Exception)
            {
            }
        }

        private int GetSysVolume()
        {
            int result = 0;
            try
            {
                MMDeviceEnumerator mMDeviceEnumerator = new MMDeviceEnumerator();
                MMDevice defaultAudioEndpoint = mMDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                result = (int)(defaultAudioEndpoint.AudioEndpointVolume.MasterVolumeLevelScalar * 100f);
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        public void VolumeUpTest()
        {
            timer1.Enabled = false;
            Thread.Sleep(500);
            SetSysVolume(1);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int num = 1;
            while (true)
            {
                double num2 = (double)stopwatch.ElapsedMilliseconds / 1000.0;
                if (num2 > 15.0)
                {
                    base.DialogResult = DialogResult.No;
                    Close();
                    return;
                }

                int num3 = GetSysVolume() + 1;
                if (num > num3)
                {
                    base.DialogResult = DialogResult.No;
                    Close();
                    return;
                }

                num = num3;
                if (num3 >= 20)
                {
                    break;
                }

                progressBar1.Value = num3 * 5;
                Application.DoEvents();
                Thread.Sleep(20);
            }

            base.DialogResult = DialogResult.Yes;
            Close();
        }

        public void VolumeDownTest()
        {
            timer1.Enabled = false;
            SetSysVolume(99);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int num = 99;
            while (true)
            {
                double num2 = (double)stopwatch.ElapsedMilliseconds / 1000.0;
                if (num2 > 15.0)
                {
                    base.DialogResult = DialogResult.No;
                    Close();
                    return;
                }

                int sysVolume = GetSysVolume();
                if (num < sysVolume)
                {
                    base.DialogResult = DialogResult.No;
                    Close();
                    return;
                }

                num = sysVolume;
                if (100 - sysVolume >= 20)
                {
                    break;
                }

                progressBar1.Value = (100 - sysVolume) * 5;
                Application.DoEvents();
                Thread.Sleep(20);
            }

            base.DialogResult = DialogResult.Yes;
            Close();
        }

        public void VolumeUpTestNO()
        {
            timer1.Enabled = false;
            Thread.Sleep(500);
            SetSysVolume(1);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int num = 1;
            while (true)
            {
                double num2 = (double)stopwatch.ElapsedMilliseconds / 1000.0;
                if (num2 > 15.0)
                {
                    base.DialogResult = DialogResult.No;
                    Close();
                    return;
                }

                int num3 = GetSysVolume() + 1;
                num = num3;
                if (num3 >= 20)
                {
                    break;
                }

                progressBar1.Value = num3 * 5;
                Application.DoEvents();
                Thread.Sleep(20);
            }

            base.DialogResult = DialogResult.Yes;
            Close();
        }

        public void VolumeDownTestNO()
        {
            timer1.Enabled = false;
            SetSysVolume(99);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int num = 99;
            while (true)
            {
                double num2 = (double)stopwatch.ElapsedMilliseconds / 1000.0;
                if (num2 > 15.0)
                {
                    base.DialogResult = DialogResult.No;
                    Close();
                    return;
                }

                int sysVolume = GetSysVolume();
                num = sysVolume;
                if (100 - sysVolume >= 20)
                {
                    break;
                }

                progressBar1.Value = (100 - sysVolume) * 5;
                Application.DoEvents();
                Thread.Sleep(20);
            }

            base.DialogResult = DialogResult.Yes;
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            i += 1;
            progressBar1.Value = i;
            if (i >= 100)
            {
                base.DialogResult = DialogResult.No;
                timer1.Enabled = false;
                Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(2, 199);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(852, 95);
            this.progressBar1.TabIndex = 2;
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("NSimSun", 18.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(28, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(822, 191);
            this.label1.TabIndex = 3;
            this.label1.Text = "label1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ProgressBars1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 304);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ProgressBars1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ProgressBars";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ProgressBars_Load);
            this.ResumeLayout(false);

        }
    }
}