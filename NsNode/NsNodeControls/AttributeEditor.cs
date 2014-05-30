using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NsNodes
{
	public partial class AttributeEditor : Form
	{
		public AttributeEditor(bool bnew)
		{
			InitializeComponent();
			keybox.Enabled = bnew;
			KeyLabel = "Key";
			ValueLabel = "Value";
		}
		public string Value
		{
			get { return valuebox.Text; }
			set { valuebox.Text = value; }
		}
		public string Key
		{
			get { return keybox.Text; }
			set { keybox.Text = value; }
		}
		public string ValueLabel
		{
			get { return valuebox.Label; }
			set { valuebox.Label = value; } 
		}
		public string KeyLabel
		{
			get { return keybox.Label; }
			set { keybox.Label = value; } 
		}
		public NsNodeControls.LabelTextbox KeyBox
		{
			get { return keybox; }
		}
		public NsNodeControls.LabelTextbox ValueBox
		{
			get { return valuebox; }
		}
	}
}
