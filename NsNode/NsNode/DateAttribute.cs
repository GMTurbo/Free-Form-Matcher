using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;
using System.Xml;

namespace NsNodes
{
	public class DateAttribute : IAttribute
	{
		public DateAttribute(NsNode parent, string label, DateTime date )
		{
			m_parent = parent;
			m_date = date;
			m_label = label;
		}
		public DateAttribute(NsNode parent, XmlNode xml)
			: this(parent, null, m_nullDate)
		{
			if (!FromXml(xml))
				throw new AttributeXmlFormatException(this, xml, "Failed to read xml");
		}
		public DateAttribute(NsNode parent)
			: this(parent, null, m_nullDate)
		{ }
		public DateAttribute()
			: this(null)
		{ }

		NsNode m_parent;
		DateTime m_date;

		public DateTime Date
		{
			get { return m_date; }
		}
		//public String Note
		//{
		//     get { return m_note; }
		//}

		string m_label;
		static DateTime m_nullDate = new DateTime(1);

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
		//     try
		//     {
		//          m_date = Convert.ToDateTime(label);
		//     }
		//     catch
		//     {
		//          m_date = m_nullDate;
		//          m_note = xml;
		//          return false;
		//     }
		//     m_note = value;
		//     return m_date != m_nullDate;
		//}

		#region IAttribute Members

		public string Type
		{
			get { return GetType().ToString(); }
		}
		public string Label
		{
			//get { return m_date == m_nullDate ? "" : m_date.ToString(); }
			get { return m_label; }
		}

		public object Value
		{
			get
			{
				return Date;
			}
			set
			{
				DateTime dt;
				if (value is DateTime)
					m_date = (DateTime)value;
				else if (value is string)
					dt = Convert.ToDateTime(value);
				else
					throw new ArgumentException("DateAttribute.Value must be DateTime"); 
			}
		}

		public NsNode Parent
		{
			get
			{
				return m_parent;
			}
		}

		public bool Query(string query)
		{
			// >DATE, <DATE, or =DATE
			char op = query[0];
			DateTime dt;
			try
			{
				dt = Convert.ToDateTime(query.Substring(1));
			}
			catch
			{
				return false;
			}
			switch (op)
			{
				case '<':
					return m_date < dt && !IsEqual(m_date, dt);
				case '>':
					return m_date > dt && !IsEqual(m_date, dt);
				case '=':
					return IsEqual(m_date, dt);
				default:
					return false;
			}
		}
		bool IsEqual(DateTime d1, DateTime d2)
		{
			return (d1 - d2).Duration().Ticks < TimeSpan.TicksPerMinute; //1minute tolerance 

		}
		public XmlElement ToXml(XmlDocument doc)
		{
			XmlElement el = doc.CreateElement(Type);
			NsXmlHelper.AddAttribute(doc, el, "Label", Label.ToString());
			NsXmlHelper.AddAttribute(doc, el, "Date", m_date.ToBinary().ToString());
			return el;
		}
		public bool FromXml(XmlNode xml)
		{
			if (xml.Attributes == null || xml.Attributes.Count != 2)
				return false;
			m_label = null; m_date = m_nullDate;
			foreach (XmlAttribute atr in xml.Attributes)
			{
				if (atr.Name == "Date")
				{
					try
					{
						long bin = Convert.ToInt64(atr.Value);
						DateTime date = DateTime.FromBinary(bin);
						m_date = date;
					}
					catch (Exception e)
					{
						m_date = m_nullDate;
						m_label = xml.OuterXml;
						return false;
					}
				}
				else if (atr.Name == "Label")
				{
					m_label = atr.Value;
				}
			}
			return m_label != null && m_date != m_nullDate;
		}
		public bool Remove()
		{
			if (Parent != null)
				return Parent.Remove(this);
			return false;
		}

		public IAttribute CopyTo(NsNode newParent)
		{
			return newParent.Add(new DateAttribute(newParent, m_label, Date));
		}

		#endregion
		public string ToString(string format)
		{
			return m_date == m_nullDate && m_label != null ? m_label : m_label + "=" + m_date.ToString(format);
		}

		public override string ToString()
		{
			return ToString("");
		}
	}
}
