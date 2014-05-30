using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NsNodes
{
	public static class NsXmlHelper
	{
		public static XmlElement MakeElement(XmlDocument doc, string type, string label)
		{
			if (label == null)
				label = "";
			XmlElement xn;
			// create an element
			xn = doc.CreateElement(type);
			// add the label
			XmlAttribute xa;
			xa = doc.CreateAttribute("Label");
			xa.Value = label;
			xn.Attributes.Append(xa);
			return xn;
		}
		public static XmlAttribute AddAttribute(XmlDocument doc, XmlElement el, string name, string value)
		{
			//name = name.Replace(' ', '_');
			XmlAttribute atr = doc.CreateAttribute(name);
			atr.Value = value;
			el.Attributes.Append(atr);
			return atr;
		}
	}
}