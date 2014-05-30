using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NsNodeControls
{
	public partial class PointList : UserControl, IAttributeEditor
	{
		public PointList()
		{
			InitializeComponent();
			InitializeGrid();
		}

		DataGridView Grid
		{
			get { return dataGridView1; }
		}
		void InitializeGrid()
		{
			Grid.Rows.Clear();
			Grid.Columns.Clear();

			Grid.Columns.AddRange(
				new DataGridViewTextBoxColumn(),
				new DataGridViewTextBoxColumn(),
				new DataGridViewTextBoxColumn()
				);

			Grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
			int i = 0;
			Grid.Columns[i].Name = "x1";
			Grid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			i++;
			Grid.Columns[i].Name = "x2";
			Grid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			i++;
			Grid.Columns[i].Name = "x3";
			Grid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

			for (i = 0; i < Grid.Columns.Count; i++)
			{
				Grid.Columns[i].ValueType = typeof(double);
				Grid.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
				Grid.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
			}

			Grid.RowHeadersVisible = false;
			Grid.AllowUserToOrderColumns = false;
		}

		public string Label
		{
			get { return label1.Text; }
			set { label1.Text = value; }
		}
		public List<double[]> Points
		{
			get
			{

				List<Double[]> list = new List<double[]>(Grid.Rows.Count);
				foreach (DataGridViewRow row in Grid.Rows)
				{
					if (row.Cells[0].Value == null || row.Cells[1].Value == null || row.Cells[2].Value == null)
						continue;
					list.Add(new double[] { (double)row.Cells[0].Value, (double)row.Cells[1].Value, (double)row.Cells[2].Value });
				}

				return list;
			}
			set
			{
				Grid.Rows.Clear();
				if (value == null)
					return;
				foreach (double[] d in value)
				{
					if (d.Length < 3)
						continue;
					Grid.Rows.Add(d[0], d[1], d[2]);
				}
			}
		}

		#region IAttributeEditor Members

		[System.ComponentModel.Browsable(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public object AttributeValue
		{
			get
			{
				return Points;
			}
			set
			{
				Points = value as List<double[]>;
			}
		}

		#endregion
	}
}
