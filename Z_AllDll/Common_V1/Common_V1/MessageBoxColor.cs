using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Common_V1
{
    public partial class MessageBoxColor : Form
    {
        int i;
        string mess;
        string val;
        public MessageBoxColor(string Message,string value)
        {
            InitializeComponent();
            mess = Message;
            val = value;
        }
        public int IValue
        {
            get { return i; }
        }
        private void MessageBoxColor_Load(object sender, EventArgs e)
        {
            if (val == "RED")
            {
                this.BackColor = Color.Red;
            }
            else if(val == "GREEN")
            {
                this.BackColor = Color.Green;
            }
            else if (val == "BLUE")
            {
                this.BackColor = Color.Blue;
            }
            else if (val == "WHITE")
            {
                this.BackColor = Color.White;
            }
            else if (val == "ORANGE")
            {
                this.BackColor = Color.Orange;
            }
            this.WindowState = FormWindowState.Maximized;
            DialogResult result = MessageBox.Show(mess, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                i = 1;
               
            }
            else
            {
                i = 0;
               
              
            }
            // Kiểm tra kết quả

        }
    }
}
