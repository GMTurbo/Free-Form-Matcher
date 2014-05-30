using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;

namespace NsNodes
{
	public class TapeRefAttribute : IAttribute
	{
		public TapeRefAttribute(NsNode parent, string Label, string ItemNumber, double Width)
		{
			m_parent = parent;
			m_width = Width;
			m_label = Label;
			m_itemnumber = ItemNumber;
		}

		public TapeRefAttribute(NsNode parent, XmlNode xml)
			:this(parent, null, null, -1)
		{
			if (!FromXml(xml))
                    throw new AttributeXmlFormatException(this, xml, "Failed to read xml\n" + parent.Label);
		}

		TapeRefAttribute()
			:this(null, null, null, -1)
		{}
		public double Width
		{
			get { return m_width; }
			set { m_width = value; }
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
				return m_itemnumber;
			}
			set
			{
				m_itemnumber = value.ToString();
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
			System.Xml.XmlElement el = NsXmlHelper.MakeElement(doc, Type, Label);
			NsXmlHelper.AddAttribute(doc, el, "Item", Value.ToString());
			NsXmlHelper.AddAttribute(doc, el, "Width", Width.ToString());
		
			return el;
		}

		public bool FromXml(System.Xml.XmlNode xml)
		{
			if (xml.Attributes == null || xml.Attributes.Count < 3)
				return false;
			m_label = null; m_width = -1; m_itemnumber = null;
			foreach (XmlAttribute atr in xml.Attributes)
			{
				if (atr.Name == "Label")
				{
					m_label = atr.Value;
				}
				else if (atr.Name == "Item")
				{
					try
					{
						m_itemnumber = atr.Value;
					}
					catch
					{
						m_itemnumber = null;
					}
				}
				else if (atr.Name == "Width")
				{
					try
					{
						m_width = Convert.ToDouble(atr.Value);
					}
					catch
					{
						m_width = -1;
					}
				}
			}

			return m_label != null && m_width != -1 && m_itemnumber != null;
		}

		public bool Remove()
		{
			if (Parent != null)
				return Parent.Remove(this);
			return false;
		}

		public IAttribute CopyTo(NsNode newParent)
		{
			return newParent.Add(new TapeRefAttribute(newParent, m_label, m_itemnumber, m_width));
		}

		string m_label;
		double m_width;
		string m_itemnumber;
		NsNode m_parent;

		#endregion
		public string ToString(string format)
		{
			return ToString();
		}

		public override string ToString()
		{
			return "Material: " + m_label + " \n" + "Item Number: " + m_itemnumber;
		}
	}
}
