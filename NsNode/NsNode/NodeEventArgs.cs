using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NsNodes
{
	public class NodeEventArgs : EventArgs
	{
		public NodeEventArgs(NsNode node, NsNode[] nodes)
		{
			m_nodes = nodes;
			m_node = node;
		}
		public NodeEventArgs(NsNode node)
			:this(node, null)
		{
		}
		public NodeEventArgs(NsNode[] nodes)
			:this( null, nodes ) 
		{
		}

		public NsNode Node
		{
			get { return m_node; }
		}

		public NsNode[] Nodes
		{
			get { return m_nodes; }
		}

		public int Count
		{
			get { return m_nodes.Length; }
		}

		public bool MultipleNodes
		{
			get { return m_nodes == null ? true : false; }
		}

		NsNode m_node;
		NsNode[] m_nodes;
	}


}
