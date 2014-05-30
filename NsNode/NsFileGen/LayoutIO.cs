using System;
using System.Collections.Generic;
using System.Text;
using NsNodes;
using devDept;
using devDept.Eyeshot;
using devDept.Geometry;
using devDept.Eyeshot.Entities;
using System.Xml;
using System.IO;

namespace NsFileGen
{
	static class LayoutIO
	{
		public static int Read3DLFile(string Path, out Point3D[][] xyzs, out Point2D[][] uvs, out double[][] densities)
		{
			List<Point3D[]> xvertices = new List<Point3D[]>(100);
			List<Point2D[]> uvertices = new List<Point2D[]>(100);
			List<double[]> dens = new List<double[]>(100);

			if (!File.Exists(Path))
			{
				xyzs = null;
				uvs = null;
				densities = null;
				return -1; //file not found
			}
			bool bdens = System.IO.Path.GetExtension(Path).EndsWith(".3dld", StringComparison.CurrentCultureIgnoreCase);
			using (StreamReader sr = new StreamReader(Path))
			{
				string line;
				line = sr.ReadLine(); //read header
				Point3D[] pnts;
				Point2D [] us;
				double[] d;
				while (ReadPass(sr, out pnts, out us, out d, bdens) > 0)
				{
					xvertices.Add(pnts);
					uvertices.Add(us);
					dens.Add(d);
					pnts = null;
					us = null;
					d = null;
				}
			}
			xyzs = xvertices.ToArray();
			uvs = uvertices.ToArray();
			densities = dens.ToArray();
			return dens.Count;
		}
		static int  ReadPass(StreamReader sr, out Point3D[] xyzs, out Point2D[] uvs, out double[] densities, bool bReadDens)
		{
			List<Point3D> xvertices = new List<Point3D>(100);
			List<Point2D> uvertices = new List<Point2D>(100);
			List<double> dens = new List<double>(100);

			string line = sr.ReadLine();//read header
			int count = 0; //get the point count
			try
			{
				count = Convert.ToInt32(line.Substring(51, 4));
			}
			catch
			{
				count = -1;
			}
			while (count-- >= 0 && (line = sr.ReadLine()) != null)
			{
				if (line[1] == ' ')
					break;
				if (line.StartsWith("<PAUS>"))
					break;

				Point2D uv = new Point2D(0, 0);
				Point3D pnt = new Point3D(0, 0, 0);
				double d = 0;
				try
				{
					uv.X = Convert.ToDouble(line.Substring(0, 8));
					uv.Y = Convert.ToDouble(line.Substring(8, 8));

					pnt.X = Convert.ToDouble(line.Substring(21, 10));
					pnt.Y = Convert.ToDouble(line.Substring(31, 10));
					pnt.Z = Convert.ToDouble(line.Substring(41, 10));

					if (bReadDens)
					{
						d = Convert.ToDouble(line.Substring(91));
						dens.Add(d);
					}

					xvertices.Add(pnt);
					uvertices.Add(uv);
				}
				catch { }
			}
			System.Diagnostics.Debug.Assert(count < 0);
			System.Diagnostics.Debug.Assert( xvertices.Count == uvertices.Count);
			xyzs = xvertices.ToArray();
			uvs = uvertices.ToArray();
			densities = dens.ToArray();
			return uvertices.Count;
		}
	}
}
