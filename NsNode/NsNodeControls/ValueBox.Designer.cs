namespace NsNodeControls
{
	partial class ValueBox
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
			this.label = new System.Windows.Forms.Label();
			this.valueText = new System.Windows.Forms.TextBox();
			this.units = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.Location = new System.Drawing.Point(0, 3);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(0, 13);
			this.label.TabIndex = 0;
			this.label.Click += new System.EventHandler(this.units_Click);
			// 
			// valueText
			// 
			this.valueText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
					  | System.Windows.Forms.AnchorStyles.Right)));
			this.valueText.Location = new System.Drawing.Point(38, 0);
			this.valueText.Name = "valueText";
			this.valueText.Size = new System.Drawing.Size(110, 20);
			this.valueText.TabIndex = 1;
			this.valueText.Text = "0.0";
			this.valueText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.valueText.Leave += new System.EventHandler(this.valueText_Leave);
			this.valueText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
			// 
			// units
			// 
			this.units.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.units.AutoSize = true;
			this.units.Location = new System.Drawing.Point(154, 3);
			this.units.Name = "units";
			this.units.Size = new System.Drawing.Size(31, 13);
			this.units.TabIndex = 2;
			this.units.Text = "Units";
			this.units.Click += new System.EventHandler(this.units_Click);
			// 
			// ValueBox
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.units);
			this.Controls.Add(this.valueText);
			this.Controls.Add(this.label);
			this.Name = "ValueBox";
			this.Size = new System.Drawing.Size(188, 21);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.ValueBox_Layout);
			this.Click += new System.EventHandler(this.units_Click);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label;
		private System.Windows.Forms.TextBox valueText;
		private System.Windows.Forms.Label units;
	}
}
