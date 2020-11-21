using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace ScoreKeeper
{
    class Program
    {
        private static HttpListener _http;
        private static ServiceHost _host;

        static void Main(string[] args)
        {
            //open an http server to handle the requests from the score viewer (web browser)
            try
            {
                DataAccess.Open();
                StartHttpServer();
                StartWebService();

                //start browser
                var psi = new ProcessStartInfo("iexplore.exe", "http://localhost:8088/Display/Default.htm");
                Process.Start(psi);

                //wait for kill signal
                Console.WriteLine("Press <Ctrl-Break> to stop.");
                while (true)
                {
                    var key = Console.ReadKey();
                    if (key.Modifiers == ConsoleModifiers.Control && 
                        key.Key == ConsoleKey.Pause)
                    {
                        return;
                    }
                }
            }
            finally
            {
                StopHttpServer();
                StopWebService();
                DataAccess.Close();
            }
        }

        private static void StopWebService()
        {
            if (_host != null)
            {
                _host.Close();
                _host = null;
            }
        }

        private static void StartWebService()
        {
            // Create ServiceHost.
            ServiceHost host = new ServiceHost(typeof(ScoreService));
                
            // Create HTTP binding with a large capacity.
            BasicHttpBinding binding = new BasicHttpBinding()
            {
                MaxReceivedMessageSize = Int32.MaxValue
            };

            Uri baseAddress = new Uri("http://localhost:8088/Score.svc"); 
            host.AddServiceEndpoint(typeof(ScoreService), binding, baseAddress);

            // Enable metadata publishing.
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetUrl = baseAddress;
            smb.HttpGetEnabled = true;
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            host.Description.Behaviors.Add(smb);

            // Open service and start listening.
            host.Open();

            // Report address.
            Console.WriteLine("Service ready at: " + baseAddress);
        }

        private static void StopHttpServer()
        {
            if (_http != null)
            {
                _http.Stop();
                _http = null;
            }
        }

        private static void StartHttpServer()
        {
            _http = new HttpListener();
            _http.IgnoreWriteExceptions = true;
            _http.Prefixes.Add(String.Format("http://+:8088/Display/"));
            _http.Start();
            _http.BeginGetContext(ProcessHttpRequest, null);

            Console.WriteLine("Display ready at: http://localhost:8088/Display/Default.htm");
        }

        private static void ProcessHttpRequest(IAsyncResult ar)
        {
            if (_http != null)
            {
                HttpListenerContext context = _http.EndGetContext(ar);
                try
                {
                    if (String.Equals(context.Request.RawUrl, "/Display/TopScores.xml", StringComparison.OrdinalIgnoreCase))
                    {
                        ScoreContentHandler handler = new ScoreContentHandler();
                        handler.ProcessRequest(context);
                        context.Response.Close();
                    }
                    else if (String.Equals(context.Request.RawUrl, "/Display/TotalFunds.xml", StringComparison.OrdinalIgnoreCase))
                    {
                        FundContentHandler handler = new FundContentHandler();
                        handler.ProcessRequest(context);
                        context.Response.Close();
                    }
                    else if (
                        String.Equals(context.Request.RawUrl, "/Display/Movie1.mp4", StringComparison.OrdinalIgnoreCase) ||
                        String.Equals(context.Request.RawUrl, "/Display/Movie2.mp4", StringComparison.OrdinalIgnoreCase) ||
                        String.Equals(context.Request.RawUrl, "/Display/Movie3.mp4", StringComparison.OrdinalIgnoreCase) ||
                        String.Equals(context.Request.RawUrl, "/Display/Movie4.mp4", StringComparison.OrdinalIgnoreCase)
                        )
                    {
                        MovieContentHandler handler = new MovieContentHandler();
                        handler.ProcessRequest(context);
                        context.Response.Close();
                    }
                    else
                    {
                        HttpContentHandler handler = new HttpContentHandler();
                        handler.ProcessRequest(context);
                        context.Response.Close();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(String.Format("Failed to process the http request. {0}", e.Message));
                    context.Response.Abort();
                }
                finally
                {
                    _http.BeginGetContext(ProcessHttpRequest, null);
                }
            }
        }
    }
}
