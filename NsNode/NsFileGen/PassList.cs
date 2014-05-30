using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;
using devDept;
using devDept.Eyeshot;
using devDept.Geometry;
using devDept.Eyeshot.Entities;
using System.Xml;


namespace NsFileGen
{
	public class PassList: NsNode
	{
		public PassList(NsNode parent, string label, Point3D[][] xyzs, Point3D[][] uvws, double[][] speeds, double[][] caxes)
			:this(parent, label)
		{
			for (int i = 0; i < xyzs.GetLength(0); i++)
			{
				Add(new PassNode(this, xyzs[i], uvws[i], speeds[i], caxes[i]));
			}
		}
		public PassList(NsNode parent, string label, IList<IList<Point3D>> xyzs, IList<IList<Point3D>> uvws, IList<IList<double>> speeds, IList<IList<double>> caxes)
			:this(parent, label)
		{
			for (int i = 0; i < xyzs.Count; i++)
			{
				Add(new PassNode(this, xyzs[i], uvws[i], speeds[i], caxes[i]));
			}
		}
		public PassList(NsNode parent, string label)
			:base(label, parent)
		{

		}
		public List<PassNode> Passes
		{
			get
			{
				List<PassNode> pent = new List<PassNode>(Nodes.Count);
				foreach (NsNode n in Nodes)
					if (n is PassNode)
						pent.Add(n as PassNode);
				return pent;
			}
		}
		public int IndexOf(PassNode pass)
		{
			int count = 0;
			foreach (PassNode p in Passes)
			{
				if (p == pass)
					return count;
				count++;
			}
			return -1;
		}
	}
}
