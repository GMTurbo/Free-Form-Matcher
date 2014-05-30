using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NsNodes
{
	public class BoolAttribute : IAttribute
	{
		public BoolAttribute(NsNode parent, string label, bool value)
		{
			m_parent = parent;
			m_label = label;
			m_value = value;
		}
		public BoolAttribute(NsNode parent, System.Xml.XmlNode xml)
			:this(parent, "", false)
		{
			if (!FromXml(xml))
				throw new AttributeXmlFormatException(this, xml, "Failed to read xml");
		}
		public BoolAttribute(NsNode parent)
			:this(parent, "", false)	{		}
		public BoolAttribute()
			:this(null, "", false)	{		}

		NsNode m_parent;
		string m_label;
		bool m_value;

		public bool Bool
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
				bool i;
				i = Convert.ToBoolean(value);
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
			bool val;
			try
			{
				val = Convert.ToBoolean(value);
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
			return newParent.Add(new BoolAttribute(newParent, m_label, m_value));
		}

		#endregion

		public string ToString(string format)
		{
			if (format == "d")
				return Label + "=" + (m_value ? "1" : "0");
			return Label + "=" + m_value.ToString();
		}
		public override string ToString()
		{
			return ToString("");
		}
	}
}

