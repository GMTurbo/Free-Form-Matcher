using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NsNodes;
using System.IO;
using System.Reflection;

namespace NsNodes
{
	public static class NodeCSV
	{
		public static NsNode ReadConfigCsv(ref string path)
		{
			return ReadConfigCsv(ref path, false);
		}
		public static NsNode ReadConfigCsv(ref string path, bool bParse)
		{
			if (!Path.IsPathRooted(path))
				//path = Path.Combine(Path.Combine(ExeDir, "Config"), path);
				path = Path.Combine(NodeIO.ExeDir, path);

			return ReadCSV(path, bParse);
		}

		public static NsNode ReadCSV(string pathname)
		{
			return ReadCSV(pathname, false);
		}
		public static NsNode ReadCSV(string pathname, bool bParse)
		{
			if (!File.Exists(pathname))
				return null;

			NsNode root = new NsNode(pathname);
			try
			{
				using (StreamReader sr = new StreamReader(pathname))
				{
					string line = null;
					string[] headers;
					int nheader = 0;
					double d;
					string[] cols;
					NsNode node;
					line = sr.ReadLine();
					headers = line.Split(',');
					while ((line = sr.ReadLine()) != null)
					{
						cols = line.Split(',');
						node = new NsNode();
						nheader = 0;
						foreach (string s in cols)
						{
							if (bParse)
							{
								try
								{
									d = Convert.ToDouble(s);
									if (nheader < headers.Length)
										node.Add(new DoubleAttribute(node, headers[nheader++], d));
									else
										node.Add(new DoubleAttribute(node, "", d));
								}
								catch
								{
									if (nheader < headers.Length)
										node.Add(new StringAttribute(node, headers[nheader++], s));
									else
										node.Add(new StringAttribute(node, "", s));
								}
							}
							else
							{
								if (nheader < headers.Length)
									node.Add(new StringAttribute(node, headers[nheader++], s));
								else
									node.Add(new StringAttribute(node, "", s));
							}
						}
						root.Add(node);
					}
				}
			}
			catch
			{
				return null;
			}
			return root;
		}
	}
}
