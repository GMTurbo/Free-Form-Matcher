using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;
using NsNodes;
//using NLog;

namespace NsNodes
{
	/// <summary>
	/// static class for loading and saving NsNode in xml format
	/// </summary>
	public static class NodeIO
	{

#if !DEBUG
           static bool log = false;
#else
          static bool log = true;
#endif

          #region Logging

          private static void Log(string message)
          {        
               Log(message, NsNodes.LogPriority.Debug);
          }

          private static void Log(string message, NsNodes.LogPriority p)
          {
               if (log)
                    NsNodes.LumberJackSingleton.Instance.Log("NsNodeIO: " + message, p);
          }

          #endregion

          #region Write

          /// <summary>
		/// Store a node to an xml file
		/// </summary>
		/// <param name="root">the node to save</param>
		/// <param name="path">path to save to, will overwrite, if null: doesn't save the xml</param>
		/// <returns>the XmlDocument created by the process</returns>
		public static XmlDocument WriteXml(NsNode root, string path)
		{
			XmlDocument doc = new XmlDocument();

			//necessary
			XmlNode node = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");

			//timestamp document
			//string date = DateTime.Now.ToString("f");
			//XmlAttribute dn = doc.CreateAttribute("DATE");
			//dn.Value = date;
			//node.Attributes.Append(dn);
			//add declaration node
			doc.AppendChild(node);
			node = doc.CreateComment(DateTime.Now.ToString("F"));
			doc.AppendChild(node);
			//create root node(thus recursively create entire tree)
			node = CreateXml(root, doc);
			//add root node
			doc.AppendChild(node);

			//save it to a file if specified
			if (path != null)
			{
				string dir = Path.GetDirectoryName(path);
				if (dir.Length > 0 && !Directory.Exists(dir))
					Directory.CreateDirectory(dir);
				doc.Save(path);
			}

			return doc;
		}
		static XmlNode CreateXml(NsNode node, XmlDocument doc)
		{
			XmlNode xn = node.ToXml(doc);
			//XmlNode xn = NsXmlHelper.MakeElement(doc, node.Type, node.Label);

			// add each attribute
			XmlElement xe;
			foreach (IAttribute attr in node.Attributes)
			{
				xe = attr.ToXml(doc);
				xn.AppendChild(xe);
				//xa = doc.CreateAttribute(attr.Type, attr.Label, "");
				//xa.Value = attr.Value.ToString();
				//xn.Attributes.Append(xa);
			}
			//recursively add the children
			foreach (NsNode nn in node)
				xn.AppendChild(CreateXml(nn, doc));
			return xn;
		}

		public static void WriteSettingsXfg(NsNode settings)
		{
			WriteXml(settings, SettingsXfg);
		}

		#endregion

		#region Read

		/// <summary>
		/// creats an NsNode from an xml file
		/// </summary>
		/// <param name="path">path to the xml file</param>
		/// <returns>a new NsNode created from the xml, or null if failed </returns>
		public static NsNode ReadXml(string path)
		{
               //logger.Trace("Begin reading path: " + path);
			return ReadXml(path, null);
		}
		/// <summary>
		/// creates an NsNode from an xml file
		/// </summary>
		/// <param name="path">path to the xml file</param>
		/// <param name="parent">the parent node for the newly constructed node, can be null for root nodes</param>
		/// <returns>The new NsNode</returns>
		public static NsNode ReadXml(string path, NsNode parent)
		{
			XmlDocument doc = new XmlDocument();
			try
			{
                    Log("loading path: " + path);
				doc.Load(path);
                    Log("loading done");
			}
			catch
			{
				return null;
			}
			return ReadXml(doc, parent);

		}
		/// <summary>
		/// creates an NsNode from an xml file stream
		/// </summary>
		/// <param name="xml">an stream of xml data</param>
		/// <param name="parent">the parent node for the newly constructed node, can be null for root nodes</param>
		/// <returns>The new NsNode</returns>
		public static NsNode ReadXml(Stream xml, NsNode parent)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(xml);
			return ReadXml(doc, parent);
		}
		/// <summary>
		/// creates an NsNode from a XmlDocument object
		/// </summary>
		/// <param name="doc">the xml object</param>
		/// <param name="parent">the parent node for the newly constructed node, can be null for root nodes</param>
		/// <returns>the new NsNode</returns>
		public static NsNode ReadXml(XmlDocument doc, NsNode parent)
		{
               Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();

               Log("Loaded Assemblies:{");
               foreach (Assembly asm in asms)
                    Log("\t " + asm.FullName);
               Log("}");

               List<Type> atrs = GetAllOf(typeof(IAttribute));

               Log("Loaded Types:{");
               foreach (Type asm in atrs)
                    Log("\t " + asm.FullName);
               Log("}");

			foreach (XmlNode xn in doc.ChildNodes)
				if (!(xn is XmlDeclaration) && !(xn is XmlComment))
					return CreateNsNode(xn, parent);
			return null;
		}

		static NsNode CreateNsNode(XmlNode xn, NsNode parent)
		{
			object o = CreateInstance(xn, parent);
			NsNode node = o as NsNode;
			if (node == null)
				return null;
			//create and add child nodes and attributes
			if (!AddSubNodes(xn, node))
				throw new AttributeXmlFormatException(null, xn, "Invalid node or attribute type" + node.Label);

			return node;
		}

		static bool AddSubNodes(XmlNode xn, NsNode node)
		{
			object o = null;
			bool success = true;
			foreach (XmlNode xc in xn.ChildNodes)
			{
				if (xc.Name.ToLower().Contains("transformer"))
					continue;
				o = CreateInstance(xc, node);
				if (o is NsNode)
				{
                         Log(string.Format("NsNode created: {0}", o.GetType().ToString()));
					NsNode c = o as NsNode;
					success &= AddSubNodes(xc, c);
					node.Add(c);
				}
                    else if (o is IAttribute)
                    {
                         Log(string.Format("IAttribute created: {0}", o.GetType().ToString()));
                         node.Add(o as IAttribute);
                    }
                    else
                    {
                         Log(string.Format("Garbage created: {0}", o.GetType().ToString()));
                         success = false;
                    }
			}
			node.ChildrenLoaded();
			return success;
		}

		static object CreateInstance(XmlNode xn, NsNode parent)
		{
			string stype = xn.Name;
			object o = null;
			Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly asm in asms)
			{
				try
				{
                         //Log("Trying to Match Instance of type:" + asm.FullName + " with " + xn.Name);
					o = asm.CreateInstance(stype, false, BindingFlags.CreateInstance, null, new object[] { parent, xn }, null, null);
					if (o != null)
						return o;
				}
				catch (Exception e)
				{
                         Log("Creating Instance of type:" + xn.Name + " failed\n" + e.Message, LogPriority.Error);
				}
			}
			return null;
		} 

		#endregion

		#region Text files

		public static NsNode ReadTxt(string path)
		{
			if (!File.Exists(path))
				return null;
			NsNode root = new NsNode(Path.GetFileName(path));
			using (StreamReader sr = new StreamReader(path))
			{
				string line = sr.ReadLine();
				if (line == null)
					return root;
				while (line != null && line.Length == 0)
					line = sr.ReadLine();

				NsNode node = MakeNode(root, line);

				while ((line = sr.ReadLine()) != null)
				{
					if (line.Length == 0 || line == "\t" )
						continue;
					node = MakeNode(node, line);
				}
			}
			return root;
		}
		static NsNode MakeNode(NsNode node, string line)
		{

			int i = 0;
			while (i < line.Length && line[i] == '\t')
				i++;

			while (node.Depth > i)
			{
				if (node.Parent == null)
					return null;
				node = node.Parent;
			}
			node.Add(new NsNode(line.Substring(i)));
			node = node.Last();
			return node;
		}

		#endregion

		#region Config files
		/// <summary>
		/// "Settings.xfg" will default to the exe directory
		/// </summary>
		public readonly static string SettingsXfg = Path.Combine(ExeDir, "Settings.xfg");
		/// <summary>
		/// returns the exe directory
		/// </summary>
		public static string ExeDir
		{
			get 
			{
				Assembly a = Assembly.GetEntryAssembly();
				if (a == null)
					return "C:\\";
				return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); 
			}
			//get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
		}
		/// <summary>
		/// reads the default settings xfg file
		/// </summary>
		/// <returns>the root settings node, null on failure</returns>
		public static NsNode ReadSettingsXfg()
		{
			string path = SettingsXfg;
			return NodeIO.ReadConfigXml(ref path);
		}
		/// <summary>
		/// Reads an NsNode xml file given either a local path or full path
		/// </summary>
		/// <param name="path"> in: A local path relative to the exe directory or a full path.
		/// out: the full path read</param>
		/// <returns>a NsNode loaded from the file, null on failure</returns>
		public static NsNode ReadConfigXml(ref string path)
		{
			if (!Path.IsPathRooted(path))
				//path = Path.Combine(Path.Combine(ExeDir, "Config"), path);
				path = Path.Combine(ExeDir, path);

			return ReadXml(path);
		}
		/// <summary>
		/// Reads an NsNode tabbed-txt file given either a local path or full path
		/// </summary>
		/// <param name="path"> in: A local path relative to the exe directory or a full path.
		/// out: the full path read</param>
		/// <returns>a NsNode loaded from the file, null on failure</returns>
		public static NsNode ReadConfigTxt(ref string path)
		{
			if (!Path.IsPathRooted(path))
				//path = Path.Combine(Path.Combine(ExeDir, "Config"), path);
				path = Path.Combine(ExeDir, path);

			return ReadTxt(path);
		}

		#endregion

		public static List<Type> GetAllAttributeTypes()
		{
               List<Type> atrs = GetAllOf(typeof(IAttribute));

               Log("Loaded Types:{");
               foreach (Type asm in atrs)
                    Log("\t " + asm.FullName);
               Log("}");

			return atrs;
		}

		public static List<Type> GetAllOf(Type baseType)
		{
			List<Type> atrs = new List<Type>();
			Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
               IEnumerable<Type> ret;
               foreach (Assembly asm in asms)
               {
                    ret = FindDerivedTypes(asm, baseType);
                    if (ret != null)
                         atrs.AddRange(ret);
               }
			return atrs;
		}
		public static IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType)
		{
               IEnumerable<Type> ret=null;
               try{
                    ret = assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t));
               }
               catch (Exception ex)
               {
                    string message = ex.Message;
               }
               return ret;
		}

		public static void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
		{
			if (!destination.Exists)
			{
				destination.Create();
			}

			List<string> failed = new List<string>();
			// Copy all files.
			FileInfo[] files = source.GetFiles();
			foreach (FileInfo file in files)
			{
				try
				{
					file.CopyTo(Path.Combine(destination.FullName,
						file.Name));
				}
				catch { }

			}

			// Process subdirectories.
			DirectoryInfo[] dirs = source.GetDirectories();
			foreach (DirectoryInfo dir in dirs)
			{
				// Get destination directory.
				string destinationDir = Path.Combine(destination.FullName, dir.Name);

				// Call CopyDirectory() recursively.
				CopyDirectory(dir, new DirectoryInfo(destinationDir));
			}
		}

		#region Old
		//static NsNode CreateNsNode(XmlNode xn, NsNode parent)
		//{
		//     //read off label and other attributes
		//     string label = "NsNode";
		//     //create the node
		//     NsNode node = new NsNode(label, parent);
		//     foreach (XmlAttribute atr in xn.Attributes)
		//     {
		//          if (atr.Name == "Label")
		//               node.Label = atr.Value;
		//     }
		//     //create and add child nodes and attributes
		//     //Type T = null;
		//     foreach (XmlNode xc in xn.ChildNodes)
		//     {
		//          if (IsNsNode(xc))
		//               node.Add(CreateNsNode(xc, node));
		//          else //if ( (T = IsIAttribute(xc)) != null )
		//          {
		//               if (xc.Name == DateAttribute.Typeof)
		//                    node.Attributes.Add(new DateAttribute(node, xc));
		//               else if (xc.Name == StringAttribute.Typeof)
		//                    node.Attributes.Add(new StringAttribute(node, xc));
		//               else if (xc.Name == MACAttribute.Typeof)
		//                    node.Attributes.Add(new MACAttribute(node, xc));
		//               else if (xc.Name == IntAttribute.Typeof)
		//                    node.Attributes.Add(new IntAttribute(node, xc));
		//               else if (xc.Name == DoubleAttribute.Typeof)
		//                    node.Attributes.Add(new DoubleAttribute(node, xc));
		//               else if (xc.Name == PolylineAttribute.Typeof)
		//                    node.Attributes.Add(new PolylineAttribute(node, xc));
		//               else if (xc.Name == TapeRefAttribute.Typeof)
		//                    node.Attributes.Add(new TapeRefAttribute(node, xc));
		//               else if (xc.Name == TapeAttribute.Typeof)
		//                    node.Attributes.Add(new TapeAttribute(node, xc));
		//               else if (xc.Name == MarkAttribute.Typeof)
		//                    node.Attributes.Add(new MarkAttribute(node, xc));
		//               else if (xc.Name == PlyAttribute.Typeof)
		//                    node.Attributes.Add(new PlyAttribute(node, xc));
		//               else
		//                    throw new AttributeXmlFormatException(null, xc, "Invalid attribute type");
		//          }
		//     }
		//     return node;
		//}
		//static bool IsNsNode(XmlNode xn)
		//{
		//     //return xn.Name == new NsNode().GetType().ToString();
		//     string dir = System.Windows.Forms.Application.ExecutablePath;
		//     dir = Path.GetDirectoryName(dir);
		//     Assembly asm;
		//     foreach(string path in Directory.GetFiles(dir, "*.dll") )
		//     {
		//          try
		//          {
		//               asm = Assembly.ReflectionOnlyLoadFrom(path);
		//          }
		//          catch
		//          {
		//               asm = null;
		//          }
		//          if (asm != null)
		//          {
		//               foreach (Type T in asm.GetExportedTypes())
		//                    if (T.FullName == xn.Name)
		//                    {
		//                         // test if it derives from node
		//                         return T.IsSubclassOf(new NsNode().GetType());
		//                    }
		//          }
		//     }
		//     return false;
		//}
		//static Type IsIAttribute(XmlNode xn)
		//{
		//     foreach (Type T in System.Reflection.Assembly.GetExecutingAssembly().GetExportedTypes())
		//          if (T.FullName == xn.Name)
		//          {
		//               // ensure it is IAttribute
		//               Type[] iface = T.GetInterfaces();
		//               foreach (Type t in iface)
		//                    if (t.FullName == "NsNodes.IAttribute")
		//                         return T;
		//          }
		//     return null;
		//}

		#endregion

	}
}
