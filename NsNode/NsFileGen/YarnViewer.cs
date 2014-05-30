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
	public partial class YarnViewer : Form
	{
		//NsNode m_docs = new NsNode("YarnViewer", null);
		public YarnViewer()
		{
			InitializeComponent();
			nsNodeView1.Attach(nsNodeTree1);
			//nsNodeTree1.Attach(nsNodeView1);
			//m_docs.Attach(nsNodeTree1);
			//m_docs.Attach(nsNodeView1);
		}

		private void nsNodeView1_DragEnter(object sender, DragEventArgs e)
		{
			// make sure they're actually dropping files (not text or anything else)
			if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
				e.Effect = DragDropEffects.Copy;// allow them to continue(without this, the cursor stays a "NO" symbol)

		}

		private void nsNodeView1_DragDrop(object sender, DragEventArgs e)
		{
			string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (string s in filenames)
			{
				string path = s.ToLower();
				if (path.EndsWith(".3dl") || path.EndsWith(".3dld")) 
				{
					YarnDoc d = new YarnDoc(path, null);
					d.Node.Attach(nsNodeView1);
					d.Node.Attach(nsNodeTree1);
					d.Node.Update();
					//m_docs.Add(d.Node);
				}
			}
			//m_docs.Update();
		}
	}
}
