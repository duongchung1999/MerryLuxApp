using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Common_V1
{
    public partial class ProgressBars : Form
    {

        public List<string> MsgList = new List<string>();
        public bool CountDown_(Func<bool> func, string MSG, string title = "标题", int TimeOut_S = 20)
        {
            Text = title;
            label1.Text = MSG;
            avg = 0;
            Span = timer1.Interval;
            progressBar1.Maximum = TimeOut_S * 1000;
            Max = progressBar1.Maximum - Span;

            bool flag = true;
            Task.Run(() =>
            {
                Thread.Sleep(50);
                while (flag)
                {
                    try
                    {
                        bool Result = func.Invoke();
                        if (IsDisposed || DialogResult == DialogResult.Cancel || DialogResult == DialogResult.No)
                            return;

                        if (Result)
                        {
                            DialogResult = DialogResult.Yes;
                        }
                        Thread.Sleep(150);
                    }
                    catch
                    {
                        return;
                    }

                }
            });
            ShowDialog();
            bool result = DialogResult == DialogResult.Yes;
            flag = false;
            return result;
        }



        #region 窗体部分

        public ProgressBars(string title = "提示")
        {
            InitializeComponent();
            Text = title;

        }
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        static extern bool SetForegroundWindow(IntPtr hWnd);//设置此窗体为活动窗体
        private async void ProgressBars_Load(object sender, EventArgs e)
        {
            await Task.Run(() => Thread.Sleep(100));
            SetForegroundWindow(Handle);
            TopMost = true;
            timer1.Enabled = true;
            timer_log.Enabled = true;

        }
        public int Span = 0;
        public int avg = 0;
        public int Max = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (avg >= Max)
            {
                DialogResult = DialogResult.No;
                this.Close();
            }
            avg += Span;
            progressBar1.Value = avg;
        }
        private void timer_log_Tick(object sender, EventArgs e)
        {
            lock (MsgList)
            {

                if (MsgList.Count <= 0) return;
                foreach (var item in MsgList)
                {
                    tb_log.Text += $"{item}\r\n";
                }
                MsgList.Clear();
                this.tb_log.SelectionStart = this.tb_log.Text.Length;
                this.tb_log.ScrollToCaret();


            }
        }



        /// <summary>
        /// 控制窗体
        /// </summary>
        /// <param name="lpClassName">传null</param>
        /// <param name="lpWindowName">需要控制窗体得名字</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        /// <summary>
        /// 结束窗体
        /// </summary>
        /// <param name="hWnd">信息发往的窗口的句柄</param>
        /// <param name="Msg">消息ID ：0x10</param>
        /// <param name="wParam">参数1 ：</param>
        /// <param name="lParam">参数2 ：0</param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        extern static int PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        #endregion

    }
}
