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
	public partial class NsNodeFlow : UserControl, INodeView, INodeCtrl
	{
		/// <summary>
		/// Opens a new modeless dialog box with a flow
		/// </summary>
		/// <param name="node">the node to display</param>
		/// <param name="owner">the parent form to attach to, can be null</param>
		/// <returns>the form containing the flow</returns>
		public static Form Show(NsNode node)
		{
			Form f = new Form();
			f.SuspendLayout();
			NsNodeControls.NsNodeFlow flow = new NsNodeControls.NsNodeFlow();
			flow.Dock = DockStyle.Fill;
			f.Controls.Add(flow);
			f.ResumeLayout();
			flow.Node = node;
			f.StartPosition = FormStartPosition.CenterParent;
			f.FormBorderStyle = FormBorderStyle.SizableToolWindow;
			f.Width = 150;
			f.Height = 300;
			f.Text = "";
			flow.SelectionChanged += delegate(object s, NodeEventArgs ne)
			{
				if (ne.Node == null)
					f.Close();
			};
			f.Show();
			f.BringToFront();
			return f;
		}
		/// <summary>
		/// Constructs a node flow on a given node
		/// </summary>
		/// <param name="node">the node to flow</param>
		public NsNodeFlow(NsNode node)
		{
			InitializeComponent();
			InitializeFlow();
			m_node = node;
			if (Node != null)
				Reload();
		}
		public NsNodeFlow()
			: this(null)
		{
		}

		void InitializeFlow()
		{
			Flow.Dock = DockStyle.Fill;
			Flow.FlowDirection = FlowDirection.TopDown;
			Flow.WrapContents = false;
			Flow.AutoScroll = true;
		}

		/// <summary>
		/// the NsNode shown in the flow, raises a SelectionChanged event when changed
		/// </summary>
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
				if (m_node != null)
					m_node.Attach(this);
				if (SelectionChanged != null)
					SelectionChanged(this, new NodeEventArgs(m_node));
				Reload();
			}
		}
		NsNode m_node;
		FlowLayoutPanel Flow
		{
			get { return flowLayoutPanel1; }
		}

		#region Apply Reload

		private void Apply()
		{
			foreach (Control c in Flow.Controls)
			{
				if (c is TextBox)
					UpdateLabel(c);
				else
					UpdateAttribute(c); //update all attributes on apply 
			}
			UpdateNode();
		}
		void Reload()
		{
			Flow.Controls.Clear();
			if (m_node == null)
			{
				toolStripParent.Enabled = false;
				return;
			}

			Flow.SuspendLayout();

			if (Node.Parent != null)
			{
				toolStripParent.Enabled = true;
			}
			else
			{
				toolStripParent.Enabled = false;
			}


			// Node Label
			TextBox t = new TextBox();
			t.Text = Node.Label;
			//t.KeyUp += new KeyEventHandler(label_KeyUp);
			//t.Validated += new EventHandler(label_Validated);
			t.Width = Flow.Width - 10;
			Flow.Controls.Add(t);

			Button b = new Button();
			//Node Buttons
			foreach (NsNode n in Node)
			{
				b = new Button();
				b.Text = n.Label;
				b.Tag = n;
				b.Click += new EventHandler(subnode_Click);
				Flow.Controls.Add(b);
			}
			//Attrbute editors
			foreach (IAttribute atr in Node.Attributes)
			{
				if (atr is DoubleAttribute || atr is IntAttribute)
				{
					ValueBox vb = new ValueBox();
					vb.Units = "";
					vb.Text = atr.Label;
					if (atr is IntAttribute)
						vb.Value = (double)(int)atr.Value;
					else
						vb.Value = (double)atr.Value;
					Flow.Controls.Add(vb);
					//vb.TBWidth = Flow.Width / 3;
				}
				else if (atr is StringAttribute)
				{
					LabelTextbox lb = new LabelTextbox();
					lb.Label = atr.Label;
					lb.Text = atr.Value as string;
					Flow.Controls.Add(lb);
				}
				else if (atr is Point3Attribute)
				{
					PointBox pb = new PointBox();
					pb.Text = atr.Label;
					pb.Units = "";
					pb.Point3D = true;
					pb.Point = atr.Value as double[];
					Flow.Controls.Add(pb);
				}
				else if (atr is PointAttribute)
				{
					PointBox pb = new PointBox();
					pb.Text = atr.Label;
					pb.Units = "";
					pb.Point3D = false;
					pb.Point = atr.Value as double[];
					Flow.Controls.Add(pb);
				}
				else if (atr is BoolAttribute)
				{
					AttributeCheckBox cb = new AttributeCheckBox();
					cb.Text = atr.Label;
					cb.Checked = (bool)atr.Value;
					cb.CheckAlign = ContentAlignment.MiddleRight;
					Flow.Controls.Add(cb);
				}
				else if (atr is PolylineAttribute)
				{
					PointList pl = new PointList();
					pl.Label = atr.Label;
					pl.Points = atr.Value as List<double[]>;
					Flow.Controls.Add(pl);
				}
				else if (atr is DateAttribute)
				{
					DateAttributeEditor dtp = new DateAttributeEditor();
					dtp.Text = atr.Label;
					dtp.AttributeValue = atr.Value;
					Flow.Controls.Add(dtp);
				}
				else
				{
					Label lb = new Label();
					lb.Text = atr.Label + "=" + atr.Value;
					Flow.Controls.Add(lb);
				}
				Flow.Controls[Flow.Controls.Count - 1].Tag = atr;
			}
			for (int i = 0; i < Flow.Controls.Count; i++)
			{
				//Flow.Controls[i].Width = Flow.Width - 10;
				//Flow.Controls[i].Validated += new EventHandler(NsNodeFlow_Validated);
			}
			//force a resize
			Flow.ResumeLayout(true);
			NsNodeFlow_Resize(this, new EventArgs());
		}

		private void UpdateNode()
		{
			if (Node != null)
				Node.Update(this);
		}
		private void UpdateLabel(object sender)
		{
			if ((sender as TextBox).Text != "" && Node != null)
			{
				Node.Label = (sender as TextBox).Text;
				//UpdateNode();
			}
		}
		bool UpdateAttribute(object sender)
		{
			if (sender is Control && sender is IAttributeEditor)
				if ((sender as Control).Tag is IAttribute)
				{
					((sender as Control).Tag as IAttribute).Value = (sender as IAttributeEditor).AttributeValue;
					//UpdateNode();
					return true;
				}
			return false;
		}

		#endregion

		#region Toolstrip

		private void toolStripX_Click(object sender, EventArgs e)
		{
			Node = null;
		}
		private void toolStripParent_Click(object sender, EventArgs e)
		{
			Node = Node.Parent;
		}

		private void applybtn_Click(object sender, EventArgs e)
		{
			Apply();
		}
		private void reloadbtn_Click(object sender, EventArgs e)
		{
			Reload();
		}

		void subnode_Click(object sender, EventArgs e)
		{
			if (sender is Control)
			{
				Node = (sender as Control).Tag as NsNode;
				if (SelectionChanged != null)
					SelectionChanged(this, new NodeEventArgs(Node));
			}
		}

		private void NsNodeFlow_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F5)
				Apply();
			if (e.KeyCode == Keys.F9)
				Reload();
		}

		#endregion

		#region Resize

		int CtrlWidth
		{
			get
			{
				if (Flow.VerticalScroll.Visible)
					return Flow.Width - 28;
				return Flow.Width - 10;
			}
		}
		private void NsNodeFlow_Resize(object sender, EventArgs e)
		{
			int w = CtrlWidth;
			foreach (Control c in Flow.Controls)
			{
				c.Width = w;
				if (c is ValueBox)
					(c as ValueBox).TBWidth = Flow.Width / 3;
			}
		}
		
		#endregion

		#region INodeView Members

		public void OnNodeUpdated(object sender, NodeEventArgs e)
		{
			Node = e.Node;
		}
		public void OnNodeRemoved(object sender, NodeEventArgs e)
		{
			if (e.Node == Node)
				Node = null;
			if (e.Node != null && Node.HasChild(e.Node))
				Reload();
		}
		public void OnNodeSelected(object sender, NodeEventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void Attach(INodeCtrl ictrl)
		{
			ictrl.SelectionChanged += OnNodeUpdated;
		}

		#endregion

		#region INodeCtrl Members

		public event NsNodeEventHandler SelectionChanged;
		public event NsNodeEventHandler NodeCopied;

		#endregion

		//void label_KeyUp(object sender, KeyEventArgs e)
		//{
		//    if (e.KeyCode == Keys.Enter)
		//    {
		//        if (sender is TextBox)
		//        {
		//            UpdateLabel(sender);
		//            e.Handled = true;
		//        }
		//        else if (UpdateAttribute(sender))
		//            e.Handled = true;
		//    }
		//}

		//void label_Validated(object sender, EventArgs e)
		//{
		//    //UpdateLabel(sender);
		//}
		//void NsNodeFlow_Validated(object sender, EventArgs e)
		//{
		//    //UpdateAttribute(sender);
		//}
	}
}
