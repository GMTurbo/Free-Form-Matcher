using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NsNodes
{
	public static class BLAS
	{
		#region Solvers

		static public double[] FitCubic(List<double[]> pnts)
		{
			//fit a quadratic or least squares fit one
			double[,] A = new double[pnts.Count, 3];
			double[,] b = new double[pnts.Count, 1];
			double max = -1.0e9;
			double min = 1.0e9;
			int i = 0;
			foreach (double[] p in pnts)
			{
				A[i, 0] = p[0] * p[0];
				A[i, 1] = p[0];
				A[i, 2] = 1;
				b[i, 0] = p[1];
				i++;
				max = Math.Max(max, p[0]);
				min = Math.Min(min, p[0]);
			}
			double[,] coef;
			if (pnts.Count == 3)
				coef = BLAS.SimultaneousSolver(A, b);
			else
				coef = BLAS.LeastSquaresSolver(A, b);
			if (coef == null)
				return null;
			return new double[]{coef[0,0], coef[1,0], coef[2,0]};
		}

		static public double EvalCubic(double[] coef, double x)
		{
			return coef[0] * x * x + coef[1] * x + coef[2];
		}

		static public double Interpolate(double p, double max, double min)
		{
			return (max - min) * p + min;
		}

		static public double[,] LeastSquaresSolver(double[,] A, double[,] b)
		{
			// calculate normal arrays
			double[,] AtA = MatrixProduct(MatrixTranspose(A), A);
			double[,] Atb = MatrixProduct(MatrixTranspose(A), b);

			// if this fails due to mismatched sizes quit
			if (AtA == null || Atb == null) return null;

			// solve the system
			return SimultaneousSolver(AtA, Atb);
		}

		static public double[,] SimultaneousSolver(double[,] A, double[,] b)
		{
			// create the [A|b] matrix
			double[,] Ab = MatrixColConcat(A, b);
			// reduce it
			int[] index;
			Ab = RowEchelon(Ab, out index);
               //Ab = rref(Ab, out index);
			// return the solution
			return BackSolver(Ab,index);
		}

		static public bool Invert3x3(ref double[,] M)
		{
			//double[,] inv = new double[3, 3];
			//double det;

			//inv[0, 0] = M[1, 1] * M[2, 2] - M[1, 2] * M[2, 1];
			//inv[0, 1] = M[0, 2] * M[2, 1] - M[0, 1] * M[2, 2];
			//inv[0, 2] = M[0, 1] * M[1, 2] - M[0, 2] * M[1, 1];

			//inv[1, 0] = M[1, 2] * M[2, 0] - M[1, 0] * M[2, 2];
			//inv[1, 1] = M[0, 0] * M[2, 2] - M[0, 2] * M[2, 0];
			//inv[1, 2] = M[0, 2] * M[1, 0] - M[0, 0] * M[1, 2];

			//inv[2, 0] = M[1, 0] * M[2, 1] - M[1, 1] * M[2, 0];
			//inv[2, 1] = M[0, 1] * M[2, 0] - M[0, 0] * M[2, 1];
			//inv[2, 2] = M[0, 0] * M[1, 1] - M[0, 1] * M[1, 0];

			//det = M[0, 0] * inv[0, 0] + M[0, 1] * inv[1, 0] + M[0, 2] * inv[2, 0];

			//double[] ma = new double[9];
			double[] mr;

			//for (int i = 0; i < 3; i++)
			//     for (int j = 0; j < 3; j++)
			//          ma[i * 3 + j] = M[i, j];

			if (m3_inverse(out mr, M) == 0)
				return false;

			for (int i = 0; i < 3; i++)
				for (int j = 0; j < 3; j++)
				{
					//inv[i, j] /= det;
					//M[i, j] = inv[i, j] / det;
					M[i, j] = mr[i * 3 + j];
				}
			return true;
			//return inv;
		}

		static public bool Invert4x4(ref double[,] M)
		{
			//double[] ma = new double[16];
			double[] mr;

			//for (int i = 0; i < 4; i++)
			//     for (int j = 0; j < 4; j++)
			//          ma[i * 4 + j] = M[i, j];

			if (m4_inverse(out mr, M) == 0)
				return false;

			for (int i = 0; i < 4; i++)
				for (int j = 0; j < 4; j++)
				{
					//inv[i, j] /= det;
					//M[i, j] = inv[i, j] / det;
					M[i, j] = mr[i * 4 + j];
				}
			return true;		
		}

		#region Row Echelon

		static double[,] RowEchelon(double[,] A, out int[] index)
		{
			// make a copy
			double[,] M = MatrixCopy(A);

			int[] i = ArrangeRows(M);

			ReduceRows(M);

			index = i;
			return M;
		}

		static int[] ArrangeRows(double[,] M)
		{
			int col = 0;
			int r2 = 0;
			int i = 0;
			int[] index = new int[M.GetLength(0)];
			for (i = 0; i < index.Length; i++)
				index[i] = i;
               //return index;
			//double[,] order = new double[M.GetLength( 0 ) , 1];
			//for( int o = 0 ; o < M.GetLength( 0 ) ; o++ )
			//    order[o,0] = o;

			//M = MatrixColConcat( M , order );

			// order the matrix by searching for leading 0's
			for (int r1 = 0; r1 < M.GetLength(0); r1++)
			{
				r2 = r1;
				while (M[r2, col] == 0)
				{
					// check the next row
					r2++;
					if (r2 >= M.GetLength(0))
					{
						// go to the next column
						if (++col >= M.GetLength(1))
							return index;
						// go back to the first row
						r2 = r1;
						continue;
					}
				}
				if (r2 != r1)
				{
					RowSwap(M, r2, r1);
					i = index[r1];
					index[r1] = index[r2];
					index[r2] = i;
				}
				//else col++;
			}
			return index;
		}

		static void ReduceRows(double[,] M)
		{
			for (int col = 0, r1 = 0; r1 < M.GetLength(0); r1++, col++)
			{
				SetPivotToOne(M, r1);

				for (int r2 = r1 + 1; r2 < M.GetLength(0); r2++)
					SubtractRowMultiple(M, M[r2, col], r1, r2);
			}
		}

		static void RowSwap(double[,] M, int r1, int r2)
		{
			double temp;

			for (int k = 0; k < M.GetLength(1); k++)
			{
				temp = M[r1, k];
				M[r1, k] = M[r2, k];
				M[r2, k] = temp;
			}
		}

		static void SetPivotToOne(double[,] M, int row)
		{
			//find pivot
			int col = 0;
			while (M[row, col] == 0)
			{
				if (++col >= M.GetLength(1))
					return;
			}

			double pivot = M[row, col];

			for (int k = 0; k < M.GetLength(1); k++)
				M[row, k] /= pivot;
		}

		static void SubtractRowMultiple(double[,] M, double multiple, int r1, int r2)
		{
			for (int k = 0; k < M.GetLength(1); k++)
				M[r2, k] -= multiple * M[r1, k];
		}

		#endregion

		static double[,] BackSolver(double[,] Ab, int[] index)
		{
			double[,] x = new double[Ab.GetLength(0), 1];

			for (int i = Ab.GetLength(0) - 1; i >= 0; i--)
			{
				// xi = bi
				x[i, 0] = Ab[i, Ab.GetLength(1) - 1];

				// xi = bi - ( Aij xj )
				for (int j = i + 1; j < Ab.GetLength(0); j++)
					x[i, 0] -= Ab[i, j] * x[j, 0];
			}
			return x;
			//unswap
			//double[,] ret = new double[x.GetLength(0), 1];
			//for (int i = 0; i < ret.GetLength(0); i++)
			//{
			//     ret[index[i], 0] = x[i, 0];
			//}
			//return ret;
		}

		#endregion

		#region Matrix Math

		static public double[,] MatrixProduct(double[,] A, double[,] B)
		{
			if (A.GetLength(1) != B.GetLength(0)) return null;

			double[,] prod = new double[A.GetLength(0), B.GetLength(1)];

			for (int i = 0; i < prod.GetLength(0); i++)
				for (int j = 0; j < prod.GetLength(1); j++)
				{
					prod[i, j] = 0;
					for (int k = 0; k < A.GetLength(1); k++)
						prod[i, j] += A[i, k] * B[k, j];
				}

			return prod;
		}

		static public double[,] MatrixTranspose(double[,] A)
		{
			double[,] T = new double[A.GetLength(1), A.GetLength(0)];

			for (int i = 0; i < A.GetLength(0); i++)
				for (int j = 0; j < A.GetLength(1); j++)
					T[j, i] = A[i, j];
			return T;
		}

		static public double[,] MatrixCopy(double[,] A)
		{
			double[,] M = new double[A.GetLength(0), A.GetLength(1)];
			for (int i = 0; i < A.GetLength(0); i++)
				for (int j = 0; j < A.GetLength(1); j++)
					M[i, j] = A[i, j];
			return M;
		}

		static double[,] MatrixColConcat(double[,] A, double[,] B)
		{
			// check height
			if (A.GetLength(0) != B.GetLength(0)) return null;

			double[,] M = new double[A.GetLength(0), A.GetLength(1) + B.GetLength(1)];

			for (int i = 0; i < M.GetLength(0); i++)
				for (int j = 0; j < M.GetLength(1); j++)
				{
					M[i, j] = j < A.GetLength(1) ? A[i, j] : B[i, j - A.GetLength(1)];
				}
			return M;
		}

		static double[,] MatrixRowConcat(double[,] A, double[,] B)
		{
			// check height
			if (A.GetLength(1) != B.GetLength(1)) return null;

			double[,] M = new double[A.GetLength(0) + B.GetLength(0), A.GetLength(1)];

			for (int i = 0; i < M.GetLength(0); i++)
				for (int j = 0; j < M.GetLength(1); j++)
				{
					M[i, j] = i < A.GetLength(0) ? A[i, j] : B[i - A.GetLength(0), j];
				}
			return M;
		}

		static int m4_inverse(out double[] mr, double[,] ma)
		{
			mr = new double[16];
			double mdet = m4_det(ma);
			double[,] mtemp = new double[3,3];
			int i, j, sign;
			if (Math.Abs(mdet) < 0.0005)
			{
				//m4_identity(mr);
				return (0);
			}
			for (i = 0; i < 4; i++)
				for (j = 0; j < 4; j++)
				{
					sign = 1 - ((i + j) % 2) * 2;
					m4_submat(ma, mtemp, i, j);
					mr[i + j * 4] = (m3_det(mtemp) * sign) / mdet;
				}
			return (1);
		}
		static void m4_submat(double[,] mr, double[,] mb, int i, int j)
		{
			int di, dj, si, sj;
			// loop through 3x3 submatrix
			for (di = 0; di < 3; di++)
			{
				for (dj = 0; dj < 3; dj++)
				{
					// map 3x3 element (destination) to 4x4 element (source)
					si = di + ((di >= i) ? 1 : 0);
					sj = dj + ((dj >= j) ? 1 : 0);
					// copy element
					mb[di,  dj] = mr[si , sj];
				}
			}
		}
		static double m4_det(double[,] mr)
		{
			double det, result = 0, i = 1;
			double[,] msub3 = new double[3,3];
			int n;
			for (n = 0; n < 4; n++, i *= -1)
			{
				m4_submat(mr, msub3, 0, n);
				det = m3_det(msub3);
				result += mr[0,n] * det * i;
			}
			return (result);
		}
		static double m3_det(double[,] mat)
		{
			double det;
			det = mat[0,0] * (mat[1,1] * mat[2,2] - mat[2,1] * mat[1,2])
			    - mat[0,1] * (mat[1,0] * mat[2,2] - mat[2,0] * mat[1,2])
			    + mat[0,2] * (mat[1,0] * mat[2,1] - mat[2,0] * mat[1,1]);
			return (det);
		}
		static int m3_inverse(out double[] mr, double[,] ma)
		{
			mr = new double[9];
			double det = m3_det(ma);
			if (Math.Abs(det) < 0.0005)
			{

				//m3_identity(mr);
				return (0);
			}

			mr[0] = ma[1,1] * ma[2,2] - ma[1,2] * ma[2,1] / det;
			mr[1] = -(ma[0,1] * ma[2,2] - ma[2,1] * ma[0,2]) / det;
			mr[2] = ma[0,1] * ma[1,2] - ma[1,1] * ma[0,2] / det;
			mr[3] = -(ma[1,0] * ma[2,2] - ma[1,2] * ma[2,0]) / det;
			mr[4] = ma[0,0] * ma[2,2] - ma[2,0] * ma[0,2] / det;
			mr[5] = -(ma[0,0] * ma[1,2] - ma[1,0] * ma[0,2]) / det;
			mr[6] = ma[1,0] * ma[2,1] - ma[2,0] * ma[1,1] / det;
			mr[7] = -(ma[0,0] * ma[2,1] - ma[2,0] * ma[0,1]) / det;
			mr[8] = ma[0,0] * ma[1,1] - ma[0,1] * ma[1,0] / det;
			return (1);
		}

		#endregion

		#region Utilities

		static public double dot(double[] a, double[] b)
		{
			return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
		}
		static public double[] cross(double[] a, double[] b)
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
		static public  void split(ref double[] dx, out double[] dy)
		{
			dy = new double[] { 0, 1, dx[1] };
			dx = new double[] { 1, 0, dx[0] };
		}
		static public double magnitude(double[] p)
		{
			return Math.Sqrt(dot(p, p));
		}
		static public bool is_equal(double a, double b)
		{
			return Math.Abs(a - b) < 1e-7;
		}
		static public bool is_equal(double a, double b, double tol)
		{
			return Math.Abs(a - b) <  Math.Abs(tol);
		}

		static public double[] subtract(double[] a, double[] b)
		{
			double[] ret = new double[a.Length];
			for (int i = 0; i < ret.Length; i++)
				ret[i] = a[i] - b[i];
			return ret;
		}
		static public double[] add(double[] a, double[] b)
		{
			double[] ret = new double[a.Length];
			for (int i = 0; i < ret.Length; i++)
				ret[i] = a[i] + b[i];
			return ret;
		}

		#endregion

		public static double StandardDeviation(IList<double> doubleList, out double average, out double max, out double min)
		{
			average = doubleList.Average();
			double sumOfDerivation = 0;
			max = -1e9;
			min = 1e9;
			foreach (double value in doubleList)
			{
				max = Math.Max(max, value);
				min = Math.Min(min, value);
				sumOfDerivation += (value) * (value);
			}
			double sumOfDerivationAverage = sumOfDerivation / doubleList.Count;
			return Math.Sqrt(sumOfDerivationAverage - (average * average));
		}

          private static double[,] rref(double[,] matrix, out int[] index)
          {
               int[] inn = ArrangeRows(matrix);
               index = inn;
               int lead = 0, rowCount = matrix.GetLength(0), columnCount = matrix.GetLength(1);
               for (int r = 0; r < rowCount; r++)
               {
                    if (columnCount <= lead) break;
                    int i = r;
                    while (matrix[i, lead] == 0)
                    {
                         i++;
                         if (i == rowCount)
                         {
                              i = r;
                              lead++;
                              if (columnCount == lead)
                              {
                                   lead--;
                                   break;
                              }
                         }
                    }
                    for (int j = 0; j < columnCount; j++)
                    {
                         double temp = matrix[r, j];
                         matrix[r, j] = matrix[i, j];
                         matrix[i, j] = temp;
                    }
                    double div = matrix[r, lead];
                    for (int j = 0; j < columnCount; j++) matrix[r, j] /= div;
                    for (int j = 0; j < rowCount; j++)
                    {
                         if (j != r)
                         {
                              double sub = matrix[j, lead];
                              for (int k = 0; k < columnCount; k++) matrix[j, k] -= (sub * matrix[r, k]);
                         }
                    }
                    lead++;
               }
               
               return matrix;
          }
	}

}
