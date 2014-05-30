using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;
using System.Xml;

namespace NsNodes
{
	public class StringAttribute : IAttribute
	{
		public StringAttribute()
			: this(null, "", "") { }
		public StringAttribute(NsNode parent)
			: this(parent, "", "") { }
		public StringAttribute(NsNode parent, XmlNode xml)
			:this(parent, "", "")
		{
			if (!FromXml(xml))
                    throw new AttributeXmlFormatException(this, xml, "Failed to read xml\n" + parent.Label);
		}
		public StringAttribute(NsNode parent, string label, string value)
		{
			m_label = label;
			m_value = value;
			m_parent = parent;
		}
		//public StringAttribute(string label, string value)
		//     :this(null, label, value)
		//{
		//}
		string m_label;
		string m_value;
		NsNode m_parent;

		public string ToString(string format)
		{
			//return ToString();
			StringBuilder sb = new StringBuilder();
			bool bnum = true;
			if (format == null || format == "")
				return Label;
			if (format.StartsWith("[lbl]"))
			{
				sb.Append(Label);
				format = format.Substring(5);
				bnum = false;
			}
			if (format.StartsWith("="))
			{
				sb.Append("=");
				format = format.Substring(1);
				bnum = true; //write value if "=" otherwise only write label;
			}
			if (bnum || format.StartsWith("[val]"))
				sb.Append(m_value);

			return sb.ToString();
		}

		public override string ToString()
		{
			return ToString("[lbl]=");
			//return string.Format("{0}={1}", Label, Value);
		}

		#region IAttribute Members

		public string Type
		{
			get { return GetType().ToString(); }
		}

		public string Label
		{
			get { return m_label; }
			private set { m_label = value; }
		}

		public object Value
		{
			get
			{
				return m_value; ;
			}
			set
			{
				if (value == null)
					m_value = "";
				else
					m_value = value.ToString();
			}
		}

		public NsNode Parent
		{
			get { return m_parent; }
			//set { m_parent = value; }
		}

		public bool Query(string query)
		{
			// label=value
			int i = query.IndexOf('=');//find the operator
			if (i == -1)
				return false;//invalid format

			string label = query.Substring(0, i);
			string value = query.Substring(i+1);

			return Label == label && Value.ToString() == value;
		}

		public XmlElement ToXml(XmlDocument doc)
		{
			XmlElement el = doc.CreateElement(Type);
			//NsXmlHelper.AddAttribute(doc, el, Label, Value.ToString());
			NsXmlHelper.AddAttribute(doc, el, "Label", Label);
			NsXmlHelper.AddAttribute(doc, el, "Value", Value.ToString());
			return el;
		}

		public bool FromXml(XmlNode xml)
		{
			if (xml.Attributes != null && xml.Attributes.Count == 1)
			{
				//if (xml.Attributes == null || xml.Attributes.Count != 1)
				//     return false;
				Label = xml.Attributes[0].Name;
				Value = xml.Attributes[0].Value;
				return true;
			}
			else if (xml.Attributes != null && xml.Attributes.Count == 2)
			{
				//if (xml.Attributes == null || xml.Attributes.Count != 2)
				//    return false;
				Label = xml.Attributes[0].Value;
				Value = xml.Attributes[1].Value;
				return true;
			}
			return false;
		}
		//bool FromXml(string xml)
		//{
		//     int i = xml.IndexOf('=');//find the operator
		//     if (i == -1)
		//          return false;//invalid format

		//     string label = xml.Substring(0, i);
		//     string value = xml.Substring(i);
		//     value.Trim('"');

		//     if (label.StartsWith(Type))
		//          label.Remove(0, Type.Length);

		//     m_label = label;
		//     m_value = value;
		//     return m_label != "" && m_value != "";
		//}
		public bool Remove()
		{
			if (Parent != null)
				return Parent.Remove(this);
			return false;
		}

		#endregion


		#region IAttribute Members


		public IAttribute CopyTo(NsNode newParent)
		{
			return newParent.Add(new StringAttribute(newParent, m_label, m_value));
		}

		#endregion
	}
}
