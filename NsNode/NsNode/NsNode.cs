using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using NsNodes;

namespace NsNodes
{
	public class NsNode : IEnumerable<NsNode>, IEnumerator<NsNode>//, IList<NsNode>
	{
		#region Ctor

		private NsNode(String label, NsNode parent, List<NsNode> nodes, List<IAttribute> attributes)
		{
			m_parent = parent;
			m_nodes = nodes;
			m_attributes = attributes;
			Label = label;
		}
		private NsNode(String label, NsNode parent, List<NsNode> nodes)
			: this(label, parent, nodes, new List<IAttribute>())
		{ }
		private NsNode(String label, NsNode parent, List<IAttribute> attributes)
			: this(label, parent, new List<NsNode>(), attributes)
		{ }
		public NsNode(String label, NsNode parent)
			: this(label, parent, new List<NsNode>(), new List<IAttribute>())
		{ }
		public NsNode(String label)
			: this(label, null)
		{ }
		public NsNode()
			: this("", null)
		{ }

		public NsNode(NsNode parent, XmlNode xml)
			:this("NsNode", parent)
		{
			if (!FromXml(xml))
                    throw new AttributeXmlFormatException(null, xml, "Failed to read xml\n" + parent.Label);
		}
		public virtual bool FromXml(XmlNode xml)
		{
			foreach (XmlAttribute atr in xml.Attributes)
			{
				if (atr.Name == "Label")
				{
					Label = atr.Value;
					return true;
				}
			}
			return false;
		}
		public virtual XmlNode ToXml(XmlDocument doc)
		{
			return NsXmlHelper.MakeElement(doc, Type, Label);
		}

		private NsNode(NsNode copy)
			: this(copy.Label, copy.Parent, copy.Nodes, copy.Attributes)
		{
			throw new Exception("probably unsafe to copy nodes");
		}

		#endregion

		#region Parent
		NsNode m_parent;

		public NsNode Parent
		{
			get { return m_parent; }
            set { m_parent = value; }
		}

		public NsNode Root
		{
			get
			{
				NsNode node = this;
				while (node.Parent != null)
					node = node.Parent;
				return node;
			}
		}
		public int Depth
		{
			get
			{
				if (Parent == null)
					return 0;
				else
					return Parent.Depth + 1;
			}
		}
		public string Path
		{
			get
			{
				if (Parent == null)
					return Label + "\\";
				else
					return Parent.Path + Label + "\\";
			}
		}
		public bool IsRoot
		{
			get { return m_parent == null; }
		}

		#endregion

		#region Nodes

		List<NsNode> m_nodes;
		protected List<NsNode> Nodes
		{
			get { return m_nodes; }
		}
		public NsNode this[int index]
		{
			get
			{
				return Nodes[index];
			}
		}

		public int Count
		{
			get { return Nodes.Count; }
		}
		public int IndexOf(NsNode node)
		{
			int c = Nodes.IndexOf(node);
            //return 
			//foreach (NsNode n in Nodes)
			//{
			//	if (n == node)
			//		return c;
			//	c++;
			//}
			return c;
		}
		public NsNode FindNode(string label, bool recursive)
		{
			NsNode ret = FindNode(label);
			if( ret == null && recursive )
				foreach (NsNode n in Nodes)
				{
					ret = n.FindNode(label, recursive);
					if (ret != null)
						return ret;//return first find
				}
			return ret;
		}
		public NsNode FindNode(string label)
		{
			int i = label.IndexOf('\\');
			string test;
			if (i > 0)
				test = label.Substring(0, i);
			else
				test = label;

            foreach (NsNode n in Nodes)
                if (n.Label.Equals(test, StringComparison.InvariantCultureIgnoreCase))
                    if (i == -1 || i == label.Length - 1)//trailing 
                        return n;
                    else
                        return n.FindNode(label.Substring(i + 1));
            return null;
		}
		public NsNode FindAddNode(string label)
		{
			NsNode ret = FindNode(label);
			if (ret != null)
				return ret;

			int i = label.IndexOf('\\');
			string test;
			if (i > 0)
				test = label.Substring(0, i);
			else
				test = label;

			ret = FindNode(test);
			if (ret == null)
				ret = Add(test);

			if( -1 < i && i < label.Length ) //more subnodes to add
				ret = ret.FindAddNode(label.Substring(i + 1));

			return ret;
		}
		public NsNode FindParent(string parentlabel)
		{
			if (Parent == null || parentlabel == null)
				return null;
			if (Parent.Label.Equals(parentlabel, StringComparison.InvariantCultureIgnoreCase))
				return Parent;
			return Parent.FindParent(parentlabel);		
		}
		public bool HasParent(string parentlabel)
		{
			if (Parent == null || parentlabel == null)
				return false;
			if (Parent.Label == parentlabel)
				return true;
			return Parent.HasParent(parentlabel);
		}
		public bool HasChild(object target)
		{
			return HasChild(target, false);
		}
		public bool HasChild(object target, bool recursive)
		{
			//search attributes
			foreach (IAttribute atr in Attributes)
				if (atr == target)
					return true;
			//search nodes
			foreach (NsNode n in Nodes)
				if (n == target)
					return true;
				else if (recursive && n.HasChild(target, recursive))
					return true;
			//false if not found
			return false;
		}
		public NsNode Add(NsNode node)
		{
            if (node == this)
            {
                System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame(1, true);
                throw (new Exception("Cannot add a node to itself (" + stackFrame.GetMethod() + " ln: " + stackFrame.GetFileLineNumber() + ")"));
            }

			if (Nodes.Contains(node) || node == this)
				return null;

			node.m_parent = this;
			Nodes.Add(node);
			node.NodeUpdated += OnChildUpdated;//subscribe to childs notifications
			//node.ChildRemoved += OnChildRemoved;
			//if( NodeUpdated != null )
			//     foreach (Delegate ne in NodeUpdated.GetInvocationList())
			//     {
			//     if (ne.Target is INodeView)
			//         node.Attach(ne.Target as INodeView);
			//     //if( ne is NsNodeEventHandler )
			//     //    node.NodeUpdated += ne as NsNodeEventHandler;
			//     }

			//Update();
			return node;
		}

		//void OnChildRemoved(object sender, NodeEventArgs e)
		//{
		//    if (!bignorechildren && ChildRemoved != null)
		//        ChildRemoved(this, e);
		//}
		public NsNode Add(string newnode)
		{
			return Add(new NsNode(newnode));
		}

		//public event NsNodeEventHandler ChildRemoved;

		// Remove() has been replaced with Delete()
		//public bool Remove()
		//{
		//    if (IsRoot)
		//    {
		//        Clear();
		//        return true;
		//    }
		//    else
		//    {
		//        return Parent.Remove(this);
		//    }
		//}
		//public bool Remove(NsNode child)
		//{
		//    bool success = Nodes.Remove(child);
		//    child.Clear();
		//    if (ChildRemoved != null)
		//        ChildRemoved(this, new NodeEventArgs(child));
		//    return success;
		//}
		//void Clear()
		//{
		//    RemoveAll();
		//    m_parent = null;
		//    NodeUpdated = null;
		//    ChildRemoved = null;
		//}

		/// <summary>
		/// Removes all child nodes and attributes.
		/// After calling this method its recommended to call Update() to notify all views.
		/// </summary>
		public void RemoveAll()
		{
            //foreach (NsNode child in Nodes)
            Nodes.ForEach(delegate(NsNode child)
            {
                child.RemoveAll();
            });

			Attributes.Clear();
			Nodes.Clear();
		}

		/// <summary>
		/// Places a copy of this node and all subnodes into the specified parent
		/// </summary>
		/// <param name="newParent">The parent node to copy into</param>
		public virtual NsNode CopyTo(NsNode newParent) 
		{
			if (newParent == null)
				throw new NullReferenceException("cannot copyto null node");
			NsNode cpy = newParent.Add(new NsNode(m_label, newParent));
			//foreach(NsNode n in Nodes) 
            Nodes.ForEach(delegate(NsNode n){
                n.CopyTo(cpy);//copy children to this node's copy
            });
            Attributes.ForEach(delegate(IAttribute atr)
            {
                atr.CopyTo(cpy);//copy children to this node's copy
            });
			//foreach(IAttribute atr in Attributes)
			//	atr.CopyTo(cpy);//copy attributes to this node's copy
			return cpy;
		}

        ///// <summary>
        ///// return a copy of the current NsNode
        ///// </summary>
        ///// <returns> a copy of your desired node</returns>
        //public virtual NsNode Copy()
        //{
        //    NsNode cpy = new NsNode(m_label, null);
        //    foreach (NsNode n in Nodes)
        //        n.CopyTo(cpy);//copy children to this node's copy
        //    foreach (IAttribute atr in Attributes)
        //        atr.CopyTo(cpy);//copy attributes to this node's copy
        //    return cpy;
        //}

		/// <summary>
		/// Deletes this node and all children. 
		/// After calling this method its recommended to call Update() on it's parent to notify all views.
		/// </summary>
		public void Delete()
		{
			while (Nodes.Count > 0)
				Nodes[0].Delete();

			Attributes.Clear();
			if( Nodes.Count > 0 )
				Nodes.Clear(); //this should never get hit as the above calls to delete will remove all nodes

			if( Parent != null )
				Parent.Delete(this);

			Parent = null;
			//ChildRemoved = null;
			NodeUpdated = null;
		}

		void Delete(NsNode child)
		{
			Nodes.Remove(child);
		}

		#endregion

		#region Attributes
		List<IAttribute> m_attributes;
		public List<IAttribute> Attributes
		{
			get { return m_attributes; }
		}
		public IAttribute this[string attributename]
		{
			get
			{
				return FindAttribute(attributename, true);
				//IAttribute a = FindAttribute(attributename);
				//if (a == null)
				//{
				//    return null;
				//    throw new ArgumentOutOfRangeException("attributename", string.Format("Attribute[{0}] not found in Node[{1}]", attributename, Path));
				//}
				//return a.Value;
			}
			set
			{
				IAttribute a = FindAttribute(attributename);
				if (a == null)
					throw new ArgumentOutOfRangeException("attributename", string.Format("Attribute[{0}] not found in Node[{1}]", attributename, Path));
				a.Value = value.Value;
			}
		}

		public IAttribute Add(IAttribute attribute)
		{
               IAttribute atr = Attributes.Find((IAttribute atrib) => { return attribute.Label == atrib.Label; });
               if (atr != null)
               {
                    atr.Value = attribute.Value;
                    return atr;
               }
			//attribute.Parent = this;
			Attributes.Add(attribute);
			//Update();
			return attribute;
		}
		public bool Remove(IAttribute attribute)
		{
			bool ret = Attributes.Remove(attribute);
			if( ret )
				Update();
			return ret;
		}
		public int IndexOf(IAttribute atr)
		{
            return Attributes.IndexOf(atr);
            //int c = 0;
            //foreach (IAttribute a in Attributes)
            //{
            //    if (a == atr)
            //        return c;
            //    c++;
            //}
            //return -1;
		}
		public IAttribute FindAttribute(string label)
		{
			return FindAttribute(label, true);
		}
		public IAttribute FindAttribute(string label, bool ignorecase)
		{
			int i = label.LastIndexOf('\\');
			string test;
			NsNode node = this;
			if (i > 0)
			{
				test = label.Substring(0, i);//trim off the attribute name
				node = FindNode(test);
				if (node == null)
					return null;
				test = label.Substring(i + 1);//extract the attribute name
			}
			else
				test = label;

			StringComparison c = ignorecase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
			foreach (IAttribute atr in node.Attributes)
				if ( string.Equals( atr.Label, test, c ) )
					return atr;

			return null;
		}
		public IAttribute FindAttribute(string label, IAttribute def)
		{
			return FindAttribute(label, true, def);
		}
		public IAttribute FindAttribute(string label, bool ignorecase, IAttribute def)
		{
			IAttribute atr = FindAttribute(label, ignorecase);
			if (atr == null)
				return def;
			else 
				return atr;
		}
		public IAttribute FindInherited(string label)
		{
			return FindInherited(label, true);
		}
		public IAttribute FindInherited(string label, bool ignorecase)
		{
			IAttribute atr = FindAttribute(label, ignorecase);
			if (atr != null)
			return atr;

			if (Parent == null)
				return null;
			else return Parent.FindInherited(label, ignorecase);

		}
		public IAttribute FindAddAttribute(IAttribute target)
		{
			foreach (IAttribute atr in Attributes)
			{
				if (atr.Label == target.Label)
					return atr;
			}
			return Add(target);
		}
		/// <summary>
		/// Returns a refernce to the IAttribute.Value of the requested attribute
		/// </summary>
		/// <param name="attributename">the attribute to return the value of</param>
		/// <returns>the value of the attribute</returns>
		#endregion

		#region Properties

		public virtual string Type
		{
			get { return GetType().ToString(); }
		}
		public virtual string Label
		{
			get
			{
				if (m_label != null)
					return m_label;
				else if (Parent != null)
					return Parent.Label + "[" + Parent.IndexOf(this) + "]";
				else
					return "";
			}
			set
			{
				if (value == "")
					m_label = null;
				else
					m_label = value;
	
			}
		}
		string m_label;

		#endregion

		#region Update & OnDrag

		public virtual bool Attach(INodeView iview)
		{
			bool ret = true;

			//add delegates here
			NodeUpdated += iview.OnNodeUpdated;
			//ChildRemoved += iview.OnNodeRemoved;
			NodeSelected += iview.OnNodeSelected;
			//foreach (NsNode n in Nodes)
			//     ret &= n.Attach(iview);
			return ret;
		}
		public virtual bool Detach(INodeView iview)
		{
			bool ret = true;
			NodeUpdated -= iview.OnNodeUpdated;
			//ChildRemoved -= iview.OnNodeRemoved;
			NodeSelected -= iview.OnNodeSelected;

			//foreach (NsNode n in Nodes)
			//     ret &= n.Detach(iview);
			return ret;
		}

		NsNodeEventHandler NodeUpdated;
		NsNodeEventHandler NodeSelected;

		public virtual void Update(object sender)
		{
			if (m_bUpdate && NodeUpdated != null)
				NodeUpdated(sender, new NodeEventArgs(this));

		}
		public virtual void Update()
		{
			Update(this);
		}
		protected virtual void OnChildUpdated(object sender, NodeEventArgs e)
		{
			if ( !m_bIgnoreChildren && m_bUpdate && NodeUpdated != null)
				NodeUpdated(sender, e);
		}
		public virtual void OnNodeSelected(object sender, NodeEventArgs e)
		{
			if (NodeSelected != null)
				NodeSelected(sender, e);
		}

		/// <summary>
		/// Stops sending Update events
		/// </summary>
		public void PauseUpdating()
		{
			m_bUpdate = false;
		}
		/// <summary>
		/// Resumes sending Update events and sends one now
		/// </summary>
		public void ResumeUpdating()
		{
			ResumeUpdating(true);
		}
		/// <summary>
		/// Resumes sending Update events, optionally sends one now
		/// </summary>
		/// <param name="updatenow"></param>
		public void ResumeUpdating(bool updatenow)
		{
			m_bUpdate = true;
			if( updatenow )
				Update();
		}		
		bool m_bUpdate = true;

		public void IgnoreChildUpdates()
		{
			m_bIgnoreChildren = true;
		}
		public void ResumeChildUpdates()
		{
			ResumeChildUpdates(true);
		}
		public void ResumeChildUpdates(bool updatenow)
		{
			m_bIgnoreChildren = false;
			if (updatenow)
				Update();
		}
		bool m_bIgnoreChildren = false;

		public virtual void OnDrag(object sender, NodeTrackerEventArgs e)
		{
			foreach (IAttribute a in Attributes)
				if (a is IDragable)
					(a as IDragable).Drag(e.DeltaX, e.DeltaY, e.DeltaZ);
			foreach (NsNode n in Nodes)
				n.OnDrag(sender, e);
		}

		#endregion

		#region Query

		public bool SimpleQuery(string query, List<IAttribute> results, bool searchChildren)
		{
			foreach (IAttribute atr in Attributes)
			{
				if (atr.Query(query))
					results.Add(atr);
			}
			if (searchChildren)
				foreach (NsNode n in Nodes)
					n.SimpleQuery(query, results, searchChildren);
			return results.Count > 0;
		}

		//public bool Query(List<string> queries, ref List<IAttribute> results)
		//{
		//     // root\node\node\...\key
		//     // root\node\...\node\key=val
		//     //string[] queries = query.Split('\\');
		//     bool bexact = true;
		//     string query = queries[0];
		//     if (query.Contains('*'))//wildcard
		//     {
		//          query.Trim('*');//remove leading and trailing wildcards
		//          bexact = false;
		//     }
		//     bool success = true;
		//     if (queries.Count == 1)
		//     {
		//          QueryAttributes(query, ref results);
		//          foreach (NsNode n in Nodes)//pass it down
		//               success &= n.Query(queries, ref results);
		//          return success;
		//     }
		//     else if (bexact ? Label.Equals(query, StringComparison.InvariantCultureIgnoreCase) : Label.Contains(query)) //matched node
		//     {
		//          if (queries.Count == 2)// final node, search attributes
		//               success &= QueryAttributes(queries[1], ref results);
		//          else //mid node, pass search to children
		//          {
		//               queries.RemoveAt(0); //remove this
		//               success &= Query(queries, ref results);
		//          }
		//     }

		//     return false;
		//}
		//bool QueryAttributes(string query, ref List<IAttribute> results)
		//{
		//     char[] delims = { '=', ' ' };
		//     string[] target = query.Split(delims, StringSplitOptions.RemoveEmptyEntries); //only support equals for now
		//     if( target.Length != 2 )
		//          return false;
		//     bool bexact = true;
		//     if (target[0].Contains('*'))
		//     {
		//          target[0].Trim('*');//remove wildcards
		//          bexact = false;
		//     }

		//     IAttribute atr = null;
		//     int count = 0;
		//     if (bexact) //no wildcards
		//     {
		//          atr = FindAttribute(target[0]);
		//          if (atr != null )
		//          {
		//               results.Add(atr);
		//               return true;
		//          }
		//     }
		//     else //wildcards
		//     {
		//          foreach (IAttribute attr in Attributes)
		//          {
		//               if (attr.Label.Contains(target[0]))
		//               {
		//                    results.Add(attr);
		//                    count++;
		//               }
		//          }
		//          return count > 0;
		//     }
		//     return false;
		//}

		#endregion

		#region Editor

		//public virtual DialogResult ShowEditor()
		//{
		//     NsNodeEditor edit = new NsNodeEditor(this);
		//     DialogResult ret = edit.ShowDialog();
		//     edit.Dispose();
		//     return ret;
		//}

		#endregion

		#region ToString

		public override string ToString()
		{
			//System.Xml.XmlDocument doc = NodeIO.WriteXml(this, null);
			//return doc.OuterXml;
			return Label;
		}

		/// <summary>
		/// Returns a string representation of the NsNode
		/// </summary>
		/// <param name="format">A format specifier: 
		/// [id] will print the index of the node
		/// {numericformat} will format all attributes using the standard format specified
		/// _ will append each attribute's value to the label seperated by underscores</param>
		/// <param name="attrs">a list of indices of the attributes to include, leave blank to include all</param>
		/// <returns>the formatted string</returns>
		public virtual string ToString(string format, params int[] attrs)
		{
			StringBuilder sb = new StringBuilder();
			string numformat = "";
			if( format.StartsWith("[id]") )
			{
				if( Parent != null )
				sb.AppendFormat("[{0}]", Parent.IndexOf(this));
				else
					sb.Append("[-1]");
				format = format.Remove(0,4);//remove id string
			}
			if (format.StartsWith("{"))
			{
				int end = format.IndexOf("}", 1);
				if (end != -1)
					numformat = format.Substring(1, end-1);
				format = format.Substring(end+1);
			}
			if (format.StartsWith("_")) //underscore appended attribute values
			{
				foreach (IAttribute a in Attributes)
				{
					if (attrs.Length == 0 || attrs.Contains(IndexOf(a)))
					{
						if (sb.Length > 0) 
							sb.AppendFormat("_{0}", a.ToString(numformat));
						else
							sb.AppendFormat("{0}", a.ToString(numformat));
					}
				}
				format = format.Remove(0, 1);
			}

			return sb.ToString();
		}
		#endregion

		#region IEnumerator
		#region IEnumerator<NsNode> Members

		int Cnt = -1;

		public NsNode Current
		{
			get { return Nodes[Cnt]; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			//DO NOTHING HERE! or foreach will dispose your objects

			//foreach (NsNode n in Nodes)
			//     n.Dispose();
			//m_parent = null;
			//NodeUpdated = null;
			//Nodes.Clear();
			//Attributes.Clear();
		}

		#endregion

		#region IEnumerator Members

		object System.Collections.IEnumerator.Current
		{
			get { return Nodes[Cnt]; }
		}

		public bool MoveNext()
		{
			return (++Cnt < Nodes.Count);
		}

		public void Reset()
		{
			Cnt = -1;
		}

		#endregion

		#region IEnumerable<NsNode> Members

		public IEnumerator<NsNode> GetEnumerator()
		{
			Reset();
			return (IEnumerator<NsNode>)this;
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			Reset();
			return (System.Collections.IEnumerator)this;
		}

		#endregion

		#endregion

		public virtual void ChildrenLoaded()
		{
		}
	}
}
