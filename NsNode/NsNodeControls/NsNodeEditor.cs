using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NsNodes;

namespace NsNodes
{
	public partial class NsNodeEditor : Form
	{
		public NsNodeEditor(NsNode node)
		{
			InitializeComponent();
			m_node = node;
			ReadNode();
		}
		NsNode m_node;

		public NsNode Node
		{
			get { return m_node; }
			set
			{
				m_node = value;
				ReadNode();
			}
		}
		String Label
		{
			get
			{
				return textBox1.Text; 
			}
			set
			{
				textBox1.Text = value;
			}
		}

		void ReadNode()
		{
			Label = Node.Label;

			//clear the items, not the columns
			attrList.Items.Clear();
			nodeList.Clear();

			List<string> list = new List<string>(2);
			foreach (IAttribute kvp in Node.Attributes)
			{
				attrList.Items.Add(CreateAttributeItem(kvp));
			}
			foreach (NsNode n in Node)
			{
				nodeList.Items.Add(CreateNodeItem(n));
			}
			Refresh();
		}
		void WriteNode()
		{
			Node.Label = Label;
			foreach (ListViewItem item in attrList.Items)
			{
				if (item.Tag != null)
				{
					IAttribute atr = item.Tag as IAttribute;
					atr.Value = item.SubItems[1].Text;
					if (!Node.Attributes.Contains(atr))
						Node.Add(atr);
				}
				else
				{
					StringAttribute atr = new StringAttribute(Node, item.SubItems[0].ToString(), item.SubItems[1].ToString());
					item.Tag = atr;
					Node.Add(atr);
				}
					//Node.Attributes[item.SubItems[0]] = item.SubItems[1].Text;
			}
			foreach (ListViewItem item in nodeList.Items)
			{
				if( item.Tag is NsNode )
					Node.Add(item.Tag as NsNode); //attempt to add, wont double add node
			}
			Node.Update();
		}

		ListViewItem CreateNodeItem(NsNode n)
		{
			ListViewItem item;
			item = new ListViewItem(n.Label);
			item.Tag = n;
			return item;
		}
		ListViewItem CreateAttributeItem(IAttribute at)
		{
			ListViewItem item;
			List<string> list = new List<string>(2);
			list.Clear();
			list.Add(at.Label);
			list.Add(at.Value.ToString());
			item = new ListViewItem(list.ToArray());
			item.Tag = at;
			return item;
		}


		private void AddNode()
		{
			NsNode node = new NsNode("new", Node);
			NsNodeEditor edit = new NsNodeEditor(node);
			if (edit.ShowDialog() == DialogResult.OK)
			{
				nodeList.Items.Add(CreateNodeItem(node));
				nodeList.Refresh();
			}
		}
		private void AddAttribute()
		{
			AttributeEditor ed = new AttributeEditor(true);
			ed.Key = "key";
			ed.Value = "value";
			if (ed.ShowDialog() == DialogResult.OK)
			{
				attrList.Items.Add(CreateAttributeItem(new StringAttribute(Node, ed.Key, ed.Value)));
				attrList.Refresh();
			}
		}

		private void EditSelectedAttribute()
		{
			if (attrList.SelectedItems != null && attrList.SelectedItems.Count > 0)
			{
				AttributeEditor ed = new AttributeEditor(false);
				ed.Key = attrList.SelectedItems[0].SubItems[0].Text;
				ed.Value = attrList.SelectedItems[0].SubItems[1].Text;
				if (ed.ShowDialog() == DialogResult.OK)
				{
					attrList.SelectedItems[0].SubItems[1].Text = ed.Value;
				}
			}
		}
		void EditSelectedNode()
		{
			if (nodeList.SelectedItems != null && nodeList.SelectedItems.Count > 0)
			{
				NsNode n = nodeList.SelectedItems[0].Tag as NsNode;
				if (n != null)
				{
					NsNodeEditor edit = new NsNodeEditor(n);
					if (edit.ShowDialog() == DialogResult.OK)
						nodeList.SelectedItems[0].Text = n.Label;
				}
			}
		}

		private void addToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (nodeList.Focused)
			{
				AddNode();
			}
			else if (attrList.Focused)
			{
				AddAttribute();
			}
		}

		private void editToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (nodeList.Focused)
			{
				EditSelectedNode();
			}
			else if (attrList.Focused)
			{
				EditSelectedAttribute();
			}
		}

		private void attrList_DoubleClick(object sender, EventArgs e)
		{
			if ( attrList.SelectedItems != null)
				EditSelectedAttribute();
			else
				AddAttribute();
		}

		private void nodeList_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (nodeList.SelectedItems != null)
				EditSelectedNode();
			else
				AddNode();
		}

		private void okBtn_Click(object sender, EventArgs e)
		{
			WriteNode();
		}

	}
}
