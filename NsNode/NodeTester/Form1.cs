using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NsNodes;

namespace NodeTester
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			//m_node = NodeIO.ReadXml( Environment.GetFolderPath( Environment.SpecialFolder.Desktop) + "\\total.xml");
			m_node = NodeIO.ReadTxt(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\build.txt");
			
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "xml files (*.xml)|*.xml|txt files (*.txt)|*.txt|All files (*.*)|*.*";
			if (m_node == null && ofd.ShowDialog() == DialogResult.OK)
			{
				string path = ofd.FileName;
				if (System.IO.Path.GetExtension(path) == ".txt")
					m_node = NodeIO.ReadTxt(path);
				else
					m_node = NodeIO.ReadXml(path);
			}

			if (m_node != null)
			{
				m_node.Attach(nsNodeTree1);
				m_node.Update();
			}

			queryView1.Attach(nsNodeTree1);
			//nsNodeView1.Attach(nsNodeTree1);
			BringToFront();
		}
		NsNode m_node;
	}
}
