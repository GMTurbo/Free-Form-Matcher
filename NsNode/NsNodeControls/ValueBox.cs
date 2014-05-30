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
	public partial class ValueBox : UserControl, IAttributeEditor
	{
		public ValueBox()
		{
			InitializeComponent();
		}

		[Category("Appearance"), Browsable(true), DefaultValue("label")]
		public override string Text
		{
			get { return label.Text; }
			set { label.Text = value; }
		}

		[Category("Appearance"), Browsable(true), DefaultValue("units")]
		public string Units
		{
			get { return units.Text; }
			set { units.Text = value; }
		}

		[Category("Data"), Browsable(true), DefaultValue(0)]
		public double Value
		{
			get {
				try
				{
					return double.Parse(valueText.Text);
				}
				catch
				{
					return double.MinValue;
				}
			}
			set
			{
				try
				{
					valueText.Text = value.ToString(m_format);
					if (ValueChanged != null)
						ValueChanged(this, new EventArgs());
				}
				catch( FormatException e )
				{
					m_format = e.Message;
					m_format = "g";
					Value = value;
				}
				catch
				{
					m_format = "g";
					valueText.Text = "";
				}
			}
		}
		public event EventHandler ValueChanged;
		string m_format = "g";
		public string Format
		{
			get { return m_format; }
			set
			{
				m_format = value;
				Value = Value;
			}
		}

		[Category("Behavior"), Browsable(true), DefaultValue(false)]
		public bool ReadOnly
		{
			get { return valueText.ReadOnly; }
			set { valueText.ReadOnly = value; }
		}

		int m_tbwidth =0;

		[Category("Appearance"), Browsable(true), DefaultValue(0)]
		public int TBWidth
		{
			get
			{
				return m_tbwidth;
			}
			set
			{
				if (value <= 0)
					m_tbwidth = 0;
				else m_tbwidth = value;
				this.PerformLayout();
			}
		}
		bool FixedWidth
		{
			get { return m_tbwidth > 0; }
		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
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

		private void ValueBox_Layout(object sender, LayoutEventArgs e)
		{
			if (FixedWidth)
			{
				FixedWidthLayout();
				return;
			}
			this.SuspendLayout();
			const int buf = 5; //10 px buffer between

			//calculate textbox width
			int width = this.Width - label.Width - units.Width;
			if (Units != "")
				width -= buf;
			if (Text != "")
				width -= buf;

			valueText.Width = width;

			// layout each control
			int pos = 0;
			if (Text != "")
			{
				label.Left = pos;
				pos = label.Width;
				pos += buf;
			}

			valueText.Left = pos;
			pos += valueText.Width;
			pos += buf;

			units.Left = pos;

			//this.Width = pos;
			this.Height = valueText.Height;
			this.ResumeLayout(true);
		}
		void FixedWidthLayout()
		{
			this.SuspendLayout();
			const int buf = 5; //10 px buffer between
			//calculate buffer width
			int buffer = this.Width - label.Width - units.Width - TBWidth;
			if (Units != "")
				buffer -= buf;
			if (Text != "")
				buffer -= buf;

			valueText.Width = TBWidth;

			// layout each control
			int pos = 0;
			if (Text != "")
			{
				label.Left = pos;
				pos = label.Width;
				pos += buf;
			}

			pos += buffer;
			valueText.Left = pos;
			pos += valueText.Width;
			pos += buf;

			units.Left = pos;

			//this.Width = pos;
			this.Height = valueText.Height;
			this.ResumeLayout(true);
		}
		private void valueText_Leave(object sender, EventArgs e)
		{
			Value = Value;
		}

		private void units_Click(object sender, EventArgs e)
		{
			valueText.Focus();
		}


		#region IAttributeEditor Members

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object AttributeValue
		{
			get
			{
				return Value;
			}
			set
			{
				Value = (double)value;
			}
		}

		#endregion
	}
}
