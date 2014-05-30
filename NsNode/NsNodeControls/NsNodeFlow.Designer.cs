namespace NsNodeControls
{
	partial class NsNodeFlow
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NsNodeFlow));
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripX = new System.Windows.Forms.ToolStripButton();
			this.toolStripParent = new System.Windows.Forms.ToolStripButton();
			this.applybtn = new System.Windows.Forms.ToolStripButton();
			this.reloadbtn = new System.Windows.Forms.ToolStripButton();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 25);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(148, 123);
			this.flowLayoutPanel1.TabIndex = 0;
			// 
			// toolStrip1
			// 
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripX,
            this.toolStripParent,
            this.applybtn,
            this.reloadbtn});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.toolStrip1.Size = new System.Drawing.Size(148, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripX
			// 
			this.toolStripX.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripX.Image = ((System.Drawing.Image)(resources.GetObject("toolStripX.Image")));
			this.toolStripX.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripX.Name = "toolStripX";
			this.toolStripX.Size = new System.Drawing.Size(23, 22);
			this.toolStripX.Text = "X";
			this.toolStripX.ToolTipText = "Close";
			this.toolStripX.Click += new System.EventHandler(this.toolStripX_Click);
			// 
			// toolStripParent
			// 
			this.toolStripParent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripParent.Image = ((System.Drawing.Image)(resources.GetObject("toolStripParent.Image")));
			this.toolStripParent.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripParent.Name = "toolStripParent";
			this.toolStripParent.Size = new System.Drawing.Size(45, 22);
			this.toolStripParent.Text = "Parent";
			this.toolStripParent.Click += new System.EventHandler(this.toolStripParent_Click);
			// 
			// applybtn
			// 
			this.applybtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.applybtn.Image = ((System.Drawing.Image)(resources.GetObject("applybtn.Image")));
			this.applybtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.applybtn.Name = "applybtn";
			this.applybtn.Size = new System.Drawing.Size(42, 22);
			this.applybtn.Text = "Apply";
			this.applybtn.Click += new System.EventHandler(this.applybtn_Click);
			// 
			// reloadbtn
			// 
			this.reloadbtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.reloadbtn.Image = ((System.Drawing.Image)(resources.GetObject("reloadbtn.Image")));
			this.reloadbtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.reloadbtn.Name = "reloadbtn";
			this.reloadbtn.Size = new System.Drawing.Size(47, 19);
			this.reloadbtn.Text = "Reload";
			this.reloadbtn.Click += new System.EventHandler(this.reloadbtn_Click);
			// 
			// NsNodeFlow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.flowLayoutPanel1);
			this.Controls.Add(this.toolStrip1);
			this.Name = "NsNodeFlow";
			this.Size = new System.Drawing.Size(148, 148);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NsNodeFlow_KeyUp);
			this.Resize += new System.EventHandler(this.NsNodeFlow_Resize);
			this.SizeChanged += new System.EventHandler(this.NsNodeFlow_Resize);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripX;
		private System.Windows.Forms.ToolStripButton toolStripParent;
		private System.Windows.Forms.ToolStripButton applybtn;
		private System.Windows.Forms.ToolStripButton reloadbtn;
	}
}
