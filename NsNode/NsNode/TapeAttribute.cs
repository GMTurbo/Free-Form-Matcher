using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
//using devDept;
//using devDept.Eyeshot;
//using devDept.Geometry;
//using devDept.Eyeshot.Entities;
using System.Xml;

namespace NsNodes
{
	public class TapeAttribute: IAttribute//, IEntity
	{
		string m_label = null;
		NsNode m_parent = null;
		int m_plyid;

		public int PlyID
		{
			get { return m_plyid; }
			set { m_plyid = value; }
		}
		IList<PointF> m_points;// = new List<Point3D>();
		PolylineAttribute m_strip;

		public TapeAttribute(NsNode parent, string label, IList<PointF> points, int plyID, PolylineAttribute strip)
		{
			m_parent = parent;
			m_label = label;
			if (points != null)
				m_points = new List<PointF>(points);
						else
				m_points = new List<PointF>();
			m_plyid = plyID;
			m_strip = strip.CopyTo(null) as PolylineAttribute;
		}
		public TapeAttribute(NsNode parent, string label, IList<PointF> points)
		{
			m_parent = parent;
			m_label = label;// Parent.IndexOf(this).ToString();
			if (points != null)
				m_points = new List<PointF>(points);
			else
				m_points = new List<PointF>();
		}
		public TapeAttribute(NsNode parent, XmlNode xml)
			: this(parent, null, null)
		{
			if (!FromXml(xml))
                    throw new AttributeXmlFormatException(this, xml, "Failed to read xml\n" + parent.Label);
		}
		TapeAttribute()
			: this(null, null, null)
		{ }

		public PointF Start
		{
			get { return m_points[0]; }
			set 
			{
				if (m_points.Count < 2)
					m_points.Add(value);
				else
					m_points[0] = value;
			}
		}

		public PointF End
		{
			get { return m_points[1]; }
			set 
			{
				if (m_points.Count < 2)
					m_points.Add(value);
				else
					m_points[1] = value;
			}
		}

		public List<double[]> TriStrip
		{
			get
			{
				
				return m_strip == null ? null : m_strip.Value as List<double[]>;
			}
		}

		#region IAttribute Members

		public string Label
		{
			get { return m_label + Parent.IndexOf(this).ToString(); }
		}

		public object Value
		{
			get
			{
				return m_points;
			}
			set
			{
				if (value is IList<PointF>)
					m_points = new List<PointF>(value as IList<PointF>);
				else
					throw new FormatException("Invalid type for TapeAttribute.Value, must be IList<PointF>");
			}
		}

		public NsNode Parent
		{
			get { return m_parent; }
		}

		public bool Query(string query)
		{
			throw new NotImplementedException();
		}

		public string Type
		{
			get { return GetType().ToString(); }
		}

		public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
		{
			System.Xml.XmlElement el = doc.CreateElement(Type);
			//NodeIO.AddAttribute(doc, el, "TapeCount", Label);
			NsXmlHelper.AddAttribute(doc, el, "PlyID", PlyID.ToString());
			NsXmlHelper.AddAttribute(doc, el, "Start", (Start.X / 1000).ToString() + "," + (Start.Y / 1000).ToString());
			NsXmlHelper.AddAttribute(doc, el, "End", (End.X / 1000).ToString() + "," + (End.Y / 1000).ToString());
			if (m_strip != null && m_strip.Count > 0)
			{
				m_strip.Scale(.001);
				el.AppendChild(m_strip.ToXml(doc));
				m_strip.Scale(1000);
			}
			return el;
		}

		public bool FromXml(System.Xml.XmlNode xml)
		{
			if (xml.Attributes == null || xml.Attributes.Count < 3)
				return false;
			PlyID = -1;
			Start = PointF.Empty;
			End = PointF.Empty;
			foreach (XmlAttribute atr in xml.Attributes)
			{
				if (atr.Name == "Start")
				{
					try
					{
						string[] data = null;
						data = atr.Value.Split(new Char[] { ',' });

						float startx = (float)(Convert.ToDouble(data[0]) * 1000);
						float starty = (float)(Convert.ToDouble(data[1]) * 1000);

                        if (Math.Abs(startx) == float.PositiveInfinity || Math.Abs(starty) == float.PositiveInfinity)
                        {
                            System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame(1, true);
                            throw (new Exception("Error (" + stackFrame.GetMethod() + " ln: " + stackFrame.GetFileLineNumber() +")"));
                        }

						Start = new PointF(startx, starty);
					}
					catch
					{
						Start = PointF.Empty;
						throw (new Exception("Error converting value from 3di file: " + this.ToString()));
					}
				}
				else if (atr.Name == "End")
				{
					try
					{
						string[] data = null;
						data = atr.Value.Split(new Char[] { ',' });

						float endx = (float)(Convert.ToDouble(data[0]) * 1000);
						float endy = (float)(Convert.ToDouble(data[1]) * 1000);

						if (Math.Abs(endx) == float.PositiveInfinity || Math.Abs(endy) == float.PositiveInfinity )
							throw (new Exception());

						End = new PointF(endx, endy);
					}
					catch
					{
						End = PointF.Empty;
						throw (new Exception("Error converting value from 3di file: " + this.ToString()));
					}
				}
				else if (atr.Name == "PlyID")
				{
					int i = 0;
					try
					{
						i = Convert.ToInt32(atr.Value);
						PlyID = i;
					}
					catch
					{
						PlyID = -1;
					}
				}
			}
			//check for a 3d triangle strip
			if (xml.ChildNodes.Count != 0)
			{
				ReadTriStrip(xml);
			}
			return Start != null && End != null && PlyID != -1;
		}
		int ReadTriStrip(System.Xml.XmlNode xml)
		{
			XmlNode poly = null;
			foreach (XmlNode n in xml)
				if (n.Name == (new PolylineAttribute()).Type)
				{
					poly = n;
					break;
				}
			//use a polyline attribute to do the serialization
			m_strip = new PolylineAttribute(null, poly);
			m_strip.Scale(1000);
			return m_strip.Count;
		}
		public bool Remove()
		{
			if (Parent != null)
return Parent.Remove(this);
			return false;
		}

		public IAttribute CopyTo(NsNode newParent)
		{
			return newParent.Add(new TapeAttribute(newParent, m_label, m_points));
		}

		#endregion

		#region IEntity Members

		//public devDept.Eyeshot.Entities.BlockReference BlockRef
		//{
		//     get
		//     {
		//          if (m_br == null)
		//          {
		//               m_br = new BlockReference(0, 0, 0, Label, 1, 1, 1, 0);
		//          }
		//          m_br.EntityData = this;
		//          return m_br;
		//     }
		//}

		//BlockReference m_br;

		//public devDept.Eyeshot.Block GetBlock()
		//{
		//     Block b = new Block(Label);
		//     b.Entities.Add(CreateQuad());
		//     return b;
		//}
		//private PointF Rotate2DVector(PointF p_vect, float p_angle)
		//{
		//     PointF l_new = new PointF();
		//     l_new.X = p_vect.X * Math.Cos(p_angle) - p_vect.Y * Math.Sin(p_angle);
		//     l_new.Y = p_vect.X * Math.Sin(p_angle) + p_vect.Y * Math.Cos(p_angle);
		//     return l_new;
		//}

		//Quad CreateQuad()
		//{
		//     Point2D Vector = new Point2D(End.X - Start.X, End.Y - Start.Y);

		//     Point2D Normal = Rotate2DVector(Vector, Math.PI / 2.0);
		//     Vector3D Nvec = new Vector3D(Normal.X, Normal.Y, 0);
		//     Nvec.Normalize();
		//     Nvec.Length = 120 / 2;

		//     Point3D pnt1 = new Point3D();
		//     pnt1.X = Start.X + Nvec.X;
		//     pnt1.Y = Start.Y + Nvec.Y;
		//     pnt1.Z = Start.Z;

		//     Point3D pnt2 = new Point3D();
		//     pnt2.X = End.X + Nvec.X;
		//     pnt2.Y = End.Y + Nvec.Y;
		//     pnt2.Z = Start.Z;

		//     Point3D pnt3 = new Point3D();
		//     pnt3.X = End.X - Nvec.X;
		//     pnt3.Y = End.Y - Nvec.Y;
		//     pnt3.Z = Start.Z;

		//     Point3D pnt4 = new Point3D();
		//     pnt4.X = Start.X - Nvec.X;
		//     pnt4.Y = Start.Y - Nvec.Y;
		//     pnt4.Z = Start.Z;

		//     Quad tape = new Quad(pnt1, pnt2, pnt3, pnt4);
		//     tape.ColorMethod = colorMethodType.byEntity;
		//     //devDept.Eyeshot.Standard.QuadWithTexture tapetex = new QuadWithTexture(pnt1, pnt2, pnt3, pnt4, 
		//     tape.EntityData = this;
		//     tape.Color = System.Drawing.Color.FromArgb(100, System.Drawing.Color.Red);

		//     return tape;
		//}
		#endregion

		public string ToString(string format)
		{
			return Label;
		}
		public override string ToString()
		{
			return ToString("");
		}
	}
}
