using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;
using devDept.Geometry;
using NsViewers;

namespace NsFileGen
{
	class Point2dTracker
	{
		public Point2dTracker()
			: this("Points")
		{
		}
		public Point2dTracker(string label)
		{
			m_node = new NsNode(label);
		}
		NsNode m_node;

		public NsNode Node
		{
			get { return m_node; }
			set { m_node = value; }
		}

		public void Attach(ITracker view)
		{
			view.TrackerDown += new NsNodeTrackerEventHandler(view_TrackerDown);
			view.TrackerDrag += new NsNodeTrackerEventHandler(view_TrackerDrag);
			view.TrackerUp += new NsNodeTrackerEventHandler(view_TrackerUp);
			view.TrackerDetach += new NsNodeTrackerEventHandler(view_TrackerDetach);
		}

		void view_TrackerDetach(object sender, NodeTrackerEventArgs e)
		{
			Node.Update();
		}
		//public void Attach(INodeView view)
		//{
		//     Node.Attach(view);
		//}

		const double radius = 1;
		NsViewers.Point3dAttribute m_hit;
		void view_TrackerDown(object sender, NodeTrackerEventArgs e)
		{
			Point3dAttribute p = null;
            Point3D pt = new Point3D(e.DeltaX, e.DeltaY, e.DeltaZ);
			m_hit = null;
			foreach (IAttribute atr in Node.Attributes)
			{
				if (atr is Point3dAttribute)
				{
					p = atr as Point3dAttribute;
					if (p.DistanceTo(pt) < radius)
						m_hit = p;
				}
			}
			if (m_hit == null)
			{
                m_hit = new Point3dAttribute(Node, e.DeltaX, e.DeltaY, e.DeltaZ);
				Node.Add(m_hit);
			}
			Node.Update();
		}

		void view_TrackerDrag(object sender, NodeTrackerEventArgs e)
		{
			if (m_hit != null)
			{
                m_hit.Value = new Point3D(e.DeltaX, e.DeltaY, e.DeltaZ);
				Node.Update();
			}
		}

		void view_TrackerUp(object sender, NodeTrackerEventArgs e)
		{
			if (m_hit != null)
			{
                m_hit.Value = new Point3D(e.DeltaX, e.DeltaY, e.DeltaZ);
				Node.Update();
			}
			m_hit = null;
		}
	}
}
