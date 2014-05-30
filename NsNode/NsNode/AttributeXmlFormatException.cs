using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NsNodes
{
	public class AttributeXmlFormatException : Exception
	{
		public AttributeXmlFormatException(IAttribute attribute, XmlNode xml, string msg)
			:base(msg)
		{
			m_attr = attribute;
			m_xn = xml;
		}

		IAttribute m_attr;

		public IAttribute Attribute
		{
			get { return m_attr; }
			set { m_attr = value; }
		}
		XmlNode m_xn;

		public XmlNode Xml
		{
			get { return m_xn; }
			set { m_xn = value; }
		}
	}
}
