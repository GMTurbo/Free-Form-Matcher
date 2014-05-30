namespace NodeTester
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
			this.components = new System.ComponentModel.Container();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.nsNodeTree1 = new NsNodes.NsNodeTree();
			this.queryView1 = new NsNodes.QueryView();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.nsNodeTree1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(811, 452);
			this.splitContainer1.SplitterDistance = 270;
			this.splitContainer1.TabIndex = 1;
			// 
			// nsNodeTree1
			// 
			this.nsNodeTree1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.nsNodeTree1.Location = new System.Drawing.Point(0, 0);
			this.nsNodeTree1.Name = "nsNodeTree1";
			this.nsNodeTree1.Size = new System.Drawing.Size(270, 452);
			this.nsNodeTree1.TabIndex = 0;
			// 
			// queryView1
			// 
			this.queryView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.queryView1.Location = new System.Drawing.Point(0, 0);
			this.queryView1.Name = "queryView1";
			this.queryView1.Size = new System.Drawing.Size(179, 452);
			this.queryView1.TabIndex = 0;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.queryView1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Size = new System.Drawing.Size(537, 452);
			this.splitContainer2.SplitterDistance = 179;
			this.splitContainer2.TabIndex = 2;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(811, 452);
			this.Controls.Add(this.splitContainer1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private NsNodes.NsNodeTree nsNodeTree1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private NsNodes.QueryView queryView1;
		private System.Windows.Forms.SplitContainer splitContainer2;

	}
}

