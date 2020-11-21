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
	public partial class Llama : Form
	{
		MainForm _mainForm;

		public Llama(MainForm mainForm)
		{
			_mainForm = mainForm;

			InitializeComponent();
		}
	}
}
