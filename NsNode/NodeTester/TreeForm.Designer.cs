namespace NodeTester
{
	partial class TreeForm
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
			this.nsNodeTree1 = new NsNodes.NsNodeTree();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// nsNodeTree1
			// 
			this.nsNodeTree1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
					  | System.Windows.Forms.AnchorStyles.Right)));
			this.nsNodeTree1.Location = new System.Drawing.Point(0, 0);
			this.nsNodeTree1.Name = "nsNodeTree1";
			this.nsNodeTree1.Size = new System.Drawing.Size(191, 264);
			this.nsNodeTree1.TabIndex = 0;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(197, 12);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "toFlow";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// TreeForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 264);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.nsNodeTree1);
			this.Name = "TreeForm";
			this.Text = "Rig Calc";
			this.ResumeLayout(false);

		}

		#endregion

		private NsNodes.NsNodeTree nsNodeTree1;
		private System.Windows.Forms.Button button1;
	}
}