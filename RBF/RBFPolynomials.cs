using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;
using RBF;

namespace RBFPolynomials
{
	/////////////////////
	// 3D
	////////////////////
	public class ParaboloidConst: IRBFPolynomial, IAttribute
	{
		public ParaboloidConst(NsNodes.NsNode parent, System.Xml.XmlNode xml)
		{
			if (!(parent is SurfaceRBF))
				throw new Exception("RBFParabolid's Parent must be a SurfaceRBF");
			if (!FromXml(xml))
				throw new NsNodes.AttributeXmlFormatException(this, xml, "Invalid Format in RBFParabolid");

		}

		public ParaboloidConst(NsNode surf)
			: this(surf as SurfaceRBF) { }
		public ParaboloidConst(SurfaceRBF surf)
		{
			m_surf = surf;
		}
		public ParaboloidConst(NsNode surf, double[] cofs)
			:this(surf)
		{
			cofs.CopyTo(polycofs, 0);
		}

		SurfaceRBF m_surf;
		double[] polycofs = new double[3];

		#region IRBFPolynomial Members

		public int Terms
		{
			get { return 3; }
		}

		public void Poly(double[] p, double[] d, double[] dd)
		{
			//Parabaloid
			p[2] += polycofs[0] * Math.Pow(p[0] - m_surf.Middle[0], 2) + polycofs[1] * Math.Pow(p[1] - m_surf.Middle[1], 2) + polycofs[2]; // A(x-h)^2 - B(y-k)^2 + C
			if (d != null)
			{
				d[0] += 2 * polycofs[0] * (p[0] - m_surf.Middle[0]);
				d[1] += 2 * polycofs[1] * (p[1] - m_surf.Middle[1]);
			}
		}

		public double FitMat(int i, int j)
		{
			return j >= Terms - 1 ? 1 : Math.Pow(m_surf.CenterNode[i][j] - m_surf.Middle[j], 2);
		}


		public double this[int i]
		{
			get
			{
				return polycofs[i];
			}
			set
			{
				polycofs[i] = value;
			}
		}

		public IAttribute CopyTo(NsNode newParent)
		{
			return newParent.Add(new ParaboloidConst(newParent, polycofs));
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0:g5}(x-h)^2 + {1:g5}(y-k)^2 + {2:g5}", polycofs[0], polycofs[1], polycofs[2]);
		}
		public string ToString(string format)
		{
			return ToString();
		}

		#region IAttribute Members

		public string Label
		{
			get { return "Polynomial"; }
		}

		public object Value
		{
			get
			{
				return polycofs;
			}
			set
			{
				if (!(value is double[]) || (value as double[]).Length != 3)
					throw new System.ArgumentException("DAMMIT");
				polycofs = value as double[];
			}
		}

		public NsNodes.NsNode Parent
		{
			get { return m_surf; }
		}

		public bool Query(string query)
		{
			throw new NotImplementedException();
		}

		public string Type
		{
			get { return this.GetType().ToString(); }
		}

		public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
		{
			System.Xml.XmlElement el = NsNodes.NsXmlHelper.MakeElement(doc, Type, "Parabaloid");
			NsNodes.NsXmlHelper.AddAttribute(doc, el, "A", polycofs[0].ToString());
			NsNodes.NsXmlHelper.AddAttribute(doc, el, "B", polycofs[1].ToString());
			NsNodes.NsXmlHelper.AddAttribute(doc, el, "C", polycofs[2].ToString());
			return el;
		}

		public bool FromXml(System.Xml.XmlNode xml)
		{
			if (xml.Attributes.Count != Terms + 1)
				return false;
			int c = 0;
			try
			{
				foreach (System.Xml.XmlAttribute atr in xml.Attributes)
				{
					if (atr.Name == "A")
					{
						polycofs[0] = Convert.ToDouble(atr.Value);
						c++;
					}
					if (atr.Name == "B")
					{
						polycofs[1] = Convert.ToDouble(atr.Value);
						c++;
					}
					if (atr.Name == "C")
					{
						polycofs[2] = Convert.ToDouble(atr.Value);
						c++;
					}
				}
			}
			catch
			{
				return false;
			}
			return c == Terms;
		}

		public bool Remove()
		{
			return Parent.Remove(this);
		}

		#endregion

	}
	public class Paraboloid : IRBFPolynomial, IAttribute
	{
		public Paraboloid(NsNodes.NsNode parent, System.Xml.XmlNode xml)
		{
			if (!(parent is SurfaceRBF))
				throw new Exception("RBFParabolid's Parent must be a SurfaceRBF");
			if (!FromXml(xml))
				throw new NsNodes.AttributeXmlFormatException(this, xml, "Invalid Format in RBFParabolid");

		}
		private Paraboloid(NsNode surf, double[] cofs)
			: this(surf)
		{
			cofs.CopyTo(polycofs, 0);
		}

		public Paraboloid(NsNode surf)
			: this(surf as SurfaceRBF) { }
		public Paraboloid(SurfaceRBF surf)
		{
			m_surf = surf;
		}
		SurfaceRBF m_surf;

		#region IRBFPolynomial Members

		public int Terms
		{
			get { return 2; }
		}

		public void Poly(double[] p, double[] d, double[] dd)
		{
			//Parabaloid
			p[2] += polycofs[0] * Math.Pow(p[0] - m_surf.Middle[0], 2) + polycofs[1] * Math.Pow(p[1] - m_surf.Middle[1], 2); // A(x-h)^2 - B(y-k)^2
			if (d != null)
			{
				d[0] += 2 * polycofs[0] * (p[0] - m_surf.Middle[0]);
				d[1] += 2 * polycofs[1] * (p[1] - m_surf.Middle[1]);
			}
		}

		public double FitMat(int i, int j)
		{
			return Math.Pow(m_surf.CenterNode[i][j] - m_surf.Middle[j], 2);
		}

		double[] polycofs = new double[2];

		public double this[int i]
		{
			get
			{
				return polycofs[i];
			}
			set
			{
				polycofs[i] = value;
			}
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0:g5}(x-h)^2 + {1:g5}(y-k)^2", polycofs[0], polycofs[1]);
		}

		public string ToString(string format)
		{
			return ToString();
		}

		public IAttribute CopyTo(NsNode newParent)
		{
			return newParent.Add(new Paraboloid(newParent, polycofs));
		}

		#region IAttribute Members

		public string Label
		{
			get { return "Polynomial"; }
		}

		public object Value
		{
			get
			{
				return polycofs;
			}
			set
			{
				if (!(value is double[]) || (value as double[]).Length != 2)
					throw new System.ArgumentException("DAMMIT");
				polycofs = value as double[];
			}
		}

		public NsNodes.NsNode Parent
		{
			get { return m_surf; }
		}

		public bool Query(string query)
		{
			throw new NotImplementedException();
		}

		public string Type
		{
			get { return this.GetType().ToString(); }
		}

		public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
		{
			System.Xml.XmlElement el = NsNodes.NsXmlHelper.MakeElement(doc, Type, "Parabaloid");
			NsNodes.NsXmlHelper.AddAttribute(doc, el, "A", polycofs[0].ToString());
			NsNodes.NsXmlHelper.AddAttribute(doc, el, "B", polycofs[1].ToString());
			return el;
		}

		public bool FromXml(System.Xml.XmlNode xml)
		{
			if (xml.Attributes.Count != Terms+1)
				return false;
			int c = 0;
			try
			{
				foreach (System.Xml.XmlAttribute atr in xml.Attributes)
				{
					if (atr.Name == "A")
					{
						polycofs[0] = Convert.ToDouble(atr.Value);
						c++;
					}
					if (atr.Name == "B")
					{
						polycofs[1] = Convert.ToDouble(atr.Value);
						c++;
					}
				}
			}
			catch
			{
				return false;
			}
			return c == Terms;
		}

		public bool Remove()
		{
			return Parent.Remove(this);
		}

		#endregion

	}
	public class Conic : IRBFPolynomial, IAttribute
	{
		public Conic(NsNodes.NsNode parent, System.Xml.XmlNode xml)
		{
			if (!(parent is SurfaceRBF))
				throw new Exception("RBFParabolid's Parent must be a SurfaceRBF");
			if (!FromXml(xml))
				throw new NsNodes.AttributeXmlFormatException(this, xml, "Invalid Format in RBFParabolid");

		}
		private Conic(NsNode surf, double[] cofs)
			: this(surf)
		{
			cofs.CopyTo(polycofs, 0);
		}
		public Conic(NsNode surf)
			: this(surf as SurfaceRBF) { }
		public Conic(SurfaceRBF surf)
		{
			m_surf = surf;
		}
		SurfaceRBF m_surf;

		#region IRBFPolynomial Members

		public int Terms
		{
			get { return 6; }
		}

		public void Poly(double[] p, double[] d, double[] dd)
		{
			//Parabaloid
			//p[2] += polycofs[0] * Math.Pow(p[0] - m_surf.Middle[0], 2) + polycofs[1] * Math.Pow(p[1] - m_surf.Middle[1], 2); // A(x-h)^2+B(x-h)(y-k)+C(y-k)^2+D(x-h)+E(y-k)+F = 0
			//p[2] += polycofs[0] * Math.Pow(p[0], 2) + polycofs[1] * p[0] * p[1] + polycofs[2] * Math.Pow(p[1], 2) + polycofs[3] * p[0] + polycofs[4] * p[1] + polycofs[5]; // Ax^2+Bxy+Cy^2+2Dx+Ey+F = 0
			p[2] += polycofs[0] * Math.Pow(p[0] - m_surf.Middle[0], 2) + polycofs[1] * (p[0] - m_surf.Middle[0]) * (p[1] - m_surf.Middle[1]) + polycofs[2] * Math.Pow(p[1] - m_surf.Middle[1], 2) + polycofs[3] * (p[0] - m_surf.Middle[0]) + polycofs[4] * (p[1] - m_surf.Middle[1]) + polycofs[5];
			// A(x-h)^2+B(x-h)(y-k)+C(y-k)^2+D(x-h)+E(y-k)+F = 0
			// Ax^2+Bxy+Cy^2+2Dx+Ey+F = 0
			if (d != null)
			{
				//d[0] += 2* polycofs[0]* p[0] + p[1]*polycofs[1] + polycofs[3];
				//d[1] += polycofs[1]*p[0] + 2 * polycofs[2] * p[1] + polycofs[4];
				d[0] += 2 * polycofs[0] * (p[0] - m_surf.Middle[0]) + (p[1] - m_surf.Middle[1]) * polycofs[1] + polycofs[3];
				d[1] += polycofs[1] * (p[0] - m_surf.Middle[0]) + 2 * polycofs[2] * (p[1] - m_surf.Middle[1]) + polycofs[4];
			}
		}

		//public double FitMat(int i, int j)
		//{
		//     //return Math.Pow(m_surf.CenterNode[i][j] - m_surf.Middle[j], 2);
		//     switch (j)
		//     {
		//          case 0:
		//               return m_surf.CenterNode[i][0] * m_surf.CenterNode[i][0];
		//          case 1:
		//               return m_surf.CenterNode[i][0] * m_surf.CenterNode[i][1];
		//          case 2:
		//               return m_surf.CenterNode[i][1] * m_surf.CenterNode[i][1];
		//          case 3:
		//               return m_surf.CenterNode[i][0];
		//          case 4:
		//               return m_surf.CenterNode[i][1];
		//          default:
		//               return 1;
		//     }
		//}
		public double FitMat(int i, int j)
		{
			//return Math.Pow(m_surf.CenterNode[i][j] - m_surf.Middle[j], 2);
			// A(x-h)^2+B(x-h)(y-k)+C(y-k)^2+D(x-h)+E(y-k)+F = 0
			switch (j)
			{
				case 0:
					return Math.Pow(m_surf.CenterNode[i][0] - m_surf.Center[0], 2);
				case 1:
					return (m_surf.CenterNode[i][0] - m_surf.Center[0]) * (m_surf.CenterNode[i][1] - m_surf.Center[1]);
				case 2:
					return Math.Pow(m_surf.CenterNode[i][1] - m_surf.Center[1], 2);
				case 3:
					return m_surf.CenterNode[i][0] - m_surf.Center[0];
				case 4:
					return m_surf.CenterNode[i][1] - m_surf.Center[1];
				default:
					return 1;
			}
		}

		double[] polycofs = new double[6];

		public double this[int i]
		{
			get
			{
				return polycofs[i];
			}
			set
			{
				polycofs[i] = value;
			}
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0:g5}(x-h)^2 + {1:g5}(y-k)^2", polycofs[0], polycofs[1]);
		}

		public string ToString(string format)
		{
			return ToString();
		}

		public IAttribute CopyTo(NsNode newParent)
		{
			return newParent.Add(new Conic(newParent, polycofs));
		}

		#region IAttribute Members

		public string Label
		{
			get { return "Polynomial"; }
		}

		public object Value
		{
			get
			{
				return polycofs;
			}
			set
			{
				if (!(value is double[]) || (value as double[]).Length != 2)
					throw new System.ArgumentException("DAMMIT");
				polycofs = value as double[];
			}
		}

		public NsNodes.NsNode Parent
		{
			get { return m_surf; }
		}

		public bool Query(string query)
		{
			throw new NotImplementedException();
		}

		public string Type
		{
			get { return this.GetType().ToString(); }
		}

		public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
		{
			System.Xml.XmlElement el = NsNodes.NsXmlHelper.MakeElement(doc, Type, "Parabaloid");
			NsNodes.NsXmlHelper.AddAttribute(doc, el, "A", polycofs[0].ToString());
			NsNodes.NsXmlHelper.AddAttribute(doc, el, "B", polycofs[1].ToString());
			return el;
		}

		public bool FromXml(System.Xml.XmlNode xml)
		{
			if (xml.Attributes.Count != Terms + 1)
				return false;
			int c = 0;
			try
			{
				foreach (System.Xml.XmlAttribute atr in xml.Attributes)
				{
					if (atr.Name == "A")
					{
						polycofs[0] = Convert.ToDouble(atr.Value);
						c++;
					}
					if (atr.Name == "B")
					{
						polycofs[1] = Convert.ToDouble(atr.Value);
						c++;
					}
				}
			}
			catch
			{
				return false;
			}
			return c == Terms;
		}

		public bool Remove()
		{
			return Parent.Remove(this);
		}

		#endregion

	}
	public class Plane : IRBFPolynomial, IAttribute
	{
		public Plane(NsNode parent, System.Xml.XmlNode xml)
		{
			if (parent != null && !(parent is SurfaceRBF))
				throw new Exception("RBFPoly1's Parent must be a SurfaceRBF");
			else
				m_surf = parent as SurfaceRBF;
			if (!FromXml(xml))
				throw new NsNodes.AttributeXmlFormatException(this, xml, "Invalid Format in RBFPoly1");

		}

		public Plane(NsNode surf)
			: this(surf as SurfaceRBF) { }
		public Plane(SurfaceRBF surf)
		{
			m_surf = surf;
		}

		private Plane(NsNode surf, double[] cofs)
		{
			if (!(surf is SurfaceRBF))
				throw new Exception("RBFPoly1's Parent must be a SurfaceRBF");
			else
				m_surf = surf as SurfaceRBF;
			cofs.CopyTo(polycofs, 0);
		}

		double[] polycofs = new double[3];
		SurfaceRBF m_surf;

		#region IRBFPolynomial Members

		public int Terms
		{
			get { return 3; }
		}

		public void Poly(double[] p, double[] d, double[] dd)
		{
			p[2] += polycofs[0] * p[0] + polycofs[1] * p[1] + polycofs[2]; // Ax + By + C
			if (d != null)
			{
				d[0] += polycofs[0];
				d[1] += polycofs[1];
			}
		}

		public double FitMat(int i, int j)
		{
			return j >= Terms - 1 ? 1 : m_surf.CenterNode[i][j];
		}

		public double this[int i]
		{
			get
			{
				return polycofs[i];
			}
			set
			{
				polycofs[i] = value;
			}
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0:g5}x + {1:g5}y + {2:g5}", polycofs[0], polycofs[1], polycofs[2]);
		}

		#region IAttribute Members

		public string Label
		{
			get { return "Planar"; }
		}

		public object Value
		{
			get
			{
				return polycofs;
			}
			set
			{
				if (!(value is double[]) || (value as double[]).Length != 3)
					throw new System.ArgumentException("DAMN IT");
				polycofs = value as double[];
			}
		}

		public NsNodes.NsNode Parent
		{
			get { return m_surf; }
		}

		public bool Query(string query)
		{
			throw new NotImplementedException();
		}

		public string Type
		{
			get { return this.GetType().ToString(); }
		}

		public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
		{
			System.Xml.XmlElement el = NsNodes.NsXmlHelper.MakeElement(doc, Type, "Polynomial");
			NsNodes.NsXmlHelper.AddAttribute(doc, el, "A", polycofs[0].ToString());
			NsNodes.NsXmlHelper.AddAttribute(doc, el, "B", polycofs[1].ToString());
			NsNodes.NsXmlHelper.AddAttribute(doc, el, "C", polycofs[2].ToString());
			return el;
		}

		public bool FromXml(System.Xml.XmlNode xml)
		{
			if (xml.Attributes.Count != 4)
				return false;
			int c = 0;
			try
			{
				foreach (System.Xml.XmlAttribute atr in xml.Attributes)
				{
					if (atr.Name == "A")
					{
						polycofs[0] = Convert.ToDouble(atr.Value);
						c++;
					}
					if (atr.Name == "B")
					{
						polycofs[1] = Convert.ToDouble(atr.Value);
						c++;
					}
					if (atr.Name == "C")
					{
						polycofs[2] = Convert.ToDouble(atr.Value);
						c++;
					}
				}
			}
			catch
			{
				return false;
			}
			return c == 3;
		}

		public bool Remove()
		{
			return Parent.Remove(this);
		}

		public IAttribute CopyTo(NsNode newParent)
		{
			return newParent.Add(new Plane(newParent, polycofs));
		}

		public string ToString(string format)
		{
			return ToString();
		}

		#endregion
	}

	/////////////////////
	// 2D
	////////////////////
	public class Linear : IRBFPolynomial, IAttribute
	{
		public Linear(NsNode curve)
			: this(curve as RBFCurve) { }

		public Linear(RBFCurve curve)
		{
			m_curve = curve;
		}

		private Linear(NsNode curve, double[] cofs)
		{
			if (!(curve is RBFCurve))
				throw new Exception("Linera Polynomial Parent must be a RBF Curve");
			else
				m_curve = curve as RBFCurve;
			cofs.CopyTo(polycofs, 0);
		}

		double[] polycofs = new double[2];//mx + b
		RBFCurve m_curve;

		#region IRBFPolynomial Members

		public int Terms
		{
			get { return 2; }
		}

		public void Poly(double[] p, double[] d, double[] dd)
		{
			p[1] += polycofs[0] * p[0] + polycofs[1]; // Ax + B
			if (d != null)
			{
				d[1] += polycofs[0];
			}
		}

		public double FitMat(int i, int j)
		{
			return j >= Terms - 1 ? 1 : m_curve.CenterNode[i][j];
		}

		public double this[int i]
		{
			get
			{
				return polycofs[i];
			}
			set
			{
				polycofs[i] = value;
			}
		}

		#endregion

		#region IAttribute Members

		public string Label
		{
			get { return "Linear"; }
		}

		public object Value
		{
			get
			{
				return polycofs;
			}
			set
			{
				if (!(value is double[]) || (value as double[]).Length != 2)
					throw new System.ArgumentException("Value must be double[2] for Linear Polynomial");
				polycofs = value as double[];
			}
		}

		public NsNode Parent
		{
			get { return m_curve; }
		}

		public bool Query(string query)
		{
			return false;
		}

		public string Type
		{
			get { return this.GetType().ToString(); }
		}

		public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
		{
			System.Xml.XmlElement el = NsNodes.NsXmlHelper.MakeElement(doc, Type, "Polynomial");
			NsNodes.NsXmlHelper.AddAttribute(doc, el, "A", polycofs[0].ToString());
			NsNodes.NsXmlHelper.AddAttribute(doc, el, "B", polycofs[1].ToString());
			return el;
		}

		public bool FromXml(System.Xml.XmlNode xml)
		{
			if (xml.Attributes.Count != 3)
				return false;
			int c = 0;
			try
			{
				foreach (System.Xml.XmlAttribute atr in xml.Attributes)
				{
					if (atr.Name == "A")
					{
						polycofs[0] = Convert.ToDouble(atr.Value);
						c++;
					}
					if (atr.Name == "B")
					{
						polycofs[1] = Convert.ToDouble(atr.Value);
						c++;
					}
				}
			}
			catch
			{
				return false;
			}
			return c == 2;
		}

		public bool Remove()
		{
			return Parent.Remove(this);
		}

		public IAttribute CopyTo(NsNode newParent)
		{
			return newParent.Add(new Linear(newParent, polycofs));
		}

		public string ToString(string format)
		{
			return ToString();
		}
		#endregion
	}
}
