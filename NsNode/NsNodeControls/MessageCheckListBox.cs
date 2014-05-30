using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GabesaBalla
{
	public partial class MessageCheckListBox : Form
	{
		public MessageCheckListBox()
		{
			InitializeComponent();
		}

		public MessageCheckListBox(string message, string text, string select)
		{
			InitializeComponent();
			Message = message;
			this.Text = text;
			SelectString = select;
		}
		public string Message
		{
			get { return m_label.Text; }
			set
			{
				//string message = value.TrimEnd('\n');
				//string[] lines = message.Split('\n');
				m_label.Text = value;
			}
		}
		string m_def;
		public string SelectString
		{
			get { return m_def; }
			set { m_def = value; }
		}

		/// <summary>
		/// Creates a messageCheckListBox object that allows selection different inputted string items
		/// </summary>
		/// <param name="message"></param>
		/// <param name="text"></param>
		/// <param name="items">items to be shown in the checklistbox</param>
		/// <param name="selecteditems"> if dialogboxresult == ok, fills the entered List of strings with the checked items in the checklistbox</string></param>
		/// <param name="defaultcheckstring">enter a string here that will be contained in the items list for default checking</param>
		/// <returns></returns>
		public static DialogResult Show(string message, string text, string[] items, ref List<string> selecteditems, string defaultcheckstring, bool show)
		{
			MessageCheckListBox msb = new MessageCheckListBox(message, text, defaultcheckstring);
			msb.AddItems(items);
			msb.Focus();
			if (show)
			{
				DialogResult dr = msb.ShowDialog();
				if (dr == DialogResult.OK)
					selecteditems = msb.SelectedItems;

				return dr;
			}
			else
			{
				selecteditems = msb.SelectedItems;
				return DialogResult.OK;
			}

		}
		public static DialogResult Show(string message, string text, string[] items, ref List<string> selecteditems)
		{
			return Show(message, text, items, ref selecteditems, null, true);
		}

		private void AddItems(string[] items)
		{
			foreach (string item in items)
			{
				if (item.ToLower().Contains("mark"))
					continue;
				if (SelectString != null)
				{
					if (item.Contains(SelectString))
						m_checklistbox.Items.Add(item, true);
					else
						m_checklistbox.Items.Add(item);
				}
				else
					m_checklistbox.Items.Add(item);
			}
		}
		List<string> SelectedItems
		{
			get
			{
				List<string> items = new List<string>(m_checklistbox.CheckedItems.Count);
				foreach (string s in m_checklistbox.CheckedItems)
				{
					items.Add(s);
				}

				return items;
			}
		}
		private void m_applyButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = sender == m_cancelButton ? DialogResult.Cancel : DialogResult.OK;
			this.Close();
		}

		private void m_cancelButton_Click(object sender, EventArgs e)
		{
			m_applyButton_Click(sender, e);
		}
	}
}
