using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;
using System.Xml;

namespace NsNodes
{
	public class MACAttribute : IAttribute
	{
		public MACAttribute(NsNode parent, string mac)
		{
			m_parent = parent;
			m_mac = mac;
		}
		public MACAttribute(NsNode parent, XmlNode xml)
			:this(parent, "")
		{
			if (!FromXml(xml))
				throw new AttributeXmlFormatException(this, xml, "Failed to read xml");
		}
		MACAttribute()
			: this(null, "")
		{ }
		NsNode m_parent;
		string m_mac;

		#region IAttribute Members

		public string Label
		{
			get { return m_mac; }
		}

		public object Value
		{
			get
			{
				return m_mac;
			}
			set
			{
				m_mac = value.ToString();
			}
		}

		public NsNode Parent
		{
			get { return m_parent; }
		}

		public bool Query(string query)
		{
			return m_mac == query;
		}

		public string Type
		{
			get { return GetType().ToString(); }
		}

		public XmlElement ToXml(XmlDocument doc)
		{
			XmlElement el = doc.CreateElement(Type);
			NsXmlHelper.AddAttribute(doc, el, "MAC", m_mac);
			return el;
		}
		public bool FromXml(XmlNode xml)
		{
			if (xml.Attributes == null || xml.Attributes.Count != 1)
				return false;
			m_mac = null;
			foreach (XmlAttribute atr in xml.Attributes)
			{
				if (atr.Name == "MAC")
				{
					m_mac = atr.Value;
				}
			}
			return m_mac != null;
		}
		public bool Remove()
		{
			if (Parent != null)
				return Parent.Remove(this);
			return false;
		}

		public IAttribute CopyTo(NsNode newParent)
		{
			return new MACAttribute(newParent, m_mac);
		}

		public string ToString(string format)
		{
			return ToString();
		}

		#endregion

		public override string ToString()
		{
			return Value.ToString();
		}
	}
	//class MACAttribute : IAttribute
	//{
	//     public MACAttribute(NsNode parent, string designer, string mac)
	//     {
	//          m_parent = parent;
	//          m_designer = designer;
	//          if (mac != null)
	//               m_macs.Add(mac);
	//     }
	//     public MACAttribute(NsNode parent, XmlNode xml)
	//          : this(parent, null, null)
	//     {
	//          if (!FromXml(xml))
	//               throw new AttributeXmlFormatException(this, xml, "Failed to read xml");
	//     }
	//     MACAttribute()
	//          : this(null, null, null)
	//     { }
	//     NsNode m_parent;
	//     string m_designer;
	//     List<string> m_macs = new List<string>(3);

	//     #region IAttribute Members

	//     public string Label
	//     {
	//          get { return m_designer; }
	//     }

	//     public object Value
	//     {
	//          get
	//          {
	//               return m_macs.Count > 0 ? m_macs[0] : "";
	//          }
	//          set
	//          {
	//               if (!m_macs.Contains(value.ToString()))
	//                    m_macs.Add(value.ToString());
	//          }
	//     }

	//     public NsNode Parent
	//     {
	//          get { return m_parent; }
	//     }

	//     public bool Query(string query)
	//     {
	//          //designer=MAC
	//          int i = query.IndexOf('=');
	//          if (i == -1)
	//               return false;
	//          string des = query.Substring(0, i);
	//          if (string.Compare(Label, des, true) != 0)
	//               return false;
	//          des = query.Substring(i);
	//          return m_macs.Contains(des);
	//     }

	//     public string Type
	//     {
	//          get { return GetType().ToString(); }
	//     }
	//     public static string Typeof
	//     {
	//          get { return new MACAttribute().Type; }
	//     }

	//     public XmlElement ToXml(XmlDocument doc)
	//     {
	//          XmlElement el = doc.CreateElement(Type);
	//          NodeIO.AddAttribute(doc, el, "Designer", m_designer);
	//          int i = 0;
	//          foreach (string str in m_macs)
	//          {
	//               NodeIO.AddAttribute(doc, el, string.Format("MAC{0}", i++), str);
	//          }
	//          return el;
	//     }
	//     public bool FromXml(XmlNode xml)
	//     {
	//          if (xml.Attributes == null || xml.Attributes.Count < 1)
	//               return false;
	//          m_macs.Clear(); m_designer = null;

	//          foreach (XmlAttribute atr in xml.Attributes)
	//          {
	//               if (atr.Name == "Designer")
	//               {
	//                    m_designer = atr.Value;
	//               }
	//               else if (atr.Name.StartsWith("MAC"))
	//               {
	//                    m_macs.Add(atr.Value);
	//               }
	//          }
	//          return m_designer != null;
	//     }
	//     #endregion

	//     public override string ToString()
	//     {
	//          return string.Format("{0}: {1}", Label, Value);
	//     }
	//}

}
