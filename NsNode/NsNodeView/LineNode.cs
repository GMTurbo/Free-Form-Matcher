using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;
using System.Drawing;
using devDept.Eyeshot;
using devDept.Eyeshot.Standard;

namespace NsNodeView
{
     public class LineNode : NsNode, IEntityGroup
     {
          public LineNode(NsNode parent, System.Xml.XmlNode xml)
               : base(parent, xml)
          { }

          public LineNode(System.Drawing.Color color)
          {
               if (color != null)
                    Add(new StringAttribute(this, COLOR, color.Name));
          }
          public LineNode(IList<Point3DEntity> points, System.Drawing.Color color)
               : this(color)
          {
               foreach (Point3DEntity e in points)
                    Add(e);
          }
          public LineNode(IList<double[]> points, System.Drawing.Color color)
               : this(color)
          {
               foreach (double[] p in points)
                    Add(new Point3DEntity(this, null, p));
          }
          public LineNode(string label, double[,] points)
               : base(label)
          {
               for (int i = 0; i < points.GetLength(0); i++)
                    Add(new Point3DEntity(this, points[i, 0], points[i, 1], points[i, 2]));

               List<Point3D> pts = new List<Point3D>(Attributes.Count);
               foreach (IAttribute atr in Attributes)
                    if (atr is Point3Attribute)
                         pts.Add(new Point3D((atr as Point3Attribute).Value as double[]));

               LinearPath lp = new LinearPath(pts.ToArray(), System.Drawing.Color.Black);
               Length = lp.Length();
          }

          public LineNode(string label, IList<double[]> points)
               : base(label)
          {
               foreach (double[] p in points)
                    Add(new Point3DEntity(this, null, p));

               List<Point3D> pts = new List<Point3D>(Attributes.Count);
               foreach (IAttribute atr in Attributes)
                    if (atr is Point3Attribute)
                         pts.Add(new Point3D((atr as Point3Attribute).Value as double[]));

               LinearPath lp = new LinearPath(pts.ToArray(), System.Drawing.Color.Black);
               Length = lp.Length();
          }

          Color m_color;
          public System.Drawing.Color Color
          {
               get
               {
                    IAttribute atr = FindInherited(COLOR);
                    System.Drawing.Color c = System.Drawing.Color.Black;

                    if (atr != null)
                         c = System.Drawing.Color.FromName(atr.Value.ToString());
                    else
                         Color = c; //add a new color attribute

                    //return c;
                    return m_color;
               }
               set
               {
                    IAttribute atr = FindAttribute(COLOR);
                    System.Drawing.Color c = value;

                    if (value == System.Drawing.Color.Empty)
                         if (atr != null)
                              atr.Remove();
                         else
                              return;

                    if (atr != null)
                         atr.Value = c.Name;
                    else
                         Add(new StringAttribute(this, COLOR, c.Name));

                    m_color = value;
               }

          }
          public float Thickness
          {
               get
               {
                    IAttribute atr = FindAttribute("Thickness");
                    float f = 1;
                    if (atr != null)
                         return (float)(double)atr.Value;
                    return f;
               }
               set
               {
                    IAttribute atr = FindAttribute("Thickness");

                    if (atr != null)
                         atr.Value = value;
                    else
                         Add(new DoubleAttribute(this, "Thickness", value));
               }

          }
          public double Length
          {
               get
               {
                    IAttribute atr = FindAttribute("Length");
                    float f = 1;
                    if (atr != null)
                         return (double)atr.Value;
                    return f;
               }
               set
               {
                    IAttribute atr = FindAttribute("Length");

                    if (atr != null)
                         atr.Value = value;
                    else
                         Add(new DoubleAttribute(this, "Length", value));
               }

          }
          public bool DrawPoints
          {
               get
               {
                    IAttribute atr = FindInherited("DrawPoints");
                    if (atr != null && atr is BoolAttribute)
                         return (bool)atr.Value;
                    else
                         DrawPoints = true; //add a new attribute
                    return true;

               }
               set
               {
                    IAttribute atr = FindAttribute("DrawPoints");
                    if (atr != null)
                         atr.Value = value;
                    else
                         Add(new BoolAttribute(this, "DrawPoints", value));
               }
          }

          public void TransformBy(devDept.Eyeshot.Geometry.Transformation T)
          {
               Point3Attribute p;
               Point3D p3d;
               foreach (IAttribute a in Attributes)
               {
                    p = a as Point3Attribute;
                    if (p != null)
                    {
                         p3d = new Point3D(p.Pt);
                         p3d = T * p3d;
                         p[0] = p3d.X;
                         p[1] = p3d.Y;
                         p[2] = p3d.Z;
                    }
               }
          }

          #region IEntityGroup Members
          public static string DRAW = "DrawLines";
          public static string COLOR = "LineColor";

          Entity E;
          public Entity[] Entity
          {
               get
               {
                    //float thickness = 1.0f;
                    if (this.Label.ToLower().Contains("batten"))
                    {
                         Color = Color.Gray;
                         Thickness = 2.0f;
                    }

                    IAttribute a = FindInherited(DRAW);
                    if (a != null && a is BoolAttribute && (a as BoolAttribute).Bool == false)
                         return null;

                    List<Point3D> pts = new List<Point3D>(Attributes.Count);
                    foreach (IAttribute atr in Attributes)
                         if (atr is Point3Attribute)
                              pts.Add(new Point3D((atr as Point3Attribute).Value as double[]));
                    if (pts.Count > 1)
                    {
                         List<devDept.Eyeshot.Standard.Entity> ent = new List<devDept.Eyeshot.Standard.Entity>(1);
                         LinearPath path = E as LinearPath;
                         if (path == null)
                         {
                              E = path = new LinearPath(pts.ToArray(), m_color);
                              Length = path.Length();
                         }
                         else
                         {
                              path.Color = m_color;
                              path.ResizeVertices(pts.Count);
                              path.Vertices = pts.ToArray();
                              path.Thickness = Thickness;
                              Length = path.Length();
                         }
                         path.EntityData = this;
                         ent.Add(path);
                         return ent.ToArray();
                    }
                    else
                         return null;
               }
          }

          public devDept.Eyeshot.Labels.LabelBase devpDeptLabel
          {
               get
               {
                    IAttribute att = FindInherited("ShowBattens");
                    if (att != null && att is BoolAttribute && (bool)att.Value && this.Label.ToLower().Contains("batten"))
                    {
                         Point3D local = new Point3D((Attributes[0] as Point3Attribute).Value as double[]);
                         return new devDept.Eyeshot.Labels.OutlinedText(new Point3D(local.X - 100, local.Y, local.Z), this.Label, new Font("Helvetica", 10.0f), System.Drawing.Color.Black);
                    }
                    return null;
               }
          }

          #endregion

          #region IDragable Members

          public override void OnDrag(object sender, NodeTrackerEventArgs e)
          {
               //List<Point3D> pts = new List<Point3D>(Attributes.Count);
               //foreach (IAttribute atr in Attributes)
               //{
               //    if (atr is Point3Attribute)
               //    {
               //        double[] pnt = (atr.Value as double[]);
               //        pnt[0] += e.DeltaX;
               //        pnt[1] += e.DeltaY;
               //        pnt[2] += e.DeltaZ;
               //    }
               //}
               if (e.DeltaX + e.DeltaY + e.DeltaZ != 0)
                    TransformBy(new devDept.Eyeshot.Geometry.Translation(e.DeltaX, e.DeltaY, e.DeltaZ));
          }

          #endregion
     }
}
