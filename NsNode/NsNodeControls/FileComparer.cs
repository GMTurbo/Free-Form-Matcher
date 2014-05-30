using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace NsNodeControls
{
    public partial class FileComparer : Form
    {
        public event CompareHandler Compare;

        public delegate void CompareHandler(object sender, CompareEvents e);

        public FileComparer()
        {
            InitializeComponent();
        }

        string m_path1 = null;

        string m_path2 = null;

        List<double[]> diff = null;

        public List<double[]> Diff
        {
            get { return diff; }
            set { diff = value; }
        }

        public string Path1
        {
            get { return m_path1; }
            set 
            { 
                m_path1 = value;
                pictureBox1.Visible = value != null;
                label1.Text = value != null ? System.IO.Path.GetFileName(value) : "Drop File #1 Here";
            }
        }

        public string Path2
        {
            get { return m_path2; }
            set 
            {
                m_path2 = value;
                pictureBox2.Visible = value != null;
                label2.Text = value != null ? System.IO.Path.GetFileName(value) : "Drop File #2 Here";
            }
        }

        private void FileComparer_DragEnter(object sender, DragEventArgs e)
        {
            // make sure they're actually dropping files (not text or anything else)
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                e.Effect = DragDropEffects.Copy;// allow them to continue(without this, the cursor stays a "NO" symbol)
        }

        private string FormatedFileSize(string fileName)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
            double fileSize = (fileInfo.Length / 1024.0);

            if (fileSize < 1024)
                return fileSize.ToString("F01") + " KB";
            else
            {
                fileSize /= 1024.0;

                if (fileSize < 1024)
                    return fileSize.ToString("F01") + " MB";
                else
                {
                    fileSize /= 1024;
                    return fileSize.ToString("F01") + " GB";
                }
            }
        }

        private void FileComparer_DragDrop(object sender, DragEventArgs e)
        {
            string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string s in filenames)
                LoadFile(s);
        }

        private void LoadFile(string s)
        {
            if (Path1 == null)
                Path1 = s;
            if (Path2 == null && Path1 != null )
                Path2 = Path1 == s ? null : s;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Path1 = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Path2 = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Path1 != null && Path2 != null)
                CompareFiles();

            this.WindowState = FormWindowState.Minimized;
        }

        private void CompareFiles()
        {
             string ext = Path.GetExtension(Path1);

             if (Path.GetExtension(Path1) != Path.GetExtension(Path2))
             {
                  MessageBox.Show("You can only compare files with the same extension", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                  return;
             }

             if (ext != ".surf" && ext != ".nht")
             {
                  MessageBox.Show("You can only compare .surf and .nht files right now", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                  return;
             }

             switch (ext)
             {
                  case ".nht":
                       {
                            List<string> filepaths = new List<string>(2);
                            filepaths.Add(Path1);
                            filepaths.Add(Path2);

                            if (Compare != null)
                                 Compare(this, new CompareEvents(filepaths));

                            break;
                       }
                  case ".surf":
                       {
                            List<double[]> points1 = ReadSurfFile(Path1);
                            List<double[]> points2 = ReadSurfFile(Path2);

                            if (points1.Count == points2.Count)
                            {
                                 Diff = new List<double[]>(points1.Count);
                                 for (int i = 0; i < points1.Count; i++)
                                      Diff.Add(new double[] { points1[i][0], points1[i][1], points2[i][2] - points1[i][2] });
                            }

                            if (Compare != null)
                                 Compare(this, new CompareEvents(Diff, points1, points2));
                            break;
                       }
             }

        }

        private List<double[]> ReadSurfFile(string path)
        {
            List<double[]> vertices = new List<double[]>();

            if (!File.Exists(path))
                return null; //file not found

            using (StreamReader sr = new StreamReader(path))
            {
                string line = sr.ReadLine();
                string[] vals;
                char[] param = new char[] { ',', ' ' };
                while (line != null)
                {
                    vals = line.Split(param, StringSplitOptions.RemoveEmptyEntries);
                    if (vals.Length < 3)
                        return null;

                    double[] pnt = new double[]{0, 0, 0};
                    try
                    {
                        pnt[0] = Convert.ToDouble(vals[0]);
                        pnt[1] = Convert.ToDouble(vals[1]);
                        pnt[2] = Convert.ToDouble(vals[2]);
                        vertices.Add(pnt);
                    }
                    catch { }
                    line = sr.ReadLine();

                }
            }

            return vertices;
        }
    }

    public class CompareEvents : EventArgs
    {
        List<double[]> m_data = null;
        List<double[]> m_pnt1 = null;
        List<double[]> m_pnt2 = null;
        List<string> m_filepaths = null;

        public CompareEvents() { }

        public CompareEvents(List<double[]> d)
        {
            m_data = new List<double[]>(d);
        }

         /// <summary>
         /// surf file comparisons
         /// </summary>
         /// <param name="d"></param>
         /// <param name="pnts1"></param>
         /// <param name="pnts2"></param>
        public CompareEvents(List<double[]> d, List<double[]> pnts1, List<double[]> pnts2)
        {
            m_data = new List<double[]>(d);
            m_pnt1 = pnts1;
            m_pnt2 = pnts2;
        }

         /// <summary>
         /// nht file comparisons
         /// </summary>
         /// <param name="filepaths">nht file paths</param>
        public CompareEvents(List<string> filepaths)
        {
             m_filepaths = new List<string>(filepaths);
        }

        public List<double[]> Data
        {
            get { return m_data; }
        }

        public List<double[]> Points1
        {
            get { return m_pnt1; }
        }

        public List<double[]> Points2
        {
            get { return m_pnt2; }
        }

        public List<string> FilePaths
        {
             get { return m_filepaths; }
        }
    }
}
