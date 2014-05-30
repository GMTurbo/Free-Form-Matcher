using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;

namespace RBFBasis
{
     //public interface IBasisFunction : IAttribute
     //{
     //     double val(double r);
     //     /* returns the value of the first derivative of this function with respect to the radius */
     //     double dr(double r);
     //     /* returns the value of the second derivative of this function with respect to the radius */
     //     double ddr(double r);
     //}

     /// <summary>
     /// Thin Plate Spline Basis Function (r*r*log(r))
     /// </summary>
     public class ThinPlateSpline : IBasisFunction
     {
          #region IBasisFunction Members

          public ThinPlateSpline(NsNode parent)
          {
               m_parent = parent;
          }
          public ThinPlateSpline(NsNode parent, System.Xml.XmlNode xml)
               : this(parent)
          {

          }

          public double val(double r)
          {
               return r == 0 ? 0 : r * r * Math.Log(r);
          }

          public double dr(double r)
          {
               return r == 0 ? 0 : (2 * r * Math.Log(r) + r);
          }

          public double ddr(double r)
          {
               return r == 0 ? 0 : (2 * Math.Log(r) + 3);
          }

          #endregion

          #region IAttribute Members

          public string Label
          {
               get { return "ThinPlateSpline"; }
          }

          public object Value
          {
               get
               {
                    return "r*r*ln(r)";
               }
               set
               {
               }
          }

          NsNode m_parent;
          public NsNode Parent
          {
               get { return m_parent; }
          }

          public bool Query(string query)
          {
               return false;
          }

          public string Type
          {
               get { return GetType().ToString(); }
          }

          public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
          {
               return NsXmlHelper.MakeElement(doc, Type, ToString());
          }

          public bool FromXml(System.Xml.XmlNode xml)
          {
               return true;
          }

          public bool Remove()
          {
               return Parent.Remove(this);
          }

          public string ToString(string format)
          {
               return ToString();
          }

          public IAttribute CopyTo(NsNode newParent)
          {
               return newParent.Add(new ThinPlateSpline(newParent));
          }

          #endregion

          public override string ToString()
          {
               return "r*r*log(r)";
          }
     }

     public class ThinPlateSpline2 : IBasisFunction
     {
          #region IBasisFunction Members

          public ThinPlateSpline2(NsNode parent)
          {
               m_parent = parent;
          }
          public ThinPlateSpline2(NsNode parent, System.Xml.XmlNode xml)
               : this(parent)
          {

          }
          public ThinPlateSpline2(NsNode parent, double coef)
          {
               x = coef;
               m_parent = parent;
          }

          double x = 0.5;

          public double val(double r)
          {
               return r == 0 ? 0 : x * r * r * Math.Log(r);
          }

          public double dr(double r)
          {
               return r == 0 ? 0 : (2 * r * Math.Log(r) + r) * x;
          }

          public double ddr(double r)
          {
               return r == 0 ? 0 : (2 * Math.Log(r) + 3) * x;
          }

          #endregion

          #region IAttribute Members

          public string Label
          {
               get { return "ThinPlateSpline2"; }
          }

          public object Value
          {
               get
               {
                    return x + "*r*r*ln(r)";
               }
               set
               {
               }
          }

          NsNode m_parent;
          public NsNode Parent
          {
               get { return m_parent; }
          }

          public bool Query(string query)
          {
               return false;
          }

          public string Type
          {
               get { return GetType().ToString(); }
          }

          public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
          {
               return NsXmlHelper.MakeElement(doc, Type, ToString());
          }

          public bool FromXml(System.Xml.XmlNode xml)
          {
               return true;
          }

          public bool Remove()
          {
               return Parent.Remove(this);
          }

          public string ToString(string format)
          {
               return ToString();
          }

          public IAttribute CopyTo(NsNode newParent)
          {
               return newParent.Add(new ThinPlateSpline(newParent));
          }

          #endregion

          public override string ToString()
          {
               return x + "*r*r*log(r)";
          }
     }

     public class ThinPlateSpline3 : IBasisFunction
     {
          #region IBasisFunction Members

          public ThinPlateSpline3(NsNode parent)
          {
               m_parent = parent;
          }
          public ThinPlateSpline3(NsNode parent, System.Xml.XmlNode xml)
               : this(parent)
          {

          }

          public double val(double r)
          {
               return r == 0 ? 0 : Math.Pow(r, 3) * Math.Log(r);
          }

          public double dr(double r)
          {
               return r == 0 ? 0 : (3 * Math.Pow(r, 2) * Math.Log(r) + Math.Pow(r, 2));
          }

          public double ddr(double r)
          {
               return r == 0 ? 0 : (6 * r * Math.Log(r) + 5 * r);
          }

          #endregion

          #region IAttribute Members

          public string Label
          {
               get { return "ThinPlateSpline"; }
          }

          public object Value
          {
               get
               {
                    return "r*r*ln(r)";
               }
               set
               {
               }
          }

          NsNode m_parent;
          public NsNode Parent
          {
               get { return m_parent; }
          }

          public bool Query(string query)
          {
               return false;
          }

          public string Type
          {
               get { return GetType().ToString(); }
          }

          public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
          {
               return NsXmlHelper.MakeElement(doc, Type, ToString());
          }

          public bool FromXml(System.Xml.XmlNode xml)
          {
               return true;
          }

          public bool Remove()
          {
               return Parent.Remove(this);
          }

          public string ToString(string format)
          {
               return ToString();
          }

          public IAttribute CopyTo(NsNode newParent)
          {
               return newParent.Add(new ThinPlateSpline3(newParent));
          }

          #endregion

          public override string ToString()
          {
               return "r*r*log(r)";
          }
     }

     public class Gaussian : IBasisFunction
     {
          const double B = 0.01;

          public Gaussian(NsNode parent)
          {
               m_parent = parent;
          }
          public Gaussian(NsNode parent, System.Xml.XmlNode xml)
               : this(parent)
          {

          }

          #region IBasisFunction Members

          public double val(double r)
          {
               return r == 0 ? 0 : Math.Exp(-Math.Pow(B, 2) * Math.Pow(r, 2));
          }

          public double dr(double r)
          {
               return r == 0 ? 0 : -2 * r * Math.Exp(-Math.Pow(r, 2) * Math.Pow(B, 2)) * Math.Pow(B, 2);
          }

          public double ddr(double r)
          {
               return r == 0 ? 0 : (4 * Math.Pow(r, 2) * Math.Pow(B, 2) - 2 * Math.Pow(B, 2)) * Math.Exp(-Math.Pow(r, 2) * Math.Pow(B, 2));
          }

          #endregion

          #region IAttribute Members


          public string Label
          {
               get { return "Gaussian"; }
          }

          public object Value
          {
               get
               {
                    return "Math.Exp(-B^2*r^2)";
               }
               set
               {
               }
          }

          NsNode m_parent;
          public NsNode Parent
          {
               get { return m_parent; }
          }

          public bool Query(string query)
          {
               return false;
          }

          public string Type
          {
               get { return GetType().ToString(); }
          }

          public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
          {
               return NsXmlHelper.MakeElement(doc, Type, ToString());
          }

          public string ToString(string format)
          {
               return ToString();
          }

          public IAttribute CopyTo(NsNode newParent)
          {
               return newParent.Add(new Gaussian(newParent));
          }

          public bool FromXml(System.Xml.XmlNode xml)
          {
               return true;
          }

          public bool Remove()
          {
               return Parent.Remove(this);
          }

          #endregion
     }

     public class Multiquadratic : IBasisFunction
     {
          const double B = 0.5;

          public Multiquadratic(NsNode parent)
          {
               m_parent = parent;
          }
          public Multiquadratic(NsNode parent, System.Xml.XmlNode xml)
               : this(parent)
          {

          }
          #region IBasisFunction Members

          public double val(double r)
          {
               return r == 0 ? 0 : Math.Sqrt((r * r) + (B * B));
          }

          public double dr(double r)
          {
               return r == 0 ? 0 : r / Math.Sqrt(B * B + r * r); //r/(B**2 + r**2)**(1/2)
          }

          public double ddr(double r)
          {
               return r == 0 ? 0 : -(r * r) / Math.Pow(B * B + r * r, 3 / 2) + Math.Pow((B * B + r * r), -1 / 2); //-r**2/(B**2 + r**2)**(3/2) + (B**2 + r**2)**(-1/2)
          }

          #endregion

          #region IAttribute Members


          public string Label
          {
               get { return "Multiquadratic"; }
          }

          public object Value
          {
               get
               {
                    return "Math.Sqrt((r * r) + (B * B))";
               }
               set
               {
               }
          }

          NsNode m_parent;
          public NsNode Parent
          {
               get { return m_parent; }
          }

          public bool Query(string query)
          {
               return false;
          }

          public string Type
          {
               get { return GetType().ToString(); }
          }

          public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
          {
               return NsXmlHelper.MakeElement(doc, Type, ToString());
          }

          public bool FromXml(System.Xml.XmlNode xml)
          {
               return true;
          }

          public bool Remove()
          {
               return Parent.Remove(this);
          }

          public string ToString(string format)
          {
               return ToString();
          }

          public IAttribute CopyTo(NsNode newParent)
          {
               return newParent.Add(new Multiquadratic(newParent));
          }

          #endregion
     }

     public class InversMultiquadratic : IBasisFunction
     {
          const double B = 0.5;

          public InversMultiquadratic(NsNode parent)
          {
               m_parent = parent;
          }
          public InversMultiquadratic(NsNode parent, System.Xml.XmlNode xml)
               : this(parent)
          {

          }
          #region IBasisFunction Members

          public double val(double r)
          {
               return r == 0 ? 0 : 1 / Math.Sqrt((r * r) + (B * B));
          }

          public double dr(double r)
          {
               return r == 0 ? 0 : -r / Math.Pow(B * B + r * r, 3 / 2);
          }

          public double ddr(double r)
          {
               return r == 0 ? 0 : (2 * Math.Pow(r, 2) - Math.Pow(B, 2)) / Math.Pow(r * r + B * B, 5 / 2);
          }

          #endregion

          #region IAttribute Members


          public string Label
          {
               get { return "Multiquadratic"; }
          }

          public object Value
          {
               get
               {
                    return "Math.Sqrt((r * r) + (B * B))";
               }
               set
               {
               }
          }

          NsNode m_parent;
          public NsNode Parent
          {
               get { return m_parent; }
          }

          public bool Query(string query)
          {
               return false;
          }

          public string Type
          {
               get { return GetType().ToString(); }
          }

          public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
          {
               return NsXmlHelper.MakeElement(doc, Type, ToString());
          }

          public bool FromXml(System.Xml.XmlNode xml)
          {
               return true;
          }

          public bool Remove()
          {
               return Parent.Remove(this);
          }

          public string ToString(string format)
          {
               return ToString();
          }

          public IAttribute CopyTo(NsNode newParent)
          {
               return newParent.Add(new InversMultiquadratic(newParent));
          }

          #endregion
     }

     public class PolyHarmonic1 : IBasisFunction
     {
          #region IBasisFunction Members

          public PolyHarmonic1(NsNode parent)
          {
               m_parent = parent;
          }
          public PolyHarmonic1(NsNode parent, System.Xml.XmlNode xml)
               : this(parent)
          {

          }

          public double val(double r)
          {
               return r == 0 ? 0 : r;
          }

          public double dr(double r)
          {
               return r == 0 ? 0 : 1;
          }

          public double ddr(double r)
          {
               return r == 0 ? 0 : 0;
          }

          #endregion

          #region IAttribute Members

          public string Label
          {
               get { return "Poly Harmonic 3rd Order"; }
          }

          public object Value
          {
               get
               {
                    return "r^k*ln(r)";
               }
               set
               {
               }
          }

          NsNode m_parent;
          public NsNode Parent
          {
               get { return m_parent; }
          }

          public bool Query(string query)
          {
               return false;
          }

          public string Type
          {
               get { return GetType().ToString(); }
          }

          public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
          {
               return NsXmlHelper.MakeElement(doc, Type, ToString());
          }

          public bool FromXml(System.Xml.XmlNode xml)
          {
               return true;
          }

          public bool Remove()
          {
               return Parent.Remove(this);
          }
          public string ToString(string format)
          {
               return ToString();
          }

          public IAttribute CopyTo(NsNode newParent)
          {
               return newParent.Add(new PolyHarmonic1(newParent));
          }
          #endregion

          public override string ToString()
          {
               return "r^3*ln(r)";
          }
     }

     public class PolyHarmonic3 : IBasisFunction
     {
          #region IBasisFunction Members

          public PolyHarmonic3(NsNode parent)
          {
               m_parent = parent;
          }
          public PolyHarmonic3(NsNode parent, System.Xml.XmlNode xml)
               : this(parent)
          {

          }

          public double val(double r)
          {
               return r == 0 ? 0 : Math.Pow(r, 3);
          }

          public double dr(double r)
          {
               return r == 0 ? 0 : (3 * Math.Pow(r, 2));
          }

          public double ddr(double r)
          {
               return r == 0 ? 0 : (6 * r);
          }

          #endregion

          #region IAttribute Members

          public string Label
          {
               get { return "Poly Harmonic 3rd Order"; }
          }

          public object Value
          {
               get
               {
                    return "r^3";
               }
               set
               {
               }
          }

          NsNode m_parent;
          public NsNode Parent
          {
               get { return m_parent; }
          }

          public bool Query(string query)
          {
               return false;
          }

          public string Type
          {
               get { return GetType().ToString(); }
          }

          public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
          {
               return NsXmlHelper.MakeElement(doc, Type, ToString());
          }

          public bool FromXml(System.Xml.XmlNode xml)
          {
               return true;
          }

          public bool Remove()
          {
               return Parent.Remove(this);
          }

          public string ToString(string format)
          {
               return ToString();
          }

          public IAttribute CopyTo(NsNode newParent)
          {
               return newParent.Add(new PolyHarmonic3(newParent));
          }

          #endregion

          public override string ToString()
          {
               return "r^3";
          }
     }

     public class PolyHarmonic4 : IBasisFunction
     {
          #region IBasisFunction Members

          public PolyHarmonic4(NsNode parent)
          {
               m_parent = parent;
          }
          public PolyHarmonic4(NsNode parent, System.Xml.XmlNode xml)
               : this(parent)
          {

          }

          public double val(double r)
          {
               return r == 0 ? 0 : Math.Pow(r, 4) * Math.Log(r);
          }

          public double dr(double r)
          {
               return r == 0 ? 0 : (4 * Math.Pow(r, 3) * Math.Log(r) + Math.Pow(r, 3));
          }

          public double ddr(double r)
          {
               return r == 0 ? 0 : (12 * Math.Pow(r, 2) * Math.Log(r) + 7 * Math.Pow(r, 2));
          }

          #endregion

          #region IAttribute Members

          public string Label
          {
               get { return "Poly Harmonic 3rd Order"; }
          }

          public object Value
          {
               get
               {
                    return "r^k*ln(r)";
               }
               set
               {
               }
          }

          NsNode m_parent;
          public NsNode Parent
          {
               get { return m_parent; }
          }

          public bool Query(string query)
          {
               return false;
          }

          public string Type
          {
               get { return GetType().ToString(); }
          }

          public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
          {
               return NsXmlHelper.MakeElement(doc, Type, ToString());
          }

          public bool FromXml(System.Xml.XmlNode xml)
          {
               return true;
          }

          public bool Remove()
          {
               return Parent.Remove(this);
          }

          public string ToString(string format)
          {
               return ToString();
          }

          public IAttribute CopyTo(NsNode newParent)
          {
               return newParent.Add(new PolyHarmonic4(newParent));
          }

          #endregion

          public override string ToString()
          {
               return "r^3*ln(r)";
          }
     }

     public class PolyHarmonic5 : IBasisFunction
     {
          #region IBasisFunction Members

          public PolyHarmonic5(NsNode parent)
          {
               m_parent = parent;
          }
          public PolyHarmonic5(NsNode parent, System.Xml.XmlNode xml)
               : this(parent)
          {

          }

          public double val(double r)
          {
               return r == 0 ? 0 : Math.Pow(r, 5);
          }

          public double dr(double r)
          {
               return r == 0 ? 0 : (5 * Math.Pow(r, 4));
          }

          public double ddr(double r)
          {
               return r == 0 ? 0 : (20 * Math.Pow(r, 3));
          }

          #endregion

          #region IAttribute Members

          public string Label
          {
               get { return "Poly Harmonic 5th Order"; }
          }

          public object Value
          {
               get
               {
                    return "r^5";
               }
               set
               {
               }
          }

          NsNode m_parent;
          public NsNode Parent
          {
               get { return m_parent; }
          }

          public bool Query(string query)
          {
               return false;
          }

          public string Type
          {
               get { return GetType().ToString(); }
          }

          public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
          {
               return NsXmlHelper.MakeElement(doc, Type, ToString());
          }

          public bool FromXml(System.Xml.XmlNode xml)
          {
               return true;
          }

          public bool Remove()
          {
               return Parent.Remove(this);
          }

          public string ToString(string format)
          {
               return ToString();
          }

          public IAttribute CopyTo(NsNode newParent)
          {
               return newParent.Add(new PolyHarmonic5(newParent));
          }

          #endregion

          public override string ToString()
          {
               return "r^5";
          }
     }

     public class CustomBasis : IBasisFunction
     {
          #region IBasisFunction Members

          double a = 1, b = 1, c = 1, d = 1;

          public CustomBasis(NsNode parent)
          {
               m_parent = parent;
          }
          public CustomBasis(NsNode parent, System.Xml.XmlNode xml)
               : this(parent)
          {

          }

          public CustomBasis(double A, double B, double C, double D)
               : this(null)
          {
               a = A;
               b = B;
               c = C;
               d = D;
          }
          public CustomBasis(NsNode parent, double A, double B, double C, double D)
               : this(parent)
          {
               a = A;
               b = B;
               c = C;
               d = D;
          }
          public double val(double r)
          {
               return r == 0 ? 0 : a * Math.Pow(r, 3) + b * Math.Pow(r, 2) + c * r + d;
          }

          public double dr(double r)
          {
               return r == 0 ? 0 : 3 * a * Math.Pow(r, 2) + 2 * b * r + c;
          }

          public double ddr(double r)
          {
               return r == 0 ? 0 : 6 * a * r + 2 * b;
          }

          #endregion

          #region IAttribute Members

          public string Label
          {
               get { return "Custom: " + a.ToString("#0.000") + "r^3 + " + b.ToString("#0.000") + " r^2+ " + c.ToString("#0.000") + "r + " + d.ToString("#0.000"); }
          }

          public object Value
          {
               get
               {
                    return Label;
               }
               set
               {
               }
          }

          NsNode m_parent;
          public NsNode Parent
          {
               get { return m_parent; }
          }

          public bool Query(string query)
          {
               return false;
          }

          public string Type
          {
               get { return GetType().ToString(); }
          }

          public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
          {
               return NsXmlHelper.MakeElement(doc, Type, ToString());
          }

          public bool FromXml(System.Xml.XmlNode xml)
          {
               return true;
          }

          public bool Remove()
          {
               return Parent.Remove(this);
          }

          public string ToString(string format)
          {
               return ToString();
          }

          public IAttribute CopyTo(NsNode newParent)
          {
               return newParent.Add(new CustomBasis(newParent, a, b, c, d));
          }

          #endregion

          public override string ToString()
          {
               return Label;
          }
     }

     public class CustomBasisLog : IBasisFunction
     {
          #region IBasisFunction Members

          double a = 1, b = 1, c = 1, d = 1, e = 1;

          public CustomBasisLog(NsNode parent)
          {
               m_parent = parent;
          }
          public CustomBasisLog(NsNode parent, System.Xml.XmlNode xml)
               : this(parent)
          {

          }

          public CustomBasisLog(double A, double B, double C, double D, double E)
               : this(null)
          {
               e = E;
               a = A;
               b = B;
               c = C;
               d = D;
          }
          public CustomBasisLog(NsNode parent, double A, double B, double C, double D, double E)
               : this(parent)
          {
               e = E;
               a = A;
               b = B;
               c = C;
               d = D;
          }
          public double val(double r)
          {
               //a*x*x*x+b*x*x+c*x+d+log(e*x)
               return r == 0 ? 0 : (a * Math.Pow(r, 3) + b * Math.Pow(r, 2) + c * r + d) * Math.Log(e * r);
          }

          public double dr(double r)
          {
               //((3*a*r^3+2*b*r^2+c*r)*log(r)+(3*a*log(e)+a)*r^3+(2*b*log(e)+b)*r^2+(c*log(e)+c)*r+d)/r
               return r == 0 ? 0 : ((3 * a * Math.Pow(r, 3) + 2 * b * Math.Pow(r, 2) + c * r) * Math.Log(r) + (3 * a * Math.Log(e) + a) * Math.Pow(r, 3) + (2 * b * Math.Log(e) + b) * Math.Pow(r, 2) + (c * Math.Log(e) + c) * r + d) / r;
          }

          public double ddr(double r)
          {
               //((6*a*r^3+2*b*r^2)*log(r)+(6*a*log(e)+5*a)*r^3+(2*b*log(e)+3*b)*r^2+c*r-d)/r^2
               return r == 0 ? 0 : ((6 * a * Math.Pow(r, 3) + 2 * b * Math.Pow(r, 2)) * Math.Log(r) + (6 * a * Math.Log(e) + 5 * a) * Math.Pow(r, 3) + (2 * b * Math.Log(e) + 3 * b) * Math.Pow(r, 2) + c * r - d) / Math.Pow(r, 2);
          }

          #endregion

          #region IAttribute Members

          public string Label
          {
               get { return "CustomBasisLog: (" + a.ToString("#0.00") + "r^3 + " + b.ToString("#0.00") + " r^2+ " + c.ToString("#0.00") + "r + " + d.ToString("#0.00") + String.Format(") * Log({0}*r)", e.ToString("#0.00")); }
          }

          public object Value
          {
               get
               {
                    return Label;
               }
               set
               {
               }
          }

          NsNode m_parent;
          public NsNode Parent
          {
               get { return m_parent; }
          }

          public bool Query(string query)
          {
               return false;
          }

          public string Type
          {
               get { return GetType().ToString(); }
          }

          public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
          {
               return NsXmlHelper.MakeElement(doc, Type, ToString());
          }

          public bool FromXml(System.Xml.XmlNode xml)
          {
               return true;
          }

          public bool Remove()
          {
               return Parent.Remove(this);
          }

          public string ToString(string format)
          {
               return ToString();
          }

          public IAttribute CopyTo(NsNode newParent)
          {
               return newParent.Add(new CustomBasisLog(newParent, a, b, c, d, e));
          }

          #endregion

          public override string ToString()
          {
               return Label;
          }
     }
}
