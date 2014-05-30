using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NsNodes;

namespace NsNodeControls
{
     public partial class ColorScale : Form
     {
          public ColorScale()
          {
               InitializeComponent();
               ColorScaleMin = 1;
               ColorScaleMax = 1000;
               RecreateLabels();
               Refresh();
          }

          /// <summary>
          /// use this to check who is sending the info for coloring (mold vs sail)
          /// </summary>
          object m_sender = null;

          List<double[]> m_ranges = new List<double[]>
          {
               new double[2],
               new double[2]
          };
          bool m_firstG = true;
          bool m_firstD = true;
          private double[] RangesG
          {
               get { return m_ranges[0]; }
               set 
               {
                    m_ranges[0] = value;
                    if (m_firstG)
                    {
                         Originals[0] = value;
                         m_firstG = false;
                    }
               }
          }

          private double[] RangesD
          {
               get { return m_ranges[1]; }
               set
               {
                    m_ranges[1] = value;
                    if (m_firstD)
                    {
                         Originals[1] = value;
                         m_firstD = false;
                    }
               }
          }

          List<double[]> m_originals = new List<double[]>
          {
               new double[2],
               new double[2]
          };

          private List<double[]> Originals
          {
               get { return m_originals; }
               set { m_originals = value; }
          }

          public object Sender
          {
               get { return m_sender; }
               set 
               { 
                    m_sender = value;

                    if (value.ToString().ToLower().Contains("mold"))
                    {
                         ColorScaleMin = m_ranges[0][0];
                         ColorScaleMax = m_ranges[0][1];
                    }
                    else
                    {
                         ColorScaleMin = m_ranges[1][0];
                         ColorScaleMax = m_ranges[1][1];
                    }

                    RecreateLabels();
                    Refresh();
               }
          }

          public EventHandler<EventArgs<double[]>> ColorRangeChanged;

          public double ColorScaleMin
          {
               set { textBox1.Text = value.ToString("#0.000"); }
               get
               {
                    double v;
                    try
                    {
                         v = Convert.ToDouble(textBox1.Text);
                         return (v); //convert to log scale
                    }
                    catch
                    {
                         ColorScaleMin = 0;
                         return 0;
                    }
               }
          }
          public double ColorScaleMax
          {
               set { textBox2.Text = value.ToString("#0.000"); }

               get
               {
                    double v;
                    try
                    {
                         v = Convert.ToDouble(textBox2.Text);
                         return (v); //convert to log scale
                    }
                    catch
                    {
                         ColorScaleMax = 1;
                         return 1;
                    }
               }

          }

          //double[,] m_pnts = null;
          //double[,] m_dens = null;

          void RecreateLabels()
          {
               panel1.Controls.Clear();
               const int SEG = 7;
               float height = (float)panel1.Height / (float)SEG;
               for (int i = 1; i < SEG; i++)
               {
                    Label l = new Label();
                    panel1.Controls.Add(l);
                    double val = (ColorScaleMax - ColorScaleMin) * (double)i / (double)SEG + ColorScaleMin;
                    l.Text = val.ToString("G3");
                    l.Location = new Point(panel1.Width / 2, (int)(i * height));
                    l.AutoSize = true;
                    l.TextAlign = ContentAlignment.MiddleRight;
                    l.Visible = true;
               }
               panel1.Refresh();
          }

          void panel1_Resize(object sender, System.EventArgs e)
          {
               RecreateLabels();
          }

          void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
          {
               const int SEG = 200;
               float height = (float)panel1.Height / (float)SEG;
               RectangleF rect;
               double max = ColorScaleMax;
               double min = ColorScaleMin;

               for (int i = 0; i < SEG; i++)
               {
                    Brush b = new SolidBrush(ColorHelper.GetColor(SEG, 0, i));
                    rect = new RectangleF(0, i * height, panel1.Width / 2, height);
                    e.Graphics.FillRectangle(b, rect);
               }
          }

          public void Update_Range(object sender, EventArgs<double[]> range)
          {
               Sender = sender;

               if (sender.ToString().ToLower().Contains("mold"))
               {
                    RangesG = range.Value;
                    label1.Text = "Gaussian";
               }
               else
               {
                    RangesD = range.Value;
                    label1.Text = "Densities";
               }

               ColorScaleMin = range.Value[0];
               ColorScaleMax = range.Value[1];
               RecreateLabels();
               Refresh();
          }

          private void textBox1_KeyUp(object sender, KeyEventArgs e)
          {
               if (e.KeyCode == Keys.Enter)
               {
                    if (ColorRangeChanged != null)
                         ColorRangeChanged(Sender, new EventArgs<double[]>(new double[] { ColorScaleMin, ColorScaleMax }));
               }
          }

          private void textBox2_KeyUp(object sender, KeyEventArgs e)
          {
               if (e.KeyCode == Keys.Enter)
               {
                    if (ColorRangeChanged != null)
                         ColorRangeChanged(Sender, new EventArgs<double[]>(new double[] { ColorScaleMin, ColorScaleMax }));
               }
          }

          private void ColorScale_FormClosing(object sender, FormClosingEventArgs e)
          {
               if (e.CloseReason != CloseReason.FormOwnerClosing)
               {
                    e.Cancel = true;
                    this.Hide();
               }
          }

          private void button1_Click(object sender, EventArgs e)
          {
               RangesG = Originals[0];
               RangesD = Originals[1];

               if (Sender.ToString().ToLower().Contains("mold"))
               {
                    ColorScaleMin = RangesG[0];
                    ColorScaleMax = RangesG[1];
               }
               else
               {
                    ColorScaleMin = RangesD[0];
                    ColorScaleMax = RangesD[1];
               }
              
               if (ColorRangeChanged != null)
                    ColorRangeChanged(Sender, new EventArgs<double[]>(new double[] { ColorScaleMin, ColorScaleMax }));
          }
     }

     static class ColorHelper
     {
          public static System.Drawing.Color GetColor(double max, double min, double val)
          {
               double del = (max - min) / 7;
               double[] nodes = new double[8];
               for (int i = 0; i < nodes.Length; i++)
                    nodes[i] = ((7 - i) * min + i * max) / 7;

               int r, g, b;
               if (val >= nodes[7])
               {
                    r = 255;
                    g = 255;
                    b = 255;
               }
               else if (val >= nodes[6])
               {
                    r = 255;
                    g = (int)(255 * (val - nodes[6]) / del);
                    b = 255;
               }
               else if (val >= nodes[5])
               {
                    r = 255;
                    g = 0;
                    b = (int)(255 * (val - nodes[5]) / del);
               }
               else if (val >= nodes[4])
               {
                    r = 255;
                    g = 255 - (int)(255 * (val - nodes[4]) / del);
                    b = 0;
               }
               else if (val >= nodes[3])
               {
                    r = (int)(255 * (val - nodes[3]) / del);
                    g = 255;
                    b = 0;
               }
               else if (val >= nodes[2])
               {
                    r = 0;
                    g = 255;
                    b = 255 - (int)(255 * (val - nodes[2]) / del);
               }
               else if (val >= nodes[1])
               {
                    r = 0;
                    g = (int)(255 * (val - nodes[1]) / del);
                    b = 255;
               }
               else if (val >= nodes[0])
               {
                    r = 0;
                    g = 0;
                    b = (int)(255 * (val - nodes[0]) / del);
               }
               else
               {
                    r = 0;
                    g = 0;
                    b = 0;
               }

               // clamps color values at 0-255

               LimitRange(0, ref r, 255);
               LimitRange(0, ref g, 255);
               LimitRange(0, ref b, 255);

               return System.Drawing.Color.FromArgb(r, g, b);
          }
          public static void LimitRange(int low, ref int val, int high)
          {
               if (val < low) val = low;
               if (high < val) val = high;
          }
     }

}
