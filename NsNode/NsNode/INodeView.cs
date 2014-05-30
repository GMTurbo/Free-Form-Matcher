using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NsNodes
{
	public delegate void NsNodeEventHandler(object sender, NodeEventArgs e);
	public delegate void NsNodeDragEventHandler(object sender, NodeTrackerEventArgs e);
	public interface INodeView
	{
		void OnNodeUpdated(object sender, NodeEventArgs e);
		void OnNodeRemoved(object sender, NodeEventArgs e);
		void OnNodeSelected(object sender, NodeEventArgs e);
		void Attach(INodeCtrl ictrl);
	}
}
