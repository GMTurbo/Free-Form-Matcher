using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;
using System.Drawing;

namespace NsNodeView
{
	public class Point3DEntity : Point3Attribute , IEntityGroup, IDragable
	{
		public Point3DEntity(NsNode parent, System.Xml.XmlNode xml)
			:base(parent, xml)
		{

		}
		public Point3DEntity(NsNode parent, string label, double[] point)
			:base(parent, label, point)
		{

		}
		public Point3DEntity(NsNode parent, double x, double y, double z)
			: base(parent, null, x, y, z)
		{

		}

		#region IEntityGroup Members
		public static string DRAW = "DrawPoints";
		public static string COLOR = "PointColor";

		public devDept.Eyeshot.Standard.Entity[] Entity
		{
			get {
				
				IAttribute a = Parent.FindInherited(DRAW);
				if (a is BoolAttribute && (bool)a.Value == false)
					return null;

				a = Parent.FindInherited(COLOR);
				System.Drawing.Color c = System.Drawing.Color.Black;
				if( a != null )
					c = System.Drawing.Color.FromName(a.Value.ToString());
				List<devDept.Eyeshot.Standard.Entity> ent = new List<devDept.Eyeshot.Standard.Entity>(1);
				devDept.Eyeshot.Standard.Point p = new devDept.Eyeshot.Standard.Point(this[0], this[1], this[2], c);
				p.EntityData = this;
				ent.Add(p);
				return ent.ToArray();
			}
		}



		public devDept.Eyeshot.Labels.LabelBase devpDeptLabel
		{
			get
			{
                return null;
				//return new devDept.Eyeshot.Labels.TextOnly(new devDept.Eyeshot.Point3D(0, 0, 0), this.Label, NsNodeShot.LABELFONT, System.Drawing.Color.Black);
			}
		}

		#endregion

		#region IDragable Members

		public void Drag(double x, double y, double z)
		{
			base[0] += x;
			base[1] += y;
			base[2] += z;
		}

		#endregion

	}
}
