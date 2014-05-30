namespace NsNodes
{
	partial class AttributeEditor
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
			this.Ok = new System.Windows.Forms.Button();
			this.Cancel = new System.Windows.Forms.Button();
			this.valuebox = new  NsNodeControls.LabelTextbox();
			this.keybox = new  NsNodeControls.LabelTextbox();
			this.SuspendLayout();
			// 
			// Ok
			// 
			this.Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Ok.Location = new System.Drawing.Point(72, 56);
			this.Ok.Name = "Ok";
			this.Ok.Size = new System.Drawing.Size(75, 23);
			this.Ok.TabIndex = 2;
			this.Ok.Text = "Ok";
			this.Ok.UseVisualStyleBackColor = true;
			// 
			// Cancel
			// 
			this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel.Location = new System.Drawing.Point(177, 56);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(75, 23);
			this.Cancel.TabIndex = 3;
			this.Cancel.Text = "Cancel";
			this.Cancel.UseVisualStyleBackColor = true;
			// 
			// valuebox
			// 
			this.valuebox.Label = "label1";
			this.valuebox.Location = new System.Drawing.Point(166, 12);
			this.valuebox.Name = "valuebox";
			this.valuebox.Numeric = false;
			this.valuebox.ReadOnly = false;
			this.valuebox.Size = new System.Drawing.Size(146, 40);
			this.valuebox.TabIndex = 1;
			// 
			// keybox
			// 
			this.keybox.Label = "label1";
			this.keybox.Location = new System.Drawing.Point(12, 12);
			this.keybox.Name = "keybox";
			this.keybox.Numeric = false;
			this.keybox.ReadOnly = false;
			this.keybox.Size = new System.Drawing.Size(146, 40);
			this.keybox.TabIndex = 0;
			// 
			// AttributeEditor
			// 
			this.AcceptButton = this.Ok;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.Cancel;
			this.ClientSize = new System.Drawing.Size(324, 91);
			this.Controls.Add(this.valuebox);
			this.Controls.Add(this.keybox);
			this.Controls.Add(this.Cancel);
			this.Controls.Add(this.Ok);
			this.Name = "AttributeEditor";
			this.Text = "AttributeEditor";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button Ok;
		private System.Windows.Forms.Button Cancel;
		private NsNodeControls.LabelTextbox keybox;
		private NsNodeControls.LabelTextbox valuebox;
	}
}