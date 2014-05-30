using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NsNodes;

namespace NodeTester
{
	public partial class TreeForm : Form
	{
		public TreeForm()
		{
			InitializeComponent();
            NsNode node = new NsNode("root");
            node.Attach(nsNodeTree1);
            node.Update();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			NsNodes.NsNode node = nsNodeTree1.SelectedNode;
			NodeFlow flow ;
			if (node != null)
			{
				flow = new NodeFlow(node);
				flow.Show();
			}
		}
	}
}
