using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBFPolynomials;
using RBFBasis;
using RBFTools;
using NsNodes;
using System.Xml;

namespace RBF
{
	public class RBFCurve : NsNode
	{
		public RBFCurve(NsNode parent, XmlNode xml)
			: base(parent, xml)
		{
		}
		public RBFCurve(NsNode parent, string label, List<double[]> fitPoints, IBasisFunction basis, IRBFPolynomial poly, double relaxation)
			:base(label, parent)
		{
			Add(new CenterArrayNode(this));
			basis.CopyTo(this);
			poly.CopyTo(this);
			Relaxation = relaxation;
               OriginalFitPoints = fitPoints;
			Fit(fitPoints, relaxation);
		}

          List<double[]> m_fits;

          public List<double[]> OriginalFitPoints
          {
               get { return m_fits; }
               set { m_fits = new List<double[]>(value); }
          }

		internal IRBFPolynomial Poly
		{
			get
			{
				foreach (IAttribute atr in Attributes)
					if (atr is IRBFPolynomial)
						return atr as IRBFPolynomial;
				return null;
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

		double[] m_max = new double[2];
		public double[] Max
		{
			get { return m_max; }
		}

		double[] m_min = new double[2];
		public double[] Min
		{
			get { return m_min; }
		}
		internal double[] Middle
		{
			get { return new double[] { (m_max[0] + m_min[0]) / 2, (m_max[1] + m_min[1]) / 2, (m_max[2] + m_min[2]) / 2 }; }
		}

		public CenterArrayNode CenterNode
		{
			get
			{
				foreach (NsNode n in Nodes)
					if (n is CenterArrayNode)
						return n as CenterArrayNode;
				return null;
			}
		}

          public List<double[]> FitPoints
          {
               get
               {
                    return CenterNode.CentersAsList;
               }
          }

		IBasisFunction m_basis;

		IBasisFunction rbf
		{
			get
			{
				if (m_basis == null)
					foreach (IAttribute a in Attributes)
						if (a is IBasisFunction)
							m_basis = a as IBasisFunction;
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

		int Fit(List<double[]> fitPoints, double Relax)
		{
			Relaxation = Relax;
			BendingEnergy = 0; //reset bending energy and error
			Error = 0;
			m_max[0] = m_max[1] = -1e9; //start max low
			m_min[0] = m_min[1] = +1e9; //start min high
			int i, j;

			CenterNode.ClearCenters(); //allocate space for center
			CenterNode.PauseUpdating();

			List<double> fitz = new List<double>(fitPoints.Count); //temp vector for rhs

			foreach (double[] v in fitPoints)
			{
				fitz.Add(v[1]);
				CenterNode.Add(new Center2d(v[0]));

				for (i = 0; i < v.Length; i++) //get fit points' bounding box
				{
					m_max[i] = Math.Max(v[i], m_max[i]);
					m_min[i] = Math.Min(v[i], m_min[i]);
				}
			}
			//polynomial conditons
			for (i = 0; i < Poly.Terms; i++)
				fitz.Add(0);
			//create the fitting matrix
			double[,] A;
			RBFSolver.fit_mat(out A, CenterNode, rbf, Poly, Relaxation);

			int err = RBFSolver.solve(A, fitz.ToArray(), CenterNode, Poly, true);
			if (err != 0)
				return err;

			//calculate bending energy (wT * A * w)
			double be = 0;
			List<double> wtA = new List<double>(CenterNode.Centers.Count); //temp vector for w-transpose * A
			for (i = 0; i < CenterNode.Centers.Count; i++)
				wtA.Add(0);
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
		/// returns the error between the RBF Surface Z locations and the lifters actual Z location
		/// </summary>
		/// <param name="pnts">Lifter locations</param>
		/// <returns>error value</returns>
		public double CheckFit(IList<double[]> pnts)
		{
			double error = 0;
			double[] p = new double[2];
			foreach (double[] v in pnts)
			{
				p = new double[] { v[0], v[1] };
				Value(ref p);
				error += Math.Abs(v[1] - p[1]) / v[1];
			}
			return error;
		}

		private void PolyTerms(double[] p, double[] d, double[] dd)
		{
			Poly.Poly(p, d, dd);
			return;
		}
		public void Value(ref double[] p)
		{
			p[1] = 0;
			double r;
			foreach (ICenter<double> c in CenterNode.Centers)
			{
				r = c.radius(p);
				p[1] += c.w * rbf.val(r); // sum the weight * rbf values
				//rbf()->value(c, p); 
			}
			PolyTerms(p, null, null); // add the polynomial
		}
		public void First(ref double[] p, ref double[] d)
		{
               if (p.Length == 2)
               {
                    p[1] = 0;
                    d[0] = 1;
                    d[1] = 0;
                    double r, dr, drdx;
                    foreach (ICenter<double> c in CenterNode.Centers)
                    {
                         r = c.radius(p); // radius
                         if (BLAS.is_equal(r, 0)) continue;

                         p[1] += c.w * rbf.val(r); // sum the weight * rbf values

                         dr = rbf.dr(r); // dRBF/dr

                         drdx = (p[0] - c[0]) / r; // dr/dx
                         d[1] += c.w * dr * drdx; // accumulate weighted derivatives
                    }
                    PolyTerms(p, d, null); // add the polynomial
               }
               else if (p.Length == 3)
               {
                    p[1] = 0;
                    d[0] = 1;
                    d[1] = 0;
                    double r, dr, drdx;
                    foreach (ICenter<double> c in CenterNode.Centers)
                    {
                         r = c.radius(p); // radius
                         if (BLAS.is_equal(r, 0)) continue;

                         p[2] += c.w * rbf.val(r); // sum the weight * rbf values

                         dr = rbf.dr(r); // dRBF/dr

                         drdx = (p[0] - c[0]) / r; // dr/dx
                         d[1] += c.w * dr * drdx; // accumulate weighted derivatives
                    }
                    PolyTerms(p, d, null); // add the polynomial
               }

               
		}
		public void Second(ref double[] p, ref double[] d, ref double[] dd)
		{
			p[1] = 0; //initialize z
			d[0] = 1;
			d[1] = 0;
			dd[0] = 1;
			dd[1] = 0;
			double r, dr, drdx, ddrdxx;
			foreach (ICenter<double> c in CenterNode.Centers)
			{
				r = c.radius(p); // radius
				if (BLAS.is_equal(r, 0)) continue;
				p[1] += c.w * rbf.val(r); // sum the weight * rbf values

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
				//dd[2] += c.w * 2 * (p[0] - c[0]) * (p[1] - c[1]) / (r * r); // d^2r/dxdy
			}
			PolyTerms(p, d, dd); // add the polynomial
		}

		/// <summary>
		/// return Unit normal vector
		/// </summary>
		/// <param name="p">point to get normal at, y value will be filled on return</param>
		/// <param name="d">tangent at point</param>
		/// <param name="nor">normal at point</param>
		public void Normal(ref double[] p, ref double[] d, ref double[] nor)
		{
			First(ref p, ref d); //get the first derivatives and z value
			nor = new double[] { d[1], -d[0] };
		}

		/// <summary>
		/// finds closest point to given p paramter
		/// </summary>
		/// <param name="p">in: target point, out: closest on curve</param>
		/// <param name="dist">out: distance to curve</param>
		/// <param name="tol">tolerance</param>
		/// <returns>true if successful, false otherwise</returns>
		public bool Closest(ref double[] p, ref double dist, double tol)
		{
			if (CenterNode.Count == 0)
				return false;

			double[] x = new double[] { p[0], p[1] };
			double[] dx = new double[2];
			double[] ddx = new double[2];

			double[] h = new double[2];
			double[] e = new double[2];
			double dedx;

			int loop = 0, max_loops = 100;
			while (loop++ < max_loops)
			{
				Second(ref x, ref dx, ref ddx);

				h = BLAS.subtract(x, p);
				dist = BLAS.magnitude(h);//.magnitude();

				e[0] = x[0];
				e[1] = BLAS.dot(h, dx); // error, dot product is 0 at pi/2

				if (Math.Abs(e[1]) < tol) // error is less than the tolerance
				{
					x.CopyTo(p,0);// return point to caller
					return true;
				}

				dedx = BLAS.dot(dx, dx) + BLAS.dot(h, ddx);

				// calculate a new x
				x[0] = e[0] - e[1] / dedx;
				//logger.write_format_line("%.5g\t%.5g\t%.5g\t%.5g\t%.5g\t", x[ox], x[oy], e[ox], e[oy], dist);
			}
			return false;
		}

		public List<double[]> GetMeshPoints(int p)
		{	
			List<double[]> vals = new List<double[]>();

			double deltax = Max[0] - Min[0];
			double[] x = new double[2];

			for (int i = 0; i < p; i++)
			{
				x = new double[]{ (double)i / (double)(p - 1) * deltax + Min[0], 0 };
				Value(ref x);
				vals.Add(x);
			}
			return vals;
		}
	}
}
