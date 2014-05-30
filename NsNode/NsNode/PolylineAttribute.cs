using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NsNodes
{
	public class PolylineAttribute: IAttribute
	{
		public PolylineAttribute(NsNode parent, string label, IList<double[]> points)
		{
			m_parent = parent;
			if (points != null)
				m_points = new List<double[]>(points);
			else
				m_points = new List<double[]>();

			Label = label;
		}
		public PolylineAttribute(NsNode parent, XmlNode xml)
			:this(parent, null, null)
		{
			if (!FromXml(xml))
				throw new AttributeXmlFormatException(this, xml, "Failed to read xml\n" + parent.Label);
		}
		public PolylineAttribute()
			:this(null, null, null)
		{		}

		public double[] this[int i]
		{
			get { return m_points.Count <= i ? null : m_points[i]; }
		}
		public double Length
		{
			get 
			{
				double l = 0;
				for( int i = 1; i < m_points.Count; i++ )
					l += Distance(m_points[i], m_points[i - 1]);
				return l;
			}
		}
		public int Count
		{
			get { return m_points.Count; }
		}

		public bool Add(double[] pnt)
		{
			if (pnt.Length != 3)
				return false;
			m_points.Add(pnt);
			return true;
		}
		public bool Set(int index, double[] pnt)
		{
			if (0 < index || index >= m_points.Count || pnt.Length != 3)
				return false;
			m_points[index] = pnt;
			return true;
		}

		#region IAttribute Members

		public string Label
		{
			get
			{
				if (m_label != null)
					return m_label;
				else if (Parent != null)
					// + "{" + m_points.Count.ToString() + "}"
					return Parent.Label + "[" + Parent.IndexOf(this) + "]";
				else
					return "";
			}
			set
			{
				if (value == "")
					m_label = null;
				else
					m_label = value;
			}
		}
		/// <summary>
		/// returns the xyz points for this polyline
		/// </summary>
		public object Value
		{
			get
			{
				return m_points;
			}
			set
			{
				if (value is IList<double[]>)
					m_points = new List<double[]>(value as IList<double[]>);
				else
					throw new FormatException("Invalid type for PolylineAttribute.Value, must be IList<double[]>");
			}
		}

		public NsNode Parent
		{
			get { return m_parent; }
		}
		public bool Query(string query)
		{
			// query by name and type
			int i = query.IndexOf('=');//find the operator
			if (i == -1)
				return false;//invalid format

			string type = query.Substring(0, i);
			string label = query.Substring(i + 1);
			string cur = this.GetType().ToString().Split('.')[1];
			if (label == "*") label = Label; //wildcard name
			return cur == type && label == Label;

		}

		public string Type
		{
			get { return GetType().ToString(); }
		}
		public XmlElement ToXml(XmlDocument doc)
		{

			XmlElement el = NsXmlHelper.MakeElement(doc, Type, Label);
			foreach (double[] pnt in m_points)
			{
				el.AppendChild(CreatePoint(doc, pnt));
			}
			return el;
		}
		XmlElement CreatePoint(XmlDocument doc, double[] pnt)
		{
			XmlElement el = doc.CreateElement("Point");
			NsXmlHelper.AddAttribute(doc, el, "x", pnt[0].ToString());
			NsXmlHelper.AddAttribute(doc, el, "y", pnt[1].ToString());
			NsXmlHelper.AddAttribute(doc, el, "z", pnt[2].ToString());
			return el;
		}
		public bool FromXml(System.Xml.XmlNode xml)
		{
			if (xml.Attributes == null || xml.Attributes.Count != 1)
				return false;
			m_points.Clear();
			Label = xml.Attributes[0].Value;
			double[] pnt;
			foreach (XmlNode child in xml.ChildNodes)
			{
				pnt = CreatePoint(child);
				if (pnt == null)
					return false;
				m_points.Add(pnt);
			}
			return m_points.Count>=0;
		}
		double[] CreatePoint(XmlNode xml)
		{
			if (xml.Name != "Point" || xml.Attributes.Count != 3)
				return null;
			double val;
			double[] pnt = new double[3];
			foreach (XmlAttribute atr in xml.Attributes)
			{
				try
				{
					val = Convert.ToDouble(atr.Value);
				}
				catch
				{
					return null;
				}
				if (atr.Name == "x")
					pnt[0] = val;
				else if (atr.Name == "y")
					pnt[1] = val;
				else if (atr.Name == "z")
					pnt[2] = val;
				else
					return null;
			}
			return pnt;
		}

		public bool Remove()
		{
			if (Parent != null)
				return Parent.Remove(this);
			return false;
		}

		#endregion

		string m_label;
		NsNode m_parent;
		List<double[]> m_points;

		double Distance(double[] p1, double[] p2)
		{
			double d = 0;
			for (int i = 0; i < 3; i++)
			{
				d += Math.Pow(p1[i] - p2[i], 2);
			}
			return Math.Sqrt(d);
		}
		public void Scale(double scalar)
		{
			for (int i = 0; i < Count; i++)
				for (int j = 0; j < 3; j++)
					m_points[i][j] *= scalar;
		}
		public override string ToString()
		{
			return Label + "{" + m_points.Count.ToString() + "}";
		}

		#region IAttribute Members


		public IAttribute CopyTo(NsNode newParent)
		{
			return newParent.Add(new PolylineAttribute(newParent, m_label, m_points));
		}

		public string ToString(string format)
		{
			return ToString();
		}

		#endregion
	}
}
