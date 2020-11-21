using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scoring
{
    public partial class ReportScore : Form
    {
        public ReportScore()
        {
            InitializeComponent();
        }

        public string GamerTag
        {
            get { return textBox1.Text.Trim(); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim().Length < 1)
            {
                MessageBox.Show("You must provide a name.");
            }
            else
            {
                this.Close();
            }
        }
    }
}
