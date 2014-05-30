namespace NsNodeControls
{
    partial class FileTextBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
             this.m_txt = new System.Windows.Forms.TextBox();
             this.m_btn = new System.Windows.Forms.Button();
             this.SuspendLayout();
             // 
             // m_txt
             // 
             this.m_txt.Dock = System.Windows.Forms.DockStyle.Fill;
             this.m_txt.Location = new System.Drawing.Point(0, 0);
             this.m_txt.Name = "m_txt";
             this.m_txt.Size = new System.Drawing.Size(163, 20);
             this.m_txt.TabIndex = 5;
             // 
             // m_btn
             // 
             this.m_btn.Dock = System.Windows.Forms.DockStyle.Right;
             this.m_btn.Location = new System.Drawing.Point(163, 0);
             this.m_btn.Name = "m_btn";
             this.m_btn.Size = new System.Drawing.Size(31, 19);
             this.m_btn.TabIndex = 4;
             this.m_btn.Text = "...";
             this.m_btn.UseVisualStyleBackColor = true;
             this.m_btn.Click += new System.EventHandler(this.m_btn_Click);
             // 
             // FileTextBox
             // 
             this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
             this.Controls.Add(this.m_txt);
             this.Controls.Add(this.m_btn);
             this.Margin = new System.Windows.Forms.Padding(2);
             this.Name = "FileTextBox";
             this.Size = new System.Drawing.Size(194, 19);
             this.ResumeLayout(false);
             this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox m_txt;
        private System.Windows.Forms.Button m_btn;
    }
}
