using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NsNodeView
{
	public static class Utilities
	{
		public static System.Drawing.Color GetColor(double max, double min, double val)
		{
			double del = (max - min) / 7;
			double[] nodes = new double[8];
			for (int i = 0; i < nodes.Length; i++)
				nodes[i] = ((7 - i) * min + i * max) / 7;

			int r, g, b;
			if (val >= nodes[7])
			{
				r = 255;
				g = 255;
				b = 255;
			}
			else if (val >= nodes[6])
			{
				r = 255;
				g = (int)(255 * (val - nodes[6]) / del);
				b = 255;
			}
			else if (val >= nodes[5])
			{
				r = 255;
				g = 0;
				b = (int)(255 * (val - nodes[5]) / del);
			}
			else if (val >= nodes[4])
			{
				r = 255;
				g = 255 - (int)(255 * (val - nodes[4]) / del);
				b = 0;
			}
			else if (val >= nodes[3])
			{
				r = (int)(255 * (val - nodes[3]) / del);
				g = 255;
				b = 0;
			}
			else if (val >= nodes[2])
			{
				r = 0;
				g = 255;
				b = 255 - (int)(255 * (val - nodes[2]) / del);
			}
			else if (val >= nodes[1])
			{
				r = 0;
				g = (int)(255 * (val - nodes[1]) / del);
				b = 255;
			}
			else if (val >= nodes[0])
			{
				r = 0;
				g = 0;
				b = (int)(255 * (val - nodes[0]) / del);
			}
			else
			{
				r = 0;
				g = 0;
				b = 0;
			}

			// clamps color values at 0-255

			LimitRange(0, ref r, 255);
			LimitRange(0, ref g, 255);
			LimitRange(0, ref b, 255);

			return System.Drawing.Color.FromArgb(r, g, b);
		}
		public static void LimitRange(int low, ref int val, int high)
		{
			if (val < low) val = low;
			if (high < val) val = high;
		}
	}
}
