namespace Common_V1
{
    partial class MessageBoxs
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageBoxs));
            this.Message_label = new System.Windows.Forms.Label();
            this.BarCode_textBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Message_label
            // 
            this.Message_label.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Message_label.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Message_label.Font = new System.Drawing.Font("微软雅黑", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Message_label.Location = new System.Drawing.Point(6, 21);
            this.Message_label.Name = "Message_label";
            this.Message_label.Size = new System.Drawing.Size(877, 290);
            this.Message_label.TabIndex = 5;
            this.Message_label.Text = "此处显示消息内容";
            this.Message_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BarCode_textBox
            // 
            this.BarCode_textBox.Font = new System.Drawing.Font("微软雅黑", 25.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BarCode_textBox.Location = new System.Drawing.Point(6, 315);
            this.BarCode_textBox.Margin = new System.Windows.Forms.Padding(4);
            this.BarCode_textBox.Name = "BarCode_textBox";
            this.BarCode_textBox.Size = new System.Drawing.Size(877, 64);
            this.BarCode_textBox.TabIndex = 6;
            this.BarCode_textBox.Visible = false;
            this.BarCode_textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Message_label);
            this.groupBox1.Controls.Add(this.BarCode_textBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(907, 393);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Messagebox";
            // 
            // MessageBoxs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(927, 416);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MessageBoxs";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MessageBox";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.messageBox_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MessageBoxs_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        internal System.Windows.Forms.Label Message_label;
        internal System.Windows.Forms.TextBox BarCode_textBox;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}