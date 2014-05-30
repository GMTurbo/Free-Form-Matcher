using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using devDept;
using devDept.Eyeshot;
using devDept.Geometry;
using devDept.Eyeshot.Entities;
using System.Xml;
using NsNodes;
using System.IO;

namespace NsFileGen
{
	class YarnDoc
	{
		public YarnDoc(string label, NsNode parent)
		{
			if (Path.GetExtension(label) != string.Empty)
			{
				m_doc = new NsNode(Path.GetFileName(label), parent);
				Open3dl(label);
			}
			else
				m_doc = new NsNode(label, parent);
		}

		public void Open3dl(string path3dl)
		{
			Point2D[][] uvs;
			Point3D[][] xyzs;
			double[][] dens;
			int yarns = LayoutIO.Read3DLFile(path3dl, out xyzs, out uvs, out dens);
			if (yarns <= 0)
				throw new Exception("Invalid 3dl file");
			
			string filename = Path.GetFileName(path3dl);

			for (int i = 0; i < yarns; i++)
			{
				m_doc.Add(new PassNode(m_doc, xyzs[i]));
			}
		}

		NsNode m_doc;
		public NsNode Node
		{
			get { return m_doc; }
		}
	}
}
