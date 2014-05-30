using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NsNodes;

namespace NsNodeControls
{
	public partial class NsNodePlot : UserControl, INodeView
	{
		public NsNodePlot()
			:this(null)
		{

		}
		public NsNodePlot(NsNode node)
		{
			InitializeComponent();
			m_node = node;
		}

		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			ReNode();
		}

		NsNode m_node;
		public NsNode Node
		{
			get
			{
				return m_node;
			}
			set
			{
				//if (m_node == value) return;
				if (m_node != null)
					m_node.Detach(this);
				m_node = value;
				Data.Node = value;
				if (m_node != null)
					m_node.Attach(this);
			}
		}

		public string LabelX
		{ 
			get { return comboX.SelectedItem == null ? comboX.Text : comboX.SelectedItem.ToString(); }
			set
			{
				int i = comboX.FindStringExact(value);
				if (i > 0)
					comboX.SelectedIndex = i;
			}

		}
		public string LabelY
		{
			get { return comboY.SelectedItem == null ? comboY.Text : comboY.SelectedItem.ToString(); }
			set
			{
				int i = comboY.FindStringExact(value);
				if (i > 0)
					comboY.SelectedIndex = i;
			}
		}

		public bool ShowGrid
		{
			get { return !splitContainer1.Panel1Collapsed; }
			set
			{
				splitContainer1.Panel1Collapsed = !value;
			}
		}
		NsAttributeGridView Data
		{
			get { return nsAttributeGridView1; }
		}
		NPlot.Windows.PlotSurface2D nplot
		{
			get { return plotSurface2D1; }
		}

		private void ReNode()
		{
			Data.ReadNode();
			FillCombos();
			nplot.Clear();
			//RePlot();
		}

		void FillCombos()
		{
			comboX.Items.Clear();
			comboY.Items.Clear();
			foreach (DataGridViewColumn col in Data.Grid.Columns)
			{
				comboX.Items.Add(col.HeaderText);
				comboY.Items.Add(col.HeaderText);
			}
		}

		private void combo_SelectedIndexChanged(object sender, EventArgs e)
		{
			//RePlot();
			nplot.Clear();
		}
		private void RePlot()
		{
			nplot.Clear();

			// ensure user has selected both x and y
			if (Node == null || LabelX == null || LabelY == null)
				return;



			//find the associated col index
			int colx = -1, coly = -1, i = 0;
			foreach (DataGridViewColumn col in Data.Grid.Columns)
			{
				if (col.HeaderText == LabelX)
					colx = i;
				if (col.HeaderText == LabelY)
					coly = i;
				i++;
			}

			if (colx == -1 || coly == -1)
				return;

			// get the data from the gridview
			List<double> x = new List<double>(Data.Grid.Rows.Count);
			List<double> y = new List<double>(Data.Grid.Rows.Count);
			double tx, ty;
			foreach (DataGridViewRow row in Data.Grid.Rows)
			{
				try
				{
					if (row.Cells[colx].Value == null || row.Cells[coly].Value == null)
						continue;
					tx = Double.Parse(row.Cells[colx].Value.ToString());
					ty = Double.Parse(row.Cells[coly].Value.ToString());
					x.Add(tx);
					y.Add(ty);
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message);
				}
			}

			nplot.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

			//Add a background grid for better chart readability.
			NPlot.Grid grid = new NPlot.Grid();
			grid.VerticalGridType = NPlot.Grid.GridType.Coarse;
			grid.HorizontalGridType = NPlot.Grid.GridType.Coarse;
			grid.MinorGridPen = new Pen(Color.Blue, 1.0f);
			grid.MajorGridPen = new Pen(Color.LightGray, 1.0f);
			nplot.Add(grid);


			//create a lineplot from it
			NPlot.LinePlot lp = new NPlot.LinePlot();
			lp.AbscissaData = x;
			lp.OrdinateData = y;
			nplot.Add(lp);

			//point plot for showing points
			NPlot.PointPlot pp = new NPlot.PointPlot();
			pp.AbscissaData = x;
			pp.OrdinateData = y;
			nplot.Add(pp);

			//format axes labels
			nplot.YAxis1.Label = LabelY;
			nplot.YAxis1.LabelFont = new Font(this.Font, FontStyle.Regular);
			nplot.YAxis1.LabelOffsetAbsolute = true;
			nplot.YAxis1.LabelOffset = 40;

			nplot.XAxis1.Label = LabelX;
			nplot.XAxis1.LabelFont = new Font(this.Font, FontStyle.Regular);
			nplot.XAxis1.HideTickText = false;
			nplot.Padding = 5;

			//enable dragging etc
			nplot.RightMenu = NPlot.Windows.PlotSurface2D.DefaultContextMenu;
			nplot.AddInteraction(new NPlot.Windows.PlotSurface2D.Interactions.AxisDrag(false));
			nplot.AddInteraction(new NPlot.Windows.PlotSurface2D.Interactions.HorizontalDrag());
			nplot.AddInteraction(new NPlot.Windows.PlotSurface2D.Interactions.VerticalDrag());

			nplot.Refresh();

		}

		/// <summary>
		/// If true Node must be set manually, if false Node will be set by attached INodeCtrl
		/// Default is false.
		/// </summary>
		public bool StaticNode = false;
		private string m_nodePath = "";
		public string NodePath
		{
			get { return m_nodePath; }
			set { m_nodePath = value; }
		}

		#region INodeView Members

		public void OnNodeUpdated(object sender, NodeEventArgs e)
		{
			if( e.Node == null )
				return;

			if (StaticNode)
			{
				if (Node == null || Node.Root != e.Node.Root)
					Node = e.Node.Root.FindNode(NodePath);
			}
			else
				Node = e.Node;
		}

		public void OnNodeRemoved(object sender, NodeEventArgs e)
		{
			if (e.Node == Node)
			{
				Node = null;
				ReNode();
			}
		}

		public void Attach(INodeCtrl ictrl)
		{
			ictrl.SelectionChanged += OnNodeUpdated;
		}

		public void OnNodeSelected(object sender, NodeEventArgs e)
		{
			//throw new NotImplementedException();
		}
		#endregion

		private void plotBtn_Click(object sender, EventArgs e)
		{
			RePlot();
		}
	}
}
