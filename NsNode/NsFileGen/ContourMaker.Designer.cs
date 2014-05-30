namespace NsFileGen
{
	partial class ContourMaker
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
			this.ply = new  NsNodeControls.LabelTextbox();
			this.numtapes = new  NsNodeControls.LabelTextbox();
			this.radius = new  NsNodeControls.LabelTextbox();
			this.addrosette = new System.Windows.Forms.Button();
			this.tapecontour = new System.Windows.Forms.Button();
			this.angles = new NsNodeControls.PointBox();
			this.offset = new NsNodeControls.PointBox();
			this.direction = new NsNodeControls.PointBox();
			this.tapedensity = new  NsNodeControls.LabelTextbox();
			this.tapewidth = new  NsNodeControls.LabelTextbox();
			this.nsNodeTree1 = new NsNodes.NsNodeTree();
			this.button1 = new System.Windows.Forms.Button();
			this.nsNodeView1 = new NsViewers.NsNodeView();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
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
			this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
			this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
			this.splitContainer1.Panel1.Controls.Add(this.ply);
			this.splitContainer1.Panel1.Controls.Add(this.addrosette);
			this.splitContainer1.Panel1.Controls.Add(this.tapecontour);
			this.splitContainer1.Panel1.Controls.Add(this.tapedensity);
			this.splitContainer1.Panel1.Controls.Add(this.tapewidth);
			this.splitContainer1.Panel1.Controls.Add(this.nsNodeTree1);
			this.splitContainer1.Panel1.Controls.Add(this.button1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.nsNodeView1);
			this.splitContainer1.Size = new System.Drawing.Size(747, 563);
			this.splitContainer1.SplitterDistance = 302;
			this.splitContainer1.TabIndex = 0;
			// 
			// ply
			// 
			this.ply.Label = "Ply";
			this.ply.Location = new System.Drawing.Point(10, 10);
			this.ply.Name = "ply";
			this.ply.Numeric = true;
			this.ply.ReadOnly = false;
			this.ply.Size = new System.Drawing.Size(138, 40);
			this.ply.TabIndex = 6;
			// 
			// numtapes
			// 
			this.numtapes.Label = "Number of Tapes";
			this.numtapes.Location = new System.Drawing.Point(3, 65);
			this.numtapes.Name = "numtapes";
			this.numtapes.Numeric = true;
			this.numtapes.ReadOnly = false;
			this.numtapes.Size = new System.Drawing.Size(138, 40);
			this.numtapes.TabIndex = 6;
			// 
			// radius
			// 
			this.radius.Label = "Radius";
			this.radius.Location = new System.Drawing.Point(1, 19);
			this.radius.Name = "radius";
			this.radius.Numeric = true;
			this.radius.ReadOnly = false;
			this.radius.Size = new System.Drawing.Size(138, 40);
			this.radius.TabIndex = 6;
			// 
			// addrosette
			// 
			this.addrosette.Location = new System.Drawing.Point(156, 32);
			this.addrosette.Name = "addrosette";
			this.addrosette.Size = new System.Drawing.Size(143, 23);
			this.addrosette.TabIndex = 5;
			this.addrosette.Text = "Add Rosette";
			this.addrosette.UseVisualStyleBackColor = true;
			this.addrosette.Click += new System.EventHandler(this.addrosette_Click);
			// 
			// tapecontour
			// 
			this.tapecontour.Location = new System.Drawing.Point(7, 528);
			this.tapecontour.Name = "tapecontour";
			this.tapecontour.Size = new System.Drawing.Size(75, 23);
			this.tapecontour.TabIndex = 4;
			this.tapecontour.Text = "Tape Contour";
			this.tapecontour.UseVisualStyleBackColor = true;
			this.tapecontour.Click += new System.EventHandler(this.tape_Click);
			// 
			// angles
			// 
			this.angles.Label = "Angle Start/End";
			this.angles.Location = new System.Drawing.Point(3, 111);
			this.angles.Name = "angles";
			this.angles.Point = new double[] {
        0,
        0};
			this.angles.Point3D = false;
			this.angles.Size = new System.Drawing.Size(138, 66);
			this.angles.TabIndex = 3;
			this.angles.Units = "deg";
			// 
			// offset
			// 
			this.offset.Label = "Initial Offset";
			this.offset.Location = new System.Drawing.Point(3, 90);
			this.offset.Name = "offset";
			this.offset.Point = new double[] {
        0,
        0};
			this.offset.Point3D = false;
			this.offset.Size = new System.Drawing.Size(138, 66);
			this.offset.TabIndex = 3;
			this.offset.Units = "";
			// 
			// direction
			// 
			this.direction.Label = "Taping Direction";
			this.direction.Location = new System.Drawing.Point(3, 19);
			this.direction.Name = "direction";
			this.direction.Point = new double[] {
        0,
        0};
			this.direction.Point3D = false;
			this.direction.Size = new System.Drawing.Size(138, 66);
			this.direction.TabIndex = 3;
			this.direction.Units = "";
			// 
			// tapedensity
			// 
			this.tapedensity.Label = "Tape Density";
			this.tapedensity.Location = new System.Drawing.Point(12, 86);
			this.tapedensity.Name = "tapedensity";
			this.tapedensity.Numeric = true;
			this.tapedensity.ReadOnly = false;
			this.tapedensity.Size = new System.Drawing.Size(138, 40);
			this.tapedensity.TabIndex = 2;
			// 
			// tapewidth
			// 
			this.tapewidth.Label = "Tape Width";
			this.tapewidth.Location = new System.Drawing.Point(12, 48);
			this.tapewidth.Name = "tapewidth";
			this.tapewidth.Numeric = true;
			this.tapewidth.ReadOnly = false;
			this.tapewidth.Size = new System.Drawing.Size(138, 40);
			this.tapewidth.TabIndex = 2;
			// 
			// nsNodeTree1
			// 
			this.nsNodeTree1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
					  | System.Windows.Forms.AnchorStyles.Left)
					  | System.Windows.Forms.AnchorStyles.Right)));
			this.nsNodeTree1.Location = new System.Drawing.Point(156, 61);
			this.nsNodeTree1.Name = "nsNodeTree1";
			this.nsNodeTree1.Size = new System.Drawing.Size(143, 502);
			this.nsNodeTree1.TabIndex = 1;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(156, 3);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(143, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Add Contour";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.addcontour_Click);
			// 
			// nsNodeView1
			// 
			this.nsNodeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.nsNodeView1.Location = new System.Drawing.Point(0, 0);
			this.nsNodeView1.Name = "nsNodeView1";
			this.nsNodeView1.ProjectionMode = devDept.Eyeshot.cameraProjectionType.Perspective;
			this.nsNodeView1.Size = new System.Drawing.Size(441, 563);
			this.nsNodeView1.TabIndex = 0;
			this.nsNodeView1.Viewtype = devDept.Eyeshot.viewType.Front;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.direction);
			this.groupBox1.Controls.Add(this.offset);
			this.groupBox1.Location = new System.Drawing.Point(9, 140);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(144, 157);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Contour Properties";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radius);
			this.groupBox2.Controls.Add(this.angles);
			this.groupBox2.Controls.Add(this.numtapes);
			this.groupBox2.Location = new System.Drawing.Point(9, 303);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(144, 184);
			this.groupBox2.TabIndex = 8;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Rosette Properties";
			// 
			// ContourMaker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(747, 563);
			this.Controls.Add(this.splitContainer1);
			this.Name = "ContourMaker";
			this.Text = "ContourMaker";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button button1;
		private NsViewers.NsNodeView nsNodeView1;
		private NsNodes.NsNodeTree nsNodeTree1;
		private NsNodeControls.LabelTextbox tapedensity;
		private NsNodeControls.LabelTextbox tapewidth;
		private NsNodeControls.PointBox offset;
		private NsNodeControls.PointBox direction;
		private System.Windows.Forms.Button tapecontour;
		private System.Windows.Forms.Button addrosette;
		private NsNodeControls.LabelTextbox ply;
		private NsNodeControls.LabelTextbox numtapes;
		private NsNodeControls.LabelTextbox radius;
		private NsNodeControls.PointBox angles;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}