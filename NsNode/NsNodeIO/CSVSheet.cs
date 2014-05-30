using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace NsNodes
{
	public class CSVSheet
	{
		public CSVSheet(int rows, int cols)
		{
			m_cells = new string[rows, cols];
		}
		string[,] m_cells;
		public int Rows { get { return m_cells.GetLength(0); } }
		public int Cols { get { return m_cells.GetLength(1); } }
		public string this[int iRow, int jCol] 
		{ 
			get { return m_cells[iRow, jCol]; }
			set { m_cells[iRow, jCol] = value; }
		}

		public bool SaveAs(ref string path)
		{
			path = Path.GetFullPath(path);
			string dir = Path.GetDirectoryName(path);
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			int i=0, j=0;
			using (StreamWriter sw = new StreamWriter(path))
			{
				for (i = 0; i < Rows; i++)
				{
					for (j = 0; j < Cols; j++)
					{
						sw.Write(this[i, j]);
						sw.Write(",");
					}
					sw.WriteLine();
				}
			}
			return i == Rows && j == Cols;
		}

		public static int XLSCol(string col)
		{
			col = col.ToUpper();
			if (col.Length == 1)
				return col[0] - 'A';
			if (col.Length == 2)
				return (col[0] - 'A') * 26 + (col[1] - 'A');
			else return -1;
		} 
	}
}
