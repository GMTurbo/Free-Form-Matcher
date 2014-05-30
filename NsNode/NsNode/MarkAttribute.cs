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
	public class MarkAttribute : IAttribute//, IEntity
	{
		string m_label;
		NsNode m_parent;
		PointF m_point;
		int m_plyid;

		public int PlyID
		{
			get { return m_plyid; }
		}
		public MarkAttribute(NsNode parent, string label, PointF point)
			: this(parent, label, point, 0) { }
		public MarkAttribute(NsNode parent, string label, PointF point, int plyid)
		{
			m_parent = parent;
			m_label = "Mark";
			m_plyid = plyid;
			if (point != PointF.Empty)
				m_point = point;
			else
				m_point = new PointF();
		}
		public MarkAttribute(NsNode parent, XmlNode xml)
			: this(parent, null, PointF.Empty)
		{
			if (!FromXml(xml))
				throw new AttributeXmlFormatException(this, xml, "Failed to read xml");
		}
		MarkAttribute()
			: this(null, null, PointF.Empty)
		{ }

		public PointF Position
		{
			get { return m_point; }
			set { m_point = value; }
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
				return m_point;
			}
			set
			{
				m_point = (PointF)value;
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
			NsXmlHelper.AddAttribute(doc, el, "Pos", (Position.X / 1000).ToString("#0.00000") + "," + (Position.Y / 1000).ToString("#0.00000"));
			NsXmlHelper.AddAttribute(doc, el, "PlyID", m_plyid.ToString());
			return el;
		}

		public bool FromXml(System.Xml.XmlNode xml)
		{
			if (xml.Attributes == null || xml.Attributes.Count < 2)
				return false;
			//m_label = null;
			foreach (XmlAttribute atr in xml.Attributes)
			{
				if (atr.Name == "Pos")
				{
					try
					{
						string[] data = null;
						data = atr.Value.Split(new Char[] { ',' });

						double startx = Convert.ToDouble(data[0]) * 1000;
						double starty = Convert.ToDouble(data[1]) * 1000;
						Position = new PointF((float)startx, (float)starty);
					}
					catch
					{
						Position = PointF.Empty;
					}
				}
				else if (atr.Name == "PlyID")
				{
					try
					{
						int i = Convert.ToInt32(atr.Value);
						m_plyid = i;
					}
					catch
					{
						m_plyid = -1;
					}
				}
			}

			return Position != null && PlyID != -1;
		}
		public bool Remove()
		{
			if (Parent != null)
				return Parent.Remove(this);
			return false;
		}

		public IAttribute CopyTo(NsNode newParent)
		{
			return newParent.Add(new MarkAttribute(newParent, m_label, m_point, m_plyid));
		}

		public string ToString(string format)
		{
			return ToString();
		}


		#endregion

		public override string ToString()
		{
			return Label;
		}
	}
}
