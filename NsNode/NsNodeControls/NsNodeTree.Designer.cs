namespace NsNodes
{
	partial class NsNodeTree
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
			this.components = new System.ComponentModel.Container();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.treeStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.expandNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.collapseNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.expandToDepthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.HideSelection = false;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(150, 150);
			this.treeView1.TabIndex = 0;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyUp);
			this.treeView1.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeSelect);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.copyToolStripItem,
            this.pasteToolStripItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.toolStripSeparator2,
            this.editToolStripMenuItem,
            this.toolStripMenuItem1,
            this.treeStripMenuItem,
            this.toolStripSeparator3,
            this.expandNodeToolStripMenuItem,
            this.collapseNodeToolStripMenuItem,
            this.expandAllToolStripMenuItem,
            this.collapseAllToolStripMenuItem,
            this.expandToDepthToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(165, 352);
			// 
			// addToolStripMenuItem
			// 
			this.addToolStripMenuItem.Name = "addToolStripMenuItem";
			this.addToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.addToolStripMenuItem.Text = "Add";
			this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
			// 
			// copyToolStripItem
			// 
			this.copyToolStripItem.Name = "copyToolStripItem";
			this.copyToolStripItem.Size = new System.Drawing.Size(164, 22);
			this.copyToolStripItem.Text = "Copy";
			this.copyToolStripItem.Click += new System.EventHandler(this.copy_Click);
			// 
			// pasteToolStripItem
			// 
			this.pasteToolStripItem.Name = "pasteToolStripItem";
			this.pasteToolStripItem.Size = new System.Drawing.Size(164, 22);
			this.pasteToolStripItem.Text = "Paste";
			this.pasteToolStripItem.Click += new System.EventHandler(this.paste_Click);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(161, 6);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// loadToolStripMenuItem
			// 
			this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
			this.loadToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.loadToolStripMenuItem.Text = "Load";
			this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(161, 6);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.editToolStripMenuItem.Text = "Flow...";
			this.editToolStripMenuItem.Click += new System.EventHandler(this.FlowToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(164, 22);
			this.toolStripMenuItem1.Text = "Grid...";
			this.toolStripMenuItem1.Click += new System.EventHandler(this.gridToolStripMenuItem_Click);
			// 
			// treeStripMenuItem
			// 
			this.treeStripMenuItem.Name = "treeStripMenuItem";
			this.treeStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.treeStripMenuItem.Text = "Tree...";
			this.treeStripMenuItem.Click += new System.EventHandler(this.TreeStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(161, 6);
			// 
			// expandNodeToolStripMenuItem
			// 
			this.expandNodeToolStripMenuItem.Name = "expandNodeToolStripMenuItem";
			this.expandNodeToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.expandNodeToolStripMenuItem.Text = "Expand Node";
			this.expandNodeToolStripMenuItem.Click += new System.EventHandler(this.expandNodeToolStripMenuItem_Click);
			// 
			// collapseNodeToolStripMenuItem
			// 
			this.collapseNodeToolStripMenuItem.Name = "collapseNodeToolStripMenuItem";
			this.collapseNodeToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.collapseNodeToolStripMenuItem.Text = "Collapse Node";
			this.collapseNodeToolStripMenuItem.Click += new System.EventHandler(this.collapseNodeToolStripMenuItem_Click);
			// 
			// expandAllToolStripMenuItem
			// 
			this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
			this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.expandAllToolStripMenuItem.Text = "Expand All";
			this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
			// 
			// collapseAllToolStripMenuItem
			// 
			this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
			this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.collapseAllToolStripMenuItem.Text = "Collapse All";
			this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
			// 
			// expandToDepthToolStripMenuItem
			// 
			this.expandToDepthToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7,
            this.toolStripMenuItem8});
			this.expandToDepthToolStripMenuItem.Name = "expandToDepthToolStripMenuItem";
			this.expandToDepthToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.expandToDepthToolStripMenuItem.Text = "Expand To Depth";
			this.expandToDepthToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.expandToDepthToolStripMenuItem_DropDownItemClicked);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem3.Text = "1";
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem4.Text = "2";
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem5.Text = "3";
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem6.Text = "4";
			// 
			// toolStripMenuItem7
			// 
			this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			this.toolStripMenuItem7.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem7.Text = "5";
			// 
			// toolStripMenuItem8
			// 
			this.toolStripMenuItem8.Name = "toolStripMenuItem8";
			this.toolStripMenuItem8.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem8.Text = "6";
			// 
			// NsNodeTree
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.treeView1);
			this.Name = "NsNodeTree";
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem expandNodeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem collapseNodeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem treeStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripItem;
		private System.Windows.Forms.ToolStripMenuItem expandToDepthToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
	}
}
