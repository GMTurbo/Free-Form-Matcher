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
	public partial class FileTextBox : UserControl
	{
		public FileTextBox(bool allowNew)
		{
			m_allownew = allowNew;
			InitializeComponent();
			m_txt.ShortcutsEnabled = true;
			this.MinimumSize = new Size(m_btn.Size.Width + 20, m_txt.MinimumSize.Height);

		}

		public FileTextBox()
			: this(false)
		{ }
		public override string Text
		{
			get
			{
				return Path;
			}
			set
			{
				Path = value;
			}
		}
		public String Path
		{
			get { return m_txt.Text; }
			set
			{
				m_txt.Text = value.Trim();
				m_txt.Select(m_txt.Text.Length, 0);
				m_txt.ScrollToCaret();
				RaisePathChanged();
			}
		}
		[Category("Appearance"), Browsable(true), DefaultValue("All files (*.*)|*.*")]
		public string Filter
		{
			get { return m_filter; }
			set { m_filter = value; }
		}
		string m_filter = "All files (*.*)|*.*";
		public event EventHandler PathChanged;


		public DialogResult SelectFile(string filter)
		{
			if( filter != null && filter.Length > 0 )
			Filter = filter;
			return SelectFile();
		}
		public DialogResult SelectFile()
		{
			OpenFileDialog pfd = new OpenFileDialog();
			pfd.CheckFileExists = false;
			pfd.AddExtension = true;
			pfd.InitialDirectory = Path;
			pfd.Filter = Filter;
			pfd.FilterIndex = 0;
			pfd.RestoreDirectory = true;

			DialogResult res = pfd.ShowDialog(ParentForm);
			if (res == DialogResult.OK)
			{
				Path = System.IO.Path.GetFullPath(pfd.FileName);
				ScrollControlIntoView(this);
			}
			return res;
		}
		private void m_btn_Click(object sender, EventArgs e)
		{
			SelectFile();
		}

		private void m_txt_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				RaisePathChanged();
		}

		bool m_allownew;
		void RaisePathChanged()
		{
			if (PathChanged != null)
				PathChanged(this, new EventArgs());
		}
	}
}
