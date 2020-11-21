using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ScoreKeeper
{
    /// <summary>
    /// Provides a custom http handler for the start page resources.
    /// </summary>
    class ScoreContentHandler
    {
        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The context.</param>
        internal void ProcessRequest(HttpListenerContext context)
        {

            var scores = DataAccess.GetTopScores();

            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = false,
            };
            using (XmlWriter writer = XmlWriter.Create(context.Response.OutputStream, settings))
            {
                writer.WriteStartElement("Scores");

                foreach (var score in scores)
                {
                    writer.WriteStartElement("Score");
                    writer.WriteElementString("Name", score.Name);
                    writer.WriteElementString("Points", score.Points.ToString());
                    writer.WriteStartElement("Picture");
                    writer.WriteBase64(score.Picture, 0, score.Picture.Length);                    
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            
            
            context.Response.ContentType = "text/xml";
            context.Response.OutputStream.Close();
        }
    }
}
