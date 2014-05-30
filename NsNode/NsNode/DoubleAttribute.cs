using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NsNodes
{
	public class DoubleAttribute : IAttribute
	{
		public DoubleAttribute(NsNode parent, string label, double value)
		{
			m_parent = parent;
			m_label = label;
			m_value = value;
		}
		public DoubleAttribute(NsNode parent, System.Xml.XmlNode xml)
			:this(parent, "", 0)
		{
			if (!FromXml(xml))
				throw new AttributeXmlFormatException(this, xml, "Failed to read xml");
		}
		public DoubleAttribute(NsNode parent)
			: this(parent, "", 0)
		{ }
		public DoubleAttribute()
			:this(null)
		{

		}

		NsNode m_parent;
		string m_label;
		double m_value;

		public double Val
		{
			get { return m_value; }
			set { m_value = value; }
		}

		#region IAttribute Members

		public string Label
		{
			get { return m_label; }
		}

		public object Value
		{
			get
			{
				return m_value;
			}
			set
			{
				double i;
				i = Convert.ToDouble(value);
				m_value = i;
			}
		}

		public NsNode Parent
		{
			get { return m_parent; }
		}

		public bool Query(string query)
		{
			// label=value
			int i = query.IndexOf('=');//find the operator
			if (i == -1)
				return false;//invalid format

			string label = query.Substring(0, i);
			string value = query.Substring(i + 1);
			double val;
			try
			{
				val = Convert.ToDouble(value);
			}
			catch
			{
				return false;
			}
			return Label == label && m_value == val;

		}

		public string Type
		{
			get { return GetType().ToString(); }
		}

		public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
		{
			System.Xml.XmlElement el = doc.CreateElement(Type);
			//NsXmlHelper.AddAttribute(doc, el, Label, Value.ToString());
			NsXmlHelper.AddAttribute(doc, el, "Label", Label);
			NsXmlHelper.AddAttribute(doc, el, "Value", m_value.ToString());
			return el;
		}

		public bool FromXml(System.Xml.XmlNode xml)
		{
			if (xml.Attributes == null || xml.Attributes.Count < 2)
				return false;
			try
			{
				//m_label = xml.Attributes[0].Name;
				//Value = xml.Attributes[0].Value;
				m_label = xml.Attributes[0].Value;
				Value = xml.Attributes[1].Value;
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
			return newParent.Add(new DoubleAttribute(newParent, m_label, m_value));
		}

		public string ToString(string format)
		{
			bool bnum = false;
			StringBuilder sb = new StringBuilder();
			if (format == null || format == "")
				return Label;
			if (format.StartsWith("[lbl]"))
			{
				sb.Append(Label);
				format = format.Substring(5);
			}
			if (format.StartsWith("="))
			{
				sb.Append("=");
				format = format.Substring(1);
				bnum = true; //write value if "=" otherwise only write label;
			}
			if (bnum || format.Length > 0) //if there are futher format specifiers pass them to the value
				sb.Append(m_value.ToString(format));

			return sb.ToString();
		}

		#endregion

		public override string ToString()
		{
			return ToString("[lbl]=");
		}


	}
}
