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
	public partial class LogTextBox : RichTextBox
	{
		public static Form Show(ILogger log, Form owner)
		{
			Form f = new Form();
			f.Owner = owner;
			f.SuspendLayout();
			LogTextBox box = new LogTextBox();
			box.Dock = DockStyle.Fill;
			box.SetLogText(log);
			box.AttachLog(log);
			f.Controls.Add(box);
			f.ResumeLayout();
			f.StartPosition = FormStartPosition.CenterParent;
			f.FormBorderStyle = FormBorderStyle.SizableToolWindow;
			f.Width = 500;
			f.Height = 300;
			f.Text = "";
			f.Show();
			f.BringToFront();

			return f;
		}
		public LogTextBox()
		{
			InitializeComponent();
			WordWrap = false;
		}
		public LogTextBox(ILogger ilog)
		{
			AttachLog(ilog);
		}

		public void DetachLog(ILogger ilog)
		{
			ilog.LogChanged -= ilog_LogChanged;
		}
		public void AttachLog(ILogger ilog)
		{
			ilog.LogChanged += new EventHandler(ilog_LogChanged);
			
		}
		void ilog_LogChanged(object sender, EventArgs e)
		{
			if (sender is ILogger)
				SetLogText(sender as ILogger);
		}

		public void SetLogText(ILogger log)
		{
			Clear();
			if (log == null)
				return;

			NsNodes.Entry[] entries = log.Entries;
			foreach (NsNodes.Entry ent in entries)
			{
				switch (ent.Priority)
				{
					case LogPriority.Warning:
						SelectionFont = Lumberjack.FONT_WARNING;
						SelectionColor = Lumberjack.COLOR_WARNING;
						break;
					case LogPriority.Error:
						SelectionFont = Lumberjack.FONT_ERROR;
						SelectionColor = Lumberjack.COLOR_ERROR;
						break;
					default:
						SelectionFont = Lumberjack.FONT_MESSAGE;
						SelectionColor = Lumberjack.COLOR_MESSAGE;
						break;
				}
				SelectedText = ent.ToString();
				SelectedText = "\n";
			}
		}

		void writeLogItem_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.DefaultExt = ".log";
			sfd.Filter = "Log files (*.log)|*.log|Rich Text files (*.rtf)|*.txt|All files (*.*)|*.*";
			sfd.AddExtension = true;
			sfd.OverwritePrompt = true;
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				string path = sfd.FileName;

				if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(path)))
					System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));

				SaveFile(path);
			}
		}
		void copyAllItem_Click(object sender, System.EventArgs e)
		{
			SelectAll();
			Copy();
			SelectionLength = 0;
		}
	}
}
