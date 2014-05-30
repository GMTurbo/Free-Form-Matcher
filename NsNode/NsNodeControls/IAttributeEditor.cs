using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NsNodeControls
{
	public interface IAttributeEditor
	{
		[System.ComponentModel.Browsable(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		object AttributeValue { get; set; }
	}
}
