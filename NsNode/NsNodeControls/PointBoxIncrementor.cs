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
	public partial class PointBoxIncrementor : UserControl, IAttributeEditor
	{
		public event EventHandler Value1Changed;
		public event EventHandler Value2Changed;
		public event EventHandler Value3Changed;

		public event KeyPressEventHandler Value1KeyPressed;
		public event KeyPressEventHandler Value2KeyPressed;
		public event KeyPressEventHandler Value3KeyPressed;

		public PointBoxIncrementor()
		{
			InitializeComponent();
			numericUpDown1.Accelerations.Add(new NumericUpDownAcceleration(2, 2));
			numericUpDown2.Accelerations.Add(new NumericUpDownAcceleration(2, 2));
			numericUpDown3.Accelerations.Add(new NumericUpDownAcceleration(2, 2));
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

          double[] m_raw = new double[3];

		[Category("Data"), BrowsableAttribute(true)]
		public double[] Point
		{
			get
			{
				double[] pnt;
                    if (Point3D)
                    {
                         pnt = new double[3];
                         pnt[2] = (double)numericUpDown3.Value;
                         //pnt[2] = m_raw[2];
                    }
                    else
                         pnt = new double[2];
                    
				pnt[1] = (double)numericUpDown2.Value;
				pnt[0] = (double)numericUpDown1.Value;

                    //pnt[1] = m_raw[1];
                   //pnt[0] = m_raw[0];

				return pnt;
			}
			set
			{
				try
				{
					switch (value.Length)
					{
						case 3:
							numericUpDown3.Text = value[2].ToString(m_format);
                                   m_raw[2] = value[2];
							goto case 2;
						case 2:
							numericUpDown2.Text = value[1].ToString(m_format);
                                   m_raw[1] = value[1];
							goto case 1;
						case 1:
							numericUpDown1.Text = value[0].ToString(m_format);
                                   m_raw[0] = value[0];
							break;
						default:
							numericUpDown1.Text = "";
							numericUpDown2.Text = "";
							numericUpDown3.Text = "";
							break;
					}
				}
				catch
				{
                         m_format = "#0.0000";
					Point = new double[3] { 0, 0, 0 };
				}
			}
		}

		//string m_format = "g";
          string m_format = "#0.0000";

		[Category("Data"), BrowsableAttribute(true)][Description("Set the resolution of the display values")]
		public int DecimalPlaces
		{
			get { return numericUpDown1.DecimalPlaces; }
			set
			{
				numericUpDown1.DecimalPlaces = numericUpDown2.DecimalPlaces = numericUpDown3.DecimalPlaces = value;
			}
		}

		[Category("Data"), BrowsableAttribute(true)]
		[Description("Set the maximum value allowable for the points")]
		public decimal Maximum
		{
			get { return numericUpDown1.Maximum; }
			set
			{
				numericUpDown1.Maximum = numericUpDown2.Maximum = numericUpDown3.Maximum = value;
			}
		}

		[Category("Data"), BrowsableAttribute(true)]
		[Description("Set the minimum value allowable for the points")]
		public decimal Minimum
		{
			get { return numericUpDown1.Minimum; }
			set
			{
				numericUpDown1.Minimum = numericUpDown2.Minimum = numericUpDown3.Minimum = value;
			}
		}

		[Category("Data"), BrowsableAttribute(true)][Description("Set the increment values for increment box 1")]
		public decimal Increment_1
		{
			get { return numericUpDown1.Increment; }
			set
			{
				numericUpDown1.Increment = value;
			}
		}
		[Category("Data"), BrowsableAttribute(true)][Description("Set the increment values for increment box 2")]
		public decimal Increment_2
		{
			get { return numericUpDown2.Increment; }
			set
			{
				numericUpDown2.Increment = value;
			}
		}
		[Category("Data"), BrowsableAttribute(true)][Description("Set the increment values for increment box 3")]
		public decimal Increment_3
		{
			get { return numericUpDown3.Increment; }
			set
			{
				numericUpDown3.Increment = value;
			}
		}

		[Category("Appearance"), Browsable(true)]
		public bool Point3D
		{
			get { return m_3d; }
			set
			{
				m_3d = value;
				numericUpDown3.Visible = value;
				label3.Visible = value;
				units_3.Visible = value;
				this.Height = value ? 88 : 66;
				//if (Note != "")
				//     this.Height += 10;
				Refresh();
			}
		}
		bool m_3d;

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

		private void numericUpDown1_Layout(object sender, LayoutEventArgs e)
		{
			LayoutBoxes(numericUpDown1, label1, units_1);
		}
		private void numericUpDown2_Layout(object sender, LayoutEventArgs e)
		{
			LayoutBoxes(numericUpDown2, label2, units_2);
		}
		private void numericUpDown3_Layout(object sender, LayoutEventArgs e)
		{
			LayoutBoxes(numericUpDown3, label3, units_3);
		}
		void LayoutBoxes(NumericUpDown box, Label lbl, Label units)
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

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			//Point = Point;
			//Application.DoEvents();
			if (!bUpdate)
				return;
			if(Value1Changed!=null)
				Value1Changed(sender,e);
		}
		private void numericUpDown2_ValueChanged(object sender, EventArgs e)
		{
			//Point = Point;
			//Application.DoEvents();
			if (!bUpdate)
				return;
			if (Value2Changed != null)
				Value2Changed(sender, e);
		}
		private void numericUpDown3_ValueChanged(object sender, EventArgs e)
		{
			//Point = Point;
			//Application.DoEvents();
			if (!bUpdate)
				return;
			if (Value3Changed != null)
				Value3Changed(sender, e);
		}

		bool bUpdate = true;
		public void PauseUpdating()
		{
			bUpdate = false;
		}
		public void ResumeUpdating(bool p)
		{
			bUpdate = true;
			if (p == true)
			{
				EventArgs e = new EventArgs();
				numericUpDown1_ValueChanged(this, e);
				numericUpDown2_ValueChanged(this, e);
				numericUpDown3_ValueChanged(this, e);
			}

		}

		private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (Value1KeyPressed != null)
				Value1KeyPressed(sender, e);
		}

		private void numericUpDown2_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (Value2KeyPressed != null)
				Value2KeyPressed(sender, e);
		}

		private void numericUpDown3_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (Value3KeyPressed != null)
				Value3KeyPressed(sender, e);
		}
	}
}
