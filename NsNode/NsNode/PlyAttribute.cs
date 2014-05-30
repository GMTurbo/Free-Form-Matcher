using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NsNodes
{
	public class PlyAttribute : IAttribute
	{
		public PlyAttribute(NsNode parent, int id, int speed, string taperef, Color color)
		{
			m_parent = parent;
			m_id = id;
			m_speed = speed;
			m_taperef = taperef;
			m_color = color;
		}
		public PlyAttribute(NsNode parent, System.Xml.XmlNode xml)
			:this(parent, -1, -2, "NO_TAPE", Color.Black)
		{
			if (!FromXml(xml))
                    throw new AttributeXmlFormatException(this, xml, "Failed to read xml\n" + parent.Label);
		}
		public PlyAttribute()
			:this(null, -1, -2, "NO_TAPE", Color.Black)
		{

		}

		NsNode m_parent;
		public NsNode Parent
		{
			get { return m_parent; }
			set { m_parent = value; }
		}
		
		int m_id;
		public int ID
		{
			get { return m_id; }
			set { m_id = value; }
		}
		public string Label
		{
			get { return string.Format("Ply: {0}", ID); }
		}

		int m_speed;
		public int Speed
		{
			get { return m_speed; }
			set { m_speed = value; }
		}
		
		string m_taperef;
		public string TapeRef
		{
			get { return m_taperef; }
			set { m_taperef = value; }
		}
		
		Color m_color;
		public Color Color
		{
			get { return m_color; }
			set { m_color = value; }
		}
		String RGBColor
		{
			get
			{
				return string.Format("{0}, {1}, {2}", m_color.R, m_color.G, m_color.B);
			}
			set
			{
				int r, g, b;
				try
				{
					string[] v = value.Split(',');
					if (v.Length < 3)
						return;
					r = Convert.ToInt32(v[0]);
					g = Convert.ToInt32(v[1]);
					b = Convert.ToInt32(v[2]);
					m_color = Color.FromArgb(r, g, b);
				}
				catch
				{
					m_color = Color.Black;
				}
			}
		}

		#region IAttribute Members


		public object Value
		{
			get
			{
				return ID;
			}
			set
			{
				if (value is int)
					ID = (int)value;
			}
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
			System.Xml.XmlElement el = doc.CreateElement(Type);
			NsXmlHelper.AddAttribute(doc, el, "ID", ID.ToString());
			NsXmlHelper.AddAttribute(doc, el, "Speed", Speed.ToString());
			NsXmlHelper.AddAttribute(doc, el, "TapeRef", TapeRef.ToString());
			NsXmlHelper.AddAttribute(doc, el, "RGB", RGBColor);

			return el;
		}

		public bool FromXml(System.Xml.XmlNode xml)
		{
			if (xml.Attributes == null || xml.Attributes.Count < 4)
				return false;
			m_taperef = null; 
			m_speed = -2; 
			m_id = -1;
			m_color = Color.Black;
			foreach (System.Xml.XmlAttribute atr in xml.Attributes)
			{
				if (atr.Name == "TapeRef")
				{
					m_taperef = atr.Value;
				}
				else if (atr.Name == "ID")
				{
					try
					{
						m_id = Convert.ToInt32(atr.Value);
					}
					catch
					{
						m_id = -1;
					}
				}
				else if (atr.Name == "Speed")
				{
					try
					{
						m_speed = Convert.ToInt32(atr.Value);
					}
					catch
					{
						m_speed = -2;
					}
				}
				else if (atr.Name == "RGB")
				{
					RGBColor = atr.Value;
				}
			}

			return m_taperef != null && m_id != -1 && m_speed != -2;
		}

		public bool Remove()
		{
			if (Parent != null)
				return Parent.Remove(this);
			return false;
		}

		public IAttribute CopyTo(NsNode newParent)
		{
			return newParent.Add(new PlyAttribute(newParent, m_id, m_speed, m_taperef, m_color));
		}

		public string ToString(string format)
		{
			return ToString();
		}

		#endregion

		public override string ToString()
		{
			return Label;
		}

		#region IAttribute Members

		#endregion
	}
}
