using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NPlot.Windows;
using NPlot;
using RBF;
using RBFBasis;
using RBFPolynomials;
using GA;

namespace UI
{
     public partial class Form1 : Form
     {
          enum BasisType
          {
               Gaussian = 0,
               Line,
               Parabolic,
               Cubic,
               CubicParabolic
          }

          GA.Evaluator Eval = null;

          BackgroundWorker p1BRW = new BackgroundWorker();

          ConicalGA Algo;// = new ConicalGA(10, 60, GAType.BINARY);

          RBFCurve p1Curve;
          RBFCurve p2Curve;
          RBFCurve p3Curve;
          RBFCurve p4Curve;
          RBFCurve p5Curve;
          RBFCurve p6Curve;

          BasisType m_basis;

          private BasisType Basis
          {
               get { return m_basis; }
               set { m_basis = value; }
          }

          delegate void UpdateStatus(string message);

          UpdateStatus stat;

          public Form1()
          {
               InitializeComponent();
               SetupWorkers();
               InitializePlots();
               InstatiatePlotData();

               stat = ((string message) => { m_status.Text = message; });

               Eval = new Evaluator((ref List<int> binaryCoded, ref List<int> bestChromo) =>
                    {
                         if (binaryCoded.Count == 0)
                              return 0;

                         double error = 0;

                         List<double[]> gaPnts = new List<double[]>();

                         double[] result = EvalChromosome(ref binaryCoded, ref gaPnts);

                         CustomBasis GeneticBasis = new CustomBasis(16.383 / 2.0 - result[0], 16.383 / 2.0 - result[1], 16.383 / 2.0 - result[2], 16.383 / 2.0 - result[3]);

                         double[] pnt1, pnt2, dx1, dx2;

                         object curve = new object();
                         object plot = new object();

                         for (int j = 0; j < 1; j++)
                         {
                              switch (j + 1)
                              {
                                   case 1:
                                        curve = p1Curve;
                                        plot = plotSurface2D1;
                                        break;
                                   case 2:
                                        curve = p2Curve;
                                        plot = plotSurface2D2;
                                        break;
                                   case 3:
                                        curve = p3Curve;
                                        plot = plotSurface2D3;
                                        break;
                                   case 4:
                                        curve = p4Curve;
                                        plot = plotSurface2D4;
                                        break;
                                   case 5:
                                        curve = p5Curve;
                                        plot = plotSurface2D5;
                                        break;
                                   case 6:
                                        curve = p6Curve;
                                        plot = plotSurface2D6;
                                        break;
                              }

                              RBFCurve oldRBF = curve as RBFCurve;

                              //List<double[]> fitpoints = new List<double[]>(oldRBF.OriginalFitPoints);

                              List<double[]> fitpoints = new List<double[]>(gaPnts);

                              if (RemoveExtraFitPoints)
                              {
                                   fitpoints.RemoveAt(1);
                                   fitpoints.RemoveAt(2);
                              }

                              RBFCurve GaRBF = new RBFCurve(null, "GA RBF", fitpoints, GeneticBasis, new Linear(null), 0.0);

                              for (int i = 0; i < 30; i++)
                              {
                                   dx1 = new double[2];
                                   dx2 = new double[2];
                                   pnt1 = new double[] { (double)i, 0 };
                                   pnt2 = new double[] { (double)i, 0 };
                                   GaRBF.First(ref pnt1, ref dx1);
                                   oldRBF.First(ref pnt2, ref dx2);

                                   error += Math.Pow(pnt1[1] - pnt2[1], 2);
                                   error += Math.Pow(Math.Pow(dx2[0] - dx1[0], 2) + Math.Pow(dx2[1] - dx1[1], 2), 2);
                              }

                              //int count = 0;

                              double pntError = 1000;

                              gaPnts.ForEach((pnt) =>
                                   {
                                        dx1 = new double[2];
                                        dx2 = new double[2];
                                        pnt2 = new double[] { pnt[0], pnt[1] };
                                        GaRBF.First(ref pnt, ref dx1);
                                        oldRBF.First(ref pnt, ref dx2);
                                        error += 10 * Math.Pow(pnt2[1] - pnt[1], 2);
                                        //error += Math.Pow(Math.Pow(dx2[0] - dx1[0], 2) + Math.Pow(dx2[1] - dx1[1], 2), 2);
                                   });

                              //gaPnts.ForEach((pnt) =>
                              //     {
                              //          if (oldRBF.OriginalFitPoints.Find((Oldpnt) => { return Oldpnt[0] == pnt[0] && Oldpnt[1] == pnt[1]; }) != null)
                              //               count++;
                              //          else
                              //               error *= 4;
                              //     });

                              //if(count != 0)
                              //     pntError /= ((double)count / 5.0) * 1000.0;

                              error += pntError;
                         }

                         return Math.Pow(error, -1);
                    });

          }

          bool RemoveExtraFitPoints = false;

          double[] EvalChromosome(ref List<int> chromo)
          {
               double a = 0, b = 0, c = 0, d = 0, e = 0;
               int sign = 1;

               List<int> a_array = new List<int>(chromo.GetRange(0, 15));
               List<int> b_array = new List<int>(chromo.GetRange(15, 15));
               List<int> c_array = new List<int>(chromo.GetRange(30, 15));
               List<int> d_array = new List<int>(chromo.GetRange(45, 15));
               List<int> e_array = new List<int>(chromo.GetRange(60, 15));

               sign = a_array[0] == 0 ? 1 : -1;
               a_array.RemoveAt(0);
               //a = Convert.ToInt32(string.Join("", a_array.ToArray()), 2) / 1000.0;
               a = sign * Convert.ToInt32(string.Join("", a_array.ToArray()), 2) / 1000.0;

               sign = b_array[0] == 0 ? 1 : -1;
               b_array.RemoveAt(0);
               b = Convert.ToInt32(string.Join("", b_array.ToArray()), 2) / 1000.0;
               //b = sign * Convert.ToInt32(string.Join("", b_array.ToArray()), 2) / 1000.0;

               sign = c_array[0] == 0 ? 1 : -1;
               c_array.RemoveAt(0);
               c = Convert.ToInt32(string.Join("", c_array.ToArray()), 2) / 1000.0;
               //c = sign * Convert.ToInt32(string.Join("", c_array.ToArray()), 2) / 1000.0;

               sign = d_array[0] == 0 ? 1 : -1;
               d_array.RemoveAt(0);
               d = Convert.ToInt32(string.Join("", d_array.ToArray()), 2) / 1000.0;
               //d = Convert.ToInt32(string.Join("", d_array.ToArray()), 2) / 1000.0;

               sign = e_array[0] == 0 ? 1 : -1;
               e_array.RemoveAt(0);
               e = Convert.ToInt32(string.Join("", d_array.ToArray()), 2) / 1000.0;

               return new double[] { a, b, c, d, e };
          }

          double[] EvalChromosome(ref List<int> chromo, ref List<double[]> gapnts)
          {
               double a = 0, b = 0, c = 0, d = 0, e = 0;
               int sign = 1;

               List<int> a_array = new List<int>(chromo.GetRange(0, 14));
               List<int> b_array = new List<int>(chromo.GetRange(14, 14));
               List<int> c_array = new List<int>(chromo.GetRange(28, 14));
               List<int> d_array = new List<int>(chromo.GetRange(42, 14));
               //List<int> e_array = new List<int>(chromo.GetRange(56, 14));
               //List<int> count_array = new List<int>(chromo.GetRange(56, 4));
               List<int> y_array = new List<int>(chromo.GetRange(56, 35));

               //sign = a_array[0] == 0 ? 1 : -1;
               //a_array.RemoveAt(0);
               //a = Convert.ToInt32(string.Join("", a_array.ToArray()), 2) / 1000.0;
               a = sign * Convert.ToInt32(string.Join("", a_array.ToArray()), 2) / 1000.0;

               sign = b_array[0] == 0 ? 1 : -1;
               b_array.RemoveAt(0);
               b = Convert.ToInt32(string.Join("", b_array.ToArray()), 2) / 1000.0;
               //b = sign * Convert.ToInt32(string.Join("", b_array.ToArray()), 2) / 1000.0;

               sign = c_array[0] == 0 ? 1 : -1;
               c_array.RemoveAt(0);
               c = Convert.ToInt32(string.Join("", c_array.ToArray()), 2) / 1000.0;
               //c = sign * Convert.ToInt32(string.Join("", c_array.ToArray()), 2) / 1000.0;

               sign = d_array[0] == 0 ? 1 : -1;
               d_array.RemoveAt(0);
               d = Convert.ToInt32(string.Join("", d_array.ToArray()), 2) / 1000.0;
               //d = Convert.ToInt32(string.Join("", d_array.ToArray()), 2) / 1000.0;

               //sign = e_array[0] == 0 ? 1 : -1;
               //e_array.RemoveAt(0);
               //e = Convert.ToInt32(string.Join("", d_array.ToArray()), 2) / 1000.0;

               //int count = Convert.ToInt32(string.Join("", count_array.ToArray()), 2);
               //if (count <= 1)
               //     count = 2;

               double y = 0;
               List<int> tmp = new List<int>(7);
               List<double[]> tmp3 = new List<double[]>();
               int count = 5;
               for (double i = 0; i < count; i++)
               {
                    tmp = y_array.GetRange((int)i * 7, 7);
                    y = (double)(Convert.ToInt32(string.Join("", tmp.ToArray()), 2)) / 127.0;
                    tmp3.Add(new double[] { i * 5, (int)(Min[1] + y * (Max[1] - Min[1])) });
               }


               //tmp2.ForEach((pnt) => { tmp3.Add(new double[] { (double)(pnt[0]), (double)(pnt[1]) }); });
               gapnts = tmp3;
               return new double[] { a, b, c, d };
          }

          double[] EvalChromosomeWithType(ref List<int> chromo, ref RBFBasis.IBasisFunction basis)
          {
               double a = 0, b = 0, c = 0, d = 0;
               int sign = 1;

               List<int> a_array = new List<int>(chromo.GetRange(0, 15));
               List<int> b_array = new List<int>(chromo.GetRange(15, 15));
               List<int> c_array = new List<int>(chromo.GetRange(30, 15));
               List<int> d_array = new List<int>(chromo.GetRange(45, 15));

               sign = a_array[0] == 0 ? 1 : -1;
               a_array.RemoveAt(0);
               a = sign * Convert.ToInt32(string.Join("", a_array.ToArray()), 2) / 1000.0;

               sign = b_array[0] == 0 ? 1 : -1;
               b_array.RemoveAt(0);
               b = sign * Convert.ToInt32(string.Join("", b_array.ToArray()), 2) / 1000.0;

               sign = c_array[0] == 0 ? 1 : -1;
               c_array.RemoveAt(0);
               c = sign * Convert.ToInt32(string.Join("", c_array.ToArray()), 2) / 1000.0;

               sign = d_array[0] == 0 ? 1 : -1;
               d_array.RemoveAt(0);
               d = sign * Convert.ToInt32(string.Join("", d_array.ToArray()), 2) / 1000.0;

               return new double[] { a, b, c, d };
          }

          void Algo_NewBest(List<int> bestChromo, double fitness, double prog)
          {
               p1BRW.ReportProgress((int)(prog * 100.0), (object)bestChromo);
               stat("Best Fitness: " + fitness);
               if (Algo.Population.Count < 100)
                    System.Threading.Thread.Sleep(50);
          }

          private void InstatiatePlotData()
          {
               for (int i = 1; i < 7; i++)
                    SetPlots(i, RandomizePoints(5)/*GetPredefinedFitPoints(i)*/);
          }

          double[] m_max = new double[] { double.MinValue, double.MinValue };
          double[] m_min = new double[] { double.MaxValue, double.MaxValue };

          protected double[] Max
          {
               get
               {
                    return m_max;
               }
               set
               {
                    m_max = value;
               }
          }

          protected double[] Min
          {
               get
               {
                    return m_min;
               }
               set
               {
                    m_min = value;
               }
          }

          private void SetPlots(int p, List<double[]> fitPoints)
          {
               object plot = new object();
               object curve = new object();
               IBasisFunction basis = new RBFBasis.ThinPlateSpline(null);

               switch (Basis)
               {
                    case BasisType.Cubic:
                         basis = new RBFBasis.PolyHarmonic3(null);
                         break;
                    case BasisType.Gaussian:
                         basis = new RBFBasis.Gaussian(null);
                         break;
                    case BasisType.Line:
                         basis = new RBFBasis.CustomBasis(null, 0, 0, 1, 0);
                         break;
                    case BasisType.Parabolic:
                         basis = new RBFBasis.ThinPlateSpline2(null);
                         break;
                    case BasisType.CubicParabolic:
                         if (p % 2 == 0)
                              basis = new RBFBasis.PolyHarmonic3(null);
                         else
                              basis = new RBFBasis.CustomBasis(null, 1, 1, -1, 0);
                         break;
               }

               switch (p)
               {
                    case 1:
                         plot = plotSurface2D1;
                         //basis = new RBFBasis.Exponential(null);
                         p1Curve = new RBFCurve(null, "RBF " + p, fitPoints, basis, new RBFPolynomials.Linear(null), 0.0);
                         curve = p1Curve;
                         break;
                    case 2:
                         plot = plotSurface2D2;
                         //basis = new RBFBasis.Exponential(null);
                         p2Curve = new RBFCurve(null, "RBF " + p, fitPoints, basis, new RBFPolynomials.Linear(null), 0.0);
                         curve = p2Curve;
                         break;
                    case 3:
                         plot = plotSurface2D3;
                         //basis = new RBFBasis.Exponential(null);
                         p3Curve = new RBFCurve(null, "RBF " + p, fitPoints, basis, new RBFPolynomials.Linear(null), 0.0);
                         curve = p3Curve;
                         break;
                    case 4:
                         plot = plotSurface2D4;
                         //basis = new RBFBasis.Exponential(null);
                         p4Curve = new RBFCurve(null, "RBF " + p, fitPoints, basis, new RBFPolynomials.Linear(null), 0.0);
                         curve = p4Curve;
                         break;
                    case 5:
                         plot = plotSurface2D5;
                         //basis = new RBFBasis.Exponential(null);
                         p5Curve = new RBFCurve(null, "RBF " + p, fitPoints, basis, new RBFPolynomials.Linear(null), 0.0);
                         curve = p5Curve;
                         break;
                    case 6:
                         plot = plotSurface2D6;
                         // basis = new RBFBasis.Exponential(null);
                         p6Curve = new RBFCurve(null, "RBF " + p, fitPoints, basis, new RBFPolynomials.Linear(null), 0.0);
                         curve = p6Curve;
                         break;
               }

               if (Max[0] < fitPoints.Max(pt => pt[0]))
                    Max[0] = fitPoints.Max(pt => pt[0]);
               if (Max[1] < fitPoints.Max(pt => pt[1]))
                    Max[1] = fitPoints.Max(pt => pt[1]);

               if (Min[0] > fitPoints.Min(pt => pt[0]))
                    Min[0] = fitPoints.Min(pt => pt[0]);
               if (Min[1] > fitPoints.Min(pt => pt[1]))
                    Min[1] = fitPoints.Min(pt => pt[1]);

               NPlot.Windows.PlotSurface2D plotter = (NPlot.Windows.PlotSurface2D)plot;
               RBFCurve RBF = (RBFCurve)curve;

               //RBF = new RBFCurve(null, "RBF " + p, fitPoints, basis, new RBFPolynomials.Linear(null), 0.0);

               plotter.Clear();
               plotter.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

               //Add a background grid for better chart readability.
               NPlot.Grid grid = new NPlot.Grid();
               grid.VerticalGridType = NPlot.Grid.GridType.Coarse;
               grid.HorizontalGridType = NPlot.Grid.GridType.Coarse;
               grid.MinorGridPen = new Pen(Color.Blue, 1.0f);
               grid.MajorGridPen = new Pen(Color.LightGray, 1.0f);

               plotter.Add(grid);

               int length = fitPoints.Count * 5;
               //Create a step plot instance for the balance chart.
               LinePlot speed = new LinePlot();
               PointPlot pplot = new PointPlot();

               speed.Pen = new Pen(Color.Blue, 1);

               //Create the lists from which to pull data.
               List<Int32> countaxis = new List<Int32>();
               List<Int32> Pplotcountaxis = new List<Int32>();
               List<decimal> speedlist = new List<decimal>();

               for (int i = 0; i < length; i++)
                    countaxis.Add(i);

               for (int i = 0; i < fitPoints.Count; i++)
                    Pplotcountaxis.Add((int)fitPoints[i][0]);
               //Populate the balanceAmount list
               double[] pnt;
               List<decimal> pointsList = fitPoints.ConvertAll(new Converter<double[], decimal>((double[] dd) => { return Convert.ToDecimal(dd[1]); }));

               for (int i = 0; i < length; i++)
               {
                    pnt = new double[] { (double)i, 0 };
                    RBF.Value(ref pnt);
                    speedlist.Add(Convert.ToDecimal(pnt[1]));
               }

               //foreach (double value in pent.Speeds)
               //{
               //     speedlist.Add(Convert.ToDecimal(value) / 1200);
               //}
               pplot.AbscissaData = Pplotcountaxis;
               pplot.DataSource = pointsList;
               speed.AbscissaData = countaxis;
               speed.DataSource = speedlist;

               //Add stepBalance to plotSurfaceBalance.

               plotter.Add(speed);
               plotter.Add(pplot);

               //Balance plot general settings.
               plotter.ShowCoordinates = true;
               //plotter.YAxis1.WorldMax = 10;
               //plotter.YAxis1.WorldMin = 0;
               //this.YAxis1.
               plotter.AutoScaleAutoGeneratedAxes = true;
               plotter.AddAxesConstraint(new AxesConstraint.AxisPosition(NPlot.PlotSurface2D.YAxisPosition.Left, 60));

               //Label Label1 = new Label();
               //Label1.Font = new Font(Label1.Font, FontStyle.Bold);

               //plotter.YAxis1.

               plotter.YAxis1.Label = "Y";
               plotter.YAxis1.LabelFont = new Font(this.Font, FontStyle.Bold);
               plotter.YAxis1.LabelOffsetAbsolute = true;
               plotter.XAxis1.Label = "X";
               plotter.XAxis1.LabelFont = new Font(this.Font, FontStyle.Bold);
               plotter.YAxis1.LabelOffset = 40;
               plotter.XAxis1.HideTickText = false;
               plotter.Padding = 5;

               plotter.RightMenu = NPlot.Windows.PlotSurface2D.DefaultContextMenu;
               plotter.AddInteraction(new NPlot.Windows.PlotSurface2D.Interactions.AxisDrag(false));
               plotter.AddInteraction(new NPlot.Windows.PlotSurface2D.Interactions.HorizontalDrag());
               plotter.AddInteraction(new NPlot.Windows.PlotSurface2D.Interactions.VerticalDrag());

               plotter.Title = basis.Label;
               plotter.TitleFont = new Font(this.Font, FontStyle.Bold);
               //}

               //Refresh surfaces.
               plotter.Refresh();

          }

          Random rand = new Random();

          private List<double[]> RandomizePoints(int count)
          {
               int rangeY = 10;

               List<double[]> ret = new List<double[]>(count);

               for (int i = 0; i < count; i++)
                    ret.Add(new double[] { i * 5, rand.Next(0, rangeY) });

               return ret;
          }

          private List<double[]> GetPredefinedFitPoints(int curve)
          {
               List<double[]> ret = new List<double[]>();

               switch (curve)
               {
                    case 1:
                         ret.Add(new double[] { 0, 0 });
                         ret.Add(new double[] { 5, 5 });
                         ret.Add(new double[] { 10, 10 });
                         ret.Add(new double[] { 15, 5 });
                         ret.Add(new double[] { 20, 0 });
                         break;
                    case 2:
                         ret.Add(new double[] { 0, 0 });
                         ret.Add(new double[] { 3, 3 });
                         ret.Add(new double[] { 10, 10 });
                         ret.Add(new double[] { 12, 8 });
                         ret.Add(new double[] { 20, 0 });
                         break;
                    case 3:
                         ret.Add(new double[] { 0, 10 });
                         ret.Add(new double[] { 2, 8 });
                         ret.Add(new double[] { 8, 1 });
                         ret.Add(new double[] { 10, 0 });
                         ret.Add(new double[] { 20, 3 });
                         break;
                    case 4:
                         ret.Add(new double[] { 0, 0 });
                         ret.Add(new double[] { 5, 18 });
                         ret.Add(new double[] { 10, 25 });
                         ret.Add(new double[] { 15, 18 });
                         ret.Add(new double[] { 20, 0 });
                         break;
                    case 5:
                         ret.Add(new double[] { 0, 0 });
                         ret.Add(new double[] { 5, 18 });
                         ret.Add(new double[] { 10, 25 });
                         ret.Add(new double[] { 15, 18 });
                         ret.Add(new double[] { 20, 0 });
                         break;
                    case 6:
                         ret.Add(new double[] { 0, 0 });
                         ret.Add(new double[] { 5, 18 });
                         ret.Add(new double[] { 10, 25 });
                         ret.Add(new double[] { 15, 18 });
                         ret.Add(new double[] { 20, 0 });
                         break;
               }

               return ret;
          }

          private void SetupWorkers()
          {
               #region worker1

               p1BRW.WorkerSupportsCancellation = true;
               p1BRW.WorkerReportsProgress = true;
               p1BRW.DoWork += new DoWorkEventHandler(p1BRW_DoWork);
               p1BRW.ProgressChanged += new ProgressChangedEventHandler(p1BRW_ProgressChanged);
               p1BRW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(p1BRW_RunWorkerCompleted);

               #endregion worker1
          }

          private void InitializePlots()
          {
               object plot = new object();

               for (int j = 1; j < 7; j++)
               {
                    switch (j)
                    {
                         case 1:
                              plot = plotSurface2D1;
                              break;
                         case 2:
                              plot = plotSurface2D2;
                              break;
                         case 3:
                              plot = plotSurface2D3;
                              break;
                         case 4:
                              plot = plotSurface2D4;
                              break;
                         case 5:
                              plot = plotSurface2D5;
                              break;
                         case 6:
                              plot = plotSurface2D6;
                              break;
                    }

                    NPlot.Windows.PlotSurface2D plotter = (NPlot.Windows.PlotSurface2D)plot;

                    plotter.Clear();
                    plotter.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                    NPlot.Grid grid = new NPlot.Grid();
                    grid.VerticalGridType = NPlot.Grid.GridType.Coarse;
                    grid.HorizontalGridType = NPlot.Grid.GridType.Coarse;
                    grid.MinorGridPen = new Pen(Color.Blue, 1.0f);
                    grid.MajorGridPen = new Pen(Color.LightGray, 1.0f);

                    plotter.Add(grid);

                    StepPlot stepBalance = new StepPlot();
                    stepBalance.Pen = new Pen(Color.Green, 2);

                    List<Int32> balanceAxis = new List<Int32>();

                    List<decimal> balanceAmount = new List<decimal>();

                    for (int i = 0; i < 10; i++)
                         balanceAxis.Add(0);


                    stepBalance.AbscissaData = balanceAxis;
                    stepBalance.DataSource = balanceAmount;
                    plotter.Add(stepBalance);
                    plotter.ShowCoordinates = true;
                    plotter.AutoScaleAutoGeneratedAxes = true;
                    plotter.RightMenu = NPlot.Windows.PlotSurface2D.DefaultContextMenu;
                    plotter.AddInteraction(new NPlot.Windows.PlotSurface2D.Interactions.AxisDrag(false));
                    plotter.AddInteraction(new NPlot.Windows.PlotSurface2D.Interactions.HorizontalDrag());
                    plotter.AddInteraction(new NPlot.Windows.PlotSurface2D.Interactions.VerticalDrag());

                    //Refresh surfaces.
                    plotter.Refresh();
               }
          }

          #region thread events

          #region worker1

          void p1BRW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
          {
               //m_status.Text = "Thread Finished";
               m_progBar.Value = 0;

               m_running = false;
               m_runButton.Text = "Run";
               m_runButton.BackColor = System.Drawing.Color.Lime;

               Algo.NewBest -= new GA_EventHandler(Algo_NewBest);
          }

          void p1BRW_ProgressChanged(object sender, ProgressChangedEventArgs e)
          {
               if (p1BRW.CancellationPending == true)
                    Algo.Stop();

               m_progBar.Value = e.ProgressPercentage;
               if (e.UserState == null)
                    return;

               List<int> bestChromo = (List<int>)e.UserState;

               if (bestChromo.Count > 0)
               {
                    List<double[]> GAPnts = new List<double[]>();
                    double[] result = EvalChromosome(ref bestChromo, ref GAPnts);

                    object curve = new object();
                    object plot = new object();

                    for (int j = 0; j < 1; j++)
                    {
                         switch (j + 1)
                         {
                              case 1:
                                   curve = p1Curve;
                                   plot = plotSurface2D1;
                                   break;
                              case 2:
                                   curve = p2Curve;
                                   plot = plotSurface2D2;
                                   break;
                              case 3:
                                   curve = p3Curve;
                                   plot = plotSurface2D3;
                                   break;
                              case 4:
                                   curve = p4Curve;
                                   plot = plotSurface2D4;
                                   break;
                              case 5:
                                   curve = p5Curve;
                                   plot = plotSurface2D5;
                                   break;
                              case 6:
                                   curve = p6Curve;
                                   plot = plotSurface2D6;
                                   break;
                         }

                         RBFCurve oldRBF = curve as RBFCurve;
                         CustomBasis GeneticBasis = new CustomBasis(16.383 / 2.0 - result[0], 16.383 / 2.0 - result[1], 16.383 / 2.0 - result[2], 16.383 / 2.0 - result[3]);
                         //List<double[]> fitpoints = new List<double[]>(oldRBF.OriginalFitPoints);
                         List<double[]> fitpoints = new List<double[]>(GAPnts);

                         if (RemoveExtraFitPoints)
                         {
                              fitpoints.RemoveAt(1);
                              fitpoints.RemoveAt(2);
                         }

                         RBFCurve GaRBF = new RBFCurve(null, "GA RBF", fitpoints, GeneticBasis, new Linear(null), 0.0);
                         NPlot.Windows.PlotSurface2D plotter = (NPlot.Windows.PlotSurface2D)plot;

                         plotter.Clear();
                         plotter.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

                         //Add a background grid for better chart readability.
                         NPlot.Grid grid = new NPlot.Grid();
                         grid.VerticalGridType = NPlot.Grid.GridType.Coarse;
                         grid.HorizontalGridType = NPlot.Grid.GridType.Coarse;
                         grid.MinorGridPen = new Pen(Color.Blue, 1.0f);
                         grid.MajorGridPen = new Pen(Color.LightGray, 1.0f);

                         plotter.Add(grid);

                         int length = oldRBF.FitPoints.Count * 5;
                         //Create a step plot instance for the balance chart.
                         LinePlot speed = new LinePlot();
                         LinePlot speed2 = new LinePlot();
                         PointPlot pplot = new PointPlot();
                         PointPlot pplot2 = new PointPlot();

                         pplot2.Marker = new Marker(Marker.MarkerType.Cross1, 10, System.Drawing.Color.Red);

                         speed.Pen = new Pen(Color.Blue, 1);

                         //Create the lists from which to pull data.
                         List<Int32> countaxis = new List<Int32>();
                         List<Int32> Pplotcountaxis = new List<Int32>();
                         List<Int32> Pplotcountaxis2 = new List<Int32>();
                         List<decimal> speedlist = new List<decimal>();
                         List<decimal> speedlist2 = new List<decimal>();

                         for (int i = 0; i < length; i++)
                              countaxis.Add(i);

                         for (int i = 0; i < oldRBF.FitPoints.Count; i++)
                              Pplotcountaxis.Add((int)oldRBF.OriginalFitPoints[i][0]);

                         for (int i = 0; i < GaRBF.FitPoints.Count; i++)
                              Pplotcountaxis2.Add((int)GaRBF.OriginalFitPoints[i][0]);

                         //Populate the balanceAmount list
                         double[] pnt;
                         List<decimal> pointsList = oldRBF.OriginalFitPoints.ConvertAll(new Converter<double[], decimal>((double[] dd) => { return Convert.ToDecimal(dd[1]); }));
                         List<decimal> pointsList2 = GaRBF.OriginalFitPoints.ConvertAll(new Converter<double[], decimal>((double[] dd) => { return Convert.ToDecimal(dd[1]); }));

                         for (int i = 0; i < length; i++)
                         {
                              pnt = new double[] { (double)i, 0 };
                              oldRBF.Value(ref pnt);
                              speedlist.Add(Convert.ToDecimal(pnt[1]));
                              pnt = new double[] { (double)i, 0 };
                              GaRBF.Value(ref pnt);
                              speedlist2.Add(Convert.ToDecimal(pnt[1]));
                         }

                         pplot.AbscissaData = Pplotcountaxis;
                         pplot.DataSource = pointsList;
                         pplot2.AbscissaData = Pplotcountaxis2;
                         pplot2.DataSource = pointsList2;
                         speed.AbscissaData = countaxis;
                         speed.DataSource = speedlist;
                         speed2.AbscissaData = countaxis;
                         speed2.DataSource = speedlist2;

                         //Add stepBalance to plotSurfaceBalance.

                         plotter.Add(speed);
                         plotter.Add(speed2);
                         plotter.Add(pplot);
                         plotter.Add(pplot2);

                         //Balance plot general settings.
                         plotter.ShowCoordinates = true;

                         plotter.AutoScaleAutoGeneratedAxes = true;
                         plotter.AddAxesConstraint(new AxesConstraint.AxisPosition(NPlot.PlotSurface2D.YAxisPosition.Left, 60));

                         plotter.YAxis1.Label = "Y";
                         plotter.YAxis1.LabelFont = new Font(this.Font, FontStyle.Bold);
                         plotter.YAxis1.LabelOffsetAbsolute = true;
                         plotter.XAxis1.Label = "X";
                         plotter.XAxis1.LabelFont = new Font(this.Font, FontStyle.Bold);
                         plotter.YAxis1.LabelOffset = 40;
                         plotter.XAxis1.HideTickText = false;
                         plotter.Padding = 5;

                         plotter.RightMenu = NPlot.Windows.PlotSurface2D.DefaultContextMenu;
                         plotter.AddInteraction(new NPlot.Windows.PlotSurface2D.Interactions.AxisDrag(false));
                         plotter.AddInteraction(new NPlot.Windows.PlotSurface2D.Interactions.HorizontalDrag());
                         plotter.AddInteraction(new NPlot.Windows.PlotSurface2D.Interactions.VerticalDrag());

                         plotter.Title = GeneticBasis.ToString();
                         plotter.TitleFont = new Font(this.Font, FontStyle.Bold);
                         //}

                         //Refresh surfaces.
                         plotter.Refresh();
                    }
                    Application.DoEvents();
               }
          }

          void p1BRW_DoWork(object sender, DoWorkEventArgs e)
          {
               Algo = new ConicalGA(50, 91, GAType.BINARY, false, ConicalGA.GA_Type._2D);
               Algo.SetEval(Eval);
               Algo.NewBest += new GA_EventHandler(Algo_NewBest);
               string text = "";

               this.Invoke(new MethodInvoker(delegate() { text = comboBox1.Text; }));
               Algo.Begin("2doutput.csv", 0.000995, text);
          }

          #endregion worker1

          #endregion thread events

          bool m_running = false;

          private void m_runButton_Click(object sender, EventArgs e)
          {
               
               if (!m_running)
               {
                    while (p1BRW.IsBusy)
                    {
                         p1BRW.CancelAsync();
                         Application.DoEvents();
                    }
                    p1BRW.RunWorkerAsync();
               }
               else
                    p1BRW.CancelAsync();

               m_running = !m_running;
               if (m_running)
               {
                    m_runButton.Text = "Stop";
                    m_runButton.BackColor = System.Drawing.Color.Red;
               }
               else
               {
                    m_runButton.Text = "Run";
                    m_runButton.BackColor = System.Drawing.Color.Lime;
               }

          }

          private void button1_Click(object sender, EventArgs e)
          {
               InstatiatePlotData();
          }

          private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
          {
               switch (comboBox1.Text.ToLower())
               {
                    case "line":
                         Basis = BasisType.Line;
                         break;
                    case "parabolic":
                         Basis = BasisType.Parabolic;
                         break;
                    case "cubic":
                         Basis = BasisType.Cubic;
                         break;
                    case "gaussian":
                         Basis = BasisType.Gaussian;
                         break;
                    case "cubic/parabolic":
                         Basis = BasisType.CubicParabolic;
                         break;
               }
          }
     }
}
