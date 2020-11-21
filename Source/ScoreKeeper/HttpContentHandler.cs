using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScoreKeeper
{
    /// <summary>
    /// Provides a custom http handler for the start page resources.
    /// </summary>
    class HttpContentHandler
    {
        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The context.</param>
        internal void ProcessRequest(HttpListenerContext context)
        {
            string url = context.Request.RawUrl;
            if (url.IndexOf('?') > -1)
            {
                url = url.Substring(0, url.IndexOf('?'));
            }

            var fileName = Path.GetFileName(url);

            //special processing for posting money funds
            if (String.Equals(fileName, "Money.htm") && context.Request.RawUrl.Contains("?amount="))
            {
                decimal amt = Decimal.Parse(context.Request.RawUrl.Substring(context.Request.RawUrl.IndexOf('=') + 1));
                DataAccess.ReportFunds(amt);
            }

            string resource = "ScoreKeeper.Content." + fileName;
            byte[] data = GetResourceData(resource);
            context.Response.ContentLength64 = data.Length;
            context.Response.ContentType = GetContentType(resource);
            context.Response.OutputStream.Write(data, 0, data.Length);
            context.Response.OutputStream.Close();
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        private byte[] GetResourceData(string resource)
        {
            //find the embedded resource
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] names = asm.GetManifestResourceNames();
            string name = names.FirstOrDefault<string>(s => s.Equals(resource, StringComparison.OrdinalIgnoreCase));
            byte[] buffer = new byte[0];
            if (name != null)
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
                if (stream != null)
                {
                    buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                }
            }
            return buffer;
        }

        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        private string GetContentType(string resource)
        {
            switch (Path.GetExtension(resource).ToLower())
            {
                case ".png":
                    return "image/png";

                case ".gif":
                    return "image/gif";

                case ".css":
                    return "text/css";

                case ".htc":
                    return "text/x-component";

                default:
                    return "text/html";
            }
        }
    }
}
