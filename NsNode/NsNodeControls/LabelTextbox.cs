using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NsNodes;

namespace NsNodeControls
{
	public partial class LabelTextbox : UserControl, IAttributeEditor
	{
		public LabelTextbox()
		{
			InitializeComponent();
		}

		[Category("Appearance"), BrowsableAttribute(true)]
		public override string Text
		{
			get
			{
				return textBox1.Text;
			}
			set
			{
				textBox1.Text = Numeric ? double.Parse(value).ToString(): value;
			}
		}
		[Category("Appearance")]
		public string Label
		{
			get { return label1.Text; }
			set { label1.Text = value; }
		}
		[Category("Behavior")]
		public bool ReadOnly
		{
			get { return textBox1.ReadOnly; }
			set { textBox1.ReadOnly = value; }
		}
		[Category("Behavior"), Description("True for a numeric textbox")]
		public bool Numeric
		{
			get { return m_numeric; }
			set
			{
				m_numeric = value;
				textBox1.TextAlign = value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
			}
		}

		protected override void OnGotFocus(EventArgs e)
		{
			textBox1.Focus();
		}

		private void label1_Click(object sender, EventArgs e)
		{
			textBox1.Focus();
		}
		bool m_numeric = false;

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (Numeric)
			{
				System.Globalization.NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
				string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
				string groupSeparator = numberFormatInfo.NumberGroupSeparator;
				string negativeSign = numberFormatInfo.NegativeSign;

				string keyInput = e.KeyChar.ToString();

				if (Char.IsDigit(e.KeyChar))
				{
					// Digits are OK
				}
				else if (keyInput.Equals(decimalSeparator) || keyInput.Equals(groupSeparator) ||
				 keyInput.Equals(negativeSign))
				{
					// Decimal separator is OK
				}
				else if (e.KeyChar == '\b')
				{
					// Backspace key is OK
				}
				//    else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
				//    {
				//     // Let the edit control handle control and alt key combinations
				//    }
				else
				{
					// Consume this invalid key and beep
					e.Handled = true;
					//    MessageBeep();
				}
			}
		}

		#region IAttributeEditor Members
		[System.ComponentModel.Browsable(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public object AttributeValue
		{
			get
			{
				//if (Numeric)
				//     return double.Parse(Text);
				//else
					return Text;
			}
			set
			{
				Text = value.ToString();
			}
		}

		#endregion
	}
}
