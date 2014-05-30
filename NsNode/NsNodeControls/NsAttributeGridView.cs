using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NsNodes;

namespace NsNodeControls
{
	public partial class NsAttributeGridView : UserControl, INodeView
	{
		/// <summary>
		/// Opens a new modeless dialog box with a grid
		/// </summary>
		/// <param name="node">the node to display</param>
		/// <param name="owner">the parent form to attach to, can be null</param>
		/// <returns>the form containing the grid</returns>
		public static Form Show(NsNode node, Form owner)
		{
			if (node == null)
				return null;

			Form f = new Form();
			f.Owner = owner;
			f.SuspendLayout();
			NsAttributeGridView grid = new NsAttributeGridView();
			grid.Dock = DockStyle.Fill;
			f.Controls.Add(grid);
			f.ResumeLayout();
			grid.Node = node;
			f.StartPosition = FormStartPosition.CenterParent;
			f.FormBorderStyle = FormBorderStyle.SizableToolWindow;
			f.Width = 600;
			f.Height = 300;
			f.Text = node.Label;
			f.Show();
			f.BringToFront();
			grid.ReadNode();
			return f;
		}

		public NsAttributeGridView()
		{
			InitializeComponent();
		}

		public DataGridView Grid
		{
			get { return dataGridView1; }
		}

		NsNode m_node = null;
		public NsNode Node
		{
			get { return m_node; }
			set
			{
				if (m_node != null)
					m_node.Detach(this);
				m_node = value;
				if (m_node != null)
				{
					m_node.Attach(this);
					//ReadNode();
				}
			}
		}

		public NsNode ReadNode()
		{
			Grid.Rows.Clear();
			if (Node == null )
				return null;
			if (Node.Count == 0)
				return null;

			Node.PauseUpdating();
			int cnt = SetColumns(Node[0]);
			int i = 0;
			object[] cols;
			IAttribute atr = null;
			foreach (NsNode n in Node)
			{
				cols = new object[cnt];
				i = 0;
				cols[0] = n.Label;
				for (i = 1; i < cols.Length; i++)
				{
					atr = n[Grid.Columns[i].HeaderText];
					if (atr != null)
						cols[i] = atr.Value;
				}
				//foreach (IAttribute a in n.Attributes)
				//    cols[++i] = a.Value;

				i = Grid.Rows.Add(cols);
				Grid.Rows[i].Tag = n;
			}
			Node.ResumeUpdating(false);
			return Node;
		}

		/// <summary>
		/// Sets the column types and headers using the passed node as a template. 1 column per attribute
		/// </summary>
		/// <param name="n">the node used to set the columns</param>
		public int SetColumns(NsNode n)
		{
			if (n != null)
			{
				//store a template node for creating new 
				m_template = new NsNode("temparent");
				m_template = n.CopyTo(m_template);
				m_template.Label = "template";
				m_template.Parent = null;//delete temparent
			}
			if (m_template == null)
				return -1;

			
			Grid.Columns.Clear();

			//label
			Grid.Columns.Add("Label", "Label");

			//attributes
			DataGridViewColumn col = null;
			foreach (IAttribute a in m_template.Attributes)
			{
				if (a is DoubleAttribute)
				{
					col = new DataGridViewTextBoxColumn();
					col.ValueType = typeof(double);
					col.DefaultCellStyle.Format = "#0.000";
					col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
				}
				else if (a is BoolAttribute)
				{
					col = new DataGridViewCheckBoxColumn();
					col.ValueType = typeof(bool);
				}
				else if (a is IntAttribute)
				{
					col = new DataGridViewTextBoxColumn();
					col.ValueType = typeof(int);
					col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
				}
				else if (a is DateAttribute)
				{
					col = new DataGridViewTextBoxColumn();
					col.ValueType = typeof(DateTime);
				}
				else
					col = new DataGridViewTextBoxColumn();

				col.HeaderText = a.Label;
				col.Name = a.Label;
				col.AutoSizeMode = ColumnMode;
				//Grid.Columns.Add(a.Label, a.Label);
				Grid.Columns.Add(col);
			}
			if (ReadOnlys != null)
			{
				foreach (int i in ReadOnlys)
				{
					if ( i < 0 || Grid.Columns.Count <= i)
						continue;
					Grid.Columns[i].ReadOnly = true;
					Grid.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
				}
			}
			return Grid.Columns.Count;
		}
		NsNode m_template = null;

		DataGridViewAutoSizeColumnMode colmode = DataGridViewAutoSizeColumnMode.Fill;
		public DataGridViewAutoSizeColumnMode ColumnMode
		{
			get { return colmode; }
			set { colmode = value; }
		}

		public List<int> ReadOnlys
		{
			set { m_readonlys = value; }
			get { return m_readonlys; }
		}
		List<int> m_readonlys = new List<int>();

		public NsNode WriteNode()
		{
			if (Node == null)
				Node = new NsNode();
			Node.PauseUpdating();

			NsNode rowNode;
			string label;
			for(int i=0; i< Grid.Rows.Count; i++ )
			{
				if (Grid.Rows[i].Cells[0].Value == null || Grid.Rows[i].Cells[0].Value as string == "")
					continue;

				label = Grid.Rows[i].Cells[0].Value as string;
				rowNode = Grid.Rows[i].Tag as NsNode;
				if (rowNode == null)//newly inputted row
				{
					if (m_template != null)//copy node format from template
					{
						rowNode = m_template.CopyTo(Node); 
						rowNode.Label = label;
					}
					else //try to construct from the types (shouldn't happen)
					{
						rowNode = Node.Add(new NsNode(label));
						for (int col = 1; col < Grid.Columns.Count; col++)
						{
							IAttribute atr = CreateAttribute(rowNode, col);
							if (atr != null)
								rowNode.Add(atr);
						}
					}
					Grid.Rows[i].Tag = rowNode;
				}

				for( int j = 1; j < Grid.Columns.Count; j++)
				{
					//update the attribute's value
					try
					{
						if (Grid[j, i].Value == null)
							continue;
						rowNode[Grid.Columns[j].HeaderText].Value = Grid[j, i].Value;
					}
					catch (ArgumentOutOfRangeException e)
					{
						MessageBox.Show(e.Message);
					}
					catch
					{
						MessageBox.Show(string.Format("Invalid format in {2}[{0}, {1}]", j, i, Node.Label));
					}
				}
			}
			Node.ResumeUpdating(false);
			return Node;
		}

		IAttribute CreateAttribute(NsNode parent, int col)
		{
			if (Grid.Columns[col].ValueType == typeof(double))
				return new DoubleAttribute(parent, Grid.Columns[col].HeaderText, 0 );
			else if (Grid.Columns[col].ValueType == typeof(int))
				return new IntAttribute(parent, Grid.Columns[col].HeaderText, 0 );
			else if (Grid.Columns[col].ValueType == typeof(bool))
				return new BoolAttribute(parent, Grid.Columns[col].HeaderText, false);
			else
				return new StringAttribute(parent, Grid.Columns[col].HeaderText, "");
		}
		//public void SetReadOnly(params int[] columns)
		//{
		//     foreach( int i in columns )
		//          if( i < Grid.Columns.Count )
		//               Grid.Columns[i].ReadOnly = true;
		//}

		#region INodeView Members

		public void OnNodeUpdated(object sender, NodeEventArgs e)
		{
			if( e.Node == Node )
				ReadNode();
			else if (Node == null && e.Node != null)
			{
				Node = e.Node;
				ReadNode();
			}
		}
		public void OnNodeRemoved(object sender, NodeEventArgs e)
		{
		}

		public void Attach(INodeCtrl ictrl)
		{
			ictrl.SelectionChanged += new NsNodeEventHandler(ictrl_SelectionChanged);
		}

		void ictrl_SelectionChanged(object sender, NodeEventArgs e)
		{
			Node = e.Node;
			ReadNode();
		}

		public void OnNodeSelected(object sender, NodeEventArgs e)
		{
		}

		#endregion
	}
}
