using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NsNodes
{
	public interface INodeCtrl
	{
		event NsNodeEventHandler SelectionChanged;
		event NsNodeEventHandler NodeCopied;
	}
}
