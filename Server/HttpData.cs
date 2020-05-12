using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HttpUI.Server
{
    public class HttpData
    {



        public Dictionary<string, string> urlVars { get; private set; }
        public Dictionary<string, string> reqHeaders { get; private set; }
        public Dictionary<string, string> coockies { get; private set; }
        public Dictionary<string, string> bodyVars { get; private set; }
        public Dictionary<string, HttpFiles> files { get; private set; }
        public string absolutePath { get; private set; }

        public string userID { get; set; }



        public HttpData(HttpListenerContext context, string absolutePath)
        {

            this.absolutePath = absolutePath;



            //urlVars
            urlVars = new Dictionary<string, string>();
            var query = context.Request.Url.Query;
            if (!string.IsNullOrEmpty(query))
            {
                var red = Uri.UnescapeDataString(query.Substring(1));
                var sed = red.Split('&');
                foreach (var item in sed)
                {

                    if (!string.IsNullOrEmpty(item))
                    {

                        var si = item.Split('=');
                        if (si.Length > 1)
                            urlVars.Add(si[0], si[1]);
                        else
                            urlVars.Add(si[0], "");

                    }

                }

            }



            //reqHeaders
            this.reqHeaders = new Dictionary<string, string>();
            var reqHeaders = context.Request.Headers;
            foreach (var item in reqHeaders)
                this.reqHeaders.Add(item.ToString(), reqHeaders[item.ToString()]);



            //coockies
            coockies = new Dictionary<string, string>();
            foreach (var item in context.Request.Cookies)
            {

                var sc = item.ToString().Split('=');
                coockies.Add(sc[0], sc[1]);

            }



            var body = new StreamReader(context.Request.InputStream, Encoding.Default).ReadToEnd();
            if (!string.IsNullOrEmpty(body.Trim()) && reqHeaders.AllKeys.Where(i => i == "Content-Type").FirstOrDefault() != null)
            {

                var ctype = reqHeaders["Content-Type"];
                if (ctype == "application/x-www-form-urlencoded")
                {

                    //bodyVars
                    bodyVars = new Dictionary<string, string>();
                    var query0 = body;
                    if (!string.IsNullOrEmpty(query0))
                    {
                        var red = Uri.UnescapeDataString(query0.Substring(1));
                        var sed = red.Split('&');
                        foreach (var item in sed)
                        {

                            if (!string.IsNullOrEmpty(item))
                            {

                                var si = item.Split('=');
                                if (si.Length > 1)
                                    bodyVars.Add(si[0], si[1]);
                                else
                                    bodyVars.Add(si[0], "");

                            }

                        }

                    }

                }
                else if (ctype.Contains("multipart/form-data"))
                {

                    //files & bodyVars
                    files = new Dictionary<string, HttpFiles>();
                    bodyVars = new Dictionary<string, string>();
                    var boundary = ctype.Trim().Split(' ')[1].Trim().Split('=')[1];
                    var sbody = body.Split(new string[] { "--" + boundary + "--", "--" + boundary }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var item in sbody)
                    {

                        if (!string.IsNullOrEmpty(item.Trim()))
                        {

                            var drpos = item.Trim().IndexOf(@"

") + 6;
                            var sdr = new string[] { item.Substring(0, drpos), item.Substring(drpos, item.Length - drpos - 2) };

                            var shead = sdr[0].Trim().Split('\n');
                            var fdetail = shead[0].Split(';');

                            var content = sdr[1];

                            if (fdetail.Length > 2)
                                files.Add(fdetail[1].Trim().Split('=', '"')[2], new HttpFiles() { name = fdetail[2].Trim().Split('=', '"')[2], content = content, contentType = shead[1] });
                            else
                                bodyVars.Add(fdetail[1].Trim().Split('=', '"')[2], content);

                        }

                    }

                }

            }

        }

    }

    public class HttpFiles
    {

        public string name;
        public string content;
        public string contentType;

    }

}