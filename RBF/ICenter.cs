using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;

namespace RBFTools
{
     public interface ICenter<T> : IAttribute where T : struct // only allow template to take System.ValueTypes
     {
          T radius(T[] p);
          T w { get; set; }
          T this[int i] { get; set; }
          T[] ToArray();
     }

     public class Center3d : ICenter<double>
     {
          public Center3d(double x, double y, double w)
          {
               m_pt = new double[] { x, y };
               m_weight = w;
               m_parent = null;
          }
          public Center3d(double x, double y)
               : this(x, y, 0)
          { }
          Center3d()
               : this(double.MinValue, double.MinValue, double.MinValue)
          { }
          public Center3d(NsNode parent, System.Xml.XmlNode xml)
               : this()
          {
               m_parent = parent;
               if (!FromXml(xml))
                    throw new AttributeXmlFormatException(this, xml, "Failed to read xml");
          }

          Center3d(NsNode parent, double[] pt, double weight)
          {
               m_parent = parent;
               pt.CopyTo(m_pt, 0);
               m_weight = weight;
          }
          double[] m_pt = new double[2];
          double m_weight;
          NsNode m_parent;

          #region ICenter Members

          public double radius(double[] p)
          {
               return Math.Sqrt(Math.Pow(p[0] - this[0], 2) + Math.Pow(p[1] - this[1], 2));
          }

          public double w
          {
               get { return m_weight; }
               set { m_weight = value; }
          }

          public double this[int i]
          {
               get
               {
                    return m_pt[i];
               }
               set
               {
                    m_pt[i] = value;
               }
          }

          public double[] ToArray()
          {
               return new double[3] { m_pt[0], m_pt[1], m_weight };
          }

          #endregion

          #region IAttribute Members

          public string Label
          {
               //get { return Parent != null ? "Center[" + Parent.IndexOf(this).ToString() + "]" : "Center"; }
               get { return string.Format("({0}, {1}, {2})", m_pt[0], m_pt[1], m_weight); }

          }

          public object Value
          {
               get
               {
                    return ToArray();
               }
               set
               {
                    if (value is double[] && (value as double[]).Length == 3)
                    {
                         m_pt[0] = (value as double[])[0];
                         m_pt[0] = (value as double[])[1];
                         m_weight = (value as double[])[2];
                    }
                    else if (value == null)
                    {
                         m_pt = null;
                         m_weight = 0;
                    }
                    else
                         throw new Exception("Invalid type for Center<3>.Value, must be double[3]");
               }
          }

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
               get { return new Center3d().GetType().ToString(); }
          }

          public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
          {
               System.Xml.XmlElement el = NsXmlHelper.MakeElement(doc, Type, Label);
               return el;
          }

          public bool FromXml(System.Xml.XmlNode xml)
          {
               if (xml.Attributes == null || xml.Attributes.Count != 1)
                    return false;

               string label = xml.Attributes[0].Value;
               string[] vals = label.Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
               if (vals.Length != 3)
                    return false;
               try
               {
                    double x = Double.Parse(vals[0]);
                    double y = double.Parse(vals[1]);
                    m_pt = new double[2] { x, y };
                    m_weight = double.Parse(vals[2]);
               }
               catch
               {
                    return false;
               }
               return true;
          }

          public bool Remove()
          {
               if (Parent != null)
                    return Parent.Remove(this);
               return false;
          }

          #endregion

          public override string ToString()
          {
               return Label;
          }

          public string ToString(string format)
          {
               return ToString();
          }

          public IAttribute CopyTo(NsNode newParent)
          {
               return newParent.Add(new Center3d(newParent, m_pt, m_weight));
          }

     }
     public class Center2d : ICenter<double>
     {
          public Center2d(double x, double w)
          {
               m_pt = new double[] { x, w };
               m_parent = null;
          }
          public Center2d(double x)
               : this(x, 0)
          { }
          Center2d()
               : this(double.MinValue, double.MinValue)
          { }
          public Center2d(NsNode parent, System.Xml.XmlNode xml)
               : this()
          {
               m_parent = parent;
               if (!FromXml(xml))
                    throw new AttributeXmlFormatException(this, xml, "Failed to read xml");
          }

          Center2d(NsNode parent, double[] pt)
          {
               m_parent = parent;
               pt.CopyTo(m_pt, 0);
          }
          double[] m_pt = new double[2];
          NsNode m_parent;

          #region ICenter Members

          public double radius(double[] p)
          {
               return Math.Abs(p[0] - this[0]);
          }

          public double w
          {
               get { return m_pt[1]; }
               set { m_pt[1] = value; }
          }

          public double this[int i]
          {
               get
               {
                    return m_pt[i];
               }
               set
               {
                    m_pt[i] = value;
               }
          }

          public double[] ToArray()
          {
               return new double[2] { m_pt[0], m_pt[1] };
          }

          #endregion

          #region IAttribute Members

          public string Label
          {
               //get { return Parent != null ? "Center[" + Parent.IndexOf(this).ToString() + "]" : "Center"; }
               get { return string.Format("({0}, {1})", m_pt[0], m_pt[1]); }

          }

          public object Value
          {
               get
               {
                    return ToArray();
               }
               set
               {
                    if (value is double[] && (value as double[]).Length == 2)
                    {
                         m_pt[0] = (value as double[])[0];
                         m_pt[0] = (value as double[])[1];
                    }
                    else if (value == null)
                    {
                         m_pt = new double[2];
                    }
                    else
                         throw new Exception("Invalid type for Center<3>.Value, must be double[3]");
               }
          }

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
               get { return new Center2d().GetType().ToString(); }
          }

          public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
          {
               System.Xml.XmlElement el = NsXmlHelper.MakeElement(doc, Type, Label);
               return el;
          }

          public bool FromXml(System.Xml.XmlNode xml)
          {
               if (xml.Attributes == null || xml.Attributes.Count != 1)
                    return false;

               string label = xml.Attributes[0].Value;
               string[] vals = label.Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
               if (vals.Length != 2)
                    return false;
               try
               {
                    double x = Double.Parse(vals[0]);
                    double y = double.Parse(vals[1]);
                    m_pt = new double[2] { x, y };
               }
               catch
               {
                    return false;
               }
               return true;
          }

          public bool Remove()
          {
               if (Parent != null)
                    return Parent.Remove(this);
               return false;
          }

          #endregion

          public override string ToString()
          {
               return Label;
          }

          public string ToString(string format)
          {
               return ToString();
          }

          public IAttribute CopyTo(NsNode newParent)
          {
               return newParent.Add(new Center2d(newParent, m_pt));
          }
     }
     public class CenterArrayNode : NsNode
     {
          CenterArrayNode() : base("Centers", null) { }
          public CenterArrayNode(NsNode parent, System.Xml.XmlNode xml)
               : base(parent, xml)
          {
               //Parent = parent;
               ////Label = "Centers";
               //if(!FromXml(xml))
               //     throw new AttributeXmlFormatException(null, xml, "Failed to read xml");
          }
          public CenterArrayNode(NsNode parent)
               : base("Centers", parent)
          { }
          public CenterArrayNode(NsNode parent, List<ICenter<double>> centerdata)
               : base("Centers", parent)
          {
               if (centerdata != null)
                    foreach (ICenter<double> center in centerdata)
                         Add(center);
          }

          public List<IAttribute> Centers
          {
               get { return Attributes; }
               set
               {
                    PauseUpdating();
                    Attributes.Clear();
                    value.ForEach(delegate(IAttribute atr)
                    {
                         Add(atr);
                    });
                    //foreach (IAttribute atr in value)
                    //	Add(atr);
                    ResumeUpdating(false);
               }
          }

          public void ClearCenters()
          {
               Attributes.Clear();
          }

          public new ICenter<double> this[int i]
          {
               get { return Attributes[i] as ICenter<double>; }
               set { Attributes[i] = value; }
          }

          public List<double[]> CentersAsList
          {
               get
               {
                    List<double[]> ret = new List<double[]>(Centers.Count);
                    Centers.ForEach(delegate(IAttribute center)
                    {
                         ret.Add((double[])center.Value);
                    });
                    //foreach (IAttribute center in Centers)
                    //    ret.Add((double[])center.Value);

                    return ret;
               }
          }
     }
}
