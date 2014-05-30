using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using devDept.Geometry;
using NsNodes;

namespace NsFileGen
{
	class ContourTracker
	{
		public ContourTracker()
			: this("Contour")
		{
		}
		public ContourTracker(string label)
		{
			m_node = new NsNode(label);
			Random rand = new Random();
			int i = rand.Next(5345);
			m_node.Add(new ContourAttribute(m_node, null, null));
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
			Line.Add(Line[0]);//close the polygon
			Node.Update();
		}
		//public void Attach(INodeView view)
		//{
		//     Node.Attach(view);
		//}

		const double radius = 1;
		int iHit = -1;

		ContourAttribute Line
		{
			get { return Node.Attributes[0] as ContourAttribute; }
			set { Node.Attributes.Clear(); Node.Attributes.Add(value); }
		}

		void view_TrackerDown(object sender, NodeTrackerEventArgs e)
		{
            Point3D pnt = new Point3D(e.DeltaX, e.DeltaY, e.DeltaZ);
			for (iHit = 0; iHit < Line.Count; ++iHit)
			{
				if (pnt.DistanceTo(Line[iHit]) < radius)
				{
					break;
				}
			}
			if (Line.Count == 0 || iHit == Line.Count)
			{
                Line.Add(new Point3D(e.DeltaX, e.DeltaY, e.DeltaZ));
			}

			Node.Update();
		}

		void view_TrackerDrag(object sender, NodeTrackerEventArgs e)
		{
			if (iHit > 0)
			{
                Line.Set(iHit, new Point3D(e.DeltaX, e.DeltaY, e.DeltaZ));
				Node.Update();
			}
		}

		void view_TrackerUp(object sender, NodeTrackerEventArgs e)
		{
			if (iHit > 0)
			{
                Line.Set(iHit, new Point3D(e.DeltaX, e.DeltaY, e.DeltaZ));
				Node.Update();
			}
			iHit = -1;
		}
	}
}
