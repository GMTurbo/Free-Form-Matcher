using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RBF;
using devDept.Eyeshot.Standard;
using devDept.Eyeshot.Geometry;
using devDept.Eyeshot;
using GA;
using RBFBasis;

namespace UI
{
    public partial class _3dForm : Form
    {
        int GridSize = 3;

        public _3dForm()
        {
            InitializeComponent();
            SetupWorker();

            stat = ((string message) => { m_status.Text = message; });

            CustomBasis GeneticBasis = null;

            Eval = new Evaluator((ref List<int> binaryCoded, ref List<int> bestChromo) =>
            {
                //if(!m_running)

                double error = 0;

                List<double[]> gaPnts = new List<double[]>();

                double[] result = EvalChromosome(ref binaryCoded, ref gaPnts);

                GeneticBasis = new CustomBasis(16.383 / 2.0 - result[0], 16.383 / 2.0 - result[1], 16.383 / 2.0 - result[2], 16.383 / 2.0 - result[3]);

                double[] pnt1, pnt2, dx1, dx2;

                //List<double[]> fitpoints = new List<double[]>(oldRBF.OriginalFitPoints);

                List<double[]> fitpoints = new List<double[]>(gaPnts);

                found = new SurfaceRBF(null, "target", fitpoints, GeneticBasis, targetPoly, 0.0);

                //List<double[]> errorSurfPnts = new List<double[]>();
                double gridCount = GridSize;
                for (double i = 0; i < gridCount; i += 1.0)
                {
                    for (double j = 0; j < gridCount; j += 1.0)
                    {
                        pnt1 = new double[] { (i / (gridCount - 1)) * (Max[0] - Min[0]) + Min[0], (j / (gridCount - 1)) * (Max[1] - Min[1]) + Min[1], 0 };
                        pnt2 = new double[] { (i / (gridCount - 1)) * (Max[0] - Min[0]) + Min[0], (j / (gridCount - 1)) * (Max[1] - Min[1]) + Min[1], 0 };
                        target.Value(ref pnt1);
                        found.Value(ref pnt2);

                        error += 10.0 * Math.Pow(Math.Abs(pnt2[2] - pnt1[2]), 1);
                        //errorSurfPnts.Add(new double[] { i * 20.0, j * 20.0, pnt1[2] - pnt2[2] });
                    }
                }

                gridCount = 10.0;
                for (double i = 0; i < gridCount; i += 1.0)
                {
                    for (double j = 0; j < gridCount; j += 1.0)
                    {
                        pnt1 = new double[] { (i / (gridCount - 1)) * (Max[0] - Min[0]) + Min[0], (j / (gridCount - 1)) * (Max[1] - Min[1]) + Min[1], 0 };
                        pnt2 = new double[] { (i / (gridCount - 1)) * (Max[0] - Min[0]) + Min[0], (j / (gridCount - 1)) * (Max[1] - Min[1]) + Min[1], 0 };
                        target.Value(ref pnt1);
                        found.Value(ref pnt2);

                        error += Math.Pow(Math.Abs(pnt2[2] - pnt1[2]), 1);
                    }
                }

                //int count = 0;

                //double pntError = 1000;

                //gaPnts.ForEach((pnt) =>
                //{
                //     dx1 = new double[2];
                //     dx2 = new double[2];
                //     pnt2 = new double[] { pnt[0], pnt[1] };
                //     GaRBF.First(ref pnt, ref dx1);
                //     oldRBF.First(ref pnt, ref dx2);
                //     error += 2 * Math.Pow(pnt2[1] - pnt[1], 2);
                //     error += 2 * Math.Pow(Math.Pow(dx2[0] - dx1[0], 2) + Math.Pow(dx2[1] - dx1[1], 2), 2);
                //});

                //gaPnts.ForEach((pnt) =>
                //     {
                //          if (oldRBF.OriginalFitPoints.Find((Oldpnt) => { return Oldpnt[0] == pnt[0] && Oldpnt[1] == pnt[1]; }) != null)
                //               count++;
                //          else
                //               error *= 4;
                //     });

                //if (count != 0)
                //    pntError /= ((double)count / 5.0) * 1000.0;

                // error += pntError;

                //GC.Collect();
                return Math.Pow(error, -1);
            });

        }
        double[] m_max = new double[] { double.MinValue, double.MinValue, double.MinValue };
        double[] m_min = new double[] { double.MaxValue, double.MaxValue, double.MaxValue };

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

        private void SetupWorker()
        {
            GAWorker.DoWork += new DoWorkEventHandler(GAWorker_DoWork);
            GAWorker.ProgressChanged += new ProgressChangedEventHandler(GAWorker_ProgressChanged);
            GAWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GAWorker_RunWorkerCompleted);
            GAWorker.WorkerSupportsCancellation = true;
            GAWorker.WorkerReportsProgress = true;
        }

        void GAWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            m_progBar.Value = 0;

            m_running = false;
            m_runButton.Text = "Run";
            m_runButton.BackColor = System.Drawing.Color.Lime;

            Algo.NewBest -= new GA_EventHandler(Algo_NewBest);
        }

        void GAWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (GAWorker.CancellationPending == true)
                Algo.Stop();

            m_progBar.Value = e.ProgressPercentage;

            if (e.UserState == null)
                return;

            List<int> bestChromo = (List<int>)e.UserState;

            if (bestChromo.Count > 0)
            {
                List<double[]> gaPnts = new List<double[]>();

                double[] result = EvalChromosome(ref bestChromo, ref gaPnts);

                CustomBasis GeneticBasis = new CustomBasis(16.383 / 2.0 - result[0], 16.383 / 2.0 - result[1], 16.383 / 2.0 - result[2], 16.383 / 2.0 - result[3]);

                List<double[]> fitpoints = new List<double[]>(gaPnts);

                //for (int i = 0; i < fitpoints.Count; i++)
                //fitpoints[i][2] += 15; //offset

                SurfaceRBF tmpSurf = new SurfaceRBF(null, "best", fitpoints, GeneticBasis, targetPoly, 0.0);

                double[] max = new double[3]; double[] min = new double[3];
                max[0] = max[1] = max[2] = -1e9; //start max low
                min[0] = min[1] = min[2] = +1e9; //start min high

                fitpoints.ForEach((double[] v) =>
                {
                    for (int i = 0; i < v.Length; i++) //get fit points' bounding box
                    {
                        max[i] = Math.Max(v[i], Max[i]);
                        min[i] = Math.Min(v[i], Min[i]);
                    }
                });

                double[] pnt1 = new double[3]; double[] pnt2 = new double[3];

                List<double[]> errorSurfPnts = new List<double[]>();
                double gridCount = 10.0;
                List<devDept.Eyeshot.Labels.TextOnly> labels = new List<devDept.Eyeshot.Labels.TextOnly>();
                for (double i = 0; i < gridCount; i += 1.0)
                {
                    for (double j = 0; j < gridCount; j += 1.0)
                    {
                        pnt1 = new double[] { (i / (gridCount - 1)) * (Max[0] - Min[0]) + Min[0], (j / (gridCount - 1)) * (Max[1] - Min[1]) + Min[1], 0 };
                        pnt2 = new double[] { (i / (gridCount - 1)) * (Max[0] - Min[0]) + Min[0], (j / (gridCount - 1)) * (Max[1] - Min[1]) + Min[1], 0 };
                        target.Value(ref pnt1);
                        found.Value(ref pnt2);

                        errorSurfPnts.Add(new double[] { (i / (gridCount - 1)) * (Max[0] - Min[0]) + Min[0], (j / (gridCount - 1)) * (Max[1] - Min[1]) + Min[1], Math.Pow(pnt2[2] - pnt1[2], 2) });
                        labels.Add(new devDept.Eyeshot.Labels.TextOnly(new Point3D((i / (gridCount - 1)) * (Max[0] - Min[0]) + Min[0] + GridSize * 20, (j / (gridCount - 1)) * (Max[1] - Min[1]) + Min[1], Math.Pow(pnt1[2] - pnt2[2], 2)), Math.Pow(((pnt1[2] - pnt2[2]) / pnt1[2]), 1).ToString("#0.00"), new Font(FontFamily.GenericSansSerif, 12.0f), Color.White, ContentAlignment.BottomCenter));

                    }
                }

                SurfaceRBF tmpSurf2 = new SurfaceRBF(null, "error", errorSurfPnts, targetBasis, targetPoly, 0.0);
                List<KeyValuePair<double[], double>> errorpts = tmpSurf2.GetMeshPointsCvt(50, 50, max, min, null);
                MulticolorOnVerticesMesh errorMesh = new MulticolorOnVerticesMesh(0);
                CreateMesh(errorpts, 50, ref errorMesh);
                errorMesh.RegenMode = devDept.Eyeshot.Standard.Entity.regenType.RegenAndCompile;
                errorMesh.EntityData = "errorSurface";
                errorMesh.Translate(GridSize * 20, 0, 0);
                AddEntity(errorMesh, false);

                List<KeyValuePair<double[], double>> pts = tmpSurf.GetMeshPointsCvt(60, 60, max, min, null);
                MulticolorOnVerticesMesh bestMesh = new MulticolorOnVerticesMesh(0);

                CreateMesh(pts, 60, ref bestMesh);
                bestMesh.RegenMode = devDept.Eyeshot.Standard.Entity.regenType.RegenAndCompile;
                bestMesh.EntityData = "bestSurface";
                bestMesh.Translate(-2 * GridSize * 10, 0, 0);
                devDept.Eyeshot.Labels.TextOnly bestBasis = new devDept.Eyeshot.Labels.TextOnly(new Point3D(((gridCount / 2.0) / (gridCount - 1)) * (Max[0] - Min[0]) + Min[0] - (2 * GridSize * 10), ((gridCount / 2.0) / (gridCount - 1)) * (Max[1] - Min[1]) + Min[1], 20), GeneticBasis.ToString(), new Font(FontFamily.GenericSansSerif, 12.0f), Color.White, ContentAlignment.BottomCenter);

                viewportProfessional1.Labels.Clear();
                labels.ForEach((label) => { viewportProfessional1.Labels.Add(label); });
                viewportProfessional1.Labels.Add(bestBasis);

                AddEntity(bestMesh, false);
                //for (int i = 0; i < fitpoints.Count; i++)
                //fitpoints[i][2] -= 15; //offset
                PointCloud fits = new PointCloud(doubleToPoint3d(fitpoints), 7.5f, Color.Blue);
                fits.EntityData = "bestpoints";

                AddEntity(fits, true);
                pts.Clear();
                bestChromo.Clear();
                //GC.Collect();
            }
        }

        void GAWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Algo = new ConicalGA(50, 56 + (int)Math.Pow(GridSize, 2) * 7, GAType.BINARY, true, ConicalGA.GA_Type._3D);
            Algo.SetEval(Eval);
            Algo.NewBest += new GA_EventHandler(Algo_NewBest);
            string text = "";

            this.Invoke(new MethodInvoker(delegate() { text = comboBox1.Text; }));

            Algo.Begin("3doutput.csv", 0.1, text);
        }
        void Algo_NewBest(List<int> bestChromo, double fitness, double prog)
        {
            GAWorker.ReportProgress((int)(prog * 100.0), (object)bestChromo);
            stat("Best Fitness: " + fitness);
            //if (Algo.Population.Count < 100)
            //    System.Threading.Thread.Sleep(50);
        }

        enum BasisType
        {
            Gaussian = 0,
            Line,
            Parabolic,
            Cubic,
            CubicParabolic
        }

        Random rand = new Random();

        SurfaceRBF target;
        RBFBasis.IBasisFunction targetBasis;
        RBFPolynomials.Paraboloid targetPoly;
        BasisType m_targetBasis = BasisType.Cubic;
        MulticolorOnVerticesMesh m_targetMesh = new MulticolorOnVerticesMesh(0);

        public MulticolorOnVerticesMesh TargetMesh
        {
            get { return m_targetMesh; }
            set { m_targetMesh = value; }
        }

        private BasisType TargetBasis
        {
            get { return m_targetBasis; }
            set { m_targetBasis = value; }
        }

        SurfaceRBF found;
        //RBFBasis.CustomBasis foundBasis;
        //RBFPolynomials.Paraboloid foundPoly;

        GA.Evaluator Eval = null;

        BackgroundWorker GAWorker = new BackgroundWorker();

        ConicalGA Algo;// = new ConicalGA(10, 60, GAType.BINARY);

        delegate void UpdateStatus(string message);

        UpdateStatus stat;

        private List<double[]> RandomizePoints(int count)
        {
            int rangeZ = 15;

            List<double[]> ret = new List<double[]>(count);

            for (int i = 0; i < count; i++)
                for (int j = 0; j < count; j++)
                    ret.Add(new double[] { i * 20.0, j * 20.0, rand.Next(0, rangeZ) });

            return ret;
        }

        double[] EvalChromosome(ref List<int> chromo, ref List<double[]> gapnts)
        {
            double a = 0, b = 0, c = 0, d = 0, e = 0;
            int sign = 1;

            List<int> a_array = new List<int>(chromo.GetRange(0, 14));
            List<int> b_array = new List<int>(chromo.GetRange(14, 14));
            List<int> c_array = new List<int>(chromo.GetRange(28, 14));
            List<int> d_array = new List<int>(chromo.GetRange(42, 14));
            int length = (int)Math.Pow(GridSize, 2) * 7;
            List<int> Heights_array = new List<int>(chromo.GetRange(56, length));

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

            double z = 0;
            List<int> tmp = new List<int>(7);
            List<int> tmp2 = new List<int>();
            List<double[]> tmp3 = new List<double[]>();

            for (int i = 0; i < GridSize * GridSize; i++)
            {
                tmp = Heights_array.GetRange(i * 7, 7);
                z = (double)(Convert.ToInt32(string.Join("", tmp.ToArray()), 2)) / 127.0;
                tmp2.Add((int)(Min[2] + z * (Max[2] - Min[2])));
            }

            double gridCount = (double)GridSize;
            for (double i = 0; i < gridCount; i += 1.0)
            {
                for (double j = 0; j < gridCount; j += 1.0)
                    tmp3.Add(new double[] { (i / (gridCount - 1)) * (Max[0] - Min[0]) + Min[0], (j / (gridCount - 1)) * (Max[1] - Min[1]) + Min[1], tmp2[(int)i * GridSize + (int)j] });

            }

            //tmp2.ForEach((pnt) => { tmp3.Add(new double[] { (double)(pnt[0]), (double)(pnt[1]) }); });
            gapnts = tmp3;
            return new double[] { a, b, c, d };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (comboBox1.Text.ToLower())
            {
                case "line":
                    TargetBasis = BasisType.Line;
                    break;
                case "parabolic":
                    TargetBasis = BasisType.Parabolic;
                    break;
                case "cubic":
                    TargetBasis = BasisType.Cubic;
                    break;
                case "gaussian":
                    TargetBasis = BasisType.Gaussian;
                    break;
                case "cubic/parabolic":
                    TargetBasis = BasisType.CubicParabolic;
                    break;
            }

            CreateNewTargetSurface();
        }

        private void CreateNewTargetSurface()
        {
            targetPoly = new RBFPolynomials.Paraboloid(null);

            switch (TargetBasis)
            {
                case BasisType.Cubic:
                    targetBasis = new RBFBasis.PolyHarmonic3(null);
                    break;
                case BasisType.Gaussian:
                    targetBasis = new RBFBasis.Gaussian(null);
                    break;
                case BasisType.Line:
                    targetBasis = new RBFBasis.CustomBasis(null, 0, 0, 1, 0);
                    break;
                case BasisType.Parabolic:
                    targetBasis = new RBFBasis.ThinPlateSpline2(null);
                    break;
                case BasisType.CubicParabolic:
                    targetBasis = new RBFBasis.CustomBasis(null, 1, 1, -1, 0);
                    break;
            }

            List<double[]> pnts = RandomizePoints(GridSize);
            target = new SurfaceRBF(null, "target", pnts, targetBasis, targetPoly, 0.0);

            Max[0] = Max[1] = Max[2] = -1e9; //start max low
            Min[0] = Min[1] = Min[2] = +1e9; //start min high

            pnts.ForEach((double[] v) =>
            {
                for (int i = 0; i < v.Length; i++) //get fit points' bounding box
                {
                    Max[i] = Math.Max(v[i], Max[i]);
                    Min[i] = Math.Min(v[i], Min[i]);
                }
            });

            List<KeyValuePair<double[], double>> pts = target.GetMeshPointsCvt(60, 60, Max, Min, null);

            CreateMesh(pts, 60, ref m_targetMesh);
            TargetMesh.RegenMode = devDept.Eyeshot.Standard.Entity.regenType.RegenAndCompile;
            TargetMesh.EntityData = "targetSurface";
            AddEntity(TargetMesh, false);
            PointCloud fits = new PointCloud(doubleToPoint3d(pnts), 5.0f, Color.Red);
            fits.EntityData = "fitpoints";

            AddEntity(fits, true);

            GC.Collect();

        }

        private Point3D[] doubleToPoint3d(List<double[]> pnts)
        {
            List<Point3D> pnt3ds = new List<Point3D>(pnts.Count);

            pnts.ForEach((pnt) => { pnt3ds.Add(new Point3D(pnt[0], pnt[1], pnt[2])); });

            return pnt3ds.ToArray();
        }

        public void AddEntity(Entity lp, bool update)
        {
            if (lp == null)
                return;

            List<Entity> removeMe = new List<Entity>();

            foreach (Entity e in viewportProfessional1.Entities)
            {
                if (e.EntityData == lp.EntityData && e.GetType() == lp.GetType())
                    removeMe.Add(e);
            }

            removeMe.ForEach((ent) => { viewportProfessional1.Entities.Remove(ent); });

            viewportProfessional1.Entities.Add(lp);

            if (update)
            {
                //viewportProfessional1.Entities.Regen();

                viewportProfessional1.Update();
                viewportProfessional1.Refresh();
            }
        }

        public void ReplaceEntity(Entity lp, bool update)
        {
            if (lp == null)
                return;

            int replaceMe = -1;

            foreach (Entity e in viewportProfessional1.Entities)
            {
                if (e.EntityData == lp.EntityData && e.GetType() == lp.GetType())
                {
                    replaceMe = viewportProfessional1.Entities.IndexOf(e);
                    break;
                }
            }

            if (replaceMe == -1)
                AddEntity(lp, update);
            else
            {
                viewportProfessional1.Entities[replaceMe] = lp;
                if (update)
                {
                    viewportProfessional1.Update();
                    viewportProfessional1.Refresh();
                }
            }
            GC.Collect();
        }

        public int CreateMesh(List<KeyValuePair<double[], double>> vertices, int cols, ref MulticolorOnVerticesMesh surf)
        {
            List<Point3D> verts = new List<Point3D>(vertices.Count);
            List<double> colors = new List<double>(vertices.Count);
            vertices.ForEach(delegate(KeyValuePair<double[], double> d)
            {
                if (d.Key.Length == 3)
                {
                    verts.Add(new Point3D(d.Key));
                    colors.Add(d.Value);
                }
            });

            return CreateMesh(verts.ToArray(), colors.ToArray(), cols, ref surf);
        }

        public int CreateMesh(Point3D[] vertices, double[] colorvals, int cols, ref MulticolorOnVerticesMesh surf)
        {
            int rows = vertices.Length / cols;

            // create the mesh
            System.Drawing.Color c = System.Drawing.Color.SlateBlue;

            if (surf.Vertices.Length != vertices.Length)
                surf.ResizeVertices(vertices.Length);

            surf.NormalAveragingMode = devDept.Eyeshot.Standard.Mesh.normalAveragingType.Averaged;
            surf.EntityData = this; //point back to this object

            // set the vertices
            surf.Vertices = vertices;
            surf.UpdateBoundingBox();

            //color the vertices
            int i = ReColorMesh(colorvals, ref surf);
            //if (i != vertices.Length)
            //return -3;

            // set vertex indices
            surf.Triangles.Clear();
            surf.Triangles.Capacity = (rows - 1) * (cols - 1) * 2;
            for (int j = 0; j < (rows - 1); j++)
            {
                for (i = 0; i < (cols - 1); i++)
                {

                    surf.Triangles.Add(new Mesh.Triangle(i + j * cols,
                                                                              i + j * cols + 1,
                                                                              i + (j + 1) * cols + 1));
                    surf.Triangles.Add(new Mesh.Triangle(i + j * cols,
                                                                              i + (j + 1) * cols + 1,
                                                                              i + (j + 1) * cols));
                }
            }

            //Area = 0;

            //TargetMesh.Triangles.ForEach((t) =>
            //{
            //     //A = 1/2 | (x₃ - x₁) x (x₃ - x₂) | ....... "x" means cross product
            //     Vector3D vec1, vec2;
            //     vec1 = new Vector3D(m_mesh.Vertices[t.V3].X - m_mesh.Vertices[t.V1].X, m_mesh.Vertices[t.V3].Y - m_mesh.Vertices[t.V1].Y, m_mesh.Vertices[t.V3].Z - m_mesh.Vertices[t.V1].Z);
            //     vec2 = new Vector3D(m_mesh.Vertices[t.V3].X - m_mesh.Vertices[t.V2].X, m_mesh.Vertices[t.V3].Y - m_mesh.Vertices[t.V2].Y, m_mesh.Vertices[t.V3].Z - m_mesh.Vertices[t.V2].Z);
            //     Vector3D cross = Vector3D.Cross(vec1, vec2);
            //     Area += cross.Length / 2.0;
            //});

            surf.ComputeEdges();
            surf.ComputeNormals();
            surf.NormalAveragingMode = Mesh.normalAveragingType.None;
            //m_mesh.Rotate(Math.PI, new Vector3D(0, 0, 1));
            return surf.Triangles.Count;
        }

        double[] m_colorvals = null;

        double[] m_colorMaxMin = null;

        public double[] ColorMaxMin
        {
            get { return m_colorMaxMin; }
            set
            {
                m_colorMaxMin = value;
            }
        }

        public int ReColorMesh(double[] colorvals, ref MulticolorOnVerticesMesh surf)
        {
            if (colorvals == null)
                return ReColorMesh(ref surf);

            m_colorvals = colorvals;
            double max = -1e9;
            double min = 1e9;
            double avg;
            double stddev = BLAS.StandardDeviation(colorvals, out avg, out max, out min);

            ColorMaxMin = new double[] { min, max };
            //foreach (double d in colorvals)
            //{
            //     max = Math.Max(max, d);
            //     min = Math.Min(min, d);
            //}
            double q1 = avg + 2 * stddev;
            double q2 = avg - 2 * stddev;
            System.Drawing.Color c;
            int i = 0;
            foreach (double d in colorvals)
            {
                c = Utilities.GetColor(q1, q2, d);
                //c = Utilities.GetColor(max, min, d);
                surf.SetVertex(i++, c.R, c.G, c.B);
            }
            return 0;
        }

        private int ReColorMesh(ref MulticolorOnVerticesMesh surf)
        {
            System.Drawing.Color c = Color.Green;
            int i = 0;
            //double f;
            for (i = 0; i < TargetMesh.Vertices.Length; i++)
                surf.SetVertex(i, c.R, c.G, c.B);

            return i;
        }
        bool m_running = false;
        private void m_runButton_Click(object sender, EventArgs e)
        {
            if (!m_running)
            {
                while (GAWorker.IsBusy)
                {
                    GAWorker.CancelAsync();
                    Application.DoEvents();
                }
                GAWorker.RunWorkerAsync();
            }
            else
                GAWorker.CancelAsync();

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
    }

    public static class Utilities
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

    public static class BLAS
    {
        #region Solvers

        static public double[] FitCubic(List<double[]> pnts)
        {
            //fit a quadratic or least squares fit one
            double[,] A = new double[pnts.Count, 3];
            double[,] b = new double[pnts.Count, 1];
            double max = -1.0e9;
            double min = 1.0e9;
            int i = 0;
            foreach (double[] p in pnts)
            {
                A[i, 0] = p[0] * p[0];
                A[i, 1] = p[0];
                A[i, 2] = 1;
                b[i, 0] = p[1];
                i++;
                max = Math.Max(max, p[0]);
                min = Math.Min(min, p[0]);
            }
            double[,] coef;
            if (pnts.Count == 3)
                coef = BLAS.SimultaneousSolver(A, b);
            else
                coef = BLAS.LeastSquaresSolver(A, b);
            if (coef == null)
                return null;
            return new double[] { coef[0, 0], coef[1, 0], coef[2, 0] };
        }

        //
        static public double PolynomialEval(double[] coef, double x)
        {
            double sum = 0;
            for (int i = 0; i < coef.Length; i++)
                sum += coef[i] * Math.Pow(x, coef.Length - 1 - i);
            return sum;
            //return coef[0] * x * x + coef[1] * x + coef[2];
        }

        /// <summary>
        /// Finds the maximum value of a polynomial on  returns -1 if failed, 0 if max outside limits, 1 if succeeds, 2 if at lowerbound and 3 if at upperbound
        /// </summary>
        /// <param name="coef"></param>
        /// <param name="xrange"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        static public int PolynomialMax(double[] coef, double[] xrange, out double[] max)
        {
            //find the greatest of 9 starting points
            int STARTINGPTS = 9;
            double step = (xrange[1] - xrange[0]) / (double)(STARTINGPTS - 1);
            double[] start = new double[] { -1e9, -1e9 };
            double[] val = new double[] { -1e9, -1e9, -1e9, -1e9 };//x,y,d,dd
            for (int i = 0; i < STARTINGPTS; i++)
            {
                val[0] = step * i + xrange[0];
                val[1] = PolynomialEval(coef, val[0]);
                if (val[1] > start[1])
                {
                    start[0] = val[0];
                    start[1] = val[1];
                }
            }
            if (start[0] < -1e8 || start[1] < -1e8)//if the starting points failed abort
            { max = null; return -1; }

            start.CopyTo(val, 0);//set up starting point

            double[] dcoef = new double[coef.Length - 1];
            //get derivatives
            for (int i = 0; i < dcoef.Length; i++)
                dcoef[i] = coef[i] * (coef.Length - 1 - i);

            double[] ddcoef = null;
            //optional second der
            if (dcoef.Length > 1)
            {
                ddcoef = new double[dcoef.Length - 1];
                for (int i = 0; i < ddcoef.Length; i++)
                    ddcoef[i] = dcoef[i] * (dcoef.Length - 1 - i);
            }
            double res;
            //nr-loop to find max, keep it inbounds!
            for (int inwt = 0; inwt < 50; inwt++)
            {
                if (val[0] < xrange[0] || xrange[1] < val[0])//quit if sliding out of bounds
                {
                    max = new double[2];
                    max[0] = val[0] < xrange[0] ? xrange[0] : xrange[1] < val[0] ? xrange[1] : val[0];
                    max[1] = PolynomialEval(coef, max[0]);
                    return 0;
                }
                //calc derivative
                val[2] = PolynomialEval(dcoef, val[0]);
                if (Math.Abs(val[2]) < 1e-8)
                {
                    max = new double[2];
                    max[0] = val[0];
                    max[1] = PolynomialEval(coef, max[0]);
                    return Math.Abs(max[0] - xrange[1]) < 1e-8 ? 3 ://upperbound //this is stupid because only rare cases will max exactly on the bounds
                              Math.Abs(max[0] - xrange[0]) < 1e-8 ? 2 : 1;//lowerbound
                }
                if (ddcoef != null)
                {
                    val[3] = PolynomialEval(ddcoef, val[0]);
                    res = val[2] / val[3];
                }
                else
                {
                    //do shitty pertibation method
                    res = 0;
                }
                val[0] -= res;
            }
            max = null;
            return -1;
        }

        static public double Interpolate(double p, double max, double min)
        {
            return (max - min) * p + min;
        }

        static public double[,] LeastSquaresSolver(double[,] A, double[,] b)
        {
            // calculate normal arrays
            double[,] AtA = MatrixProduct(MatrixTranspose(A), A);
            double[,] Atb = MatrixProduct(MatrixTranspose(A), b);

            // if this fails due to mismatched sizes quit
            if (AtA == null || Atb == null) return null;

            // solve the system
            return SimultaneousSolver(AtA, Atb);
        }

        static public double[,] SimultaneousSolver(double[,] A, double[,] b)
        {
            // create the [A|b] matrix
            double[,] Ab = MatrixColConcat(A, b);
            // reduce it
            int[] index;
            Ab = RowEchelon(Ab, out index);
            //Ab = rref(Ab, out index);
            // return the solution
            return BackSolver(Ab, index);
        }

        static public bool Invert3x3(ref double[,] M)
        {
            //double[,] inv = new double[3, 3];
            //double det;

            //inv[0, 0] = M[1, 1] * M[2, 2] - M[1, 2] * M[2, 1];
            //inv[0, 1] = M[0, 2] * M[2, 1] - M[0, 1] * M[2, 2];
            //inv[0, 2] = M[0, 1] * M[1, 2] - M[0, 2] * M[1, 1];

            //inv[1, 0] = M[1, 2] * M[2, 0] - M[1, 0] * M[2, 2];
            //inv[1, 1] = M[0, 0] * M[2, 2] - M[0, 2] * M[2, 0];
            //inv[1, 2] = M[0, 2] * M[1, 0] - M[0, 0] * M[1, 2];

            //inv[2, 0] = M[1, 0] * M[2, 1] - M[1, 1] * M[2, 0];
            //inv[2, 1] = M[0, 1] * M[2, 0] - M[0, 0] * M[2, 1];
            //inv[2, 2] = M[0, 0] * M[1, 1] - M[0, 1] * M[1, 0];

            //det = M[0, 0] * inv[0, 0] + M[0, 1] * inv[1, 0] + M[0, 2] * inv[2, 0];

            //double[] ma = new double[9];
            double[] mr;

            //for (int i = 0; i < 3; i++)
            //     for (int j = 0; j < 3; j++)
            //          ma[i * 3 + j] = M[i, j];

            if (m3_inverse(out mr, M) == 0)
                return false;

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    //inv[i, j] /= det;
                    //M[i, j] = inv[i, j] / det;
                    M[i, j] = mr[i * 3 + j];
                }
            return true;
            //return inv;
        }

        static public bool Invert4x4(ref double[,] M)
        {
            //double[] ma = new double[16];
            double[] mr;

            //for (int i = 0; i < 4; i++)
            //     for (int j = 0; j < 4; j++)
            //          ma[i * 4 + j] = M[i, j];

            if (m4_inverse(out mr, M) == 0)
                return false;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    //inv[i, j] /= det;
                    //M[i, j] = inv[i, j] / det;
                    M[i, j] = mr[i * 4 + j];
                }
            return true;
        }

        #region Row Echelon

        static double[,] RowEchelon(double[,] A, out int[] index)
        {
            // make a copy
            double[,] M = MatrixCopy(A);

            int[] i = ArrangeRows(M);

            ReduceRows(M);

            index = i;
            return M;
        }

        static int[] ArrangeRows(double[,] M)
        {
            int col = 0;
            int r2 = 0;
            int i = 0;
            int[] index = new int[M.GetLength(0)];
            for (i = 0; i < index.Length; i++)
                index[i] = i;
            //return index;
            //double[,] order = new double[M.GetLength( 0 ) , 1];
            //for( int o = 0 ; o < M.GetLength( 0 ) ; o++ )
            //    order[o,0] = o;

            //M = MatrixColConcat( M , order );

            // order the matrix by searching for leading 0's
            for (int r1 = 0; r1 < M.GetLength(0); r1++)
            {
                r2 = r1;
                while (M[r2, col] == 0)
                {
                    // check the next row
                    r2++;
                    if (r2 >= M.GetLength(0))
                    {
                        // go to the next column
                        if (++col >= M.GetLength(1))
                            return index;
                        // go back to the first row
                        r2 = r1;
                        continue;
                    }
                }
                if (r2 != r1)
                {
                    RowSwap(M, r2, r1);
                    i = index[r1];
                    index[r1] = index[r2];
                    index[r2] = i;
                }
                //else col++;
            }
            return index;
        }

        static void ReduceRows(double[,] M)
        {
            for (int col = 0, r1 = 0; r1 < M.GetLength(0); r1++, col++)
            {
                SetPivotToOne(M, r1);

                for (int r2 = r1 + 1; r2 < M.GetLength(0); r2++)
                    SubtractRowMultiple(M, M[r2, col], r1, r2);
            }
        }

        static void RowSwap(double[,] M, int r1, int r2)
        {
            double temp;

            for (int k = 0; k < M.GetLength(1); k++)
            {
                temp = M[r1, k];
                M[r1, k] = M[r2, k];
                M[r2, k] = temp;
            }
        }

        static void SetPivotToOne(double[,] M, int row)
        {
            //find pivot
            int col = 0;
            while (M[row, col] == 0)
            {
                if (++col >= M.GetLength(1))
                    return;
            }

            double pivot = M[row, col];

            for (int k = 0; k < M.GetLength(1); k++)
                M[row, k] /= pivot;
        }

        static void SubtractRowMultiple(double[,] M, double multiple, int r1, int r2)
        {
            for (int k = 0; k < M.GetLength(1); k++)
                M[r2, k] -= multiple * M[r1, k];
        }

        #endregion

        static double[,] BackSolver(double[,] Ab, int[] index)
        {
            double[,] x = new double[Ab.GetLength(0), 1];

            for (int i = Ab.GetLength(0) - 1; i >= 0; i--)
            {
                // xi = bi
                x[i, 0] = Ab[i, Ab.GetLength(1) - 1];

                // xi = bi - ( Aij xj )
                for (int j = i + 1; j < Ab.GetLength(0); j++)
                    x[i, 0] -= Ab[i, j] * x[j, 0];
            }
            return x;
            //unswap
            //double[,] ret = new double[x.GetLength(0), 1];
            //for (int i = 0; i < ret.GetLength(0); i++)
            //{
            //     ret[index[i], 0] = x[i, 0];
            //}
            //return ret;
        }

        #endregion

        #region Matrix Math

        static public double[,] MatrixProduct(double[,] A, double[,] B)
        {
            if (A.GetLength(1) != B.GetLength(0)) return null;

            double[,] prod = new double[A.GetLength(0), B.GetLength(1)];

            for (int i = 0; i < prod.GetLength(0); i++)
                for (int j = 0; j < prod.GetLength(1); j++)
                {
                    prod[i, j] = 0;
                    for (int k = 0; k < A.GetLength(1); k++)
                        prod[i, j] += A[i, k] * B[k, j];
                }

            return prod;
        }

        static public double[,] MatrixTranspose(double[,] A)
        {
            double[,] T = new double[A.GetLength(1), A.GetLength(0)];

            for (int i = 0; i < A.GetLength(0); i++)
                for (int j = 0; j < A.GetLength(1); j++)
                    T[j, i] = A[i, j];
            return T;
        }

        static public double[,] MatrixCopy(double[,] A)
        {
            double[,] M = new double[A.GetLength(0), A.GetLength(1)];
            for (int i = 0; i < A.GetLength(0); i++)
                for (int j = 0; j < A.GetLength(1); j++)
                    M[i, j] = A[i, j];
            return M;
        }

        static double[,] MatrixColConcat(double[,] A, double[,] B)
        {
            // check height
            if (A.GetLength(0) != B.GetLength(0)) return null;

            double[,] M = new double[A.GetLength(0), A.GetLength(1) + B.GetLength(1)];

            for (int i = 0; i < M.GetLength(0); i++)
                for (int j = 0; j < M.GetLength(1); j++)
                {
                    M[i, j] = j < A.GetLength(1) ? A[i, j] : B[i, j - A.GetLength(1)];
                }
            return M;
        }

        static double[,] MatrixRowConcat(double[,] A, double[,] B)
        {
            // check height
            if (A.GetLength(1) != B.GetLength(1)) return null;

            double[,] M = new double[A.GetLength(0) + B.GetLength(0), A.GetLength(1)];

            for (int i = 0; i < M.GetLength(0); i++)
                for (int j = 0; j < M.GetLength(1); j++)
                {
                    M[i, j] = i < A.GetLength(0) ? A[i, j] : B[i - A.GetLength(0), j];
                }
            return M;
        }

        static int m4_inverse(out double[] mr, double[,] ma)
        {
            mr = new double[16];
            double mdet = m4_det(ma);
            double[,] mtemp = new double[3, 3];
            int i, j, sign;
            if (Math.Abs(mdet) < 0.0005)
            {
                //m4_identity(mr);
                return (0);
            }
            for (i = 0; i < 4; i++)
                for (j = 0; j < 4; j++)
                {
                    sign = 1 - ((i + j) % 2) * 2;
                    m4_submat(ma, mtemp, i, j);
                    mr[i + j * 4] = (m3_det(mtemp) * sign) / mdet;
                }
            return (1);
        }
        static void m4_submat(double[,] mr, double[,] mb, int i, int j)
        {
            int di, dj, si, sj;
            // loop through 3x3 submatrix
            for (di = 0; di < 3; di++)
            {
                for (dj = 0; dj < 3; dj++)
                {
                    // map 3x3 element (destination) to 4x4 element (source)
                    si = di + ((di >= i) ? 1 : 0);
                    sj = dj + ((dj >= j) ? 1 : 0);
                    // copy element
                    mb[di, dj] = mr[si, sj];
                }
            }
        }
        static double m4_det(double[,] mr)
        {
            double det, result = 0, i = 1;
            double[,] msub3 = new double[3, 3];
            int n;
            for (n = 0; n < 4; n++, i *= -1)
            {
                m4_submat(mr, msub3, 0, n);
                det = m3_det(msub3);
                result += mr[0, n] * det * i;
            }
            return (result);
        }
        static double m3_det(double[,] mat)
        {
            double det;
            det = mat[0, 0] * (mat[1, 1] * mat[2, 2] - mat[2, 1] * mat[1, 2])
                - mat[0, 1] * (mat[1, 0] * mat[2, 2] - mat[2, 0] * mat[1, 2])
                + mat[0, 2] * (mat[1, 0] * mat[2, 1] - mat[2, 0] * mat[1, 1]);
            return (det);
        }
        static int m3_inverse(out double[] mr, double[,] ma)
        {
            mr = new double[9];
            double det = m3_det(ma);
            if (Math.Abs(det) < 0.0005)
            {

                //m3_identity(mr);
                return (0);
            }

            mr[0] = ma[1, 1] * ma[2, 2] - ma[1, 2] * ma[2, 1] / det;
            mr[1] = -(ma[0, 1] * ma[2, 2] - ma[2, 1] * ma[0, 2]) / det;
            mr[2] = ma[0, 1] * ma[1, 2] - ma[1, 1] * ma[0, 2] / det;
            mr[3] = -(ma[1, 0] * ma[2, 2] - ma[1, 2] * ma[2, 0]) / det;
            mr[4] = ma[0, 0] * ma[2, 2] - ma[2, 0] * ma[0, 2] / det;
            mr[5] = -(ma[0, 0] * ma[1, 2] - ma[1, 0] * ma[0, 2]) / det;
            mr[6] = ma[1, 0] * ma[2, 1] - ma[2, 0] * ma[1, 1] / det;
            mr[7] = -(ma[0, 0] * ma[2, 1] - ma[2, 0] * ma[0, 1]) / det;
            mr[8] = ma[0, 0] * ma[1, 1] - ma[0, 1] * ma[1, 0] / det;
            return (1);
        }

        #endregion

        #region Utilities

        static public double dot(double[] a, double[] b)
        {
            return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
        }
        static public double[] cross(double[] a, double[] b)
        {
            double[] c = new double[3];
            int i1, i2;
            for (int i = 0; i < 3; i++)
            {
                i1 = (i + 1) % 3;
                i2 = (i + 2) % 3;
                c[i] = a[i1] * b[i2] - a[i2] * b[i1];
            }
            return c;
        }
        static public void split(ref double[] dx, out double[] dy)
        {
            dy = new double[] { 0, 1, dx[1] };
            dx = new double[] { 1, 0, dx[0] };
        }
        static public double magnitude(double[] p)
        {
            return Math.Sqrt(dot(p, p));
        }
        static public bool is_equal(double a, double b)
        {
            return Math.Abs(a - b) < 1e-7;
        }
        static public bool is_equal(double a, double b, double tol)
        {
            return Math.Abs(a - b) < Math.Abs(tol);
        }

        static public double[] subtract(double[] a, double[] b)
        {
            double[] ret = new double[a.Length];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = a[i] - b[i];
            return ret;
        }
        static public double[] add(double[] a, double[] b)
        {
            double[] ret = new double[a.Length];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = a[i] + b[i];
            return ret;
        }

        #endregion

        public static double StandardDeviation(IList<double> doubleList, out double average, out double max, out double min)
        {
            average = doubleList.Average();
            double sumOfDerivation = 0;
            max = -1e9;
            min = 1e9;
            foreach (double value in doubleList)
            {
                max = Math.Max(max, value);
                min = Math.Min(min, value);
                sumOfDerivation += (value) * (value);
            }
            double sumOfDerivationAverage = sumOfDerivation / doubleList.Count;
            return Math.Sqrt(sumOfDerivationAverage - (average * average));
        }

        private static double[,] rref(double[,] matrix, out int[] index)
        {
            int[] inn = ArrangeRows(matrix);
            index = inn;
            int lead = 0, rowCount = matrix.GetLength(0), columnCount = matrix.GetLength(1);
            for (int r = 0; r < rowCount; r++)
            {
                if (columnCount <= lead) break;
                int i = r;
                while (matrix[i, lead] == 0)
                {
                    i++;
                    if (i == rowCount)
                    {
                        i = r;
                        lead++;
                        if (columnCount == lead)
                        {
                            lead--;
                            break;
                        }
                    }
                }
                for (int j = 0; j < columnCount; j++)
                {
                    double temp = matrix[r, j];
                    matrix[r, j] = matrix[i, j];
                    matrix[i, j] = temp;
                }
                double div = matrix[r, lead];
                for (int j = 0; j < columnCount; j++) matrix[r, j] /= div;
                for (int j = 0; j < rowCount; j++)
                {
                    if (j != r)
                    {
                        double sub = matrix[j, lead];
                        for (int k = 0; k < columnCount; k++) matrix[j, k] -= (sub * matrix[r, k]);
                    }
                }
                lead++;
            }

            return matrix;
        }
    }
}
