using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NsNodes
{
	public class NodeTrackerEventArgs : EventArgs
	{
		public NodeTrackerEventArgs()
			:this(null, null)
		{
		}
		public NodeTrackerEventArgs(double[] start, double[] end)
		{
			m_startxyz = start;
			m_endxyz = end;
		}
		public NodeTrackerEventArgs(double deltaX, double deltay, double deltaz, bool return_to_origin)
		{
			m_bReturnToO = return_to_origin;
			m_endxyz = new double[3] { deltaX, deltay, deltaz };
			m_startxyz = new double[3] { 0, 0, 0 };
		}
		public NodeTrackerEventArgs(double deltaX, double deltay, double deltaz)
			: this(deltaX, deltay, deltaz, false)
		{
		}
		double[] m_startxyz;
		double[] m_endxyz;

		bool m_bReturnToO = false;

		public bool Return_to_Origin
		{
			get { return m_bReturnToO; }
		}

		public double this[int i]
		{
			get { return m_endxyz != null && m_startxyz != null ? m_endxyz[i] - m_startxyz[i] : 0; }
		}

		public double DeltaX
		{
            get { return this[0]; }
		}
		public double DeltaY
		{
			get { return this[1]; }
		}
		public double DeltaZ
		{
			get { return this[2]; }
		}

          public double[] Start
        {
            get { return m_startxyz; }
        }
		public double StartX
		{
			get { return m_startxyz[0]; }
		}
		public double StartY
		{
			get { return m_startxyz[1]; }
		}
		public double StartZ
		{
			get { return m_startxyz[2]; }
		}

          public double[] End
        {
            get { return m_endxyz; }
        }
		public double EndX
		{
			get { return m_endxyz[0]; }
		}
		public double EndY
		{
			get { return m_endxyz[1]; }
		}
		public double EndZ
		{
			get { return m_endxyz[2]; }
		}

		/// <summary>
		/// get unit normal vector
		/// </summary>
		public double[] Normal
		{
			get
			{
				double[] norm = new double[3] { StartY * EndZ - StartZ * EndY, -(StartX * EndZ - StartZ * EndX), StartX * EndY - StartY * EndX };
				double mag = Math.Sqrt(Math.Pow(norm[0], 2) + Math.Pow(norm[1], 2) + Math.Pow(norm[2], 2));
				norm[0] /= mag;
				norm[1] /= mag;
				norm[2] /= mag;
				return norm;
			}
		}
	}
}
