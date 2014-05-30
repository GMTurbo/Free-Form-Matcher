using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NsNodes;

namespace NsFileGen
{
	public partial class ContourMaker : Form
	{
		public ContourMaker()
		{
			InitializeComponent();
			nsNodeView1.Attach(nsNodeTree1);
			nsNodeView1.Viewtype = devDept.Eyeshot.viewType.Top;

		}
		private void addcontour_Click(object sender, EventArgs e)
		{
			ContourTracker ct = new ContourTracker();
			//ct.Attach(nsNodeTree1);
			//ct.Attach(nsNodeView1 as NsNodes.INodeView);
			ct.Attach(nsNodeView1 as ITracker);

			ct.Node.Attach(nsNodeTree1);
			ct.Node.Attach(nsNodeView1);
			//ct.Node.Update();
			//ContourNode cn = new ContourNode(m_node, "Contour", nsNodeView1);
			//cn.Attach(nsNodeTree1);
//			cn.Attach(nsNodeView1 as NsNodes.INodeView);
			//cn.Update();
		}
		private void addrosette_Click(object sender, EventArgs e)
		{
			Point2dTracker pt = new Point2dTracker("Rosettes");
			pt.Attach(nsNodeView1 as ITracker);
			pt.Node.Attach(nsNodeTree1);
			pt.Node.Attach(nsNodeView1);

		}

		private void tape_Click(object sender, EventArgs e)
		{
			NsNode node = nsNodeTree1.SelectedNode;
			if (node == null)
			{
				MessageBox.Show("Please select a node to tape");
				return;
			}

			int nply = -1;
			if (ply.Text != "")
				nply = int.Parse(ply.Text);
			if (nply < 0)
			{
				MessageBox.Show("Must enter positive ply id");
				return;
			}

			double width = 0;
			if (tapewidth.Text != "")
				width = double.Parse(tapewidth.Text);
			if (width <= 0)
			{
				MessageBox.Show("Must enter a positive width");
				return;
			}

			double den = 0;
			if (tapedensity.Text != "")
				den = double.Parse(tapedensity.Text);
			if (den <= 0)
			{
				MessageBox.Show("Must enter a positive density");
				return;
			}


			if (node.Root.Label == "Contour")
				TapeContour(node, nply, den, width);
			else if (node.Root.Label == "Rosettes")
				TapeRosette(node, nply, den, width);

		}

		void TapeContour(NsNode node, int ply, double den, double width)
		{

			double[] dir = direction.Point;
			if (Math.Abs(dir[0]) < 1e-5 && Math.Abs(dir[1]) < 1e-5)
			{
				MessageBox.Show("Must specify a taping direction");
				return;
			}


			double[] ofs = offset.Point;

			List<IAttribute> lines = new List<IAttribute>(1);
			List<devDept.Geometry.Point3D> pnts = null;
			if (node.SimpleQuery("PolylineAttribute=*", lines, true))
			{
				pnts = (List<devDept.Geometry.Point3D>)lines[0].Value;
			}
			if (pnts != null && pnts.Count > 2)
				CreateContour(den, width, dir, ofs, pnts);

		}
		void CreateContour(double den, double width, double[] dir, double[] ofs, List<devDept.Geometry.Point3D> pnts)
		{

		}

		void TapeRosette(NsNode node, int ply, double den, double width)
		{

			double rad = 0;
			if (radius.Text != "")
				rad = double.Parse(radius.Text);
			if (rad <= 0)
			{
				MessageBox.Show("Must enter a positive radius");
				return;
			}


			double ntapes  = 0;
			
			if ( numtapes.Text != "")
				ntapes = double.Parse(numtapes.Text);
			if (ntapes <= 0)
			{
				MessageBox.Show("Must enter a positive number of tapes");
				return;
			}


			double[] ang = angles.Point;
			if (Math.Abs(ang[0]) < 1e-5 && Math.Abs(ang[1]) < 1e-5)
			{
				MessageBox.Show("Both angles cannot be 0");
				return;
			}


			List<IAttribute> lines = new List<IAttribute>(3);
			List<double[]> pnts = null;
			PointAttribute pnt;
			if (node.SimpleQuery("PointAttribute=*", lines, true))
			{
				pnts = new List<double[]>(lines.Count);
				foreach (IAttribute atr in lines)
				{
					if (atr is PointAttribute)
					{
						pnt = atr as PointAttribute;
						pnts.Add((double[])pnt.Value);
					}
				}
			}
			if (pnts != null && pnts.Count >= 1)
				CreateRosettes(pnts, ply, den, width, rad, ntapes, ang[0], ang[1]);


		}
		void CreateRosettes(List<double[]> pnts, int nply, double den, double width, double radius, double ntapes, double start, double end)
		{

		}
	}
}
