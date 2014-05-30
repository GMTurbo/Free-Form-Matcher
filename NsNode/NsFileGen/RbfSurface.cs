using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;
using devDept.Geometry;

namespace NsFileGen
{
	class SurfaceRBF : NsNode
	{
		public SurfaceRBF(IBasisFunction basis)
		{
			m_basis = basis;
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
				Remove(m_basis);
				m_basis = value;
				Add(m_basis);
			}
		}

		List<double> polycofs
		{
			get
			{
				IAttribute atr = FindAttribute("Coefficients");
				if (atr != null && atr is ArrayAttribute)
					return atr.Value as List<double>;
				else
				{
					return Add(new ArrayAttribute(this, "Coefficients", new List<double>( new double[]{0,0,0}))).Value as List<double>;
				}
			}
			set
			{
				IAttribute atr = FindAttribute("Coefficients");
				if (atr != null && atr is DoubleAttribute)
					atr.Value = value;
				else
					Add(new ArrayAttribute(this, "Coefficients", value));
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

		double[] m_max = new double[3];
		double[] m_min = new double[3];

		List<ICenter> Centers //move to a subnode
		{
			get
			{
				List<ICenter> cents = new List<ICenter>(Attributes.Count);
				foreach (IAttribute atr in Attributes)
					if (atr is ICenter)
						cents.Add(atr as ICenter);
				return cents;
			}
		}

		int ClearCenters()
		{
			int ret = 0;
			foreach (IAttribute a in Attributes)
				if (a is ICenter)
				{
					Remove(a);
					ret++;
				}
			return ret;
		}
		void Poly(double[] p, double[] d, double[] dd)
		{
			p[2] += polycofs[0] * p[0] + polycofs[1] * p[1] + polycofs[2]; // Ax + By + Cc
			if (d != null)
			{
				d[0] += polycofs[0];
				d[1] += polycofs[1];
			}
		}
		public void Value(ref double[] p)
		{
			System.Diagnostics.Debug.Assert(p.Length == 3);
			p[2] = 0;
			double r;
			foreach (ICenter c in Centers)
			{
				r = c.radius(p);
				if (is_equal(r, 0)) continue;

				p[2] += c.w * rbf.val(r); // sum the weight * rbf values
				//rbf()->value(c, p); 
			}
			Poly(p, null, null); // add the polynomial
		}
		public void First(ref double[] p, ref double[] d)
		{
			p[2] = 0;
			d[0] = d[1] = d[2] = 0;
			double r, dr, drdx;
			foreach (ICenter c in Centers)
			{
				r = c.radius(p); // radius
				if (is_equal(r, 0)) continue;

				p[2] += c.w * rbf.val(r); // sum the weight * rbf values

				dr = rbf.dr(r); // dRBF/dr

				drdx = (p[0] - c[0]) / r; // dr/dx
				d[0] += c.w * dr * drdx; // accumulate weighted derivatives

				drdx = (p[1] - c[1]) / r; // dr/dy
				d[1] += c.w * dr * drdx; // accumulate weighted derivatives

				//dx[oz] += c.w()*dr; // accumulate weighted derivatives
				//rbf()->first(c, p, dx); 
			}
			Poly(p, d, null); // add the polynomial
		}
		public void Second(ref double[] p, ref double[] d, ref double[] dd)
		{
			p[2] = 0; //initialize z
			d[0] = d[1] = d[2] = 0;
			dd[0] = dd[1] = dd[2] = 0;
			double r, dr, drdx, ddrdxx;
			foreach (ICenter c in Centers)
			{
				r = c.radius(p); // radius
				if (is_equal(r, 0)) continue;
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
				dd[2] += 2 * (p[0] - c[0]) * (p[1] - c[1]) / (r * r); // d^2r/dxdy
			}
			Poly(p, d, dd); // add the polynomial
		}
		public void Normal(ref double[] p, ref double[] d, ref double[] nor)
		{
			First(ref p, ref d); //get the first derivatives and z value
			double[] dx = d;
			double[] dy;
			split(ref dx, out dy);
			nor = cross(dx, dy); //cross surface tangents to get normal
			// make unit-normal(magnitude is meaningless anyway)
			double mag = magnitude(nor);
			nor[0] /= mag;
			nor[1] /= mag;
			nor[2] /= mag;
			System.Diagnostics.Debug.Assert((nor[0] * nor[0] + nor[1] * nor[1] + nor[2] * nor[2]) == 1);
		}

		public bool Closest(ref double[] p, ref double dist, double tol)
		{
			if (Centers.Count == 0)
				return false;

			double[] x = new double[] { p[0], p[1], p[2] };
			double[] dx = new double[3];
			double[] dy = new double[3];
			double[] ddx = new double[3];
			double[] ddy = new double[3];

			double[] h = new double[3];
			double[] e = new double[4];

			double dedx;
			double dedy;

			int loop = 0, max_loops = 1000;
			while (loop++ < max_loops)
			{
				Second(ref x, ref dx, ref ddx);
				split(ref dx, out dy); //get independant dx and dy vectors
				for (int i = 0; i < 3; i++) h[i] = x[i] - p[i];
				dist = magnitude(h);

				e[0] = x[0];
				e[1] = x[1];
				e[2] = dot(h, dx); //x error, dot product is 0 at pi/2
				e[3] = dot(h, dy); //y error

				if (Math.Abs(e[2]) < tol && Math.Abs(e[3]) < tol) // error is less than the tolerance
				{
					p = x; // return point to caller
					return true;
				}

				split(ref ddx, out ddy);//get independant ddx and ddy vectors from combined form
				dedx = dot(dx, dx) + dot(h, ddx);
				dedy = dot(h, dy) + dot(h, ddy);

				// calculate a new x
				x[0] = e[0] - e[2] / dedx;
				x[1] = e[1] - e[3] / dedy;
			}
			return false;
		}

		double Fit(IList<double[]> fitPoints, double? pRelax)
		{
			BendingEnergy = 0; //reset bending energy and error
			Error = 0;
			m_max[0] = m_max[1] = m_max[2] = -1e9; //start max low
			m_min[0] = m_min[1] = m_min[2] = +1e9; //start min high

			ClearCenters(); //allocate space for center

			if( pRelax != null )
				Relaxation = (double)pRelax; //0 for exact, increase to reduce bending energy

			int i, j; //loops

			List<double> fitz = new List<double>(fitPoints.Count + 3); //temp vector for rhs

			foreach (double[] v in fitPoints)
			{
				fitz.Add(v[2]);
				Add(new Center3d(v[0], v[1]));
				for (i = 0; i < 3; i++) //get fit points' bounding box
				{
					m_max[i] = Math.Max(v[i], m_max[i]);
					m_min[i] = Math.Min(v[i], m_min[i]);
				}
			}

			// poly conditions
			fitz.Add(0);
			fitz.Add(0);
			fitz.Add(0);

			//assert( check_centers() == 0 ); // ensure no coincident centers

			//create the fitting matrix
			double[,] A;
			fit_mat(out A);

			// solve the system
			int err = solve(A, fitz.ToArray());
			if (err != 1)
				return err;

			//calculate bending energy (wT * A * w)
			double be = 0;
			List<double> wtA = new List<double>(Centers.Count); //temp vector for w-transpose * A
			for (i = 0; i < Centers.Count; i++)
				for (j = 0; j < Centers.Count; j++)
				{
					wtA[i] += Centers[j].w * A[j, i]; //first multiplication
				}
			for (i = 0; i < Centers.Count; i++)
				be += wtA[i] * Centers[i].w; //second multiplication
			BendingEnergy = be;

			//calculate error as the sum of % difference between target z and actual z
			double error =0;
			double[] p = new double[3];
			foreach (double[] v in fitPoints)
			{
				p = v;
				Value(ref p);
				error += Math.Abs(v[2] - p[2]) / v[2];
			}
			Error = err;

			return 1;
		}

		void fit_mat(out double[,] A)
		{
			int dim = Centers.Count;
			int fits = dim + 3;//3 for polygon

			A = new double[fits, fits];
			// create A matrix
			double r;
			double a = 0;//for scaling relaxation parameter
			int i, j;

			for (i = 0; i < dim; ++i)
			{
				for (j = i + 1; j < dim; ++j)
				{
					r = Centers[j].radius(Centers[i].ToArray());
					A[i, j] = A[j, i] = rbf.val(r); // symmetric
					a += 2 * r;
				}
			}
			a /= dim * dim; // calculate relaxation normalizer
			// calculate fit mat diagonals and poly terms
			for (i = 0; i < dim; ++i)
			{
				A[i, i] = a * a * Relaxation;

				// poly values: Ax + Bx + C SUR_POLY
				A[i, dim + 0] = A[dim + 0, i] = Centers[i][0];
				A[i, dim + 1] = A[dim + 1, i] = Centers[i][1];
				A[i, dim + 2] = A[dim + 2, i] = 1.0;
			}
		}
		int solve(double[,] A, double[] fitz)
		{
			System.Diagnostics.Debug.Assert(fitz.Length == A.GetLength(0));
			System.Diagnostics.Debug.Assert(A.GetLength(0) == A.GetLength(1));

			int[] pivot = new int[A.GetLength(0)];
			int error = 0;
			//DeComp(Tolerance, A.get(), &pivot[0], &error, A.cols(), A.rows());

			if( error <= 0 ) 
			{
				return error;
			}

			double[] w = new double[A.GetLength(0)];// create weights vector
			//DeSolv( A.get(), &w[0], &fitz[0], &pivot[0], error, A.rows() );
			
			int i = 0;
			foreach( double weight in w )
			{
				if( i < Centers.Count )
					Centers[i].w = weight; // set the center's weight
				else
					polycofs[i-Centers.Count] = weight; //store the polynomial coefficients

				++i;
			}

			return 0;
		}

		#region Utilities

		double dot(double[] a, double[] b)
		{
			return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
		}
		double[] cross(double[] a, double[] b)
		{
			double[] c = new double[3];
			int i1, i2;
			for (int i = 0; i < 3; i++)
			{
				i1 = (i + 1) % 3;
				i2 = (i + 2) % 3;
				c[i] = a[i1] * b[i2] - a[i2] * b[i1];
			}
			return c;
		}
		void split(ref double[] dx, out double[] dy)
		{
			dy = new double[] { 0, 1, dx[1] };
			dx = new double[] { 1, 0, dx[0] };
		}
		double magnitude(double[] p)
		{
			return Math.Sqrt(dot(p, p));
		}
		bool is_equal(double a, double b)
		{
			return Math.Abs(a - b) < 1e-7;
		}

		#endregion
	}

	public class ThinPlateSpline : IBasisFunction
	{

		#region IBasisFunction Members

		public ThinPlateSpline(NsNode parent)
		{
			m_parent = parent;
		}
		public ThinPlateSpline(NsNode parent, System.Xml.XmlNode xml)
			:this(parent)
		{

		}


		public double val(double r)
		{
			return r == 0 ? 0 : r * r * Math.Log(r);
		}

		public double dr(double r)
		{
			return r == 0 ? 0 : (2*r*Math.Log(r)+r);
		}

		public double ddr(double r)
		{
			return r == 0 ? 0 : (2*Math.Log(r)+3);
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
			get { return GetType().ToString();  }
		}

		public System.Xml.XmlElement ToXml(System.Xml.XmlDocument doc)
		{
			throw new NotImplementedException();
		}

		public bool FromXml(System.Xml.XmlNode xml)
		{
			throw new NotImplementedException();
		}

		public bool Remove()
		{
			return Parent.Remove(this);
		}

		#endregion
	}
	interface IBasisFunction : IAttribute
	{
		double val(double r);
		/* returns the value of the first derivative of this function with respect to the radius */
		double dr(double r);
		/* returns the value of the second derivative of this function with respect to the radius */
		double ddr(double r);
	}
	interface ICenter : IAttribute
	{
		double radius(double[] p);
		double w { get; set; }
		double this[int i] { get; set; }
		double[] ToArray();
	}
	public class Center3d : ICenter
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
			:this()
		{
			m_parent = parent;
			if (!FromXml(xml))
				throw new AttributeXmlFormatException(this, xml, "Failed to read xml");
		}
		
		double[] m_pt;
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
				return m_pt;
			}
			set
			{
				if (value is double[] && (value as double[]).Length == 2)
					m_pt = value as double[];
				else if (value == null)
					m_pt = null;
				else
					throw new Exception("Invalid type for PointAttribute.Value, must be double[2]");
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
	}
}