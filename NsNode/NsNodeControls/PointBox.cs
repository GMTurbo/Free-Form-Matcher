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
	public partial class PointBox : UserControl, IAttributeEditor
	{
		public PointBox()
		{
			InitializeComponent();
			Point3D = true;
			Point = new double[3] { 0,0,0 };
		}
		//[Category("Appearance"), BrowsableAttribute(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		//public string Note
		//{
		//     get
		//     {
		//          return note.Text;
		//     }
		//     set
		//     {
		//          note.Text = value;
		//          Point3D = Point3D; //refresh height
		//     }
		//}
		[Category("Appearance"), BrowsableAttribute(true)]
		public string Units
		{
			get
			{
				return units_1.Text;
			}
			set
			{
				units_1.Text = value;
				units_2.Text = value;
				units_3.Text = value;
			}
		}
		[Category("Appearance"), BrowsableAttribute(true)]
		public string Label
		{
			get { return groupBox1.Text; }
			set { groupBox1.Text = value; }
		}

		[Category("Appearance"), BrowsableAttribute(true)]
		public string LabelName1
		{
			get { return label1.Text; }
			set { label1.Text = value; }
		}
		[Category("Appearance"), BrowsableAttribute(true)]
		public string LabelName2
		{
			get { return label2.Text; }
			set { label2.Text = value; }
		}
		[Category("Appearance"), BrowsableAttribute(true)]
		public string LabelName3
		{
			get { return label3.Text; }
			set { label3.Text = value; }
		}

		public override string Text
		{
			get
			{
				return Label;
			}
			set
			{
				Label = value;
			}
		}

		[Category("Data"), BrowsableAttribute(true)]
		public double[] Point
		{
			get
			{
				double[] pnt;
				if (Point3D)
				{
					pnt = new double[3];
					pnt[2] = double.Parse(textBox3.Text);
				}
				else
					pnt = new double[2];

				pnt[0] = double.Parse(textBox1.Text);
				pnt[1] = double.Parse(textBox2.Text);
				return pnt;
			}
			set
			{
				try
				{
					switch (value.Length)
					{
						case 3:
							textBox3.Text = value[2].ToString(m_format);
							goto case 2;
						case 2:
							textBox2.Text = value[1].ToString(m_format);
							goto case 1;
						case 1:
							textBox1.Text = value[0].ToString(m_format);
							break;
						default:
							textBox1.Text = "";
							textBox2.Text = "";
							textBox3.Text = "";
							break;
					}
				}
				catch
				{
					m_format = "g";
					Point = new double[3] { 0, 0, 0 };
				}
			}
		}

		string m_format = "g";

		[Category("Data"), BrowsableAttribute(true)]
		public string Format
		{
			get { return m_format; }
			set
			{
				m_format = value;
				Point = Point;
			}
		}
		[Category("Appearance"), Browsable(true)]
		public bool Point3D
		{
			get { return m_3d; }
			set
			{
				m_3d = value;
				textBox3.Visible = value;
				label3.Visible = value;
				units_3.Visible = value;
				this.Height = value ? 88 : 66;
				//if (Note != "")
				//     this.Height += 10;
				Refresh();
			}
		}
		bool m_3d;
	
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
			else if (e.KeyChar == 13) // enter has been pressed
			{
				Validate(false);
			}
			else
			{
				// Consume this invalid key and beep
				e.Handled = true;
				//    MessageBeep();
			}

		}

		#region IAttributeEditor Members

		[System.ComponentModel.Browsable(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public object AttributeValue
		{
			get
			{
				return Point;
			}
			set
			{
				Point = value as double[];
			}
		}

		#endregion

		int TextBoxWidth
		{
			get
			{
				const int buf = 5; //10 px buffer between
				//calculate textbox width
				int width1 = this.Width - label1.Width - units_1.Width - 2 * buf;
				int width2 = this.Width - label2.Width - units_2.Width - 2 * buf;
				int width3 = this.Width - label3.Width - units_3.Width - 2 * buf;
				int width = Math.Min(width1, Math.Min(width2, width3));// use the min width for all tbs
				if (Units != "")
					width -= buf;
				if (Text != "")
					width -= buf;
				return width;
			}

		}

		private void textBox1_Layout(object sender, LayoutEventArgs e)
		{
			LayoutBoxes(textBox1, label1, units_1);
		}
		private void textBox2_Layout(object sender, LayoutEventArgs e)
		{
			LayoutBoxes(textBox2, label2, units_2);
		}
		private void textBox3_Layout(object sender, LayoutEventArgs e)
		{
			LayoutBoxes(textBox3, label3, units_3);
		}
		void LayoutBoxes(TextBox box, Label lbl, Label units)
		{
			this.SuspendLayout();
			const int buf = 5; //10 px buffer between

			////calculate textbox width
			//int width = this.Width - label3.Width - units_3.Width - 2 * buf;
			//if (Units != "")
			//     width -= buf;
			//if (Text != "")
			//     width -= buf;

			box.Width = TextBoxWidth;

			// layout each control right to left to align boxes
			int pos = Width - buf - units.Width;
			units.Left = pos;
			pos -= buf;
			pos -= box.Width;
			box.Left = pos;
			pos -= buf;
			pos -= lbl.Width;
			lbl.Left = pos;

			//if (Text != "")
			//{
			//     lbl.Left = pos;
			//     pos = lbl.Width;
			//     pos += buf;
			//}

			//box.Left = pos;
			//pos += box.Width;
			//pos += buf;

			//units.Left = pos;

			//this.Width = pos;
			this.ResumeLayout(true);
		}

	}
}
