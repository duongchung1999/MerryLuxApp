using MerryTest.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using MerryDllFramework;

namespace Common_V1
{
    public partial class Box : Form
    {
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);//设置此窗体为活动窗体

        public WindowControl win =  new WindowControl();
        ~Box()
        {

        }
        public Box(string msg)
        {
   
            InitializeComponent();
       
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Box_KeyDown);
            label1.Text = msg;
        }
        int testID;
        public Box(int testID, string msg)
        {
       
            InitializeComponent();
     
            CheckForIllegalCrossThreadCalls = false;
            label1.Text = $"Số hiệu 线程ID：{testID}\r\n{msg}";
            YES.Text = $"是\r\n(Ctrl+{testID})";
            NO.Text = $"否\r\n(F{testID})";
            _key = (Keys)(0x60 + testID);
            Text = $"Messagebox：{testID}";
            this.testID = testID;
            if (uias == null)
            {
                uias = new UIAdaptiveSize
                {
                    Width = Width,
                    Height = Height,
                    FormsName = this.Text,
                    X = Width,
                };
                uias.SetInitSize(this);
            }
            Thread t = new Thread(SendKeyToWindow);
            t.Start();
            MerryDllFramework.MerryDll.k_hook.KeyDownEvent += new KeyEventHandler(hook_KeyDown);//钩住键按下 
        }
        Keys _key = Keys.Control;
        private void hook_KeyDown(object sender, KeyEventArgs e)
        {

            Console.WriteLine($"{e.KeyValue}");

            //if (e.KeyValue == (int)_key && (int)Control.ModifierKeys == (int)Keys.Alt)
            //{
            //    NO_Click(null, null);
            //}
            if (e.KeyValue == 111 + testID)
            {
                NO_Click(null, null);
            }

            if (e.KeyValue == (int)_key && (int)Control.ModifierKeys == (int)Keys.Control)
            {
                YES_Click(null, null);
            }

        }
        public UIAdaptiveSize uias;

        private async void Box_Load(object sender, EventArgs e)
        {
            await Task.Run(() => Thread.Sleep(100));
            SetForegroundWindow(Handle);
            this.TopMost = true;
            YES.Focus();
        }

        private void Box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.N) NO_Click(sender, e);
            if (e.KeyCode == Keys.Y) YES_Click(sender, e);

        }


        private void YES_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();

        }

        private void NO_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void Box_FormClosed(object sender, FormClosedEventArgs e)
        {
            MerryDllFramework.MerryDll.k_hook.KeyDownEvent -= new KeyEventHandler(hook_KeyDown);//钩住键按下 

        }
        public void SendKeyToWindow()
        {
            Thread.Sleep(500);
            string a=  win.SendKeyToWindow("MoreTest", false, "1", out string text).ToString();

        }

    }
}
