using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace NsNodes
{
	public class ArrayAttribute : IAttribute
	{
		public ArrayAttribute(NsNode parent, string label, IList<double> array)
		{
			m_parent = parent;
			m_label = label;
			if (array != null)
				m_array = new List<double>(array);
			else
				m_array = new List<double>();
		}

		public ArrayAttribute(NsNode parent, XmlNode xml)
			:this(parent, null, null)
		{
            if (!FromXml(xml))
            {
                System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame(1, true);
                throw new AttributeXmlFormatException(this, xml, "Failed to read xml (" + stackFrame.GetMethod() + " ln: " + stackFrame.GetFileLineNumber() +")");
            }
		}

		string m_label;
		NsNode m_parent;
		List<double> m_array;

		public string Label
		{
			get
			{
				if (m_label == null && Parent != null)
					m_label = Parent.Label + "[" + Parent.IndexOf(this) + "]";
				return m_label;
			}
			set
			{
				if (value == "")
					m_label = null;
				else
					m_label = value;
			}
		}

		public NsNode Parent
		{
			get { return m_parent; }
		}

		public string Type
		{
			get { return GetType().ToString(); }
		}

		public double this[int i]
		{
			get { return m_array.Count < i ? double.MinValue : m_array[i]; }
		}

		#region IAttribute Members

		public object Value
		{
			get
			{
				return m_array;
			}
			set
			{
				if (value is IList<double>)
					m_array = new List<double>(value as IList<double>);
				else
					throw new FormatException("Invalid type for ArrayAttribute.Value, must be IList<double>");
				;
			}
		}

		public bool Query(string query)
		{
			throw new NotImplementedException();
		}

		public XmlElement ToXml(XmlDocument doc)
		{
			XmlElement el = NsXmlHelper.MakeElement(doc, Type, Label);
			int i = 0;
            m_array.ForEach(delegate(double pnt)
            {
                el.AppendChild(CreatePoint(doc, pnt, i++));
            });
			//foreach (double pnt in m_array)
			//{
			//	el.AppendChild(CreatePoint(doc, pnt, i++));
			//}
			return el;
		}
		XmlElement CreatePoint(XmlDocument doc, double pnt, int index)
		{
			XmlElement el = doc.CreateElement("Point");
			NsXmlHelper.AddAttribute(doc, el, index.ToString(), pnt.ToString());
			return el;
		}
		public bool FromXml(System.Xml.XmlNode xml)
		{
			if (xml.Attributes == null || xml.Attributes.Count != 1)
				return false;
			m_array.Clear();
			Label = xml.Attributes[0].Value;
            double pnt;
			foreach (XmlNode child in xml.ChildNodes)
			{
				pnt = CreatePoint(child);
				if (pnt == double.MinValue)
					return false;
				m_array.Add(pnt);
			}
			return m_array.Count > 0;
		}
		double CreatePoint(XmlNode xml)
		{
			if (xml.Name != "Point" || xml.Attributes.Count != 3)
				return double.MinValue;
			double val = double.MinValue;;
			foreach (XmlAttribute atr in xml.Attributes)
			{
				try
				{
					val = Convert.ToDouble(atr.Value);
				}
				catch
				{
					return double.MinValue;
				}
				return val;
			}
			return val;
		}

		public IAttribute CopyTo(NsNode newParent)
		{
			return newParent.Add(new ArrayAttribute(newParent, m_label, m_array));
		}

		public bool Remove()
		{
			return Parent.Remove(this);
		}

		public string ToString(string format)
		{
			return ToString();
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0}{{1}}", Label, m_array.Count);
		}
	}
}
