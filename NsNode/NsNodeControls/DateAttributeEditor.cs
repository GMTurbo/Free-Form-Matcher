using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NsNodeControls
{
	internal partial class DateAttributeEditor : UserControl , IAttributeEditor
	{
		public DateAttributeEditor()
		{
			InitializeComponent();
		}

		public override string Text
		{
			get
			{
				return label1.Text;
			}
			set
			{
				label1.Text = value;
			}
		}

		public DateTimePicker Picker
		{
			get { return dateTimePicker1; }
		}

		#region IAttributeEditor Members
		[System.ComponentModel.Browsable(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public object AttributeValue
		{
			get
			{
				return dateTimePicker1.Value;
			}
			set
			{
				//bounded to max/min, defaults to Today if oob
				dateTimePicker1.Value = (DateTime)value < dateTimePicker1.MinDate || dateTimePicker1.MaxDate < (DateTime)value ? DateTime.Today : (DateTime)value ;
			}
		}

		#endregion

	}
}
