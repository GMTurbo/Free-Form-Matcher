using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;
using devDept.Geometry;
namespace NsFileGen
{
	class ContourNode : NsNode
	{
		public ContourNode(NsNode parent, string label, ITracker tracker)
			:base(label, parent)
		{
			Attributes.Add(new ContourAttribute(this, null, null));
			if (tracker != null)
			{
				Attach(tracker);
				if (tracker is INodeView)
					Attach(tracker as INodeView);
			}
		}
		ContourAttribute Line
		{
			get { return Attributes[0] as ContourAttribute; }
			set { Attributes.Clear(); Attributes.Add(value); }
		}
		public void Attach(ITracker tracker)
		{
			tracker.TrackerDown += OnTrackerDown;
			tracker.TrackerDrag += OnTrackerDrag;
			tracker.TrackerUp += OnTrackerUp;
		}

		const double radius = 1;
		int iHit = -1;
		void OnTrackerDown(object sender, NodeTrackerEventArgs e)
		{
            Point3D pnt = new Point3D(e.DeltaX, e.DeltaY, e.DeltaZ);
			for (iHit = 0; iHit < Line.Count; ++iHit)
			{
				if (pnt.DistanceTo(Line[iHit]) < radius)
				{
					break;
				}
			}
			if( Line.Count == 0 || iHit == Line.Count)
			{
                Line.Add(new Point3D(e.DeltaX, e.DeltaY, e.DeltaZ));
			}
			Update();
		}
		void OnTrackerDrag(object sender, NodeTrackerEventArgs e)
		{
			if (iHit > 0)
			{
                Line.Set(iHit, new Point3D(e.DeltaX, e.DeltaY, e.DeltaZ));
				Update();
			}
		}
		void OnTrackerUp(object sender, NodeTrackerEventArgs e)
		{
			if (iHit > 0)
			{
                Line.Set(iHit, new Point3D(e.DeltaX, e.DeltaY, e.DeltaZ));
				Update();
			}
			iHit = -1;
		}
	}
}
