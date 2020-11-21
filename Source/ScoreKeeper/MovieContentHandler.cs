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
    class MovieContentHandler
    {
        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The context.</param>
        internal void ProcessRequest(HttpListenerContext context)
        {
            using (FileStream movie = File.OpenRead(Path.GetFileName(context.Request.RawUrl)))
            {
                context.Response.ContentLength64 = movie.Length;
                context.Response.ContentType = "video/mp4";
                
                movie.CopyTo(context.Response.OutputStream);
            }
            context.Response.OutputStream.Close();
        }
    }
}
