using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;
using devDept.Eyeshot;
using devDept.Eyeshot.Standard;
using devDept.Eyeshot.Geometry;
using System.Drawing;
using System.IO;

namespace NsNodeView
{
     public class MeshNode : NsNode, IEntityGroup
     {
          public MeshNode(string label, NsNode parent, double[,] initTransform)
               : this(label, parent)
          {
               for (int i = 0; i < 3; i++)
                    initTransform[i, 3] *= 1000;

               m_T = new Transformation(initTransform);
          }

          public MeshNode(string label, NsNode parent)
               : base(label, parent)
          {
               //Add(new MatrixNode(this, "Transformation", 4,4));
          }
          public MeshNode(string label, System.Drawing.Color color)
               : base(label)
          {
               Color = color;

               //Add(new MatrixNode(this, "Transformation", 4,4));
          }
          public MeshNode(NsNode parent, System.Xml.XmlNode xml)
               : base(parent, xml)
          {
               //if(!FromXml(xml))
               //     throw new AttributeXmlFormatException(null, xml, "Failed to read xml");

               //Add(new MatrixNode(this, "Transformation", 4, 4));

          }
          public System.Drawing.Color Color
          {
               get
               {
                    IAttribute atr = FindInherited(COLOR);
                    //System.Drawing.Color c;

                    if (atr != null)
                         return System.Drawing.Color.FromName(atr.Value.ToString());
                         //return System.Drawing.Color.FromArgb(128, System.Drawing.Color.FromName(atr.Value.ToString()));//System.Drawing.Color.FromName(atr.Value.ToString());

                    return System.Drawing.Color.Empty;
               }
               set
               {
                    IAttribute atr = FindAttribute(COLOR);
                    System.Drawing.Color c = value;

                    if (atr != null)
                         atr.Value = c.Name;
                    else
                         Add(new StringAttribute(this, COLOR, c.Name));
               }

          }

          //public MatrixNode TransformationMatrix
          //{
          //     get
          //     {
          //          foreach (NsNode node in Nodes)
          //               if (node is MatrixNode)
          //                    return node as MatrixNode;
          //          return null;
          //     }
          //}

          public int ReadCsvFile(string path, Point3D Offsets, double scale)
          {
               List<Point3D> vertices = new List<Point3D>(100);
               string Path = path;

               if (!File.Exists(Path))
                    return -1; //file not found

               using (StreamReader sr = new StreamReader(path))
               {
                    string line = sr.ReadLine();
                    string[] vals;
                    char[] param = new char[] { ',', ' ' };
                    while (line != null)
                    {
                         vals = line.Split(param, StringSplitOptions.RemoveEmptyEntries);
                         if (vals.Length < 3)
                              return -2;

                         Point3D pnt = new Point3D(0, 0, 0);
                         try
                         {
                              pnt.X = scale * Convert.ToDouble(vals[0]);
                              pnt.Y = scale * Convert.ToDouble(vals[1]);
                              pnt.Z = scale * Convert.ToDouble(vals[2]);
                              pnt += Offsets;
                              vertices.Add(pnt);
                         }
                         catch { }
                         line = sr.ReadLine();

                    }
               }

               double prev = 0;
               int rows = 0;
               for (rows = 0; rows < vertices.Count; rows++)
               {
                    if (prev == 0)
                         prev = vertices[rows].Y;
                    if (Math.Abs(prev - vertices[rows].Y) > (prev * 1e-1)) //changed rows
                         break;
               }
               int cols = (int)Math.Sqrt(vertices.Count);
               return CreateMesh(vertices.ToArray(), null, cols);
          }

          public void WriteToFile(string path, bool rotated)
          {
               using (StreamWriter sw = new StreamWriter(path))
               {
                    foreach (Point3D v in m_mesh.Vertices)
                    {
                         if (!rotated)
                              sw.WriteLine("{0},{1},{2}", v.X, v.Y, v.Z);
                         else
                              sw.WriteLine("{0},{1},{2}", -v.X, -v.Y, v.Z);
                    }
               }

          }

          /// <summary>
          /// this method assumes that the mesh is 40x60
          /// </summary>
          /// <param name="path"></param>
          /// <param name="rotated"></param>
          public void WriteToFileX(string path, bool rotated)
          {
               List<Point3D> newpnts = new List<Point3D>(m_mesh.Vertices.Length);

               for (int i = 0; i < 40; i++)
               {
                    for (int j = 0; j < 60; j++)
                    {
                         newpnts.Add(new Point3D(m_mesh.Vertices[i + 40 * j]));
                    }
               }

               using (StreamWriter sw = new StreamWriter(path))
               {
                    foreach (Point3D v in newpnts)
                    {
                         if (!rotated)
                              sw.WriteLine("{0},{1},{2}", v.X, v.Y, v.Z);
                         else
                              sw.WriteLine("{0},{1},{2}", -v.X, -v.Y, v.Z);
                    }
               }

          }

          public int CreateMesh(List<KeyValuePair<double[], double>> vertices, int cols)
          {
               List<Point3D> verts = new List<Point3D>(vertices.Count);
               List<double> colors = new List<double>(vertices.Count);
               vertices.ForEach(delegate(KeyValuePair<double[], double> d)
               {
                    if (d.Key.Length == 3)
                    {
                         verts.Add(new Point3D(d.Key));
                         colors.Add(d.Value);
                    }
               });

               return CreateMesh(verts.ToArray(), colors.ToArray(), cols);
          }

          public int CreateMesh(List<double[]> vertices, List<double> densities, int cols, double[] cvts)
          {
               List<Point3D> verts = new List<Point3D>(vertices.Count);
               List<double> colors = new List<double>(vertices.Count);
               m_densityvals = densities.ToArray();
               int count = 0;
               vertices.ForEach(delegate(double[] d)
               {
                    if (d.Length == 3)
                    {
                         verts.Add(new Point3D(d));
                         colors.Add(cvts[count++]);
                    }
               });

               return CreateMesh(verts.ToArray(), colors.ToArray(), cols);
          }

          public int CreateMesh(List<double[]> vertices, int cols, double[] cvts)
          {
               List<Point3D> verts = new List<Point3D>(vertices.Count);
               vertices.ForEach(delegate(double[] d)
               {
                    if (d.Length == 3)
                         verts.Add(new Point3D(d));
               });
               return CreateMesh(verts.ToArray(), cvts, cols);
          }

          public int CreateMesh(Point3D[] vertices, double[] colorvals, int cols)
          {
               int rows = vertices.Length / cols;

               // create the mesh
               System.Drawing.Color c = System.Drawing.Color.SlateBlue;
               if (m_mesh.Vertices.Length != vertices.Length)
                    m_mesh.ResizeVertices(vertices.Length);

               m_mesh.NormalAveragingMode = devDept.Eyeshot.Standard.Mesh.normalAveragingType.Averaged;
               m_mesh.EntityData = this; //point back to this object

               // set the vertices
               m_mesh.Vertices = vertices;
               m_mesh.UpdateBoundingBox();

               //color the vertices
               int i = ReColorMesh(colorvals);
               //if (i != vertices.Length)
               //return -3;

               // set vertex indices
               m_mesh.Triangles.Clear();
               m_mesh.Triangles.Capacity = (rows - 1) * (cols - 1) * 2;
               for (int j = 0; j < (rows - 1); j++)
               {
                    for (i = 0; i < (cols - 1); i++)
                    {

                         m_mesh.Triangles.Add(new Mesh.Triangle(i + j * cols,
                                                                                   i + j * cols + 1,
                                                                                   i + (j + 1) * cols + 1));
                         m_mesh.Triangles.Add(new Mesh.Triangle(i + j * cols,
                                                                                   i + (j + 1) * cols + 1,
                                                                                   i + (j + 1) * cols));
                    }
               }

               m_mesh.ComputeEdges();
               m_mesh.ComputeNormals();

               //m_mesh.Rotate(Math.PI, new Vector3D(0, 0, 1));
               return m_mesh.Triangles.Count;
          }

          double m_op = 0.5;

          double[] m_colorMaxMin = null;

          public double[] ColorMaxMin
          {
               get { return m_colorMaxMin; }
               set 
               { 
                    m_colorMaxMin = value;                        
               }
          }

          public double Opacity
          {
               get { return m_op; }
               set { m_op = value; }
          }

          double[] m_colorvals = null;
          double[] m_densityvals = null;

          public int ReColorMesh(double[] colorvals)
          {
               if (colorvals == null)
                    return ReColorMesh();

               m_colorvals = colorvals;
               double max = -1e9;
               double min = 1e9;
               double avg;
               double stddev = BLAS.StandardDeviation(colorvals, out avg, out max, out min);

               ColorMaxMin = new double[] { min, max };
               //foreach (double d in colorvals)
               //{
               //     max = Math.Max(max, d);
               //     min = Math.Min(min, d);
               //}
               double q1 = avg + 2 * stddev;
               double q2 = avg - 2 * stddev;
               System.Drawing.Color c;
               int i = 0;
               foreach (double d in colorvals)
               {
                    c = Utilities.GetColor(q1, q2, d);
                    //c = Utilities.GetColor(max, min, d);
                    m_mesh.SetVertex(i++, c.R, c.G, c.B);
               }
               return 0;
          }

          public int ReColorMesh(double setMin, double setMax)
          {
               if (Color.ToArgb() == Color.Transparent.ToArgb())
                    return ReColorMesh(m_densityvals, setMin, setMax);
               else if (Color.ToArgb() == Color.Empty.ToArgb())
                    return ReColorMesh(m_colorvals, setMin, setMax);

               return -1;
          }
          public int ReColorMesh(double[] colorvals, double setMin, double setMax)
          {
               if (colorvals == null)
                    return ReColorMesh();

               m_colorvals = colorvals;
               double max = -1e9;
               double min = 1e9;
               double avg;
               double stddev = BLAS.StandardDeviation(colorvals, out avg, out max, out min);

               if (m_colorMaxMin == null)
                    ColorMaxMin = new double[] { min, max };
               else
                    ColorMaxMin = new double[] { setMin, setMax };
               //foreach (double d in colorvals)
               //{
               //     max = Math.Max(max, d);
               //     min = Math.Min(min, d);
               //}
               double q1 = avg + 2 * stddev;
               double q2 = avg - 2 * stddev;
               System.Drawing.Color c;
               int i = 0;
               foreach (double d in colorvals)
               {
                    c = Utilities.GetColor(setMax, setMin, d);
                    //c = Utilities.GetColor(max, min, d);
                    m_mesh.SetVertex(i++, c.R, c.G, c.B);
               }
               return 0;
          }
          private int ReColorMesh()
          {
               System.Drawing.Color c = Color;
               int i = 0;
               //double f;
               for (i = 0; i < m_mesh.Vertices.Length; i++)
                    m_mesh.SetVertex(i, c.R, c.G, c.B);
               
               return i;
          }

          Transformation m_T = new Transformation(1);
          MulticolorOnVerticesMesh m_mesh = new MulticolorOnVerticesMesh(0);

          public Transformation T
          {
               get { return m_T; }
               set
               {
                    double[,] inv = m_T.Matrix;
                    BLAS.Invert4x4(ref inv);//invert rotations
                    m_T = value;

                    m_mesh.TransformBy(new Transformation(inv));
                    m_mesh.TransformBy(m_T);
                    foreach (NsNode n in Nodes)
                         if (n is LineNode)
                         {
                              (n as LineNode).TransformBy(new Transformation(inv));
                              (n as LineNode).TransformBy(m_T);
                         }

               }
          }
          public void TransformBy(Transformation t)
          {
               m_mesh.TransformBy(t);
               m_T = t * m_T;      
          }

          #region IEntityGroup Members

          public static string DRAW = "DrawMesh";
          public static string COLOR = "MeshColor";

          public devDept.Eyeshot.Standard.Entity[] Entity
          {
               get
               {
                    IAttribute a = FindInherited(DRAW);
                    if (a != null && a is BoolAttribute && (a as BoolAttribute).Bool == false)
                         return null;
                    if (Color.ToArgb() == System.Drawing.Color.Empty.ToArgb())
                         ReColorMesh(m_colorvals, ColorMaxMin[0], ColorMaxMin[1]);
                    else if (Color.ToArgb() == System.Drawing.Color.Transparent.ToArgb())
                         ReColorMesh(m_densityvals, ColorMaxMin[0], ColorMaxMin[1]);
                    else
                    {
                         m_mesh.Color = Color;
                         ReColorMesh();
                    }
                    
                    m_mesh.RegenMode = devDept.Eyeshot.Standard.Entity.regenType.RegenAndCompile;
                    m_mesh.EntityData = this;
                    List<devDept.Eyeshot.Standard.Entity> ent = new List<devDept.Eyeshot.Standard.Entity>(1);
                    ent.Add(m_mesh);
                    return ent.ToArray();
               }
          }

          public BoolAttribute Show
          {
               get
               {
                    IAttribute a = FindInherited(DRAW);
                    return a.Value as BoolAttribute;
               }
               set
               {
                    IAttribute a = FindInherited(DRAW);
                    a.Value = value.Value;
               }
          }

          public devDept.Eyeshot.Labels.LabelBase devpDeptLabel 
          {
               get
               {
                    return null;//new devDept.Eyeshot.Labels.OutlinedText(new Point3D(0, 0, 0), this.Label, new Font("Helvetica", 8.0f, FontStyle.Italic), System.Drawing.Color.Black);
               }
          }

          #endregion

          public override void OnDrag(object sender, NodeTrackerEventArgs e)
          {
               base.OnDrag(sender, e);
               if (Parent.Type.Contains("Sail"))
                    Parent.OnDrag(sender, e);
               if (e.DeltaX + e.DeltaY + e.DeltaZ != 0)
                    TransformBy(new Translation(e.DeltaX, e.DeltaY, e.DeltaZ));
               else
                    Update();
          }

     }
}
