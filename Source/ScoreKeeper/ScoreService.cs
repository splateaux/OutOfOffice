using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ScoreKeeper
{
    [ServiceContract]
    class ScoreService
    {
        [OperationContract]
        public void Add(Score score)
        {
            try
            {
                ////make the scope image transparent
                //if (score.Picture != null)
                //{
                //    Bitmap orig;
                //    using (MemoryStream memory = new MemoryStream(score.Picture))
                //    {
                //        orig = (Bitmap)Bitmap.FromStream(memory);
                //    }

                //    Color color = orig.GetPixel(0, 0);
                //    orig.MakeTransparent(color);

                //    using (MemoryStream memory = new MemoryStream())
                //    {
                //        orig.Save(memory, ImageFormat.Png);
                //        score.Picture = memory.ToArray();
                //    }
                //}

                DataAccess.ReportScore(score);
                Console.WriteLine("{0} scored {1} points!", score.Name, score.Points);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to report score! {0}", ex.Message);
            }
        }

        [OperationContract]
        public Score[] GetTopScores()
        {
            return DataAccess.GetTopScores();
        }

        [OperationContract]
        public string GetTime()
        {
            string time = DateTime.Now.ToShortTimeString();

            Console.WriteLine("Host requested time: {0}", time);
            return time;
        }
    }
}
