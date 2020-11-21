using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Scoring.ScoreKeeperService;

namespace Scoring
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            string character = "John";
            int points = 0;
            if (args.Length > 0)
            {
                character = args[0];
                points = Int32.Parse(args[1]);
            }

            string name;
            using (var dlg = new ReportScore())
            {
                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                name = dlg.GamerTag;
            }

            Reporter.Send(character, name, points);
        }
    }
}