namespace GabesaBalla
{
	partial class MessageCheckListBox
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
			this.m_checklistbox = new System.Windows.Forms.CheckedListBox();
			this.m_applyButton = new System.Windows.Forms.Button();
			this.m_cancelButton = new System.Windows.Forms.Button();
			this.m_label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// m_checklistbox
			// 
			this.m_checklistbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
					  | System.Windows.Forms.AnchorStyles.Left)
					  | System.Windows.Forms.AnchorStyles.Right)));
			this.m_checklistbox.CheckOnClick = true;
			this.m_checklistbox.FormattingEnabled = true;
			this.m_checklistbox.Location = new System.Drawing.Point(12, 56);
			this.m_checklistbox.Name = "m_checklistbox";
			this.m_checklistbox.Size = new System.Drawing.Size(335, 191);
			this.m_checklistbox.TabIndex = 0;
			// 
			// m_applyButton
			// 
			this.m_applyButton.Location = new System.Drawing.Point(272, 255);
			this.m_applyButton.Name = "m_applyButton";
			this.m_applyButton.Size = new System.Drawing.Size(75, 30);
			this.m_applyButton.TabIndex = 1;
			this.m_applyButton.Text = "Apply";
			this.m_applyButton.UseVisualStyleBackColor = true;
			this.m_applyButton.Click += new System.EventHandler(this.m_applyButton_Click);
			// 
			// m_cancelButton
			// 
			this.m_cancelButton.Enabled = false;
			this.m_cancelButton.Location = new System.Drawing.Point(180, 255);
			this.m_cancelButton.Name = "m_cancelButton";
			this.m_cancelButton.Size = new System.Drawing.Size(75, 30);
			this.m_cancelButton.TabIndex = 2;
			this.m_cancelButton.Text = "Cancel";
			this.m_cancelButton.UseVisualStyleBackColor = true;
			this.m_cancelButton.Visible = false;
			this.m_cancelButton.Click += new System.EventHandler(this.m_cancelButton_Click);
			// 
			// m_label
			// 
			this.m_label.AutoSize = true;
			this.m_label.Font = new System.Drawing.Font("Arial Black", 7.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_label.Location = new System.Drawing.Point(13, 13);
			this.m_label.Name = "m_label";
			this.m_label.Size = new System.Drawing.Size(48, 17);
			this.m_label.TabIndex = 3;
			this.m_label.Text = "label1";
			// 
			// MessageCheckListBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(359, 294);
			this.Controls.Add(this.m_label);
			this.Controls.Add(this.m_cancelButton);
			this.Controls.Add(this.m_applyButton);
			this.Controls.Add(this.m_checklistbox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "MessageCheckListBox";
			this.Text = "MessageCheckListBox";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckedListBox m_checklistbox;
		private System.Windows.Forms.Button m_applyButton;
		private System.Windows.Forms.Button m_cancelButton;
		private System.Windows.Forms.Label m_label;
	}
}