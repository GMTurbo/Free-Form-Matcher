namespace NsNodeControls
{
	partial class LogTextBox
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
			this.m_popup = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyAllItem = new System.Windows.Forms.ToolStripMenuItem();
			this.writeLogItem = new System.Windows.Forms.ToolStripMenuItem();
			this.m_popup.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_popup
			// 
			this.m_popup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyAllItem,
            this.writeLogItem});
			this.m_popup.Name = "m_popup";
			this.m_popup.Size = new System.Drawing.Size(135, 70);
			// 
			// copyAllItem
			// 
			this.copyAllItem.Name = "copyAllItem";
			this.copyAllItem.Size = new System.Drawing.Size(134, 22);
			this.copyAllItem.Text = "Copy All";
			this.copyAllItem.ToolTipText = "Copy the entire log\'s text to the clipboard";
			this.copyAllItem.Click += new System.EventHandler(this.copyAllItem_Click);
			// 
			// writeLogItem
			// 
			this.writeLogItem.Name = "writeLogItem";
			this.writeLogItem.Size = new System.Drawing.Size(134, 22);
			this.writeLogItem.Text = "Write Log...";
			this.writeLogItem.ToolTipText = "Writes log\'s text to a specified file";
			this.writeLogItem.Click += new System.EventHandler(this.writeLogItem_Click);
			// 
			// LogTextBox
			// 
			this.ContextMenuStrip = this.m_popup;
			this.Size = new System.Drawing.Size(619, 334);
			this.m_popup.ResumeLayout(false);
			this.ResumeLayout(false);

		}


		#endregion

		private System.Windows.Forms.ContextMenuStrip m_popup;
		private System.Windows.Forms.ToolStripMenuItem copyAllItem;
		private System.Windows.Forms.ToolStripMenuItem writeLogItem;
	}
}
