using System;
using System.Collections.Generic;
using System.Text;
using NsNodes;

using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Contexts;
using System.Xml;
using RBFPolynomials;
using RBFBasis;
using RBFTools;

namespace RBF
{
     /// <summary>
     /// shaper delegate for uneven grid
     /// </summary>
     /// <param name="input"></param>
     /// <returns></returns>
     public delegate double shaper(double input);

     [System.Runtime.Remoting.Contexts.Synchronization]
     public class SurfaceRBF : NsNode
     {
          const double SCALE = 1;
          const double Tolerance = 1e-9;

          public SurfaceRBF(NsNode parent, XmlNode xml)
               : base(parent, xml)
          {
               //if (!FromXml(xml))
               //     throw new AttributeXmlFormatException(null, xml, "Failed to read xml");
          }

          public SurfaceRBF(NsNode parent, string label, List<double[]> fitpoints, IBasisFunction basis, IRBFPolynomial poly, double relax, List<double[]> edgepnts)
               : base(label, parent)
          {
               Add(new CenterArrayNode(this));
               basis.CopyTo(this);
               poly.CopyTo(this);
               Relaxation = relax;

               //double relaxation = LoadRBFData();

               //Add(new MeshNode("Surface", this));

               if (fitpoints != null)
               {
                    Regen(fitpoints, edgepnts);
               }
          }

          public SurfaceRBF(NsNode parent, string label, List<double[]> fitpoints, IBasisFunction basis, IRBFPolynomial poly, double relax)
               : base(label, parent)
          {
               Add(new CenterArrayNode(this));
               basis.CopyTo(this);
               poly.CopyTo(this);
               Relaxation = relax;

               //double relaxation = LoadRBFData();

              // Add(new MeshNode("Surface", this));

               if (fitpoints != null)
               {
                    Regen(fitpoints, null);
               }
          }

          public SurfaceRBF(string label, NsNode parent) : base(label, parent) { }

          public override void ChildrenLoaded()
          {
               base.ChildrenLoaded();
               m_max[0] = m_max[1] = m_max[2] = -1e9; //start max low
               m_min[0] = m_min[1] = m_min[2] = +1e9; //start min high
               CenterNode.Centers.ForEach(delegate(IAttribute c)
               {
                    if (c is Center3d)
                    {
                         for (int i = 0; i < 2; i++)
                         {
                              m_max[i] = Math.Max((c as Center3d)[i], m_max[i]);
                              m_min[i] = Math.Min((c as Center3d)[i], m_min[i]);
                         }
                    }
               });
               //foreach (IAttribute c in CenterNode.Centers)
               //{
               //    if (c is Center3d)
               //    {
               //        for(int i=0;i<2;i++)
               //        {
               //            m_max[i] = Math.Max((c as Center3d)[i], m_max[i]);
               //            m_min[i] = Math.Min((c as Center3d)[i], m_min[i]);
               //        }
               //    }
               //}
               Regen();
          }

          SurfaceRBF m_sail = null;

          SurfaceRBF sail
          {
               get { return m_sail; }
          }

          IBasisFunction m_basis;

          IBasisFunction rbf
          {
               get
               {
                    if (m_basis == null)
                    {
                         IAttribute ret = Attributes.Find(delegate(IAttribute n) { return (n is IBasisFunction); });
                         m_basis = ret as IBasisFunction;
                         //foreach (IAttribute a in Attributes)
                         //  if (a is IBasisFunction)
                         //       m_basis = a as IBasisFunction;
                    }
                    return m_basis;
               }
               set
               {
                    PauseUpdating();
                    Remove(m_basis);
                    m_basis = value;
                    Add(m_basis);
                    ResumeUpdating(true);
               }
          }

          internal IRBFPolynomial Poly
          {
               get
               {
                    IAttribute ret = Attributes.Find(delegate(IAttribute n) { return (n is IRBFPolynomial); });
                    return ret as IRBFPolynomial;
                    //foreach (IAttribute atr in Attributes)
                    //    if (atr is IRBFPolynomial)
                    //        return atr as IRBFPolynomial;
                    //return null; 
               }
          }
          public double BendingEnergy
          {
               get
               {
                    IAttribute atr = FindAttribute("BendingEnergy");
                    if (atr != null && atr is DoubleAttribute)
                         return (double)atr.Value;
                    else
                    {
                         Add(new DoubleAttribute(this, "BendingEnergy", double.MinValue));
                         return double.MinValue;
                    }
               }
               private set
               {
                    IAttribute atr = FindAttribute("BendingEnergy");
                    if (atr != null && atr is DoubleAttribute)
                         atr.Value = value;
                    else
                         Add(new DoubleAttribute(this, "BendingEnergy", value));
               }
          }
          public double Error
          {
               get
               {
                    IAttribute atr = FindAttribute("Error");
                    if (atr != null && atr is DoubleAttribute)
                         return (double)atr.Value;
                    else
                    {
                         Add(new DoubleAttribute(this, "Error", double.MinValue));
                         return double.MinValue;
                    }
               }
               private set
               {
                    IAttribute atr = FindAttribute("Error");
                    if (atr != null && atr is DoubleAttribute)
                         atr.Value = value;
                    else
                         Add(new DoubleAttribute(this, "Error", value));
               }
          }
          public double Relaxation
          {
               get
               {
                    IAttribute atr = FindAttribute("Relaxation");
                    if (atr != null && atr is DoubleAttribute)
                         return (double)atr.Value;
                    else
                    {
                         Add(new DoubleAttribute(this, "Relaxation", 0));
                         return 0;
                    }
               }
               private set
               {
                    IAttribute atr = FindAttribute("Relaxation");
                    if (atr != null && atr is DoubleAttribute)
                         atr.Value = value;
                    else
                         Add(new DoubleAttribute(this, "Relaxation", value));
               }
          }

          double[] m_maxminColor = new double[2];

          /// <summary>
          /// get the max/min values used for gaussian and density mapping ([0] = min [1] = max)
          /// </summary>
          public double[] MaxMinColor
          {
               get { return m_maxminColor; }
               set { m_maxminColor = value; }
          }

          double[] m_max = new double[3];
          public double[] Max
          {
               get { return m_max; }
          }

          double[] m_min = new double[3];
          public double[] Min
          {
               get { return m_min; }
          }
          internal double[] Middle
          {
               get { return new double[] { (m_max[0] + m_min[0]) / 2, (m_max[1] + m_min[1]) / 2, (m_max[2] + m_min[2]) / 2 }; }
          }

          internal CenterArrayNode CenterNode
          {
               get
               {
                    NsNode ret = Nodes.Find((NsNode n)=> { return (n is CenterArrayNode); });
                    return ret as CenterArrayNode;
                    //foreach (NsNode n in Nodes)
                    //	if (n is CenterArrayNode)
                    //		return n as CenterArrayNode;
                    //return null;
               }
          }


          /// <summary>
          /// Get the center of the sail (calculated using bb dimension)
          /// </summary>
          public double[] Center
          {
               get { return new double[] { Min[0] + (Max[0] - Min[0]) / 2.0, Min[1] + (Max[1] - Min[1]) / 2.0, Min[2] + (Max[2] - Min[2]) / 2.0 }; }
          }

          private void PolyTerms(double[] p, double[] d, double[] dd)
          {
               Poly.Poly(p, d, dd);
               return;
          }

          public void Value(ref double[] p)
          {
               for (int i = 0; i < 3; i++)
                    p[i] /= SCALE;
               p[2] = 0;

               #region Thread bit (actually slows program down 18.5 s in Release turned on, vs 13.13 s in Release turned off)
               //double threadP2 = 0;
               //if (m_threaded)
               //{
               //     try
               //     {
               //          int length = CenterNode.Centers.Count;
               //          int start, end;
               //          for (int i = 0; i < threadcount; i++)
               //          {
               //               doneEvents[i] = new ManualResetEvent(false);
               //               start = (i * length) / threadcount;
               //               end = ((i + 1) * length) / threadcount;
               //               List<IAttribute> split = CenterNode.Centers.GetRange(start, end - start);
               //               PointThread f = new PointThread(p, split, rbf, doneEvents[i]);
               //               ptArray[i] = f;
               //               ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
               //          }

               //          // Wait for all threads in pool to calculate.
               //          //WaitHandle.WaitAll(doneEvents);
               //          foreach (var e in doneEvents)
               //               e.WaitOne();

               //          double total = 0;
               //          foreach (PointThread w in ptArray)
               //               total += w.Result;

               //          p[2] = total;

               //          PolyTerms(p, null, null); // add the polynomial

               //          for (int i = 0; i < 3; i++)
               //               p[i] *= SCALE;

               //          return;
               //     }
               //     catch (Exception e)
               //     {
               //          string message = e.Message;
               //     }
               //}
               #endregion

               double r;
               foreach (Center3d c in CenterNode.Centers)
               {
                    r = c.radius(p);
                    p[2] += c.w * rbf.val(r); // sum the weight * rbf values
               }

               //bool b = p[2] == threadP2;

               PolyTerms(p, null, null); // add the polynomial

               for (int i = 0; i < 3; i++)
                    p[i] *= SCALE;

          }
          public void First(ref double[] p, ref double[] d)
          {
               for (int i = 0; i < 3; i++)
                    p[i] /= SCALE;
               p[2] = 0;
               d[0] = d[1] = d[2] = 0;
               double r, dr, drdx;
               foreach (Center3d c in CenterNode.Centers)
               {
                    r = c.radius(p); // radius
                    if (BLAS.is_equal(r, 0)) continue;

                    p[2] += c.w * rbf.val(r); // sum the weight * rbf values

                    dr = rbf.dr(r); // dRBF/dr

                    drdx = (p[0] - c[0]) / r; // dr/dx
                    d[0] += c.w * dr * drdx; // accumulate weighted derivatives

                    drdx = (p[1] - c[1]) / r; // dr/dy
                    d[1] += c.w * dr * drdx; // accumulate weighted derivatives

                    //dx[oz] += c.w()*dr; // accumulate weighted derivatives
                    //rbf()->first(c, p, dx); 
               }
               PolyTerms(p, d, null); // add the polynomial
               for (int i = 0; i < 3; i++)
               {
                    p[i] *= SCALE;
                    d[i] *= SCALE;
               }

          }
          public void Second(ref double[] p, ref double[] d, ref double[] dd)
          {
               for (int i = 0; i < 3; i++)
                    p[i] /= SCALE;
               p[2] = 0; //initialize z
               d[0] = d[1] = d[2] = 0;
               dd[0] = dd[1] = dd[2] = 0;
               double r, dr, drdx, ddrdxx;
               foreach (Center3d c in CenterNode.Centers)
               {
                    r = c.radius(p); // radius
                    if (BLAS.is_equal(r, 0)) continue;
                    p[2] += c.w * rbf.val(r); // sum the weight * rbf values

                    dr = rbf.dr(r); // dRBF/dr

                    drdx = (p[0] - c[0]) / r; // dr/dx
                    d[0] += c.w * dr * drdx; // accumulate weighted first derivatives

                    //ddrdxx = 2*(xc*xc)/(r*r)+dr/r; // d^2r/dx^2
                    ddrdxx = 2 * drdx * drdx + dr / r;// d^2r/dx^2
                    dd[0] += c.w * ddrdxx; // accumulate weighted second derivatives

                    drdx = (p[1] - c[1]) / r; // dr/dy
                    d[1] += c.w * dr * drdx;// accumulate weighted first derivatives

                    //ddrdxx = 2*(xc*xc)/(r*r)+dr/r; // d^2r/dy^2
                    ddrdxx = 2 * drdx * drdx + dr / r;// d^2r/dx^2
                    dd[1] += c.w * ddrdxx; // accumulate weighted second derivatives

                    //dx[oz] = 0; // dphi/dz = 0, tps is a function of x and y only
                    dd[2] += c.w * 2 * (p[0] - c[0]) * (p[1] - c[1]) / (r * r); // d^2r/dxdy
               }
               PolyTerms(p, d, dd); // add the polynomial
               for (int i = 0; i < 3; i++)
               {
                    p[i] *= SCALE;
                    d[i] *= SCALE;
               }
          }

          /// <summary>
          /// return Unit normal vector
          /// </summary>
          /// <param name="p"></param>
          /// <param name="d"></param>
          /// <param name="nor"></param>
          public void Normal(ref double[] p, ref double[] d, ref double[] nor)
          {
               First(ref p, ref d); //get the first derivatives and z value
               double[] dx = d;
               double[] dy;
               BLAS.split(ref dx, out dy);
               nor = BLAS.cross(dx, dy); //cross surface tangents to get normal
               // make unit-normal(magnitude is meaningless anyway)
               double mag = BLAS.magnitude(nor);
               nor[0] /= mag;
               nor[1] /= mag;
               nor[2] /= mag;
               System.Diagnostics.Debug.Assert(Math.Abs((nor[0] * nor[0] + nor[1] * nor[1] + nor[2] * nor[2]) - 1) < 1e-7);
          }

          public void Gaussian(ref double[] p, ref double[] d, ref double[] dd, ref double k)
          {
               Second(ref p, ref d, ref dd);
               int i;
               //calculate unit normal
               double[] dx = d;
               double[] dy;
               BLAS.split(ref dx, out dy);
               double[] nor = BLAS.cross(dx, dy); //cross surface tangents to get normal
               // make unit-normal(magnitude is meaningless anyway)
               double mag = BLAS.magnitude(nor);
               for (i = 0; i < 3; i++) nor[i] /= mag;

               double[] dxx = new double[] { 1, 0, dd[0] };
               double[] dyy = new double[] { 0, 1, dd[1] };
               double[] dxy = new double[] { 0, 0, dd[2] };

               //calculate first fundamental form
               double E = BLAS.dot(dx, dx);
               double F = BLAS.dot(dx, dy);
               double G = BLAS.dot(dy, dy);
               double detI = E * G - F * F;
               //calculate second fundamental form
               double e = BLAS.dot(dxx, nor);
               double f = BLAS.dot(dxy, nor);
               double g = BLAS.dot(dyy, nor);
               double detII = e * g - f * f;
               //calculate Shape Operator
               //double s11 = (e * G - f * F) / detI;
               //double s12 = (f * G - g * F) / detI;
               //double s21 = (f * E - e * F) / detI;
               //double s22 = (g * E - f * F) / detI;
               //curvature is det(Shape Op)
               //k = s11 * s22 - s21 * s12;
               k = detII / detI;
          }

          /// <summary>
          /// Fit an rbf surface to the specified points, optionally specify a relaxation value for approximate interpolation
          /// </summary>
          /// <param name="fitPoints">the points to fit to (usually lifter locations)</param>
          /// <param name="pRelax">an optional relaxation parameter, 0 for exact fit, >0 for increasing tolerance. null will use the stored relax or default to 0</param>
          /// <returns>1 if successful, 0>= if error</returns>
          public int Fit(List<double[]> fitPoints, double? pRelax)
          {
               BendingEnergy = 0; //reset bending energy and error
               Error = 0;

               m_max[0] = m_max[1] = m_max[2] = -1e9; //start max low
               m_min[0] = m_min[1] = m_min[2] = +1e9; //start min high

               CenterNode.ClearCenters(); //allocate space for center
               CenterNode.PauseUpdating();
               if (pRelax != null)
                    Relaxation = (double)pRelax; //0 for exact, increase to reduce bending energy

               int i, j; //loops

               List<double> fitz = new List<double>(fitPoints.Count); //temp vector for rhs
               //List<int> lifterIndices = new List<int>();

               //List<ICenter<double>> centers = new List<ICenter<double>>();
               int count = 0;
               fitPoints.ForEach((double[] v) =>
               {
                    fitz.Add(v[2] / SCALE);
                    CenterNode.Add(new Center3d(v[0] / SCALE, v[1] / SCALE));
                    for (i = 0; i < v.Length; i++) //get fit points' bounding box
                    {
                         m_max[i] = Math.Max(v[i] / SCALE, m_max[i]);
                         m_min[i] = Math.Min(v[i] / SCALE, m_min[i]);
                    }
                    count++;
               });

               // poly conditions
               for (i = 0; i < Poly.Terms; i++)
                    fitz.Add(0);

               //create the fitting matrix
               double[,] A;
               //bool iterate = lifterIndices.Count != fitPoints.Count;

               // I need to pass a min value and max value for the range of i's that are under the sail

               //if (!iterate)
               RBFSolver.fit_mat(out A, CenterNode, rbf, Poly, Relaxation);

               // solve the system
               int err = RBFSolver.solve(A, fitz.ToArray(), CenterNode, Poly, true);
               if (err != 0)
                    return err;

               //calculate bending energy (wT * A * w)
               double be = 0;
               List<double> wtA = new List<double>(new double[CenterNode.Centers.Count]); //temp vector for w-transpose * A
               for (i = 0; i < CenterNode.Centers.Count; i++)
                    for (j = 0; j < CenterNode.Centers.Count; j++)
                    {
                         wtA[i] += CenterNode[j].w * A[j, i]; //first multiplication
                    }
               for (i = 0; i < CenterNode.Centers.Count; i++)
                    be += wtA[i] * CenterNode[i].w; //second multiplication

               BendingEnergy = be;

               //calculate error as the sum of % difference between target z and actual z
               Error = CheckFit(fitPoints);
               CenterNode.ResumeUpdating(false);
               return 1;
          }

          /// <summary>
          /// this one will be used for the looping method
          /// </summary>
          /// <param name="A"> output </param>
          /// <param name="lifterIndices">List of ints that correspond to the indices of the lifters under the sail area</param>
          /// <param name="p_relax"> a fixed relaxation value for the lifters outside the sail area </param>
          private void fit_mat(out double[,] A, List<int> lifterIndices, double p_relax)
          {
               // find out
               int dim = CenterNode.Centers.Count;
               int fits = dim;

               fits += Poly.Terms;// for polynomial terms

               A = new double[fits, fits];
               // create A matrix
               double r;
               double a = 0;//for scaling relaxation parameter
               int i, j;
               for (i = 0; i < dim; ++i)
               {
                    for (j = i + 1; j < dim; ++j)
                    {
                         // PHI11 - PHINN
                         r = CenterNode[j].radius(CenterNode[i].ToArray());
                         A[i, j] = A[j, i] = rbf.val(r); // symmetric
                         a += 2 * r;
                    }
               }
               a /= dim * dim; // calculate relaxation normalizer
               //a /= fits * fits; // fits = dims + Poly.Terms(3)
               //a /= Math.Pow(fits, 2);
               // calculate fit mat diagonals and poly terms
               double relax = Relaxation;
               for (i = 0; i < dim; ++i)
               {
                    if (lifterIndices.Contains(i))
                         A[i, i] = BLAS.is_equal(relax, 0) ? 0 : a * a * relax;
                    else
                         A[i, i] = BLAS.is_equal(p_relax, 0) ? 0 : a * a * p_relax;

                    for (j = 0; j < Poly.Terms; j++)
                         A[i, dim + j] = A[dim + j, i] = Poly.FitMat(i, j);

                    //A[i, dim + 0] = A[dim + 0, i] = Centers[i][0];
                    //A[i, dim + 1] = A[dim + 1, i] = Centers[i][1];

                    //// poly values: Ax + By + C POLY
                    //A[i, dim + 2] = A[dim + 2, i] = 1;

                    // poly values: Ax + By + Cxy POLY
                    //A[i, dim + 2] = A[dim + 2, i] = Centers[i][1] * Centers[i][0];

                    // parabaloid values: A(x-h)^2 + B(y-k)^2 POLY
                    //A[i, dim + 0] = A[dim + 0, i] = Math.Pow(Centers[i][0] - middle[0],2);
                    //A[i, dim + 1] = A[dim + 1, i] = Math.Pow(Centers[i][1] - middle[1], 2);
               }
          }

          #region GetMeshPoints

          /// <summary>
          /// this return a grid of points that travel up Y axis
          /// </summary>
          /// <param name="rows"></param>
          /// <param name="columns"></param>
          /// <returns></returns>
          public List<double[]> GetMeshPoints(int rows, int columns)
          {
               double[] max = new double[3];
               double[] min = new double[3];
               for (int i = 0; i < 3; i++)
               {
                    max[i] = m_max[i] * SCALE;
                    min[i] = m_min[i] * SCALE;
               }
               return GetMeshPoints(rows, columns, max, min);
          }

          /// <summary>
          /// this return a evenly spaced set of grid points that travel up X axis 
          /// </summary>
          /// <param name="rows"></param>
          /// <param name="columns"></param>
          /// <returns></returns>
          //public List<double[]> GetMeshPointsInX(int rows, int columns)
          //{
          //     double[] max = new double[3];
          //     double[] min = new double[3];
          //     for (int i = 0; i < 3; i++)
          //     {
          //          max[i] = m_max[i] * SCALE;
          //          min[i] = m_min[i] * SCALE;
          //     }
          //     return GetMeshPointsInX(rows, columns, max, min);
          //}

          /// <summary>
          /// this return a unevenly spaced set of grid points that travel up X axis 
          /// </summary>
          /// <param name="rows"></param>
          /// <param name="columns"></param>
          /// <returns></returns>
          public List<double[]> GetMeshPointsInXUneven(int rows, int columns)
          {
               double[] max = new double[3];
               double[] min = new double[3];
               for (int i = 0; i < 3; i++)
               {
                    max[i] = m_max[i] * SCALE;
                    min[i] = m_min[i] * SCALE;
               }
               return GetMeshPointsInXUneven(rows, columns, max, min);
          }

          public List<double[]> GetMeshPoints(int rows, int columns, double[] max, double[] min)
          {
               List<double[]> vals = new List<double[]>();
               double deltax = max[0] - min[0];
               double deltay = max[1] - min[1];

               if (m_threaded)
               {
                    try
                    {
                         int start, end, i;
                         meshArray.Clear();
                         for (i = 0; i < meshThreadcount; i++)
                         {
                              meshDoneEvents[i] = new ManualResetEvent(false);
                              start = (rows * i) / meshThreadcount;
                              end = ((i + 1) * rows) / meshThreadcount;
                              MeshThread mt = new MeshThread(this, rows, start, end, columns, deltax, deltay, min, null, meshDoneEvents[i]);
                              meshArray.Add(mt);
                              ThreadPool.QueueUserWorkItem(mt.ThreadPoolCallback, i);
                         }

                         // Wait for all threads in pool to calculate.
                         foreach (var e in meshDoneEvents) e.WaitOne();
                         meshArray.ForEach(delegate(MeshThread m) { vals.AddRange(m.Result); });
                    }
                    catch (Exception e)
                    {
                         string message = e.Message;
                    }
               }
               else
               {
                    int i, j;
                    double x, y;

                    for (i = 0; i < rows; i++)
                    {
                         x = (double)i / (double)(rows - 1) * deltax + min[0];
                         for (j = 0; j < columns; j++)
                         {
                              y = (double)j / (double)(columns - 1) * deltay + min[1];
                              double[] p = new double[3] { x, y, 0 };
                              Value(ref p);
                              vals.Add(p);
                         }
                    }
               }
               return vals;
          }

          /// <summary>
          /// evenly spaced
          /// </summary>
          /// <param name="rows"></param>
          /// <param name="columns"></param>
          /// <param name="max"></param>
          /// <param name="min"></param>
          /// <returns></returns>
          //public List<double[]> GetMeshPointsInX(int rows, int columns, double[] max, double[] min)
          //{
          //     List<double[]> vals = new List<double[]>();

          //     double deltax = max[0] - min[0];
          //     double deltay = max[1] - min[1];

          //     int i, j;
          //     double x, y;

          //     for (i = 0; i < rows; i++)
          //     {
          //          y = (double)i / (double)(rows - 1) * deltay + min[1];
          //          for (j = 0; j < columns; j++)
          //          {
          //               x = (double)j / (double)(columns - 1) * deltax + min[0];
          //               double[] p = new double[3] { x, y, 0 };
          //               Value(ref p);
          //               vals.Add(p);
          //          }
          //     }
          //     return vals;
          //}

          /// <summary>
          /// unevenly spaced
          /// </summary>
          /// <param name="rows"></param>
          /// <param name="columns"></param>
          /// <param name="max"></param>
          /// <param name="min"></param>
          /// <returns></returns>
          public List<double[]> GetMeshPointsInXUneven(int rows, int columns, double[] max, double[] min)
          {
               List<double[]> vals = new List<double[]>();

               double deltax = max[0] - min[0];
               double deltay = max[1] - min[1];

               int i, j;
               double y, sx;

               List<double[]> points = new List<double[]>();

               points.Add(new double[] { 0, 0 });
               points.Add(new double[] { 1.0, 0.5 });
               points.Add(new double[] { 8.0, 5.0 });
               points.Add(new double[] { 10.0, 10.0 });

               RBFCurve rbf = new RBFCurve(null, "shaper", points, new RBFBasis.ThinPlateSpline(null), new RBFPolynomials.Linear(null), 0.0);

               shaper Shape = ((double value) =>
               {
                    double[] pnt = new double[] { 10.0*value, 0 };
                    rbf.Value(ref pnt);
                    return pnt[1]/10.0;
               });

               if (m_threaded)
               {
                    try
                    {
                         int start, end;
                         meshArray.Clear();
                         for (i = 0; i < meshThreadcount; i++)
                         {
                              meshDoneEvents[i] = new ManualResetEvent(false);
                              start = (rows * i) / meshThreadcount;
                              end = ((i + 1) * rows) / meshThreadcount;
                              MeshThread mt = new MeshThread(this, rows, start, end, columns, deltax, deltay, min, null, meshDoneEvents[i], Shape, null);
                              meshArray.Add(mt);
                              ThreadPool.QueueUserWorkItem(mt.ThreadPoolCallback, i);
                         }

                         // Wait for all threads in pool to calculate.
                         foreach (var e in meshDoneEvents) e.WaitOne();
                         meshArray.ForEach(delegate(MeshThread m) { vals.AddRange(m.Result); });
                    }
                    catch (Exception e)
                    {
                         string message = e.Message;
                    }
               }
               else
               {
                    for (i = 0; i < rows; i++)
                    {
                         y = (double)i / (double)(rows - 1) * deltay + min[1];
                         for (j = 0; j < columns; j++)
                         {
                              //x = (double)j / (double)(columns - 1) * deltax + min[0];
                              sx = Shape((double)j / (double)(columns - 1)) * deltax + min[0];
                              double[] p = new double[3] { sx, y, 0 };
                              Value(ref p);
                              vals.Add(p);
                         }
                    }
               }
               return vals;
          }

          public List<double[]> GetRowPoints(double locationY, int numPnts)
          {
               double[] max = new double[3];
               double[] min = new double[3];
               for (int i = 0; i < 3; i++)
               {
                    max[i] = m_max[i] * SCALE;
                    min[i] = m_min[i] * SCALE;
               }

               double deltax = max[0] - min[0];
               double deltay = max[1] - min[1];

               List<double[]> ret = new List<double[]>(numPnts);
               double[] p = new double[3] { 0, locationY*1000.0, 0 };
               
               for (int i = 0; i < numPnts; i++)
               {
                    p[0] = (double)i / (double)(numPnts - 1) * deltax + min[0];
                    Value(ref p);
                    ret.Add(new double[] { p[0],p[1],p[2] });
               }

               return ret;
          }

          public List<KeyValuePair<double[], double>> GetMeshPointsCvt(int rows, int columns, double[] max, double[] min, List<double[]> edgepnts)
          {
               List<KeyValuePair<double[], double>> vals = new List<KeyValuePair<double[], double>>();
               double deltax = max[0] - min[0];
               double deltay = max[1] - min[1];

               if (m_threaded)
               {
                    try
                    {
                         meshArray.Clear();
                         int start, end, i;
                         for (i = 0; i < meshThreadcount; i++)
                         {
                              meshDoneEvents[i] = new ManualResetEvent(false);
                              start = (rows * i) / meshThreadcount;
                              end = ((i + 1) * rows) / meshThreadcount;
                              MeshThread mt = new MeshThread(this, rows, start, end, columns, deltax, deltay, min, meshDoneEvents[i], edgepnts, true);
                              meshArray.Add(mt);
                              ThreadPool.QueueUserWorkItem(mt.ThreadPoolCallback, i);
                         }

                         // Wait for all threads in pool to finish.
                         foreach (var e in meshDoneEvents) e.WaitOne();
                         meshArray.ForEach(delegate(MeshThread m) { vals.AddRange(m.CvtResult); });
                    }
                    catch (Exception e)
                    {
                         string message = e.Message;
                    }
               }
               else
               {
                    int i, j;
                    double x, y, k = 0;
                    double[] d = new double[3];
                    double[] dd = new double[3];
                    for (i = 0; i < rows; i++)
                    {
                         x = (double)i / (double)(rows - 1) * deltax + min[0];
                         for (j = 0; j < columns; j++)
                         {
                              y = (double)j / (double)(columns - 1) * deltay + min[1];
                              double[] p = new double[3] { x, y, 0 };
                              Gaussian(ref p, ref d, ref dd, ref k);
                              vals.Add(new KeyValuePair<double[], double>(p, k));
                         }
                    }
               }

               return vals;
          }

          public List<double[]> GetMeshPoints(List<double[]> fitpoints)
          {
               List<double[]> vals = new List<double[]>();
               double error = 0;
               double[] p;
               fitpoints.ForEach(delegate(double[] v)
               {
                    p = new double[3] { v[0], v[1], v[2] };
                    Value(ref p);
                    error += Math.Abs(v[2] - p[2]) / v[2];
                    vals.Add(new double[3] { p[0], p[1], p[2] });
               });
               return vals;
          }

          /// <summary>
          /// returns a list of double[3] that are offset by the inputted distance (mm) from the inputted fitpoints 
          /// </summary>
          /// <param name="fitpoints">get normal project of these points</param>
          /// <param name="distance">set the normal vector length to this value (mm)</param>
          /// <returns></returns>
          public List<double[]> GetNormalOffsetPoints(List<double[]> fitpoints, double distance)
          {
               List<double[]> vals = new List<double[]>();
               double error = 0;
               double[] p;
               double[] d = new double[3];
               double[] norm = new double[3];
               int i = 0;

               fitpoints.ForEach(delegate(double[] v)
               {
                    p = new double[3] { v[0], v[1], v[2] };
                    Normal(ref p, ref d, ref norm);

                    for (i = 0; i < 3; i++) norm[i] *= distance;

                    error += Math.Abs(v[2] - p[2]) / v[2];
                    vals.Add(new double[3] { p[0] + norm[0], p[1] + norm[1], p[2] + norm[2] });
               });

               return vals;
          }

          #endregion

          /// <summary>
          /// returns the error between the RBF Surface Z locations and the lifters actual Z location
          /// </summary>
          /// <param name="pnts">Lifter locations</param>
          /// <returns>error value</returns>
          public double CheckFit(List<double[]> pnts)
          {
               double error = 0;
               double[] p = new double[3];
               pnts.ForEach((double[] v) =>
                    {
                         p = new double[3] { v[0], v[1], v[2] };
                         Value(ref p);
                         error += Math.Abs(v[2] - p[2]) / v[2];
                    });

               return error;
          }

          #region Regen

          public void Regen()
          {
               Regen(null, null, false);
          }
          public void Regen(double[] xymax, double[] xymin, bool spacer)
          {
               Regen(null, xymax, xymin, null);
          }
          public void Regen(List<double[]> fitpoints, List<double[]> edgepnts)
          {
               Regen(fitpoints, null, null, edgepnts);
          }
          public void Regen(List<double[]> fitpoints, double relax, List<double[]> edgepnts )
          {
               Regen(fitpoints, null, null, relax, edgepnts);
          }
          public void Regen(List<double[]> fitpoints, double[] xyzmax, double[] xyzmin, List<double[]> edgepnts)
          {
               Regen(fitpoints, xyzmax, xyzmin, null, edgepnts);
          }
          public void Regen(List<double[]> fitpoints, double[] xyzmax, double[] xyzmin, double? relax, List<double[]> edgepnts)
          {
               if (fitpoints != null)
                    Fit(fitpoints, relax);

               if (xyzmax == null)
               {
                    xyzmax = new double[3];
                    m_max.CopyTo(xyzmax, 0);
                    for (int i = 0; i < 3; i++)
                         xyzmax[i] *= SCALE;
               }
               if (xyzmin == null)
               {
                    xyzmin = new double[3];
                    m_min.CopyTo(xyzmin, 0);
                    for (int i = 0; i < 3; i++)
                         xyzmin[i] *= SCALE;
               }
               int cols = 50;

               List<KeyValuePair<double[], double>> pts = GetMeshPointsCvt(50, cols, xyzmax, xyzmin, edgepnts);
               
               //Surface.CreateMesh(pts, cols);

               //List<KeyValuePair<double[], double>> pts = GetMeshPointsCvt(100, cols, xyzmax, xyzmin, edgepnts);
               //Surface.CreateMesh(pts, cols);
               //Add(new NsViewers.SurfaceAttribute(this, Label + " RBF " + m_iter, GetMeshPoints(50, 50, xymax, xymin), 50, 50));
          }
          
          #endregion

          #region DumpMatrix

          private void DumpA(double[,] A, string path)
          {
               using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
               {
                    for (int j = 0; j < A.GetLength(0); j++)
                    {
                         for (int k = 0; k < A.GetLength(1); k++)
                         {
                              sw.Write(A[j, k].ToString());
                              sw.Write(",");
                         }
                         sw.WriteLine();
                    }


               }
          }
          private void DumpD(double[] D, int row, int col, string path)
          {
               using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
               {
                    for (int j = 0; j < row; j++)
                    {
                         for (int k = 0; k < col; k++)
                         {
                              sw.Write(D[j * row + k]);
                              sw.Write(",");
                         }
                         sw.WriteLine();
                    }
               }
          }
          private void DumpW(double[] w, string path)
          {
               using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
               {
                    foreach (double d in w)
                         sw.WriteLine(d.ToString());
               }
          }

          #endregion

          ///// <summary>
          ///// Fit the surface using the UI Settings (SailRBFSingleton or MoldRBFSingleton will setup the basis and poly)
          ///// </summary>
          ///// <param name="fitpoints"></param>
          //public void Fit(List<double[]> fitpoints)
          //{
          //     Add(new MeshNode("Surface", this));
          //     //double relaxation = LoadRBFData();

          //     if (fitpoints != null)
          //     {
          //          Relaxation = relaxation;
          //          Regen(fitpoints);
          //     }

          //}

          ///// <summary>
          ///// loads the RBF data depending on the this type (Sail or Mold).  Sail = SailRBFSingleton data, mold = MoldRBFSingleton data
          ///// </summary>
          ///// <returns> relaxation value of specific RBF </returns>
          //private double LoadRBFData()
          //{
          //     switch (this.Label.ToLower().Contains("sail") ? SailRBFSingleton.Poly : MoldRBFSingleton.Poly)
          //     {
          //          case NHTGenComps.PolyTypes.Conic:
          //               {
          //                    Add(new RBFConic(this));
          //                    break;
          //               }
          //          case NHTGenComps.PolyTypes.Paraboloid_Long:
          //               {
          //                    Add(new RBFParaboloid3(this));
          //                    break;
          //               }
          //          case NHTGenComps.PolyTypes.Paraboloid_Short:
          //               {
          //                    Add(new RBFParaboloid2(this));
          //                    break;
          //               }
          //          case NHTGenComps.PolyTypes.Poly:
          //               {
          //                    Add(new RBFPoly1(this));
          //                    break;
          //               }
          //          default:
          //               {
          //                    Add(new RBFPoly1(this));
          //                    break;
          //               }
          //     }


          //     switch (this.Label.ToLower().Contains("sail") ? SailRBFSingleton.Basis : MoldRBFSingleton.Basis)
          //     {
          //          case NHTGenComps.BasisTypes.ThinPlate3:
          //               {
          //                    rbf = new ThinPlateSpline3(this);
          //                    break;
          //               }
          //          //case NHTGenComps.BasisTypes.Gaussian:
          //          //     {
          //          //          rbf = new Gaussian(this);
          //          //          break;
          //          //     }
          //          case NHTGenComps.BasisTypes.Multiquadratic:
          //               {
          //                    rbf = new Multiquadratic(this);
          //                    break;
          //               }
          //          case NHTGenComps.BasisTypes.ThinPlate:
          //               {
          //                    rbf = new ThinPlateSpline(this);
          //                    break;
          //               }
          //          case NHTGenComps.BasisTypes.PolyHarmonic3:
          //               {
          //                    rbf = new PolyHarmonic3(this);
          //                    break;
          //               }
          //          case NHTGenComps.BasisTypes.PolyHarmonic4:
          //               {
          //                    rbf = new PolyHarmonic4(this);
          //                    break;
          //               }
          //          case NHTGenComps.BasisTypes.PolyHarmonic5:
          //               {
          //                    rbf = new PolyHarmonic5(this);
          //                    break;
          //               }
          //          default:
          //               {
          //                    rbf = new ThinPlateSpline(this);
          //                    break;
          //               }
          //     }

          //     return this.Label.ToLower().Contains("sail") ? SailRBFSingleton.Relaxation : MoldRBFSingleton.Relaxation;

          //}

          public override NsNode CopyTo(NsNode newParent)
          {
               //return base.CopyTo(newParent);
               SurfaceRBF cpy = (SurfaceRBF)newParent.Add(new SurfaceRBF(Label, newParent));
               Nodes.ForEach(delegate(NsNode n)
               {
                    n.CopyTo(cpy);
               });
               Attributes.ForEach(delegate(IAttribute atr)
               {
                    atr.CopyTo(cpy);
               });
               //foreach (NsNode n in Nodes)
               //    n.CopyTo(cpy);//copy children to this node's copy
               //foreach (IAttribute atr in Attributes)
               //    atr.CopyTo(cpy);//copy attributes to this node's copy
               return cpy;
          }

          #region Threading Test
          
          /// <summary>
          /// if this is true, full threading is used
          /// </summary>
          public bool m_threaded = true;

          #region PointThread

         // ManualResetEvent[] doneEvents = new ManualResetEvent[5];
          //PointThread[] ptArray = new PointThread[5];
          //int threadcount = 5;

          #endregion PointThread

          #region MeshThread

          ManualResetEvent[] meshDoneEvents = new ManualResetEvent[5];
          List<MeshThread> meshArray = new List<MeshThread>(5);
          int meshThreadcount = 5;
          #endregion MeshThread

          #endregion
     }

     /// <summary>
     /// originally use in the Value function, but after thinking about it, not a good place to thread (too may calls)
     /// </summary>
     public class PointThread
     {
          /// <summary>
          /// Thread Done Event
          /// </summary>
          private ManualResetEvent m_doneEvent;

          /// <summary>
          /// CenterArrayNode
          /// </summary>
          private CenterArrayNode m_centers;
          private IBasisFunction _rbf;
          private double m_result;
          private double[] m_p;
          private CenterArrayNode CenterArray
          {
               get { return m_centers; }
          }
          private IBasisFunction Rbf
          {
               get { return _rbf; }
               set { _rbf = value; }
          }

          public double Result
          {
               get { return m_result; }
               set { m_result = value; }
          }

          /// <summary>
          /// Constructor
          /// </summary>
          /// <param name="pnt"></param>
          /// <param name="centers"></param>
          /// <param name="rbf"></param>
          /// <param name="doneEvent"></param>
          public PointThread(double[] pnt, List<IAttribute> centers, IBasisFunction rbf, ManualResetEvent doneEvent)
          {
               m_p = pnt;
               Rbf = rbf;
               List<ICenter<double>> tmp = new List<ICenter<double>>(centers.Count);
               centers.ForEach(delegate(IAttribute atr) { tmp.Add(atr as ICenter<double>); });
               m_centers = new CenterArrayNode(null, tmp);
               m_doneEvent = doneEvent;
          }

          /// <summary>
          /// Wrapper method for use with thread pool
          /// </summary>
          /// <param name="threadContext"></param>
          public void ThreadPoolCallback(Object threadContext)
          {
               int threadIndex = (int)threadContext;
               Result = Calculate();
               m_doneEvent.Set();
          }

          /// <summary>
          /// 
          /// </summary>
          /// <returns></returns>
          public double Calculate()
          {
               double sum = 0;
               double r;
               Center3d c;
               CenterArray.Centers.ForEach(delegate(IAttribute atr)
               {
                    c = (atr as Center3d);
                    r = c.radius(m_p);
                    sum += c.w * Rbf.val(r); // sum the weight * rbf values
               });
               return sum;
          }
     }

     /// <summary>
     /// This guy is used to calculate a piece of the mesh grid surface
     /// </summary>
     public class MeshThread
     {
          /// <summary>
          /// Thread Done Event
          /// </summary>
          private ManualResetEvent _doneEvent;

          /// <summary>
          /// bool to determine whether the surface curvature needs to be calculated
          /// </summary>
          private bool _getCVT = false;

          private shaper shaper1;
          private shaper shaper2;

          private int _rowStart, _rowsEnd, _columns, _rowsTotal;
          private double _deltax, _deltay;
          private double[] _min;
          private LeastSquares _edge;
          private SurfaceRBF _surf;
          private List<KeyValuePair<double[], double>> _result = new List<KeyValuePair<double[],double>>();
          private List<double[]> _result2 = new List<double[]>();

          /// <summary>
          /// get the result of this thread with curvatures (must use proper constructor for this to be filled)
          /// </summary>
          public List<KeyValuePair<double[], double>> CvtResult
          {
               get { return _result; }
               set { _result = value; }
          }

          /// <summary>
          /// get the resulting grid of points from the thread
          /// </summary>
          public List<double[]> Result
          {
               get { return _result2; }
               set { _result2 = value; }
          }

          /// <summary>
          /// A MeshThread object to calculate a piece of a rbf mesh
          /// </summary>
          /// <param name="surf">the surface rbf to get mesh from</param>
          /// <param name="rows">total # of rows</param>
          /// <param name="rowstart">rows value for this thread to start from</param>
          /// <param name="rowsend">rows value for this thread to go to</param>
          /// <param name="columns">total # of columns</param>
          /// <param name="deltax">usually something like maxX-minX (max[0] - min[0])</param>
          /// <param name="deltay">usually something like maxY-minY (max[1] - min[1])</param>
          /// <param name="min">the minimum used to calculate the deltas</param>
          /// <param name="doneEvent">your ManualResetEvent that will tell you when this is done</param>
          /// <param name="getCVT">bool to indicate if you want to calculate the surface rbf curvature value as well</param>
          public MeshThread(SurfaceRBF surf, int rows, int rowstart, int rowsend, int columns, double deltax, double deltay, double[] min, ManualResetEvent doneEvent, List<double[]> edgepnts, bool getCVT)
          {
               shaper1 = ((double input) => { return input; }); // shapers undefined so return inputted value
               shaper2 = ((double input) => { return input; }); // shapers undefined so return inputted value

               _rowStart = rowstart;
               _rowsEnd = rowsend;
               _surf = surf;
               _deltax = deltax;
               _deltay = deltay;
               _columns = columns;
               _doneEvent = doneEvent;
               _min = min;
               _rowsTotal = rows;
               _getCVT = getCVT;
               if (edgepnts != null)
               {
                    edgepnts.RemoveRange(0, edgepnts.Count - 9);
                    _edge = new LeastSquares(edgepnts, 0, 0);
               }
          }

          /// <summary>
          /// A MeshThread object to calculate a piece of a rbf mesh
          /// </summary>
          /// <param name="surf">the surface rbf to get mesh from</param>
          /// <param name="rows">total # of rows</param>
          /// <param name="rowstart">rows value for this thread to start from</param>
          /// <param name="rowsend">rows value for this thread to go to</param>
          /// <param name="columns">total # of columns</param>
          /// <param name="deltax">usually something like maxX-minX (max[0] - min[0])</param>
          /// <param name="deltay">usually something like maxY-minY (max[1] - min[1])</param>
          /// <param name="min">the minimum used to calculate the deltas</param>
          /// <param name="doneEvent">your ManualResetEvent that will tell you when this is done</param>
          public MeshThread(SurfaceRBF surf, int rows, int rowstart, int rowsend, int columns, double deltax, double deltay, double[] min, List<double[]> edgepnts, ManualResetEvent doneEvent)
          {
               shaper1 = ((double input) => { return input; }); // shapers undefined so return inputted value
               shaper2 = ((double input) => { return input; }); // shapers undefined so return inputted value

               _rowStart = rowstart;
               _rowsEnd = rowsend;
               _surf = surf;
               _deltax = deltax;
               _deltay = deltay;
               _columns = columns;
               _doneEvent = doneEvent;
               _min = min;
               _rowsTotal = rows;
               if (edgepnts != null)
               {
                    edgepnts.RemoveRange(0, edgepnts.Count - 9);// only use the slanted edge
                    _edge = new LeastSquares(edgepnts, 0, 0);
               }
          }

          /// <summary>
          /// A MeshThread object to calculate a piece of a rbf mesh
          /// </summary>
          /// <param name="surf">the surface rbf to get mesh from</param>
          /// <param name="rows">total # of rows</param>
          /// <param name="rowstart">rows value for this thread to start from</param>
          /// <param name="rowsend">rows value for this thread to go to</param>
          /// <param name="columns">total # of columns</param>
          /// <param name="deltax">usually something like maxX-minX (max[0] - min[0])</param>
          /// <param name="deltay">usually something like maxY-minY (max[1] - min[1])</param>
          /// <param name="min">the minimum used to calculate the deltas</param>
          /// <param name="doneEvent">your ManualResetEvent that will tell you when this is done</param>
          /// <param name="getCVT">bool to indicate if you want to calculate the surface rbf curvature value as well</param>
          public MeshThread(SurfaceRBF surf, int rows, int rowstart, int rowsend, int columns, double deltax, double deltay, double[] min, ManualResetEvent doneEvent, List<double[]> edgepnts, bool getCVT, shaper shpr1, shaper shpr2)
          {
               if(shpr1 != null)
                    shaper1 = shpr1;
               else
                    shaper1 = ((double input) => { return input; }); // shapers undefined so return inputted value

               if(shpr2 != null)
                    shaper2 = shpr2;
               else
                    shaper2 = ((double input) => { return input; }); // shapers undefined so return inputted value

               _rowStart = rowstart;
               _rowsEnd = rowsend;
               _surf = surf;
               _deltax = deltax;
               _deltay = deltay;
               _columns = columns;
               _doneEvent = doneEvent;
               _min = min;
               _rowsTotal = rows;
               _getCVT = getCVT;
               if (edgepnts != null)
               {
                    edgepnts.RemoveRange(0, edgepnts.Count - 9);
                    _edge = new LeastSquares(edgepnts, 0, 0);
               }
          }

          /// <summary>
          /// A MeshThread object to calculate a piece of a rbf mesh
          /// </summary>
          /// <param name="surf">the surface rbf to get mesh from</param>
          /// <param name="rows">total # of rows</param>
          /// <param name="rowstart">rows value for this thread to start from</param>
          /// <param name="rowsend">rows value for this thread to go to</param>
          /// <param name="columns">total # of columns</param>
          /// <param name="deltax">usually something like maxX-minX (max[0] - min[0])</param>
          /// <param name="deltay">usually something like maxY-minY (max[1] - min[1])</param>
          /// <param name="min">the minimum used to calculate the deltas</param>
          /// <param name="doneEvent">your ManualResetEvent that will tell you when this is done</param>
          public MeshThread(SurfaceRBF surf, int rows, int rowstart, int rowsend, int columns, double deltax, double deltay, double[] min, List<double[]> edgepnts, ManualResetEvent doneEvent, shaper shpr1, shaper shpr2)
          {
               if(shpr1 != null)
                    shaper1 = shpr1;
               else
                    shaper1 = ((double input) => { return input; }); // shapers undefined so return inputted value

               if(shpr2 != null)
                    shaper2 = shpr2;
               else
                    shaper2 = ((double input) => { return input; }); // shapers undefined so return inputted value

               _rowStart = rowstart;
               _rowsEnd = rowsend;
               _surf = surf;
               _deltax = deltax;
               _deltay = deltay;
               _columns = columns;
               _doneEvent = doneEvent;
               _min = min;
               _rowsTotal = rows;
               if (edgepnts != null)
               {
                    edgepnts.RemoveRange(0, edgepnts.Count - 9);// only use the slanted edge
                    _edge = new LeastSquares(edgepnts, 0, 0);
               }
          }

          public void ThreadPoolCallback(Object threadContext)
          {
               int threadIndex = (int)threadContext;
               if (_getCVT)
                    CalculateCVT();
               else
                    Calculate();
               _doneEvent.Set();
          }

          /// <summary>
          /// method used to calculate mesh points with curvatures
          /// </summary>
          public void CalculateCVT()
          {
               int j;
               double x, y, k = 0;
               double[] d = new double[3];
               double[] dd = new double[3];

               if (_edge == null)
               {
                    for (; _rowStart < _rowsEnd; _rowStart++)
                    {
                         x = shaper1((double)_rowStart / (double)(_rowsTotal - 1)) * _deltax + _min[0];
                         //x = (double)_rowStart / (double)(_rowsTotal - 1) * _deltax + _min[0];
                         for (j = 0; j < _columns; j++)
                         {
                              //y = (double)j / (double)(_columns - 1) * _deltay + _min[1];
                              y = shaper2((double)j / (double)(_columns - 1)) * _deltay + _min[1];
                              double[] p = new double[3] { x, y, 0 };
                              _surf.Gaussian(ref p, ref d, ref dd, ref k);
                              _result.Add(new KeyValuePair<double[], double>(p, k));
                         }
                    }
               }
               else
               {
                    int tmpStart, tmpEnd;
                    List<double> deltas = new List<double>();
                    tmpStart = _rowStart;
                    tmpEnd = _columns;
                    double newDelta = _deltax;
                    for (; tmpStart < _rowsEnd; tmpStart++)
                    {
                         x = (double)tmpStart / (double)(_rowsTotal - 1) * _deltax + _min[0];
                         newDelta = _edge.YValue(x) - _min[1];
                         deltas.Add( newDelta>_deltay ? _deltay : newDelta );
                    }

                    int inc = 0;
                    for (; _rowStart < _rowsEnd; _rowStart++)
                    {
                         //x = (double)_rowStart / (double)(_rowsTotal - 1) * _deltax + _min[0];
                         x = shaper1((double)_rowStart / (double)(_rowsTotal - 1)) * _deltax + _min[0];
                         for (j = 0; j < _columns; j++)
                         {
                              //y = (double)j / (double)(_columns - 1) * deltas[inc] + _min[1];
                              y = shaper2((double)j / (double)(_columns - 1)) * deltas[inc] + _min[1];
                              double[] p = new double[3] { x, y, 0 };
                              _surf.Gaussian(ref p, ref d, ref dd, ref k);
                              _result.Add(new KeyValuePair<double[], double>(p, k));
                         }
                         inc++;
                    }
               }
          }

          /// <summary>
          /// method used to calculate mesh points without curvatures
          /// </summary>
          public void Calculate()
          {
               int j;
               double x, y;
               double[] d = new double[3];
               double[] dd = new double[3];

               if (_edge == null)
               {
                    for (; _rowStart < _rowsEnd; _rowStart++)
                    {
                         x = shaper1((double)_rowStart / (double)(_rowsTotal - 1)) * _deltax + _min[0];
                         //x = (double)_rowStart / (double)(_rowsTotal - 1) * _deltax + _min[0];
                         for (j = 0; j < _columns; j++)
                         {
                              y = shaper2((double)j / (double)(_columns - 1)) * _deltay + _min[1];
                              //y = (double)j / (double)(_columns - 1) * _deltay + _min[1];
                              double[] p = new double[3] { x, y, 0 };
                              _surf.Value(ref p);
                              _result2.Add(p);
                         }
                    }
               }
               else
               {
                    int tmpStart, tmpEnd;
                    List<double> deltas = new List<double>();
                    tmpStart = _rowStart;
                    tmpEnd = _columns;

                    double newDelta = _deltax;

                    for (; tmpStart < _rowsEnd; tmpStart++)
                    {
                         x = (double)tmpStart / (double)(_rowsTotal - 1) * _deltax + _min[0];
                         newDelta = _edge.YValue(x) - _min[1];
                         deltas.Add(newDelta > _deltay ? _deltay : newDelta);
                    }

                    int inc = 0;
                    for (; _rowStart < _rowsEnd; _rowStart++)
                    {
                         x = shaper1((double)_rowStart / (double)(_rowsTotal - 1)) * _deltax + _min[0];
                         //x = (double)_rowStart / (double)(_rowsTotal - 1) * _deltax + _min[0];
                         for (j = 0; j < _columns; j++)
                         {
                              y = shaper2((double)j / (double)(_columns - 1)) * _deltay + _min[1];
                              //y = (double)j / (double)(_columns - 1) * deltas[inc] + _min[1];
                              double[] p = new double[3] { x, y, 0 };
                              _surf.Value(ref p);
                              _result2.Add(p);
                         }
                         inc++;
                    }
               }
          }
     }

     /// <summary>
     /// 2d least squares linear fitter
     /// </summary>
     public class LeastSquares
     {
          public LeastSquares() { }

          public LeastSquares(List<double[]> rawPnts, double xoffset, double yoffset)
          {
               _entries.Clear();
               rawPnts.ForEach((double[] d) =>
               {
                    _entries.Add(new KeyValuePair<double, double>(d[0] + xoffset, d[1] + yoffset));
               });

               Crunch();
          }

          public LeastSquares(List<double[]> rawPnts)
          {
               _entries.Clear();
               rawPnts.ForEach((double[] d) =>
               {
                    _entries.Add(new KeyValuePair<double, double>(d[0], d[1]));
               });

               Crunch();
          }

          public LeastSquares(List<KeyValuePair<double, double>> entries)
          {
               _entries.Clear();
               _entries = new List<KeyValuePair<double, double>>(entries);
               Crunch();
          }

          #region Members

          private List<KeyValuePair<double, double>> _entries = new List<KeyValuePair<double, double>>();

          private double _sumX, _sumY, _sumX2, _sumXY, _error;

          #endregion Members

          public List<KeyValuePair<double, double>> Entries
          {
               get { return _entries; }
          }

          private void Crunch()
          {
               _sumX = 0; _sumY = 0; _sumX2 = 0; _sumXY = 0;

               Entries.ForEach((KeyValuePair<double, double> entry) =>
               {
                    _sumX += entry.Key;
                    _sumY += entry.Value;
                    _sumXY += (entry.Key * entry.Value);
                    _sumX2 += Math.Pow(entry.Key, 2);
               });
          }

          /// <summary>
          /// use the Least Squares line to solve
          /// </summary>
          /// <param name="x">
          /// A <see cref="System.Double"/>
          /// </param>
          /// <returns>
          /// A <see cref="System.Double"/>
          /// </returns>
          public double YValue(double x)
          {
               return Slope * x + Intercept;
          }

          public double XValue(double y)
          {
               return (y - Intercept) / Slope;
          }

          public string CalculateBestFitLine()
          {
               return "Line Equation: y = " + String.Format("{0:0.00}", Slope) + "x + " + String.Format("{0:0.00}", Intercept) + "\nError: " + String.Format("{0:0.00}", Error);
          }

          private double Slope
          {
               get
               {

                    double n = (double)Entries.Count;
                    double num = n * _sumXY - _sumX * _sumY;
                    if (num == 0)
                         return 0;
                    double denom = n * _sumX2 - _sumX * _sumX;
                    return num / denom;
               }
          }

          private double Intercept
          {
               get
               {
                    return (_sumY - Slope * _sumX) / Entries.Count;
               }
          }

          public double Error
          {
               get
               {
                    _error = 0.0;
                    Entries.ForEach((KeyValuePair<double, double> entry) =>
                    {
                         _error += Math.Pow(entry.Value - YValue(entry.Key), 2);
                    });

                    return _error;
               }
          }
     }
}