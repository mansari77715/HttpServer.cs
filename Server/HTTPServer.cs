using System;
using System.Collections.Generic;
//my
using HttpUI.Tools;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace HttpUI.Server
{
    public class HTTPServer
    {



        //Varaibles



        //const
        public static IDictionary<string, string> _mimeTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
            #region extension to MIME type list
        {".asf", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".avi", "video/x-msvideo"},
        {".bin", "application/octet-stream"},
        {".cco", "application/x-cocoa"},
        {".crt", "application/x-x509-ca-cert"},
        {".css", "text/css"},
        {".deb", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dll", "application/octet-stream"},
        {".dmg", "application/octet-stream"},
        {".ear", "application/java-archive"},
        {".eot", "application/octet-stream"},
        {".exe", "application/octet-stream"},
        {".flv", "video/x-flv"},
        {".gif", "image/gif"},
        {".hqx", "application/mac-binhex40"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".ico", "image/x-icon"},
        {".img", "application/octet-stream"},
        {".iso", "application/octet-stream"},
        {".jar", "application/java-archive"},
        {".jardiff", "application/x-java-archive-diff"},
        {".jng", "image/x-jng"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".mml", "text/mathml"},
        {".mng", "video/x-mng"},
        {".mov", "video/quicktime"},
        {".mp3", "audio/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpg", "video/mpeg"},
        {".msi", "application/octet-stream"},
        {".msm", "application/octet-stream"},
        {".msp", "application/octet-stream"},
        {".pdb", "application/x-pilot"},
        {".pdf", "application/pdf"},
        {".pem", "application/x-x509-ca-cert"},
        {".pl", "application/x-perl"},
        {".pm", "application/x-perl"},
        {".png", "image/png"},
        {".prc", "application/x-pilot"},
        {".ra", "audio/x-realaudio"},
        {".rar", "application/x-rar-compressed"},
        {".rpm", "application/x-redhat-package-manager"},
        {".rss", "text/xml"},
        {".run", "application/x-makeself"},
        {".sea", "application/x-sea"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".swf", "application/x-shockwave-flash"},
        {".tcl", "application/x-tcl"},
        {".tk", "application/x-tcl"},
        {".txt", "text/plain"},
        {".war", "application/java-archive"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wmv", "video/x-ms-wmv"},
        {".xml", "text/xml"},
        {".xpi", "application/x-xpinstall"},
        {".zip", "application/zip"},
        #endregion
        };

        //internals
        private Thread _serverThread;
        public HttpListener _listener { get; private set; }

        public int Port { get; private set; }
        //exteranl
        public string URL { get => "http://127.0.0.1:" + Port + "/"; }

        //suorce
        /// <summary>
        /// for when we want run a site from zip
        /// </summary>
        public ZipArchive zip { get; private set; }
        /// <summary>
        /// for when we want run site from c#
        /// not need to add / at start of path
        /// </summary>
        public Dictionary<string, Tools0.OnResult> router = new Dictionary<string, Tools0.OnResult>();
        /// <summary>
        /// all request pass trouth this first
        /// if returnes is false proccess not continued
        /// </summary>
        public Tools0.OnResult1 check;
        /// <summary>
        /// for getting embed file loacation
        /// </summary>
        public string ExcecuteLocation { get => Assembly.GetExecutingAssembly().Location; }



        //constructor



        public HTTPServer(int port) => Initialize(port);
        /// <summary>
        /// should set sources after this
        /// when a request happened:
        ///     1-page router check
        ///     2-zip check
        /// </summary>
        public HTTPServer()
        {

            //get an empty port
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            Initialize(port);

        }

        private void Initialize(int port)
        {

            Port = port;
            Listen();

        }



        //Functions



        /// <summary>
        /// if send nothing or null values set to null
        /// </summary>
        public HTTPServer SetZipSource(string zipPath = null)
        {

            if (zipPath != null)
                zip = ZipFile.OpenRead(zipPath);
            else
                zip = null;
            return this;

        }
        public HTTPServer SetZipSource(Stream s)
        {
            zip = new ZipArchive(s);
            return this;

        }
        public HTTPServer SetZipSource(byte[] b)
        {

            var s = new MemoryStream(b);
            zip = new ZipArchive(s);
            return this;

        }

        /// <summary>
        /// Stop server and dispose all functions.
        /// </summary>
        public void Stop()
        {

            _listener.Stop();
            _serverThread.Abort();

        }

        /// <summary>
        /// show ui in defautl browser
        /// </summary>
        public void Show() => System.Diagnostics.Process.Start(URL);

        private void Listen()
        {

            _listener = new HttpListener();
            _listener.Prefixes.Add("http://*:" + Port.ToString() + "/");
            _listener.Prefixes.Add("http://+:" + Port.ToString() + "/");
            foreach (var item in GetLocalIPAdress())
                _listener.Prefixes.Add("http://" + item + ":" + Port.ToString() + "/");

            _listener.Start();
            _serverThread = new Thread(() =>
            {
                while (true)
                {
                    var c = _listener.GetContext();
                    new Thread(() =>
                            Process(c)
                     ).Start();
                }
            });
            _serverThread.Start();

        }

        public List<string> GetLocalIPAdress()
        {

            var res = new List<string>();
            var hs = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var item in hs.AddressList)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)
                    res.Add(item.ToString());
            }
            return res;

        }
        public string GetGlobalIPAddress()
        {

            try
            {
                return "http://" + new WebClient().DownloadString("https://api.ipify.org") + ":" + Port.ToString() + "/";
            }
            catch (Exception)
            {
                return "";
            }

        }



        private void Process(HttpListenerContext context)
        {

            //path
            var path = Uri.UnescapeDataString(context.Request.Url.AbsolutePath.Substring(1));

            //Data
            var d = new HttpData(context, path);

            //LoginCheck
            if (check != null)
            {

                var chres = check(context, d);
                if (!chres)
                {

                    context.Response.Close();
                    return;

                }

            }

            //extention
            var spltd = path.Split('.');
            var extnsn = spltd[spltd.Length - 1];

            //Router
            if (router.Count > 0)
            {

                foreach (var item in router)
                {

                    if (item.Key == path)
                    {

                        item.Value(context, d);
                        context.Response.Close();
                        return;

                    }

                }

            }

            //Path
            //new feature

            //Zip
            if (zip != null)
            {

                var ent = zip.GetEntry(path);
                if (ent == null)
                {

                    context.NotFoundAndClose();
                    return;

                }
                var strm = ent.Open();
                context.WriteAndClose(strm, _mimeTypeMappings["." + extnsn]);
                return;

            }

            context.NotFoundAndClose();

        }

    }

}