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
    class FundContentHandler
    {
        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The context.</param>
        internal void ProcessRequest(HttpListenerContext context)
        {
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = false,
            };
            using (XmlWriter writer = XmlWriter.Create(context.Response.OutputStream, settings))
            {
                writer.WriteStartElement("Funds");
                writer.WriteString(DataAccess.GetFunds().ToString("C"));
                writer.WriteEndElement();
            }            
            
            context.Response.ContentType = "text/xml";
            context.Response.OutputStream.Close();
        }
    }
}
