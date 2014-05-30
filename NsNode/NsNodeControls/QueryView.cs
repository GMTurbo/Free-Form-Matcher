using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NsNodes;

namespace NsNodes
{
	public partial class QueryView : UserControl, INodeView
	{
		public QueryView()
		{
			InitializeComponent();
			m_root = null;
		}

		string ActiveQuery
		{
			get { return comboBox1.Text; }
		}

		NsNode Root
		{
			get { return m_root; }
		}
		NsNode m_root;

		DataGridView Grid
		{
			get { return dataGridView1; }
		}

		#region INodeView Members

		public void OnNodeUpdated(object sender, NodeEventArgs e)
		{
			if (ActiveQuery != null && ActiveQuery != "")
				DoQuery();
		}

		#endregion

		public void Attach(INodeCtrl ictrl)
		{
			ictrl.SelectionChanged += new NsNodeEventHandler(ictrl_SelectionChanged);
		}

		void ictrl_SelectionChanged(object sender, NodeEventArgs e)
		{
			m_root = e.Node;
			if (ActiveQuery != null && ActiveQuery != "")
				DoQuery();
		}

		public void DoQuery()
		{
			AddSelect(ActiveQuery);
			if (Root == null)
			{
				MessageBox.Show("Please select a node to start the query at");
				return;
			}
			if (ActiveQuery == null || ActiveQuery == "")
			{
				MessageBox.Show("Please enter a query string");
				return;
			}
			List<IAttribute> results = new List<IAttribute>(5);

			Root.SimpleQuery(ActiveQuery, results, true);
			//{
			//     MessageBox.Show("Query failed\nPartial results displayed");
			//}
			dataGridView1.Rows.Clear();
			DataGridViewRow row;
			foreach (IAttribute entry in results)
			{
				row = new DataGridViewRow();
				row.CreateCells(dataGridView1, entry.Parent.Label, entry.Label, entry.Value);
				row.Tag = entry;
				Grid.Rows.Add(row);
			}
		}
		void AddSelect(string query)
		{
			if (!comboBox1.Items.Contains(query))
				comboBox1.Items.Add(query);
			comboBox1.SelectedItem = query;

		}
		private void query_Click(object sender, EventArgs e)
		{
			DoQuery();
		}

		private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Return)
			{
				DoQuery();
				e.Handled = true;
			}
		}


		#region INodeView Members


		public void OnNodeRemoved(object sender, NodeEventArgs e)
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
