﻿
namespace Audio.Forms
{
    partial class Form1
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
            this.Yes = new System.Windows.Forms.Button();
            this.NO = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Yes
            // 
            this.Yes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.Yes.Font = new System.Drawing.Font("SimSun", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Yes.Location = new System.Drawing.Point(122, 261);
            this.Yes.Name = "Yes";
            this.Yes.Size = new System.Drawing.Size(239, 74);
            this.Yes.TabIndex = 0;
            this.Yes.Text = "YES";
            this.Yes.UseVisualStyleBackColor = false;
            this.Yes.Click += new System.EventHandler(this.Yes_Click);
            // 
            // NO
            // 
            this.NO.BackColor = System.Drawing.Color.Red;
            this.NO.Font = new System.Drawing.Font("SimSun", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.NO.Location = new System.Drawing.Point(411, 261);
            this.NO.Name = "NO";
            this.NO.Size = new System.Drawing.Size(239, 74);
            this.NO.TabIndex = 1;
            this.NO.Text = "NO";
            this.NO.UseVisualStyleBackColor = false;
            this.NO.Click += new System.EventHandler(this.NO_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("SimSun", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(750, 219);
            this.label1.TabIndex = 2;
            this.label1.Text = "播放音乐/Kiểm tra phát nhạc";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 400);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NO);
            this.Controls.Add(this.Yes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Message";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Yes;
        private System.Windows.Forms.Button NO;
        public System.Windows.Forms.Label label1;
    }
}