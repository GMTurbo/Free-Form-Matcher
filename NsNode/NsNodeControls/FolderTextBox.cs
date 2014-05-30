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
	[DefaultEvent("PathChanged")]
	public partial class FolderTextBox : UserControl
	{
		public FolderTextBox(bool allowNew)
		{
			m_allownew = allowNew;
			InitializeComponent();
			m_txt.ShortcutsEnabled = true;
			this.MinimumSize = new Size(m_btn.Size.Width + 20, m_txt.MinimumSize.Height);

		}

    	public FolderTextBox() : this(false) 
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

		public event EventHandler PathChanged;

		private void m_btn_Click(object sender, EventArgs e)
		{
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Path;
            fbd.ShowNewFolderButton = m_allownew;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                Path = fbd.SelectedPath;
                //int value = this.Right;
                ScrollControlIntoView(this);
            }
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

