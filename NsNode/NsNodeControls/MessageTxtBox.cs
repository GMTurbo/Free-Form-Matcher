using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GabesaBalla
{
	public partial class MessageTxtBox : Form
	{
		public MessageTxtBox()
		{
			InitializeComponent();
		}
		public MessageTxtBox(string message, string text)
		{
			InitializeComponent();
			Text = text;
			if(message!=null)
				Message = message;
		}
		public static DialogResult Show(string message, string text)
		{
			string s = "";
			return Show(message, text, true, ref s);
		}
		public static DialogResult Show(string message, string text, bool rdonly, ref string s)
		{
			MessageTxtBox txt = new MessageTxtBox(message, text);
			//txt.textBox1.Text = s;
			txt.ReadOnly = rdonly;
			txt.Focus();
			txt.textBox1.DeselectAll();
			txt.textBox1.Refresh();
			DialogResult r = txt.ShowDialog();
			if(!rdonly)
				s = txt.textBox1.Text;
			return r;
		}
		public string Message
		{
			get { return textBox1.Text; }
			set
			{
				string message = value.TrimEnd('\n');
				string[] lines = message.Split('\n');
				textBox1.Lines = lines;
			}
		}
		public bool ReadOnly
		{
			get { return textBox1.ReadOnly; }
			set { textBox1.ReadOnly = value; }
		}
		private void button1_Click(object sender, EventArgs e)
		{
			this.DialogResult = sender == cancel ? DialogResult.Cancel : DialogResult.OK;
			this.Close();
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			button1_Click(sender, e);
		}
	}
}
