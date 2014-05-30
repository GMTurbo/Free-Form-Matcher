using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NsNodes;

namespace NsNodeControls
{
	public partial class NsNodeGridView : UserControl, INodeView
	{
		/// <summary>
		/// Opens a new modeless dialog box with a grid
		/// </summary>
		/// <param name="node">the node to display</param>
		/// <param name="owner">the parent form to attach to, can be null</param>
		/// <returns>the form containing the grid</returns>
		public static Form Show(NsNode node, Form owner)
		{
			Form f = new Form();
			f.Owner = owner;
			f.SuspendLayout();
			NsNodeGridView flow = new NsNodeGridView();
			flow.Dock = DockStyle.Fill;
			f.Controls.Add(flow);
			f.ResumeLayout();
			flow.Node = node;
			f.StartPosition = FormStartPosition.CenterParent;
			f.FormBorderStyle = FormBorderStyle.SizableToolWindow;
			f.Width = 600;
			f.Height = 300;
			f.Text = node.Label;
			f.Show();
			f.BringToFront();
			flow.ReadNode();
			return f;
		}
		public NsNodeGridView()
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
					m_node.Attach(this);
			}
		}

		public NsNode ReadNode()
		{
			if (Node == null)
				return null;
			Grid.Rows.Clear();
			if (Node.Count == 0)
				return null;
			Node.PauseUpdating();
			SetColumns(Node[0]);
			int i = 0;
			object[] cols;
			foreach (NsNode n in Node)
			{
				cols = new object[n.Attributes.Count+1];
				i = 0;
				cols[0] = n.Label;
				foreach (IAttribute a in n.Attributes)
					cols[++i] = a.Value;

				i = Grid.Rows.Add(cols);
				Grid.Rows[i].Tag = n;
			}
			Node.ResumeUpdating(false);
			return Node;
		}

		void SetColumns(NsNode n)
		{
			Grid.Columns.Clear();
			Grid.Columns.Add("Label", "Label");
			foreach (IAttribute a in n.Attributes)
				Grid.Columns.Add(a.Label, a.Label);
		}

		public NsNode WriteNode()
		{
			if (Node == null)
				return null;

			NsNode rowNode;
			string label;
			for(int i=0; i< Grid.Rows.Count; i++ )
			{
				if (Grid.Rows[i].Cells[0].Value == null || Grid.Rows[i].Cells[0].Value as string != "")
					continue;

				label = Grid.Rows[i].Cells[0].Value as string;
				rowNode = Node.FindNode(label);
				if (rowNode == null)
				{
					continue;
					//rowNode = Node.Add(new NsNode(label));
				}

				for( int j = 1; j < Grid.Columns.Count; j++)
				{
					//update the attribute's value
					try
					{
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
			return Node;
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
			ReadNode();
		}
		public void OnNodeRemoved(object sender, NodeEventArgs e)
		{
		}

		public void Attach(INodeCtrl ictrl)
		{
			throw new NotImplementedException();
		}


		#endregion

		#region INodeView Members


		public void OnNodeSelected(object sender, NodeEventArgs e)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
