namespace NsNodeView
{
	partial class NsNodeShot
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
               this.viewport = new devDept.Eyeshot.ViewportProfessional();
               this.SuspendLayout();
               // 
               // viewport
               // 
               this.viewport.AmbientLight = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
               this.viewport.Cursor = System.Windows.Forms.Cursors.Default;
               this.viewport.Dock = System.Windows.Forms.DockStyle.Fill;
               this.viewport.Location = new System.Drawing.Point(0, 0);
               this.viewport.Name = "viewport";
               this.viewport.Size = new System.Drawing.Size(666, 666);
               this.viewport.TabIndex = 0;
               this.viewport.DoubleClick += new System.EventHandler(this.viewport_DoubleClick);
               this.viewport.MouseMove += new System.Windows.Forms.MouseEventHandler(this.viewport_MouseMove);
               this.viewport.KeyUp += new System.Windows.Forms.KeyEventHandler(this.viewport_KeyUp);
               this.viewport.MouseDown += new System.Windows.Forms.MouseEventHandler(this.viewport_MouseDown);
               this.viewport.MouseUp += new System.Windows.Forms.MouseEventHandler(this.viewport_MouseUp);
               // 
               // NsNodeShot
               // 
               this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
               this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
               this.Controls.Add(this.viewport);
               this.Name = "NsNodeShot";
               this.Size = new System.Drawing.Size(666, 666);
               this.ResumeLayout(false);

		}

		#endregion

		private devDept.Eyeshot.ViewportProfessional viewport;
	}
}
