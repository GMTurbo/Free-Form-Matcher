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
	public partial class NodeFlow : Form
	{
		public NodeFlow()
			:this(null)
		{
		}
		public NodeFlow(NsNode node)
		{
			InitializeComponent();
			m_node = node;
			if (node != null)
				UpdateFlow();
		}
		NsNode m_node;
		FlowLayoutPanel Flow
		{
			get { return flowLayoutPanel1; }
		}
		void UpdateFlow()
		{
			this.SuspendLayout();
			this.Controls.Clear();
			CreateFlow(m_node);
			Flow.Dock = DockStyle.Fill;
			Flow.FlowDirection = FlowDirection.TopDown;
			Flow.WrapContents = false;
			Flow.AutoScroll = true;
			Flow.Invalidate(true);
			Flow.PerformLayout();
			this.Controls.Add(Flow);
			this.Refresh();
		}
		FlowLayoutPanel CreateFlow(NsNode node)
		{
			Flow.Controls.Clear();
			if (m_node == null)
				return null;
			if (node.Parent != null)
			{
				Button b = new Button();
				b.Text = "Parent";
				b.Tag = node.Parent;
				b.Click += new EventHandler(b_Click);
				Flow.Controls.Add(b);
			}

			foreach (NsNode n in node)
			{
				Button b = new Button();
				b.Text = n.Label;
				b.Tag = n;
				b.Click += new EventHandler(b_Click);
				Flow.Controls.Add(b);
			}
			Label lb;
			foreach (IAttribute atr in node.Attributes)
			{
				if (atr is DoubleAttribute)
				{
					NsNodeControls.ValueBox vb = new NsNodeControls.ValueBox();
					vb.Units = "";
					vb.Text = atr.Label;
					vb.Value = (double)atr.Value;
					Flow.Controls.Add(vb);
				}
				else
				{
					lb = new Label();
					lb.Text = atr.ToString();
					Flow.Controls.Add(lb);
				}
			}
			return Flow;
		}

		void b_Click(object sender, EventArgs e)
		{
			Button b = sender as Button;
			CreateFlow(b.Tag as NsNode);
		}
		//FlowLayoutPanel CreateFlow(NsNode node)
		//{
		//     FlowLayoutPanel fp = new FlowLayoutPanel();
		//     GroupBox gb = new GroupBox();
		//     gb.AutoSize = true;
		//     gb.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		//     gb.Text = node.Label;
		//     TableLayoutPanel attr = new TableLayoutPanel();
		//     attr.AutoSize = true;
		//     attr.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		//     Label lb;
		//     foreach (IAttribute atr in node.Attributes)
		//     {
		//          lb = new Label();
		//          lb.Text = string.Format("{0}={1}", atr.Label, atr.Value.ToString());
		//          attr.Controls.Add(lb);
		//     }
		//     attr.Dock = DockStyle.Fill;
		//     gb.Controls.Add(attr);
		//     fp.Controls.Add(gb);
		//     foreach (NsNode n in node)
		//     {
		//          fp.Controls.Add(CreateFlow(n));
		//     }
		//     return fp;
		//}

	}
}
