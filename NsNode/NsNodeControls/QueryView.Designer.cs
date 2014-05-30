namespace NsNodes
{
	partial class QueryView
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
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.Node = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.query = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboBox1
			// 
			this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
					  | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(3, 3);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(257, 21);
			this.comboBox1.TabIndex = 0;
			this.comboBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBox1_KeyPress);
			// 
			// dataGridView1
			// 
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Node,
            this.Key,
            this.Value});
			this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView1.Location = new System.Drawing.Point(0, 0);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.Size = new System.Drawing.Size(341, 340);
			this.dataGridView1.TabIndex = 1;
			// 
			// Node
			// 
			this.Node.FillWeight = 30F;
			this.Node.Frozen = true;
			this.Node.HeaderText = "Node";
			this.Node.Name = "Node";
			this.Node.ReadOnly = true;
			// 
			// Key
			// 
			this.Key.FillWeight = 30F;
			this.Key.HeaderText = "Key";
			this.Key.Name = "Key";
			this.Key.ReadOnly = true;
			// 
			// Value
			// 
			this.Value.HeaderText = "Value";
			this.Value.Name = "Value";
			// 
			// query
			// 
			this.query.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.query.Location = new System.Drawing.Point(266, 0);
			this.query.Name = "query";
			this.query.Size = new System.Drawing.Size(75, 27);
			this.query.TabIndex = 2;
			this.query.Text = "Query";
			this.query.UseVisualStyleBackColor = true;
			this.query.Click += new System.EventHandler(this.query_Click);
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
			this.splitContainer1.Panel1.Controls.Add(this.query);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
			this.splitContainer1.Size = new System.Drawing.Size(341, 370);
			this.splitContainer1.SplitterDistance = 26;
			this.splitContainer1.TabIndex = 3;
			// 
			// QueryView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.splitContainer1);
			this.Name = "QueryView";
			this.Size = new System.Drawing.Size(341, 370);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Node;
		private System.Windows.Forms.DataGridViewTextBoxColumn Key;
		private System.Windows.Forms.DataGridViewTextBoxColumn Value;
		private System.Windows.Forms.Button query;
		private System.Windows.Forms.SplitContainer splitContainer1;
	}
}
