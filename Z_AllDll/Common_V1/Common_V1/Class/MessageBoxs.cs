using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Common_V1
{
    /// <summary>
    /// 弹窗
    /// </summary>
    public partial class MessageBoxs : Form
    {
        #region 对外部分
        public static bool BarCodeBox(string message, int length, out string barcode)
        {
            MessageBoxs messageboxss = new MessageBoxs();
            messageboxss.message = message;
            messageboxss.length = length;
            messageboxss.uploadmes = true;
            messageboxss.BarCode_textBox.Visible = true;
            messageboxss.ShowDialog();
            var result = messageboxss.DialogResult;//先关闭会获取不到值
            barcode = messageboxss.barcode;//先关闭会获取不到值
            messageboxss.Dispose();
            return result == DialogResult.OK;
        }

        #endregion

        #region 窗体部分


        /// <summary>
        /// 窗体名
        /// </summary>
        string names = "";
        /// <summary>
        /// 窗体信息
        /// </summary>
        string message = "";
        /// <summary>
        /// 窗体条码框长度
        /// </summary>
        int length = 0;
        /// <summary>
        /// 条码框输入值
        /// </summary>
        string barcode = "";
        /// <summary>
        /// 窗体颜色
        /// </summary>
        Color color = Color.Transparent;
        /// <summary>
        /// 不良SN
        /// </summary>
        string SN = "";
        /// <summary>
        /// 是否Y,N检测按键
        /// </summary>
        bool uploadmes = false;
        /// <summary>
        /// 构造函数
        /// </summary>
        MessageBoxs()
        {
            InitializeComponent();

        }
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);//设置此窗体为活动窗体
        private async void messageBox_Load(object sender, EventArgs e)
        {
            Text = names;
            Message_label.Text = message;
            await Task.Run(() => Thread.Sleep(100));
            SetForegroundWindow(Handle);
            this.TopMost = true;
            TopLevel = true;
            TopMost = true;

        }
        private void button1_Click(object sender, EventArgs e)
        {


            DialogResult = DialogResult.OK;
            Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            if (BarCode_textBox.Text.Length != length)
            {
                System.Windows.Forms.MessageBox.Show($"扫码条码长度{BarCode_textBox.Text.Length} != {length}");
                return;
            }
            barcode = BarCode_textBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void MessageBoxs_KeyDown(object sender, KeyEventArgs e)
        {
            if (uploadmes) return;
            if (e.KeyCode == Keys.N)
            {
                button2_Click(null, null);
                return;
            }
            if (e.KeyCode == Keys.Y)
            {
                button1_Click(null, null);
                return;
            }

        }
        #endregion

    }
}

