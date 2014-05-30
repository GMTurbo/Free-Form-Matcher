using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NsNodes;
using NsNodeControls;

namespace NsNodes
{
	public delegate void NodeLoadedEventHandler(object sender, NodeLoadedEventArgs e);

	public partial class NsNodeTree : UserControl, INodeView, INodeCtrl
	{
		string m_extension = null;

		/// <summary>
		/// Set the file extension for this instance of NsNode (defaults to xml)
		/// </summary>
		public string Extension
		{
			get { return m_extension; }
			set { m_extension = value; }
		}

		/// <summary>
		/// Opens a new modeless dialog box with a tree
		/// </summary>
		/// <param name="node">the node to display</param>
		/// <param name="owner">the parent form to attach to, can be null</param>
		/// <returns>the form containing the tree</returns>
		public static Form Show(NsNode node, Form owner)
		{
			Form f = new Form();
			f.Owner = owner;
			f.SuspendLayout();
			NsNodeTree flow = new NsNodeTree();
			flow.Dock = DockStyle.Fill;
			f.Controls.Add(flow);
			f.ResumeLayout();
			flow.AddRootNode(node);
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
			f.FormClosed += delegate(object s, FormClosedEventArgs e)
			{
				if (flow.SelectedRoot != null)
					flow.SelectedRoot.Detach(flow);
			};
			f.Show();
			f.BringToFront();
			return f;

		}
		public NsNodeTree()
		{
			InitializeComponent();
			AddMenuItem();
			//deleteToolStripMenuItem.Visible = false;
			addToolStripMenuItem.Visible = false;
		}
		TreeNodeCollection Nodes
		{
			get { return treeView1.Nodes; }
		}
		TreeView Tree
		{
			get { return treeView1; }
		}
		public int Count
		{
			get { return Nodes.Count; }
		}

		#region ExpandToDepth

		public void ExpandToDepth(int depth)
		{
			Tree.BeginUpdate();
			Tree.CollapseAll();
			if (depth > -1)
			{
				TreeNodeCollection nodes = Nodes;
				foreach (TreeNode tn in Nodes)
					RecursiveExpand(tn, depth);
			}
			Tree.EndUpdate();
		}

		void RecursiveExpand(TreeNode tn, int depth)
		{
			tn.Expand();
			if (depth == 0)
				return;
			else
				foreach (TreeNode child in tn.Nodes)
					RecursiveExpand(child, depth - 1);
		}

		#endregion

		#region Selection

		/// <summary>
		/// The currently selected item's root node
		/// </summary>
		public NsNode SelectedRoot
		{
			get
			{
				if (SelectedNode != null)
					return SelectedNode.Root;
				if (SelectedAttribute != null)
					return SelectedAttribute.Parent.Root;
				return null;
			}
		}
		/// <summary>
		/// the currently selected node, null if an attribute is selected
		/// </summary>
		public NsNode SelectedNode
		{
			get
			{
				if (Tree.SelectedNode == null)
					return null;
				return Tree.SelectedNode.Tag as NsNode;
			}
			set
			{
				if (value == null)
					Tree.SelectedNode = null;
				else
				{
					TreeNode tn = GetTreeNode(value);
					if (tn != null)
						Tree.SelectedNode = tn;
				}
			}
		}
		/// <summary>
		/// the currently selected attribute, null if a node is selected
		/// </summary>
		public IAttribute SelectedAttribute
		{
			get
			{
				if (Tree.SelectedNode == null)
					return null;
				return Tree.SelectedNode.Tag as IAttribute;
			}
			set
			{
				if (value == null)
					Tree.SelectedNode = null;
				else
				{
					TreeNode tn = GetTreeNode(value.Parent);
					if (tn != null)
					{
						foreach (TreeNode ta in tn.Nodes)
						{
							if (ta.Tag == value)
							{
								Tree.SelectedNode = ta;
								return;
							}
						}
						Tree.SelectedNode = tn;//default to parent if child isnt found
					}
				}
			}
		}

		public void SelectRoot()
		{
			if (Nodes.Count > 0)
				Tree.SelectedNode = Tree.Nodes[0];
		}
		#endregion

		#region GetTreeNode

		TreeNode GetTreeNode(NsNode n)
		{
			if (n.Parent == null)//n is root node
				return GetChildNode(n, null);
			return GetChildNode(n, GetParentNode(n));
		}
		TreeNode GetParentNode(NsNode n)
		{
			return GetTreeNode(n.Parent);
		}
		TreeNode GetChildNode(object target, TreeNode parent)
		{
			if (target == null)
				return null;

			if (parent == null) //search root nodes
			{
				foreach (TreeNode tn in Nodes)
					if (tn.Tag == target)
						return tn;
				return null;
			}

			foreach (TreeNode tn in parent.Nodes)
				if (tn.Tag == target)
					return tn;
			return null;
		}

		#endregion

		#region Add and Update

		/// <summary>
		/// Adds a new root node to the tree using the given NsNode
		/// </summary>
		/// <param name="node">the node to add</param>
		public void AddRootNode(NsNode node)
		{
			if (node == null)
				return;
			TreeNode root = new TreeNode();
			UpdateNode(root, node);
			Nodes.Add(root);
			node.Attach(this);
		}

		void UpdateNode(TreeNode tn, NsNode node)
		{
			tn.Tag = node;
			tn.Text = node.Label;
			TreeNode atr;
			//add or update attribute nodes
			foreach (IAttribute attr in node.Attributes)
			{
				atr = GetChildNode(attr, tn);
				if (atr != null) //attribute found, update value
					UpdateAttribute(atr, attr);

				else //attribute not found, add new 
				{
					atr = new TreeNode();
					UpdateAttribute(atr, attr);
					tn.Nodes.Add(atr);
				}
			}
			//add or update subnodes
			foreach (NsNode n in node)
			{
				atr = GetChildNode(n, tn);
				if (atr != null)
					UpdateNode(atr, n);
				else
				{
					atr = new TreeNode();
					UpdateNode(atr, n);
					tn.Nodes.Add(atr);
				}
			}
			//remove deleted nodes
			foreach (TreeNode t in tn.Nodes)
			{
				if (!node.HasChild(t.Tag))
					t.Remove();
			}
		}
		void UpdateAttribute(TreeNode atr, IAttribute attr)
		{
			atr.Text = attr.ToString();
			atr.Tag = attr;
		}

		/// <summary>
		/// Clears the tree
		/// </summary>
		public void Clear()
		{
			NsNode n;
			foreach (TreeNode tn in Tree.Nodes)
			{
				n = tn.Tag as NsNode;
				if (n != null)
					n.Detach(this);
			}
			Tree.Nodes.Clear();
			Refresh();
		}
		void DetachNode(NsNode node)
		{
			node.Detach(this);
			foreach (NsNode n in node)
				DetachNode(n);
		}

		#endregion

		#region INodeView Members

		public void OnNodeUpdated(object sender, NodeEventArgs e)
		{
			Tree.BeginUpdate();
			TreeNode tn = null;

			NsNode node = e.Node;
			if (node != null)//attempt to find an existing treenode
				tn = GetTreeNode(node);

			if (tn != null)//update it if found
				UpdateNode(tn, node);
			else
			{
				if (sender is NsNode) //check parent if not
				{
					node = sender as NsNode;
					//if (node != e.Node)//view is attached to the parent of the updated node
					//{
					tn = GetTreeNode(node);//look for the parent
					if (tn != null)
						UpdateNode(tn, node);//update if found
				}
			}

			if (tn == null)
				AddRootNode(node);//add as a new root if not found

			Tree.EndUpdate();
			Tree.Refresh();
		}
		public void OnNodeRemoved(object sender, NodeEventArgs e)
		{
			Tree.BeginUpdate();
			TreeNode tn = null;

			NsNode node = e.Node;
			if (node != null)//attempt to find an existing treenode
				tn = GetTreeNode(node);

			if (tn != null)//update it if found
				tn.Remove();

			Tree.EndUpdate();
			Tree.Refresh();
		}
		public void OnNodeSelected(object sender, NodeEventArgs e)
		{
		}

		public void Attach(INodeCtrl ictrl)
		{
			ictrl.SelectionChanged += new NsNodeEventHandler(ictrl_SelectionChanged);
		}

		void ictrl_SelectionChanged(object sender, NodeEventArgs e)
		{
			if (e.Node != null && sender != this)
			{
				TreeNode tn = GetTreeNode(e.Node);
				if (tn != null)
					Tree.SelectedNode = tn;
				else if (sender is NsNode)
					SelectedNode = sender as NsNode;

			}
		}

		#endregion

		#region INodeCtrl Members

		public event NsNodeEventHandler SelectionChanging;
		public event NsNodeEventHandler SelectionChanged;
		private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
		{
			NsNode current = SelectedNode;
			if (current == null && SelectedAttribute != null)
				current = SelectedAttribute.Parent;
			if (SelectionChanging != null)
				SelectionChanging(this, new NodeEventArgs(current));

		}
		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Action != TreeViewAction.Unknown)
			{
				RaiseSelectionChanged();
			}
		}

		public void RaiseSelectionChanged()
		{
			NsNode node = SelectedNode;
			if (node == null)
				node = SelectedAttribute.Parent;
			if (node != null)
				if (SelectionChanged != null)
					SelectionChanged(this, new NodeEventArgs(node));
		}

		#endregion

		#region Save Load
		/// <summary>
		/// saves the currently selected node using a SaveFileDialog and a desired filename
		/// </summary>
		/// <param name="filename">name of the save file</param>
		/// <returns></returns>
		public NsNode SaveNode(string filename, string dir, out string dirpath)
		{
			dirpath = null;
			if (SelectedNode == null)
			{
				MessageBox.Show("Please select a node to save");
				return null;
			}
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.AddExtension = true;
			sfd.Filter = "nhtsf files (*.nhtsf)|*.nhtsf|All files (*.*)|*.*";
			sfd.FileName = filename == null ? SelectedNode.Label : filename;
			if (dir != null)
				sfd.InitialDirectory = dir;

			if (sfd.ShowDialog() == DialogResult.OK)
			{
				string path = sfd.FileName;
				dirpath = path;
				NodeIO.WriteXml(SelectedNode, path);
				//MessageBox.Show("Invalid File");
				if (NodeSaved != null)
					NodeSaved(this, new NodeLoadedEventArgs(SelectedNode, path));
				return SelectedNode;
			}
			return null;
		}

		/// <summary>
		/// saves the currently selected node using a SaveFileDialog
		/// </summary>
		/// <returns>the saved node</returns>
		public NsNode SaveNode()
		{
			if (SelectedNode == null)
			{
				MessageBox.Show("Please select a node to save");
				return null;
			}
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.AddExtension = true;
			sfd.Filter = "xml files|*.xml|All files (*.*)|*.*";
			if (Extension != null)
				sfd.Filter = Extension + " files|*." + Extension + "|" + sfd.Filter;
			sfd.FileName = SelectedNode.Label;
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				string path = sfd.FileName;
				NodeIO.WriteXml(SelectedNode, path);
				//MessageBox.Show("Invalid File");
				if (NodeSaved != null)
					NodeSaved(this, new NodeLoadedEventArgs(SelectedNode, path));
				return SelectedNode;
			}
			return null;
		}

		/// <summary>
		/// save a node with a newly defined extension (don't include .)
		/// </summary>
		/// <param name="ext"></param>
		/// <returns></returns>
		public NsNode SaveNode(string ext)
		{
			if (SelectedNode == null)
			{
				MessageBox.Show("Please select a node to save");
				return null;
			}
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.AddExtension = true;
			sfd.Filter = ext + " files (*." + ext + ")|*." + ext + "|All files (*.*)|*.*";
			sfd.FileName = SelectedNode.Label;
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				string path = sfd.FileName;
				NodeIO.WriteXml(SelectedNode, path);
				if (NodeSaved != null)
					NodeSaved(this, new NodeLoadedEventArgs(SelectedNode, path));
				return SelectedNode;
			}
			return null;
		}

		/// <summary>
		/// saves the currently selected node using a SaveFileDialog and a desired filename
		/// </summary>
		/// <param name="filename">name of the save file</param>
		/// <returns></returns>
		//public NsNode SaveNode(string filename, string dir, out string dirpath)
		//{
		//    dirpath = null;
		//    if (SelectedNode == null)
		//    {
		//        MessageBox.Show("Please select a node to save");
		//        return null;
		//    }
		//    SaveFileDialog sfd = new SaveFileDialog();
		//    sfd.AddExtension = true;
		//    sfd.Filter = "nhtsf files (*.nhtsf)|*.nhtsf|All files (*.*)|*.*";
		//    sfd.FileName = filename == null ? SelectedNode.Label : filename;
		//    if (dir != null)
		//        sfd.InitialDirectory = dir;

		//    if (sfd.ShowDialog() == DialogResult.OK)
		//    {
		//        string path = sfd.FileName;
		//        dirpath = path;
		//        NodeIO.WriteXml(SelectedNode, path);
		//        if (NodeSaved != null)
		//            NodeSaved(this, new NodeLoadedEventArgs(SelectedNode, path));
		//        return SelectedNode;
		//    }
		//    return null;
		//}

		/// <summary>
		/// loads a new NsNode into the tree using an OpenFileDialog
		/// </summary>
		/// <returns>the new NsNode, null on failure</returns>
		public NsNode LoadNode()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (Extension == null)
				ofd.Filter = "xml files (*.xml)|*.xml|txt files (*.txt)|*.txt|All files (*.*)|*.*";
			else
				ofd.Filter = Extension + " files (*." + Extension + ")|*." + Extension + "|xml files (*.xml)|*.xml|All files (*.*)|*.*";

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				return LoadNode(ofd.FileName);
			}
			return null;
		}

		/// <summary>
		/// loads a new NsNode from the path specified
		/// </summary>
		/// <param name="path">a path to an nsnode .xml or .txt file</param>
		/// <returns></returns>
		public NsNode LoadNode(string path)
		{
			NsNode node = null;
			try
			{
				if (System.IO.Path.GetExtension(path).ToLower() == ".txt")
					node = NodeIO.ReadTxt(path);
				if (System.IO.Path.GetExtension(path).ToLower() == ".csv")
					node = NodeCSV.ReadCSV(path);
				else
					node = NodeIO.ReadXml(path);
			}
			catch (NsNodes.AttributeXmlFormatException xe)
			{
				string s = xe.Xml.OuterXml.Split(new string[] { "><" }, StringSplitOptions.None)[0];
				MessageBox.Show("Invalid Format at:\n" + s);
				return null;
			}
			catch (Exception ee)
			{
				MessageBox.Show("Invalid File:\n" + ee.ToString());
				return null;
			}
			if (node == null)
			{
				MessageBox.Show("Invalid File:\n" + path.ToString());
				return null;
			}
			node.Attach(this);
			if (NodeLoaded != null)
				NodeLoaded(this, new NodeLoadedEventArgs(node, path));
			node.Update();
			SelectedNode = node;
			RaiseSelectionChanged();
			return node;
		}

		#endregion

		#region Toolstrip

		private void FlowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (SelectedNode == null)
				if (SelectedAttribute != null)
					SelectedNode = SelectedAttribute.Parent;
			NsNodeControls.NsNodeFlow.Show(SelectedNode);
			//Form f = new Form();
			//f.SuspendLayout();
			//NsNodeControls.NsNodeFlow flow = new NsNodeControls.NsNodeFlow();
			//flow.Dock = DockStyle.Fill;
			//f.Controls.Add(flow);
			//f.ResumeLayout();
			//flow.Node = SelectedNode;
			//f.StartPosition = FormStartPosition.CenterParent;
			//f.FormBorderStyle = FormBorderStyle.SizableToolWindow;
			//f.Width = 150;
			//f.Height = 300;
			//f.Text = "";
			//flow.SelectionChanged += delegate(object s, NodeEventArgs ne)
			//{
			//     if (ne.Node == null)
			//          f.Close();
			//};
			//f.Show();
			//if (flow.Node == null)
			//     return;

			//TreeNode tn = GetTreeNode(flow.Node);
			//if (tn != null)
			//{
			//     Tree.BeginUpdate();
			//     UpdateNode(tn, flow.Node);
			//     Tree.EndUpdate();
			//     Tree.Refresh();
			//}



			//NsNodeEditor edit = new NsNodeEditor(SelectedNode);
			//if (edit.ShowDialog() == DialogResult.OK)
			//{
			//     TreeNode tn = GetTreeNode(edit.Node);
			//     if (tn != null)
			//     {
			//          Tree.BeginUpdate();
			//          UpdateNode(tn, edit.Node);
			//          Tree.EndUpdate();
			//          Tree.Refresh();
			//     }
			//}
		}

		private void gridToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (SelectedNode == null)
				if (SelectedAttribute != null)
					SelectedNode = SelectedAttribute.Parent;
			NsNodeControls.NsAttributeGridView.Show(SelectedNode, ParentForm);

		}

		private void TreeStripMenuItem_Click(object sender, EventArgs e)
		{
			if (SelectedNode == null)
				if (SelectedAttribute != null)
					SelectedNode = SelectedAttribute.Parent;
			NsNodeTree.Show(SelectedNode, ParentForm);
		}

		private void addToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NsNode node = new NsNode("new");
			//NsNodeEditor edit = new NsNodeEditor(node);
			//if (edit.ShowDialog() == DialogResult.Cancel)
			//    return;

			if (SelectedNode != null)
			{
				SelectedNode.Add(node);
				node.Attach(this);
				SelectedNode.Update();
			}
			else
				AddRootNode(node);
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (SelectedNode != null)
			{
				//bool root = SelectedNode.IsRoot;
				NsNode parent = SelectedNode.Parent;
				SelectedNode.Delete();
				if (parent == null) //remove the root node for roots
					Tree.SelectedNode.Remove();
				else
					parent.Update(this);//otherwise update the parent's subscribers
			}
			else if (SelectedAttribute != null)
			{
				NsNode parent = SelectedAttribute.Parent;

				SelectedAttribute.Remove();
				if (parent != null)
					parent.Update(this);
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// if (Extension != null)
			//    SaveNode(Extension);
			// else
			SaveNode();
		}

		private void loadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadNode();
		}

		private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExpandAll();
		}
		public void ExpandAll()
		{
			Tree.BeginUpdate();
			Tree.ExpandAll();
			Tree.EndUpdate();
		}

		private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Tree.BeginUpdate();
			Tree.CollapseAll();
			Tree.EndUpdate();
		}

		private void expandNodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Tree.SelectedNode == null)
				return;
			Tree.BeginUpdate();
			Tree.SelectedNode.ExpandAll();
			Tree.EndUpdate();
		}

		private void collapseNodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Tree.SelectedNode == null)
				return;
			Tree.BeginUpdate();
			Tree.SelectedNode.Collapse(false);
			Tree.EndUpdate();

		}

		private void treeView1_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F5)
				F5Refresh();
			else if (e.KeyCode == Keys.Delete)
				deleteToolStripMenuItem_Click(this, new EventArgs());
		}
		public void F5Refresh()
		{
			if (SelectedNode != null)
				UpdateNode(Tree.SelectedNode, SelectedNode);
		}

		void AddMenuItem()
		{
			ToolStripDropDownButton adds = new ToolStripDropDownButton("Add");
			List<Type> atrs = NodeIO.GetAllAttributeTypes();
			ToolStripItem ts;
			ts = adds.DropDownItems.Add("NsNode");
			ts.Tag = typeof(NsNode);
			ts.Click += new EventHandler(addAttribute_Click);

			foreach (Type t in atrs)
			{
				if (t.Name == "IAttribute") //cant add interface object
					continue;
				ts = adds.DropDownItems.Add(t.Name);
				ts.Tag = t;
				ts.Click += new EventHandler(addAttribute_Click);
			}

			contextMenuStrip1.Items.Add("-");
			contextMenuStrip1.Items.Add(adds);
		}

		void addAttribute_Click(object sender, EventArgs e)
		{
			NsNode parent = SelectedNode;
			if (parent == null && SelectedAttribute != null)
				parent = SelectedAttribute.Parent;

			if (parent == null)
				return;

			ToolStripItem ts = sender as ToolStripItem;
			Type t = ts.Tag as Type;
			if (t != null)
			{
				if (t == typeof(NsNode))
				{
					parent.Add(new NsNode("new", parent));
				}
				else
				{
					System.Reflection.ConstructorInfo ci = null;
					try
					{
						ci = t.GetConstructor(new Type[] { typeof(NsNode) });
					}
					catch { return; }//fail peacefully
					object obj = null;
					try
					{
						obj = ci.Invoke(new object[] { parent });
					}
					catch { }
					if (obj != null && obj is IAttribute)
						parent.Add(obj as IAttribute);
				}
				parent.Update();
			}
		}

		/// <summary>
		/// raised when a new node is loaded using LoadNode()
		/// </summary>
		public event NodeLoadedEventHandler NodeLoaded;
		/// <summary>
		/// raised when a  node is saved using SaveNode()
		/// </summary>
		public event NodeLoadedEventHandler NodeSaved;

		#endregion

		#region Copy Paste

		public event NsNodeEventHandler NodeCopied;

		NsNode m_clipboard;
		private void copy_Click(object sender, EventArgs e)
		{
			m_clipboard = SelectedNode;
		}

		private void paste_Click(object sender, EventArgs e)
		{
			if (m_clipboard != null && SelectedNode != null)
			{
				NsNode copy = m_clipboard.CopyTo(SelectedNode);
				copy.Label += "-cpy";
				SelectedNode.Update();
				if (NodeCopied != null)
					NodeCopied(this, new NodeEventArgs(new NsNode[] { m_clipboard, copy }));
			}
			m_clipboard = null;
		}
		#endregion

		private void expandToDepthToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			int depth = expandToDepthToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem);
			if (depth > -1)
				ExpandToDepth(depth);
		}

	}
}