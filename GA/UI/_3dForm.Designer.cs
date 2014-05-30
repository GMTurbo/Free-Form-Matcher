namespace UI
{
     partial class _3dForm
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
               this.splitContainer1 = new System.Windows.Forms.SplitContainer();
               this.viewportProfessional1 = new devDept.Eyeshot.ViewportProfessional();
               this.statusStrip1 = new System.Windows.Forms.StatusStrip();
               this.m_status = new System.Windows.Forms.ToolStripStatusLabel();
               this.comboBox1 = new System.Windows.Forms.ComboBox();
               this.m_progBar = new System.Windows.Forms.ProgressBar();
               this.button1 = new System.Windows.Forms.Button();
               this.m_runButton = new System.Windows.Forms.Button();
               ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
               this.splitContainer1.Panel1.SuspendLayout();
               this.splitContainer1.Panel2.SuspendLayout();
               this.splitContainer1.SuspendLayout();
               this.statusStrip1.SuspendLayout();
               this.SuspendLayout();
               // 
               // splitContainer1
               // 
               this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
               this.splitContainer1.Location = new System.Drawing.Point(0, 0);
               this.splitContainer1.Name = "splitContainer1";
               this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
               // 
               // splitContainer1.Panel1
               // 
               this.splitContainer1.Panel1.Controls.Add(this.viewportProfessional1);
               // 
               // splitContainer1.Panel2
               // 
               this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
               this.splitContainer1.Panel2.Controls.Add(this.comboBox1);
               this.splitContainer1.Panel2.Controls.Add(this.m_progBar);
               this.splitContainer1.Panel2.Controls.Add(this.button1);
               this.splitContainer1.Panel2.Controls.Add(this.m_runButton);
               this.splitContainer1.Size = new System.Drawing.Size(674, 401);
               this.splitContainer1.SplitterDistance = 332;
               this.splitContainer1.TabIndex = 0;
               // 
               // viewportProfessional1
               // 
               this.viewportProfessional1.AmbientLight = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
               this.viewportProfessional1.Background = new devDept.Eyeshot.Background(devDept.Eyeshot.backgroundStyleType.Gradient, System.Drawing.Color.Gray, System.Drawing.Color.Black, null);
               this.viewportProfessional1.Cursor = System.Windows.Forms.Cursors.Default;
               this.viewportProfessional1.DisplayMode = devDept.Eyeshot.viewportDisplayType.Shaded;
               this.viewportProfessional1.Dock = System.Windows.Forms.DockStyle.Fill;
               this.viewportProfessional1.Grid = new devDept.Eyeshot.Grid(new devDept.Eyeshot.Point2D(0D, 0D), new devDept.Eyeshot.Point2D(50D, 50D), 10D, System.Drawing.Color.White, System.Drawing.Color.DimGray, false, true);
               this.viewportProfessional1.Location = new System.Drawing.Point(0, 0);
               this.viewportProfessional1.Name = "viewportProfessional1";
               this.viewportProfessional1.Size = new System.Drawing.Size(674, 332);
               this.viewportProfessional1.TabIndex = 0;
               this.viewportProfessional1.UnitsMode = devDept.Eyeshot.viewportUnitsType.Millimeters;
               // 
               // statusStrip1
               // 
               this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_status});
               this.statusStrip1.Location = new System.Drawing.Point(0, 43);
               this.statusStrip1.Name = "statusStrip1";
               this.statusStrip1.Size = new System.Drawing.Size(674, 22);
               this.statusStrip1.TabIndex = 13;
               this.statusStrip1.Text = "statusStrip1";
               // 
               // m_status
               // 
               this.m_status.Name = "m_status";
               this.m_status.Size = new System.Drawing.Size(39, 17);
               this.m_status.Text = "Ready";
               // 
               // comboBox1
               // 
               this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
               this.comboBox1.FormattingEnabled = true;
               this.comboBox1.Items.AddRange(new object[] {
            "Gaussian",
            "Parabolic",
            "Cubic",
            "Line",
            "Cubic/Parabolic"});
               this.comboBox1.Location = new System.Drawing.Point(162, 17);
               this.comboBox1.Name = "comboBox1";
               this.comboBox1.Size = new System.Drawing.Size(121, 21);
               this.comboBox1.TabIndex = 12;
               this.comboBox1.Text = "Gaussian";
               // 
               // m_progBar
               // 
               this.m_progBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
               this.m_progBar.Location = new System.Drawing.Point(536, 15);
               this.m_progBar.Name = "m_progBar";
               this.m_progBar.Size = new System.Drawing.Size(126, 23);
               this.m_progBar.TabIndex = 11;
               // 
               // button1
               // 
               this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
               this.button1.Location = new System.Drawing.Point(12, 15);
               this.button1.Name = "button1";
               this.button1.Size = new System.Drawing.Size(144, 23);
               this.button1.TabIndex = 10;
               this.button1.Text = "Generate Random Surface";
               this.button1.UseVisualStyleBackColor = true;
               this.button1.Click += new System.EventHandler(this.button1_Click);
               // 
               // m_runButton
               // 
               this.m_runButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
               this.m_runButton.BackColor = System.Drawing.Color.Lime;
               this.m_runButton.Location = new System.Drawing.Point(455, 15);
               this.m_runButton.Name = "m_runButton";
               this.m_runButton.Size = new System.Drawing.Size(75, 23);
               this.m_runButton.TabIndex = 9;
               this.m_runButton.Text = "Run";
               this.m_runButton.UseVisualStyleBackColor = false;
               this.m_runButton.Click += new System.EventHandler(this.m_runButton_Click);
               // 
               // _3dForm
               // 
               this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
               this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
               this.ClientSize = new System.Drawing.Size(674, 401);
               this.Controls.Add(this.splitContainer1);
               this.Name = "_3dForm";
               this.Text = "_3dForm";
               this.splitContainer1.Panel1.ResumeLayout(false);
               this.splitContainer1.Panel2.ResumeLayout(false);
               this.splitContainer1.Panel2.PerformLayout();
               ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
               this.splitContainer1.ResumeLayout(false);
               this.statusStrip1.ResumeLayout(false);
               this.statusStrip1.PerformLayout();
               this.ResumeLayout(false);

          }

          #endregion

          private System.Windows.Forms.SplitContainer splitContainer1;
          private devDept.Eyeshot.ViewportProfessional viewportProfessional1;
          private System.Windows.Forms.ComboBox comboBox1;
          private System.Windows.Forms.ProgressBar m_progBar;
          private System.Windows.Forms.Button button1;
          private System.Windows.Forms.Button m_runButton;
          private System.Windows.Forms.StatusStrip statusStrip1;
          private System.Windows.Forms.ToolStripStatusLabel m_status;
     }
}