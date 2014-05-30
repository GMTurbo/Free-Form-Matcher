using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;
using System.IO;

namespace NsNodeView
{
	public class CloudNode: NsNode
	{
          public CloudNode(string label, NsNode parent)
               : base(label, parent)
          {
          }
          public CloudNode(NsNode parent, System.Xml.XmlNode xml)
               : base(parent, xml)
          {

          }
		public CloudNode(System.Drawing.Color color)
		{
			Add(new StringAttribute(this, Point3DEntity.COLOR, color.Name));
		}
		public CloudNode(string path)
		{
			Label = System.IO.Path.GetFileNameWithoutExtension(path);
			
			using (StreamReader sr = new StreamReader(path))
			{
				string line;
				string[] cols;
				double[] x = new double[3];
				while ((line = sr.ReadLine()) != null)
				{
					try
					{
						cols = line.Split(',');
						for (int i = 0; i < 3; i++)
							x[i] = Convert.ToDouble(cols[i]);
						Add(new Point3DEntity(this, x[0], x[1], x[2]));
					}
					catch
					{
						continue;
					}
				}
			}
		}
          public CloudNode(List<double[]> pnts)
          {
               pnts.ForEach((double[] d) =>
                    {
                         Add(new Point3DEntity(this, d[0], d[1], d[2]));
                    });
               Add(new BoolAttribute(this,"DrawPoints",true));

          }
          public CloudNode(List<double[]> pnts, bool draw)
          {
               pnts.ForEach((double[] d) =>
               {
                    Add(new Point3DEntity(this, d[0], d[1], d[2]));
               });
               //Add(new BoolAttribute(this, "DrawPoints", draw));

          }
		public System.Drawing.Color Color
		{
			get { 
				IAttribute a = FindAttribute(Point3DEntity.COLOR);
				return System.Drawing.Color.FromName(a.Value.ToString());
			}
			set
			{
				IAttribute a = FindAttribute(Point3DEntity.COLOR);
				if (a == null)
					Add(new StringAttribute(this, Point3DEntity.COLOR, value.Name));
				else
					a.Value = value.Name;
			}
		}
		public void Translate(double[] xyz)
		{
			int i = 0;
			Point3DEntity pnt;
			foreach (IAttribute atr in Attributes)
			{
				pnt = atr as Point3DEntity;
				if (pnt != null)
				{
					for( i=0; i<3; i++ )
						pnt[i] += xyz[i];
				}
			}
			//Update();
		}
		public void Rotate(double angle, int axis)
		{
			devDept.Eyeshot.Geometry.Vector3D v;
			switch (axis)
			{
				case 0:
					v = devDept.Eyeshot.Geometry.Vector3D.AxisX;
					break;
				case 1:
					v = devDept.Eyeshot.Geometry.Vector3D.AxisY;
					break;
				case 2:
					v = devDept.Eyeshot.Geometry.Vector3D.AxisZ;
					break;
				default:
					return;
			}

			devDept.Eyeshot.Geometry.Transformation t = new devDept.Eyeshot.Geometry.Transformation();
			t.Rotation(angle, v, new devDept.Eyeshot.Point3D(0, 0, 0));
			Point3DEntity pnt;
			devDept.Eyeshot.Point3D p;
			foreach (IAttribute atr in Attributes)
			{
				pnt = atr as Point3DEntity;
				if (pnt != null)
				{
					p = new devDept.Eyeshot.Point3D(pnt.Pt);
					p = (t * p);
					pnt[0] = p.X;
					pnt[1] = p.Y;
					pnt[2] = p.Z;
				}
			}

			//Update();
		}
		//public void ColorByHeight()
		//{
		//     Point3DEntity pnt;
		//     double zmax = -1e9;
		//     double zmin = 1e9;
		//     foreach (IAttribute atr in Attributes)
		//     {
		//          pnt = atr as Point3DEntity;
		//          if (pnt != null)
		//          {
		//               zmax = Math.Max(zmax, pnt[2]);
		//               zmin = Math.Min(zmin, pnt[2]);
		//          }
		//     }
		//     foreach (IAttribute atr in Attributes)
		//     {
		//          pnt = atr as Point3DEntity;
		//          if (pnt != null)
		//          {
		//               pnt.Color = Utilities.GetColor(zmax, zmin, pnt[2]);
		//          }
		//     }
		//}
		public List<double[]> Points
		{
			get { List<double[]> pts = new List<double[]>(Attributes.Count);
				foreach( IAttribute atr in Attributes )
					if( atr is Point3DEntity )
						pts.Add((atr as Point3DEntity).Pt.ToArray());
				return pts;
			}
		}
	}
}
