using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using NsNodes;
using System.Collections;
using System.Threading;

namespace NsNodes
{
     static partial class Naming
     {
          /// <summary>
          /// This function will autoname a filename (just increment a numerical value at the end of the name and before the extension i.e. mold1.surf)
          /// </summary>
          /// <param name="Directory">Directory you want to put it</param>
          /// <param name="Filename">A starter filename (mold.surf for example)</param>
          /// <param name="fixedName">the part of the name you want to fix (mold for example)</param>
          /// <param name="newName">get a copy of the new filename</param>
          /// <returns>returns the complete filepath</returns>
          static public string AutoName(string Directory, string Filename, string fixedName, out string newName)
          {
               int i = 0;
               while (File.Exists(Directory + "\\" + Filename))
               {
                    if (i == 0)
                    {
                         Filename = Filename.Replace(fixedName, fixedName + "1");
                         i = 1;
                    }
                    else
                    {
                         Filename = Filename.Replace(i.ToString(), (i + 1).ToString());
                         i++;
                    }
               }
               newName = Filename;
               return Directory + "\\" + Filename;
          }
     }

     public class Lumberjack : IDisposable, ILogger
     {
          public Lumberjack()
               : this(null) { }
          public Lumberjack(string path)
          {
               m_path = path; //set the path, file will automatically be opened on first call
               Log(string.Format("Log File Created: [{0}]", DateTime.Now.ToShortDateString()), LogPriority.Message);
          }
          ~Lumberjack()
          {
               Dispose();
          }

          #region Fonts/Colors

          static public Font FONT_MESSAGE = new Font(FontFamily.GenericSansSerif, 9.0f, FontStyle.Regular);
          static public Font FONT_WARNING = new Font(FontFamily.GenericSansSerif, 9.0f, FontStyle.Italic);
          static public Font FONT_ERROR = new Font(FontFamily.GenericSansSerif, 9.0f, FontStyle.Bold);


          static public Color COLOR_MESSAGE = Color.Black;//if you change this again i will modify nsnode to format your hardrive
          static public Color COLOR_WARNING = Color.Goldenrod;
          static public Color COLOR_ERROR = Color.Firebrick;

          #endregion

          #region IDisposable Members

          public void Dispose()
          {
               if (File != null)
               {
                    File = null;
               }
          }

          #endregion

          #region Properties

          public string Path
          {
               get { return m_path; }
               set { m_path = value; }
          }
          public int Count
          {
               get { return m_entries.Count; }
          }
          public bool bReverse = false;

          #endregion

          #region ILogger Members

          public void OnLog(object sender, EventArgs<string, LogPriority> e)
          {
               Log(e.ValueT, e.ValueP);
          }
          public void OnLog(object sender, EventArgs<string> e)
          {
               Log(e.Value, LogPriority.Message);
          }
          public void OnWarning(object sender, EventArgs<string> e)
          {
               Log(e.Value, LogPriority.Warning);
          }
          public void OnError(object sender, EventArgs<string> e)
          {
               Log(e.Value, LogPriority.Error);
          }
          public void OnDebug(object sender, EventArgs<string> e)
          {
               Log(e.Value, LogPriority.Debug);
          }
          public void OnClear(object sender, EventArgs<string> e)
          {
               Clear();
               if (e != null && e.Value != null && e.Value.Length > 0)
                    Log(e.Value, LogPriority.Message);
          }

          public string[] Messages
          {
               get
               {
                    string[] ret = new string[m_entries.Count];
                    int i = bReverse ? ret.Length - 1 : 0;
                    int inc = bReverse ? -1 : 1;
                    foreach (Entry e in m_entries)
                    {
                         ret[i] = e.ToString();
                         i += inc;
                    }
                    return ret;
               }
          }
          public Entry[] Entries
          {
               get
               {
                    return m_entries.ToArray();
               }
          }

          public event EventHandler LogChanged;

          #endregion

          #region Direct Methods
          public bool Dbg(string s)
          {
               return Log(s, LogPriority.Debug);
          }
          public bool Msg(string s)
          {
               return Log(s, LogPriority.Message);
          }
          public bool Wrn(string s)
          {
               return Log(s, LogPriority.Warning);
          }
          public bool Err(string s)
          {
               return Log(s, LogPriority.Error);
          }

          public bool Log(string s, LogPriority p)
          {
               Entry e = new Entry(s, p);
               return Log(e);
          }
          bool Log(Entry e)
          {
#if !DEBUG
			if( e.Priority == LogPriority.Debug)
				return false; //dont long debug messages if release mode
#endif

               WriteLine(e);
               m_entries.Add(e);
               if (LogChanged != null)
                    LogChanged(this, new EventArgs());
               return true;
          }
          public bool Clear()
          {
               File = null; //close and clear the file
               //File = new StreamWriter(Path,false);//dont append
               m_entries.Clear();
               if (LogChanged != null)
                    LogChanged(this, new EventArgs());
               Log(string.Format("Log File Created: [{0}]", DateTime.Now.ToShortDateString()), LogPriority.Message);
               return true;
          }
          //public bool Log(string s, Font font)
          //{
          //     Entry e = new Entry(s,font);
          //     WriteLine(e);
          //     Entries.Add(e);
          //     if (LogChanged != null)
          //          LogChanged(this, new EventArgs());
          //     return true;
          //}
          //public bool Log(KeyValuePair<string, Font> error)
          //{
          //     return Log(error.Key, error.Value);
          //}

          void WriteLine(Entry e)
          {
               if (File != null)
               {
                    File.WriteLine(e.ToString());
                    File.Flush();
               }
          }

          StreamWriter File
          {
               get
               {
                    if (Path == null)
                         return null;
                    if (m_txt == null)
                    {
                         try
                         {
                              if (!Directory.Exists(System.IO.Path.GetDirectoryName(Path)))
                                   Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));

                              m_txt = new StreamWriter(Path, false);
                              m_txt.AutoFlush = true;
                         }
                         catch (Exception ex)
                         {

                         }
                    }
                    return m_txt;
               }
               set
               {
                    if (m_txt != null) // dont leak
                    {
                         try
                         {
                              m_txt.Close();
                         }
                         catch { }
                    }
                    m_txt = value;
               }
          }
          StreamWriter m_txt;

          string m_path;

          List<Entry> m_entries = new List<Entry>(10);

          #endregion

     }
     public class Entry
     {
          public Entry(string msg, LogPriority priority)
          {
               m_msg = msg;
               m_time = DateTime.Now;
               m_priority = priority;
          }

          private readonly string m_msg;
          private DateTime m_time;
          private LogPriority m_priority;

          public override string ToString()
          {
               return string.Format("[{0}] {1}: {2}", m_time.ToLongTimeString(), m_priority.ToString(), m_msg);
          }
          public LogPriority Priority
          {
               get { return m_priority; }
          }
          public string Message
          {
               get { return m_msg; }
          }
     }


     /// <summary>
     /// ThreadSafe Singleton Logger Implementation
     /// </summary>
     public class LumberJackSingleton : ILogger
     {
          private static volatile LumberJackSingleton instance;
          private static object syncRoot = new Object();

          private LumberJackSingleton() { }

          public static LumberJackSingleton Instance
          {
               get
               {
                    if (instance == null)
                    {
                         lock (syncRoot)
                         {
                              if (instance == null)
                              {
                                   instance = new LumberJackSingleton();
                                   m_tsEntriesI = Queue.Synchronized(m_EntriesInput);
                                   string now = string.Format("NSFG-{0:yyyy-MM-dd_hh-mm-ss-tt}.log", DateTime.Now);
                                   LumberJackSingleton.Instance.Cleanup(20);
                                   LumberJackSingleton.Instance.Path = Directory.GetCurrentDirectory() + "\\NSFG Logs\\" + now;
                              }
                         }
                    }

                    return instance;
               }
          }

          public bool Close
          {
               set { m_quit = value; }
          }

          /// <summary>
          /// We don't want to have a million log files, so we are going to have a rolling count of the last intput size logs
          /// </summary>
          public void Cleanup(int size)
          {
               string dir = Directory.GetCurrentDirectory() + "\\NSFG Logs\\";

               if (!Directory.Exists(dir))
                    return;

               DirectoryInfo DI = new DirectoryInfo(dir);

               FileInfo[] files = DI.GetFiles();

               int logfileCount = 0;

               foreach (FileInfo f in files)
               {
                    if (f.FullName.ToLower().Contains("nsfg") && System.IO.Path.GetExtension(f.FullName) == ".log" && !f.FullName.ToLower().Contains("copy"))
                         logfileCount++;
               }

               if (logfileCount + 1 > size)
               {
                    // Now read the creation time for each file
                    DateTime[] creationTimes = new DateTime[files.Length];
                    for (int i = 0; i < files.Length; i++)
                         creationTimes[i] = files[i].CreationTime;

                    // sort it
                    Array.Sort(creationTimes, files);
                    Array.Reverse(creationTimes);

                    if (creationTimes.Length + 1 < size)
                         return;
                    
                    List<FileInfo> toBdeleted = new List<FileInfo>();

                    for (int i = size-1; i < creationTimes.Length; i++)
                         for (int j = 0; j < files.Length; j++)
                              if (files[j].CreationTime == creationTimes[i])
                                   toBdeleted.Add(files[j]);

                    for (int i = 0; i < toBdeleted.Count; i++)
                         File.Delete(toBdeleted[i].FullName);
                   
               }
          }

          public void Log(string message, LogPriority p)
          {
               m_mutex.WaitOne();
               m_tsEntriesI.Enqueue(new Entry(message, p));
               m_mutex.ReleaseMutex();
          }

          public void Log(string message)
          {
               Log(message, LogPriority.Debug);
          }

          #region ILogger Members

          public void OnLog(object sender, EventArgs<string, LogPriority> e)
          {
               Log(e.ValueT, e.ValueP);
          }
          public void OnClear(object sender, EventArgs<string> e)
          {
               m_mutex.WaitOne();
               m_tsEntriesI.Clear();
               m_mutex.ReleaseMutex();
          }
          public void OnLog(object sender, EventArgs<string> e)
          {
               Log(e.Value, LogPriority.Message);
          }
          public void OnWarning(object sender, EventArgs<string> e)
          {
               Log(e.Value, LogPriority.Warning);
          }
          public void OnError(object sender, EventArgs<string> e)
          {
               Log(e.Value, LogPriority.Error);
          }
          public void OnDebug(object sender, EventArgs<string> e)
          {
               Log(e.Value, LogPriority.Debug);
          }

          public string[] Messages
          {
               get { return null; }
          }

          public Entry[] Entries
          {
               get
               {
                    Queue tmp = new Queue(m_tsEntriesI);
                    List<Entry> ret = new List<Entry>(tmp.Count);

                    while (tmp.Count > 0)
                         ret.Add(tmp.Dequeue() as Entry);

                    return ret.ToArray();
               }
          }

          public string Path
          {
               get { return m_path; }
               set
               {
                    m_path = value;
                    m_quit = false;
                    if (m_first)
                    {
                         if (File.Exists(m_path))
                              File.Delete(m_path);
                         m_first = false;
                    }

                    if (m_path != null)
                    {
                         Thread filewriter = new Thread(() =>
                              {
                                   if (!Directory.Exists(System.IO.Path.GetDirectoryName(Path)))
                                        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));

                                   m_txt = TextWriter.Synchronized(new StreamWriter(Path, true));

                                   while (!m_quit)
                                   {
                                        m_mutex.WaitOne();

                                        while (m_tsEntriesI.Count > 0)
                                        {
                                             Entry tmp = m_tsEntriesI.Dequeue() as Entry;
                                             m_txt.WriteLine(tmp.ToString());
                                        }

                                        m_mutex.ReleaseMutex();
                                        m_txt.Flush();
                                        Thread.Sleep(500);

                                   }
                                   m_txt.Close();

                                   FileInfo f = new FileInfo(Path);
                                   if (f.Length == 0)
                                        File.Delete(Path);
                              });

                         
                         filewriter.IsBackground = true; // THIS IS IMPORTANT.  Without this, this thread will run indefinitely even after the main program has closed
                         filewriter.Start();
                    }

               }
          }

          /// <summary>
          /// Manually stop the output file thread.  Once stopped, it can be restarted by setting a new Path
          /// </summary>
          public bool Stop
          {
               get { return m_quit; }
               set { m_quit = value; }
          }

          public int Count
          {
               get { return m_tsEntriesI.Count; }
          }

          TextWriter m_txt;

          static Queue m_EntriesInput = new Queue();
          static Queue m_tsEntriesI = new Queue();

          Mutex m_mutex = new Mutex(false);

          bool m_quit = false;

          bool m_first = true;

          string m_path;

          public event EventHandler LogChanged;

          #endregion

     }
}
