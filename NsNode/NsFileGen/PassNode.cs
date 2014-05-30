using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;

using devDept;
using devDept.Eyeshot;
using devDept.Geometry;
using devDept.Eyeshot.Entities;

namespace NsFileGen
{
	public class PassNode : NsNode
	{
		public PassNode(NsNode parent)
			: base("", parent)
		{
		}

		public PassNode(NsNode parent, IList<Point3D> xyz, IList<Point3D> uvw, IList<double> speed, IList<double> caxis)
			: this(parent)
		{
			ContourAttribute pl = new ContourAttribute(this, null, xyz);
			Attributes.Add(pl);
			//pl.BlockRef.LayerIndex = 0;

			pl = new ContourAttribute(this, null, uvw);
			Attributes.Add(pl);
			//pl.BlockRef.LayerIndex = 1;

			ArrayAttribute aa = new ArrayAttribute(this, null, speed);
			Attributes.Add(aa);
			//aa.BlockRef.LayerIndex = 2;

			aa = new ArrayAttribute(this, null, caxis);
			Attributes.Add(aa);
			//aa.BlockRef.LayerIndex = 3;

			double scale = Length / 20.0;
			int mid = xyz.Count/2;
			Vector3D[] tpn = TPN(mid);

			//add a direction arrow
			ArrowAttribute arrow = new ArrowAttribute(this, XYZ(mid), tpn[0], tpn[1], tpn[2], scale);
			Attributes.Add(arrow);
			//arrow.BlockRef.LayerIndex = 4;

			//add a gantry frame for each point
			FrameAttribute gant;
			for( int i = 0; i < uvw.Count; i++ )
			{
				tpn = TPN(i);
				if( tpn == null )
					continue;
				gant = new FrameAttribute(this, XYZ(i), tpn[0], tpn[1], tpn[2], scale);
				Attributes.Add(gant);
				//gant.BlockRef.LayerIndex = 1;
			}
		}

		/// <summary>
		/// Constructs a pass from just xyz data
		/// </summary>
		/// <param name="parent">The parent node</param>
		/// <param name="xyz">the xyz points to construct the pass from</param>
		public PassNode(NsNode parent, IList<Point3D> xyz)
			:this(parent)
		{
			ContourAttribute pl = new ContourAttribute(this, null, xyz);
			Attributes.Add(pl);
		}

		public PassNode(NsNode parent, System.Xml.XmlNode xml)
			: base(parent, xml)
		{
			if (parent != null && Label.StartsWith(Parent.Label))
				Label = null;
		}

		public double Length
		{
			get
			{
				ContourAttribute pl = Attributes[0] as ContourAttribute;
				if (pl == null)
					return 0;
				return pl.Length;
			}
		}
		public override string Label
		{
			get
			{
				if( base.Label == "" )
				{
					return Parent.Label + "[" + PassNumber.ToString() + "]";
				}
				return base.Label;
			}
			set
			{
				base.Label = value;
			}
		}
		public int PassNumber
		{
			get { return Parent.IndexOf(this); }
		}
		/// <summary>
		/// Pass=1 Pass>1 Pass=1
		/// </summary>
		/// <param name="query">"Pass=1 Pass>1 Pass=1"</param>
		/// <returns></returns>
		public bool Query(string query)
		{
			// Pass=1 Pass>1 Pass<1
			char op = query[4];
			int target = 0;
			try
			{
				target = Convert.ToInt32(query.Substring(4));
			}
			catch
			{
				return false;
			}
			switch (op)
			{
				case '<':
					return PassNumber < target;
				case '>':
					return PassNumber > target;
				case '=':
					return PassNumber == target;
				default:
					return false;
			}
		}

		public Point3D UVW(int index)
		{
			if( Attributes.Count < 2 ) 
				return Point3D.MinValue;
			ContourAttribute pl = Attributes[1] as ContourAttribute;
			if( pl == null )
				return Point3D.MinValue;
			return pl[index];
		}
		public Point3D XYZ(int index)
		{
			if (Attributes.Count < 2)
				return Point3D.MinValue;
			ContourAttribute pl = Attributes[0] as ContourAttribute;
			if (pl == null)
				return Point3D.MinValue;
			return pl[index];
		}
		Vector3D[] TPN(int index)
		{
			Vector3D[] vec = new Vector3D[3];

			Point3D uvw = UVW(index);
			if (uvw == Point3D.MinValue)
				return null;

			double radx = Utility.DegToRad(uvw.X);
			double rady = Utility.DegToRad(uvw.Y);
			double radz = Utility.DegToRad(uvw.Z);

			double c1, c2, c3, s1, s2, s3;
			c1 = c2 = c3 = s1 = s2 = s3 = 0;

			c1 = Math.Cos(radz);
			c2 = Math.Cos(radx);
			c3 = Math.Cos(rady);

			s1 = Math.Sin(radz);
			s2 = Math.Sin(radx);
			s3 = Math.Sin(rady);

			vec[0].X = c1 * c3 + s1 * s2 * s3;
			vec[0].Y = c2 * s1;
			vec[0].Z = c3 * s1 * s2 - c1 * s3;

			vec[1].X = c1 * s2 * s3 - c3 * s1;
			vec[1].Y = c1 * c2;
			vec[1].Z = c1 * c3 * s2 + s1 * s3;

			vec[2].X = c2 * s3;
			vec[2].Y = -s2;
			vec[2].Z = c2 * c3;
			return vec;
		}
	}
}
