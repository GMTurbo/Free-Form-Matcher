using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NsNodeControls
{
	class AttributeCheckBox : CheckBox, IAttributeEditor
	{

		#region IAttributeEditor Members

		[System.ComponentModel.Browsable(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public object AttributeValue
		{
			get
			{
				return Checked;
			}
			set
			{
				Checked = (bool)value;
			}
		}

		#endregion
	}
}
