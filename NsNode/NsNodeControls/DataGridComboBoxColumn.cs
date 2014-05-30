using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Security.Permissions;

namespace NsNodeControls
{
	public class DataGridComboBoxColumn : DataGridColumnStyle
	{
		private CustomComboBox customComboBoxPicker1 =
			  new CustomComboBox();

		public CustomComboBox Box
		{
			get { return customComboBoxPicker1; }
		}

		// The isEditing field tracks whether or not the user is
		// editing data with the hosted control.
		private bool isEditing;

		public DataGridComboBoxColumn()
			: base()
		{
			customComboBoxPicker1.Visible = false;
		}

		protected override void Abort(int rowNum)
		{
			isEditing = false;
			customComboBoxPicker1.TextChanged -=
			    new EventHandler(ComboBoxTextChanged);
			Invalidate();
		}

		protected override bool Commit
		    (CurrencyManager dataSource, int rowNum)
		{
			customComboBoxPicker1.Bounds = Rectangle.Empty;

			customComboBoxPicker1.TextChanged -=
			    new EventHandler(ComboBoxTextChanged);

			if (!isEditing)
				return true;

			isEditing = false;

			try
			{
				string value = customComboBoxPicker1.Text;
				SetColumnValueAtRow(dataSource, rowNum, value);
			}
			catch (Exception)
			{
				Abort(rowNum);
				return false;
			}

			Invalidate();
			return true;
		}

		protected override void Edit(
		    CurrencyManager source,
		    int rowNum,
		    Rectangle bounds,
		    bool readOnly,
		    string displayText,
		    bool cellIsVisible)
		{
			string value =
			    GetColumnValueAtRow(source, rowNum).ToString();
			if (cellIsVisible)
			{
				customComboBoxPicker1.Bounds = new Rectangle
				    (bounds.X + 2, bounds.Y + 2,
				    bounds.Width - 4, bounds.Height - 4);
				customComboBoxPicker1.Text = value;
				customComboBoxPicker1.Visible = true;
				customComboBoxPicker1.TextChanged +=
				    new EventHandler(ComboBoxTextChanged);
			}
			else
			{
				customComboBoxPicker1.Text = value;
				customComboBoxPicker1.Visible = false;
			}

			if (customComboBoxPicker1.Visible)
				DataGridTableStyle.DataGrid.Invalidate(bounds);

			customComboBoxPicker1.Focus();
		}

		protected override Size GetPreferredSize(
		    Graphics g,
		    object value)
		{
			return new Size(100, customComboBoxPicker1.PreferredHeight + 4);
		}

		protected override int GetMinimumHeight()
		{
			return customComboBoxPicker1.PreferredHeight + 4;
		}

		protected override int GetPreferredHeight(Graphics g,
		    object value)
		{
			return customComboBoxPicker1.PreferredHeight + 4;
		}

		protected override void Paint(Graphics g,
		    Rectangle bounds,
		    CurrencyManager source,
		    int rowNum)
		{
			Paint(g, bounds, source, rowNum, false);
		}

		protected override void Paint(
		    Graphics g,
		    Rectangle bounds,
		    CurrencyManager source,
		    int rowNum,
		    bool alignToRight)
		{
			Paint(
			    g, bounds,
			    source,
			    rowNum,
			    Brushes.Red,
			    Brushes.Blue,
			    alignToRight);
		}

		protected override void Paint(
		    Graphics g,
		    Rectangle bounds,
		    CurrencyManager source,
		    int rowNum,
		    Brush backBrush,
		    Brush foreBrush,
		    bool alignToRight)
		{
			string val = 
			    GetColumnValueAtRow(source, rowNum).ToString();
			Rectangle rect = bounds;
			g.FillRectangle(backBrush, rect);
			rect.Offset(0, 2);
			rect.Height -= 2;
			g.DrawString(val,
			    this.DataGridTableStyle.DataGrid.Font,
			    foreBrush, rect);
		}

		protected override void SetDataGridInColumn(DataGrid value)
		{
			base.SetDataGridInColumn(value);
			if (customComboBoxPicker1.Parent != null)
			{
				customComboBoxPicker1.Parent.Controls.Remove
				    (customComboBoxPicker1);
			}
			if (value != null)
			{
				value.Controls.Add(customComboBoxPicker1);
			}
		}

		private void ComboBoxTextChanged(object sender, EventArgs e)
		{
			// Remove the handler to prevent it from being called twice in a row.
			customComboBoxPicker1.TextChanged -=
			    new EventHandler(ComboBoxTextChanged);
			this.isEditing = true;
			base.ColumnStartedEditing(customComboBoxPicker1);
		}
	}

	public class CustomComboBox : ComboBox
	{
		[SecurityPermissionAttribute(
		SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessKeyMessage(ref Message m)
		{
			// Keep all the keys for the combobox.
			return ProcessKeyEventArgs(ref m);
		}
	}

}
