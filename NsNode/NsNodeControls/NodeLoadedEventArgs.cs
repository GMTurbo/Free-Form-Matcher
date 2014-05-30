using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;

namespace NsNodeControls
{
	public class NodeLoadedEventArgs
	{
		public NodeLoadedEventArgs(NsNode node, string path)
		{
			m_node = node;
			m_path = path;
		}
		public String Path
		{
			get { return m_path; }
		}
		public NsNode Node
		{
			get { return m_node; }
		}

		string m_path;
		NsNode m_node;
	}
}
