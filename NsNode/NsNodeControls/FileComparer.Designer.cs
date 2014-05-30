namespace NsNodeControls
{
    partial class FileComparer
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
             System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileComparer));
             this.groupBox1 = new System.Windows.Forms.GroupBox();
             this.label1 = new System.Windows.Forms.Label();
             this.groupBox2 = new System.Windows.Forms.GroupBox();
             this.label2 = new System.Windows.Forms.Label();
             this.button1 = new System.Windows.Forms.Button();
             this.button2 = new System.Windows.Forms.Button();
             this.button3 = new System.Windows.Forms.Button();
             this.pictureBox2 = new System.Windows.Forms.PictureBox();
             this.pictureBox1 = new System.Windows.Forms.PictureBox();
             this.groupBox1.SuspendLayout();
             this.groupBox2.SuspendLayout();
             ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
             ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
             this.SuspendLayout();
             // 
             // groupBox1
             // 
             this.groupBox1.BackColor = System.Drawing.Color.Magenta;
             this.groupBox1.Controls.Add(this.label1);
             this.groupBox1.Location = new System.Drawing.Point(12, 12);
             this.groupBox1.Name = "groupBox1";
             this.groupBox1.Size = new System.Drawing.Size(156, 148);
             this.groupBox1.TabIndex = 0;
             this.groupBox1.TabStop = false;
             this.groupBox1.Text = "FILE 1";
             // 
             // label1
             // 
             this.label1.AllowDrop = true;
             this.label1.AutoSize = true;
             this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
             this.label1.Location = new System.Drawing.Point(6, 28);
             this.label1.Name = "label1";
             this.label1.Size = new System.Drawing.Size(130, 16);
             this.label1.TabIndex = 2;
             this.label1.Text = "Drop File #1 Here";
             // 
             // groupBox2
             // 
             this.groupBox2.BackColor = System.Drawing.Color.Chartreuse;
             this.groupBox2.Controls.Add(this.label2);
             this.groupBox2.Location = new System.Drawing.Point(199, 12);
             this.groupBox2.Name = "groupBox2";
             this.groupBox2.Size = new System.Drawing.Size(164, 148);
             this.groupBox2.TabIndex = 1;
             this.groupBox2.TabStop = false;
             this.groupBox2.Text = "FILE 2";
             // 
             // label2
             // 
             this.label2.AllowDrop = true;
             this.label2.AutoSize = true;
             this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
             this.label2.Location = new System.Drawing.Point(6, 28);
             this.label2.Name = "label2";
             this.label2.Size = new System.Drawing.Size(130, 16);
             this.label2.TabIndex = 1;
             this.label2.Text = "Drop File #2 Here";
             // 
             // button1
             // 
             this.button1.Location = new System.Drawing.Point(142, 229);
             this.button1.Name = "button1";
             this.button1.Size = new System.Drawing.Size(75, 23);
             this.button1.TabIndex = 2;
             this.button1.Text = "Compare";
             this.button1.UseVisualStyleBackColor = true;
             this.button1.Click += new System.EventHandler(this.button1_Click);
             // 
             // button2
             // 
             this.button2.Location = new System.Drawing.Point(93, 166);
             this.button2.Name = "button2";
             this.button2.Size = new System.Drawing.Size(75, 23);
             this.button2.TabIndex = 3;
             this.button2.Text = "clear";
             this.button2.UseVisualStyleBackColor = true;
             this.button2.Click += new System.EventHandler(this.button2_Click);
             // 
             // button3
             // 
             this.button3.Location = new System.Drawing.Point(199, 166);
             this.button3.Name = "button3";
             this.button3.Size = new System.Drawing.Size(75, 23);
             this.button3.TabIndex = 4;
             this.button3.Text = "clear";
             this.button3.UseVisualStyleBackColor = true;
             this.button3.Click += new System.EventHandler(this.button3_Click);
             // 
             // pictureBox2
             // 
             this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
             this.pictureBox2.Location = new System.Drawing.Point(315, 166);
             this.pictureBox2.Name = "pictureBox2";
             this.pictureBox2.Size = new System.Drawing.Size(48, 48);
             this.pictureBox2.TabIndex = 6;
             this.pictureBox2.TabStop = false;
             this.pictureBox2.Visible = false;
             // 
             // pictureBox1
             // 
             this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
             this.pictureBox1.Location = new System.Drawing.Point(12, 166);
             this.pictureBox1.Name = "pictureBox1";
             this.pictureBox1.Size = new System.Drawing.Size(48, 48);
             this.pictureBox1.TabIndex = 7;
             this.pictureBox1.TabStop = false;
             this.pictureBox1.Visible = false;
             // 
             // FileComparer
             // 
             this.AllowDrop = true;
             this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
             this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
             this.BackColor = System.Drawing.Color.White;
             this.ClientSize = new System.Drawing.Size(375, 264);
             this.Controls.Add(this.pictureBox1);
             this.Controls.Add(this.pictureBox2);
             this.Controls.Add(this.button3);
             this.Controls.Add(this.button2);
             this.Controls.Add(this.button1);
             this.Controls.Add(this.groupBox2);
             this.Controls.Add(this.groupBox1);
             this.Name = "FileComparer";
             this.Text = "FileComparer";
             this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FileComparer_DragDrop);
             this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FileComparer_DragEnter);
             this.groupBox1.ResumeLayout(false);
             this.groupBox1.PerformLayout();
             this.groupBox2.ResumeLayout(false);
             this.groupBox2.PerformLayout();
             ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
             ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
             this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}