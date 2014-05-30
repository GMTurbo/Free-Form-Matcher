namespace NsNodes
{
	partial class NsNodeEditor
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
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.nodeList = new System.Windows.Forms.ListView();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.label3 = new System.Windows.Forms.Label();
			this.attrList = new System.Windows.Forms.ListView();
			this.Key = new System.Windows.Forms.ColumnHeader();
			this.Value = new System.Windows.Forms.ColumnHeader();
			this.label2 = new System.Windows.Forms.Label();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.okBtn = new System.Windows.Forms.Button();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(36, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Label:";
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
					  | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(45, 6);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(321, 20);
			this.textBox1.TabIndex = 0;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.textBox1);
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(378, 213);
			this.splitContainer1.SplitterDistance = 30;
			this.splitContainer1.TabIndex = 2;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer2.IsSplitterFixed = true;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.cancelBtn);
			this.splitContainer2.Panel2.Controls.Add(this.okBtn);
			this.splitContainer2.Panel2MinSize = 40;
			this.splitContainer2.Size = new System.Drawing.Size(378, 179);
			this.splitContainer2.SplitterDistance = 135;
			this.splitContainer2.TabIndex = 0;
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.nodeList);
			this.splitContainer3.Panel1.Controls.Add(this.label3);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.attrList);
			this.splitContainer3.Panel2.Controls.Add(this.label2);
			this.splitContainer3.Size = new System.Drawing.Size(378, 135);
			this.splitContainer3.SplitterDistance = 137;
			this.splitContainer3.TabIndex = 0;
			// 
			// nodeList
			// 
			this.nodeList.ContextMenuStrip = this.contextMenuStrip1;
			this.nodeList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.nodeList.Location = new System.Drawing.Point(0, 13);
			this.nodeList.Name = "nodeList";
			this.nodeList.Size = new System.Drawing.Size(137, 122);
			this.nodeList.TabIndex = 0;
			this.nodeList.UseCompatibleStateImageBehavior = false;
			this.nodeList.View = System.Windows.Forms.View.List;
			this.nodeList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.nodeList_MouseDoubleClick);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.editToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(106, 48);
			// 
			// addToolStripMenuItem
			// 
			this.addToolStripMenuItem.Name = "addToolStripMenuItem";
			this.addToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
			this.addToolStripMenuItem.Text = "Add...";
			this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
			this.editToolStripMenuItem.Text = "Edit...";
			this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Dock = System.Windows.Forms.DockStyle.Top;
			this.label3.Location = new System.Drawing.Point(0, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(38, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Nodes";
			// 
			// attrList
			// 
			this.attrList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Key,
            this.Value});
			this.attrList.ContextMenuStrip = this.contextMenuStrip1;
			this.attrList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.attrList.Location = new System.Drawing.Point(0, 13);
			this.attrList.Name = "attrList";
			this.attrList.Size = new System.Drawing.Size(237, 122);
			this.attrList.TabIndex = 0;
			this.attrList.UseCompatibleStateImageBehavior = false;
			this.attrList.View = System.Windows.Forms.View.Details;
			this.attrList.DoubleClick += new System.EventHandler(this.attrList_DoubleClick);
			// 
			// Key
			// 
			this.Key.Text = "Key";
			this.Key.Width = 53;
			// 
			// Value
			// 
			this.Value.Text = "Value";
			this.Value.Width = 65;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Dock = System.Windows.Forms.DockStyle.Top;
			this.label2.Location = new System.Drawing.Point(0, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(51, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Attributes";
			// 
			// cancelBtn
			// 
			this.cancelBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelBtn.Location = new System.Drawing.Point(204, 3);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.Size = new System.Drawing.Size(75, 23);
			this.cancelBtn.TabIndex = 1;
			this.cancelBtn.Text = "Cancel";
			this.cancelBtn.UseVisualStyleBackColor = true;
			// 
			// okBtn
			// 
			this.okBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okBtn.Location = new System.Drawing.Point(99, 3);
			this.okBtn.Name = "okBtn";
			this.okBtn.Size = new System.Drawing.Size(75, 23);
			this.okBtn.TabIndex = 0;
			this.okBtn.Text = "Ok";
			this.okBtn.UseVisualStyleBackColor = true;
			this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
			// 
			// NsNodeEditor
			// 
			this.AcceptButton = this.okBtn;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelBtn;
			this.ClientSize = new System.Drawing.Size(378, 213);
			this.Controls.Add(this.splitContainer1);
			this.Name = "NsNodeEditor";
			this.ShowInTaskbar = false;
			this.Text = "NsNodeEditor";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel1.PerformLayout();
			this.splitContainer3.Panel2.ResumeLayout(false);
			this.splitContainer3.Panel2.PerformLayout();
			this.splitContainer3.ResumeLayout(false);
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.Button okBtn;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView attrList;
		private System.Windows.Forms.ColumnHeader Key;
		private System.Windows.Forms.ColumnHeader Value;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ListView nodeList;
		private System.Windows.Forms.Label label3;
	}
}