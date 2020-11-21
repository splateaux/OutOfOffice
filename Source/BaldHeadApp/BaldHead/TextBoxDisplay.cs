using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BaldHead
{
	public partial class TextBoxDisplay : Form
	{
		public TextBoxDisplay(MainForm mainForm, string text)
		{
			InitializeComponent();

			// Make text read-only but make colors look like it is editable.
			txtDisplay.BackColor = Color.White;
			txtDisplay.ForeColor = Color.Black;
			txtDisplay.ReadOnly = true;

			// Set displayed text.
			txtDisplay.Text = text;
		}

		public override string Text
		{
			get
			{
				return txtDisplay == null ? null : txtDisplay.Text;
			}
			set
			{
				txtDisplay.Text = value;
			}
		}
	}
}
