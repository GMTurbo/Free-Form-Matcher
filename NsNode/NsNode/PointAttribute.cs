using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NsNodes
{
	public class PointAttribute : IAttribute
	{
		public PointAttribute()
			: this(null, 0, 0) { }
		public PointAttribute(NsNode parent, System.Xml.XmlNode xml)
			: this(parent, double.MinValue, double.MinValue)
		{
			if (!FromXml(xml))
                    throw new AttributeXmlFormatException(this, xml, "Failed to read xml\n" + parent.Label);
		}

		public PointAttribute(NsNode parent, string label, double x, double y)
		{
			m_parent = parent;
			m_label = label;
			m_pt = new double[2] { x, y };
		}
		public PointAttribute(NsNode parent, double x, double y)
			:this(parent, null, x, y)
		{
			//m_parent = parent;
			//m_pt = new double[2] { x, y };
		}
		public PointAttribute(NsNode parent, string label)
			:this(parent, label, 0,0)
		{
			//m_parent = parent;
			//m_label = label;
		}

		NsNode m_parent;
		double[] m_pt;
		string m_label;

		public double this[int i]
		{
			get { return m_pt[i]; }
			set { m_pt[i] = value; }
		}

		public double DistanceTo(double[] pnt2d)
		{
			double dis = 0;
			for (int i = 0; i < m_pt.Length; i++)
				dis += Math.Pow(m_pt[i] - pnt2d[i], 2);
			return Math.Sqrt(dis);
		}

		#region IAttribute Members

		public string Label
		{
			get { return m_label == null ? string.Format("({0}, {1})", m_pt[0], m_pt[1]) : m_label; }
			set { m_label = value; }
		}

		public object Value
		{
			get
			{
				return m_pt;
			}
			set
			{
				if (value is double[] && (value as double[]).Length == 2)
					m_pt = value as double[];
				else if (value == null)
					m_pt = new double[2] { 0, 0 };
				else
					throw new Exception("Invalid type for PointAttribute.Value, must be double[2]");
			}
		}

		public NsNode Parent
		{
			get { return m_parent; }
		}

		public bool Query(string query)
		{

			//x= or y=
			char op = query[0];
			op = char.ToLower(op);
			double d;
			try
			{
				d = Convert.ToDouble(query.Substring(2));
			}
			catch
			{
				return false;
			}
			switch (op)
			{
				case 'x':
					return m_pt[0] == d;
				case 'y':
					return m_pt[1] == d;
				default:
					return false;
			}

		}

		public string Type
		{
			get { return GetType().ToString(); }
		}

		public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
		{
			System.Xml.XmlElement el = NsXmlHelper.MakeElement(doc, Type, Label);
			NsXmlHelper.AddAttribute(doc, el, "point", ToString(false, null));


			return el;
		}

		public bool FromXml(System.Xml.XmlNode xml)
		{
			if (xml.Attributes == null || xml.Attributes.Count != 2)
				return false;

			m_label = xml.Attributes[0].Value;
			string label = xml.Attributes[1].Value;
			string[] vals = label.Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
			if (vals.Length != 2)
				return false;
			try
			{
				double x = double.Parse(vals[0]);
				double y = double.Parse(vals[1]);
				m_pt = new double[] { x, y };
			}
			catch
			{
				return false;
			}
			return true;
		}
		public bool Remove()
		{
			if (Parent != null)
                return Parent.Remove(this);
			return false;
		}

		public IAttribute CopyTo(NsNode newParent)
		{
			return newParent.Add(new PointAttribute(newParent, m_label, m_pt[0], m_pt[1]));
		}

		public string ToString(string format)
		{
			return m_label == null ? Label : string.Format("{2}=({0}, {1})", m_pt[0].ToString(format), m_pt[1].ToString(format), m_label);
		}

		#endregion

		public string ToString(bool bLabel, string format)
		{
			if (bLabel) 
				return ToString();
			else
				return string.Format("({0}, {1})", m_pt[0].ToString(format), m_pt[1].ToString(format));
		}
		public override string ToString()
		{
			return ToString("");
		}
	}
}
