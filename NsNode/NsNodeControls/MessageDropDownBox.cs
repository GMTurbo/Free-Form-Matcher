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
	public partial class MessageDropDownBox : Form
	{
		string m_def;

		public string SelectString
		{
			get { return m_def; }
			set { m_def = value; }
		}

		public MessageDropDownBox()
		{
			InitializeComponent();
			InitializeGrid();
		}

		Dictionary<int, string[]> m_sections = new Dictionary<int,string[]>();

		Dictionary<int, string[]> Sections
		{
			get { return m_sections; }
			set
			{
				m_sections.Clear();
				m_sections = value;
			}
		}

		static string[] Panels;

		public static DialogResult Show(string[] items, ref Dictionary<int, string[]> selecteditems)
		{
			Panels = items;
			MessageDropDownBox msb = new MessageDropDownBox();
			msb.Focus();
			DialogResult dr = msb.ShowDialog();
			if (dr == DialogResult.OK)
				selecteditems = msb.Sections;

			return dr;

		}
		void InitializeGrid()
		{
			Grid.CellValidating += new DataGridViewCellValidatingEventHandler(Grid_CellValidating);

			Grid.Columns.Clear();
			Grid.Rows.Clear();
			Grid.Columns.AddRange(new DataGridViewTextBoxColumn(),
							new DataGridViewComboBoxColumn());

			Grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
			int i = 0;
			Grid.Columns[i].Name = "Panel";
			Grid.Columns[i].ReadOnly = true;
			Grid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			i++;

			Grid.Columns[i].Name = "Section #";
			Grid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

			Grid.RowHeadersVisible = false;
			Grid.AllowUserToAddRows = false;
			Grid.AllowUserToDeleteRows = false;
		}

		void Grid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			DataGridViewComboBoxCell cell = Grid.CurrentCell as DataGridViewComboBoxCell;

			if (cell != null && !cell.Items.Contains(e.FormattedValue))
			{

				// Insert the new value into position 0
				// in the item collection of the cell
				cell.Items.Insert(0, e.FormattedValue);
				// When setting the Value of the cell, the  
				// string is not shown until it has been
				// comitted. The code below will make sure 
				// it is committed directly.
				if (Grid.IsCurrentCellDirty)
				{
					// Ensure the inserted value will 
					// be shown directly.
					// First tell the DataGridView to commit 
					// itself using the Commit context...
					Grid.CommitEdit
					    (DataGridViewDataErrorContexts.Commit);
				}
				// ...then set the Value that needs 
				// to be committed in order to be displayed directly.
				cell.Value = cell.Items[0];
			}
		}

		private void AddItems(string[] panels, int sections)
		{
			foreach (string panel in panels)
			{
				DataGridViewComboBoxCell reff = new DataGridViewComboBoxCell();
				DataGridViewTextBoxCell txt2A = new DataGridViewTextBoxCell();
				DataGridViewRow dataGridRow = new DataGridViewRow();
				//ComboBox reff = new ComboBox();
				reff.MaxDropDownItems = sections;
				//reff.DataSource = 
				txt2A.Value = panel;
				for (int i = 1; i <= sections; i++)
					reff.Items.Add(i);
				try
				{
					dataGridRow.Cells.Add(txt2A);
					dataGridRow.Cells.Add(reff);
					Grid.Rows.Add(dataGridRow);
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message, "Error");
				}
			}
		}
		void ClearAll()
		{
			//Tool = Length = Laminate = "";
			Grid.Rows.Clear();
			//Log.Clear();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			int sections = 0;
			try{
				sections = Convert.ToInt32(textBox1.Text);
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message,"Error");
			}
			if (sections > 0)
			{
				AddItems(Panels, sections);
				button1.Enabled = false;
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			string[] ret;
			string section;
			//string name;
			List<string> tmp = new List<string>();
			// fill the return dictionary with the section (key) and it's associated panel (value)
            try
            {
                foreach (DataGridViewRow row in Grid.Rows)
                {
                    tmp.Clear();
                    section = row.Cells[1].Value.ToString();
                    if (Sections.TryGetValue(Convert.ToInt32(section), out ret))
                    {
                        tmp.AddRange(ret);
                        tmp.Add(row.Cells[0].Value.ToString());
                        Sections.Remove(Convert.ToInt32(section)); // remove old one
                        Sections.Add(Convert.ToInt32(row.Cells[1].Value.ToString()), tmp.ToArray()); // add new one
                    }
                    else
                    {
                        tmp.Add(row.Cells[0].Value.ToString());
                        Sections.Add(Convert.ToInt32(section), tmp.ToArray());
                    }
                }

                this.DialogResult = sender == button3 ? DialogResult.Cancel : DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
			  string s = ex.Message;
                MessageBox.Show("All panels must have a section assigned to them before proceeding!", "MPF File Generator", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
		}

		private void button3_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void MessageDropDownBox_FormClosed(object sender, FormClosedEventArgs e)
		{
			//this.DialogResult = DialogResult.Cancel;
		}
	}
}
