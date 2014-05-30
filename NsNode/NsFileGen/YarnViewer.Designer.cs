namespace NsFileGen
{
	partial class YarnViewer
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
			this.nsNodeView1 = new NsViewers.NsNodeView();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
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
			this.splitContainer1.Panel2.Controls.Add(this.nsNodeView1);
			this.splitContainer1.Size = new System.Drawing.Size(284, 264);
			this.splitContainer1.SplitterDistance = 94;
			this.splitContainer1.TabIndex = 1;
			// 
			// nsNodeTree1
			// 
			this.nsNodeTree1.AllowDrop = true;
			this.nsNodeTree1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.nsNodeTree1.Location = new System.Drawing.Point(0, 0);
			this.nsNodeTree1.Name = "nsNodeTree1";
			this.nsNodeTree1.Size = new System.Drawing.Size(94, 264);
			this.nsNodeTree1.TabIndex = 0;
			this.nsNodeTree1.DragDrop += new System.Windows.Forms.DragEventHandler(this.nsNodeView1_DragDrop);
			this.nsNodeTree1.DragEnter += new System.Windows.Forms.DragEventHandler(this.nsNodeView1_DragEnter);
			// 
			// nsNodeView1
			// 
			this.nsNodeView1.AllowDrop = true;
			this.nsNodeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.nsNodeView1.Location = new System.Drawing.Point(0, 0);
			this.nsNodeView1.Name = "nsNodeView1";
			this.nsNodeView1.Size = new System.Drawing.Size(186, 264);
			this.nsNodeView1.TabIndex = 0;
			this.nsNodeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.nsNodeView1_DragDrop);
			this.nsNodeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.nsNodeView1_DragEnter);
			// 
			// YarnViewer
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 264);
			this.Controls.Add(this.splitContainer1);
			this.Name = "YarnViewer";
			this.Text = "YarnViewer";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private NsNodes.NsNodeTree nsNodeTree1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private NsViewers.NsNodeView nsNodeView1;
	}
}