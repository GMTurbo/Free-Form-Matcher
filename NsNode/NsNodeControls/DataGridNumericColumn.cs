using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Security.Permissions;

namespace NsNodeControls
{
	public class DataGridViewNumericColumn : DataGridViewTextBoxColumn
	{
		public override object Clone()
		{
			
			object o = base.Clone();
			(o as DataGridViewNumericColumn).m_cell = m_cell;
			return o;
		}
		DataGridViewNumericCell m_cell = new DataGridViewNumericCell();
		public override DataGridViewCell CellTemplate
		{
			get
			{
				return m_cell;
			}
			set
			{
				m_cell = value as DataGridViewNumericCell;
			}
		}
		public DataGridViewNumericColumn()
		{
		}
	}
	public class DataGridViewNumericCell : DataGridViewTextBoxCell
	{
		protected override void OnKeyPress(KeyPressEventArgs e, int rowIndex)
		{
			System.Globalization.NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
			string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
			string groupSeparator = numberFormatInfo.NumberGroupSeparator;
			string negativeSign = numberFormatInfo.NegativeSign;

			string keyInput = e.KeyChar.ToString();

			if (Char.IsDigit(e.KeyChar))
			{
				// Digits are OK
			}
			else if (keyInput.Equals(decimalSeparator) || keyInput.Equals(groupSeparator) ||
			 keyInput.Equals(negativeSign))
			{
				// Decimal separator is OK
			}
			else if (e.KeyChar == '\b')
			{
				// Backspace key is OK
			}
			//    else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
			//    {
			//     // Let the edit control handle control and alt key combinations
			//    }
			else
			{
				// Consume this invalid key and beep
				e.Handled = true;
				//    MessageBeep();
			}

			base.OnKeyPress(e, rowIndex);
		}
	}
}
