using System;
using System.IO;
using System.Net;
using System.Text;

namespace HttpUI.Server
{
    public static class HttpListenerContextExtention
    {

        /// <param name="encoding">null means utf 8</param>
        public static void WriteAndClose(this HttpListenerContext context, object val, Encoding encoding = null)
        {

            var buffer = encoding == null ? Encoding.UTF8.GetBytes(val.ToString()) : encoding.GetBytes(val.ToString());
            context.Response.ContentLength64 = buffer.Length;
            context.Response.ContentType = HTTPServer._mimeTypeMappings[".html"];
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Flush();
            context.Response.Close();

        }

        public static void WriteAndClose(this HttpListenerContext context, Stream stream, string contentType)
        {

            byte[] buffer = new byte[1024 * 16];
            byte[] fbuffer = new byte[0];
            int size;
            while ((size = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                var lastSize = fbuffer.Length;
                Array.Resize(ref fbuffer, lastSize + size);
                for (int i = lastSize; i < fbuffer.Length; i++)
                {
                    fbuffer[i] = buffer[i - lastSize];
                }
            }
            stream.Close();

            context.Response.ContentType = contentType;
            context.Response.ContentLength64 = fbuffer.Length;
            context.Response.OutputStream.Write(fbuffer, 0, fbuffer.Length);
            context.Response.OutputStream.Flush();
            context.Response.Close();

        }

        public static void NotFoundAndClose(this HttpListenerContext context)
        {

            var bytes = Encoding.UTF8.GetBytes("<h1>Page Not Found !!!</h1>");
            context.Response.ContentLength64 = bytes.Length;
            context.Response.ContentType = HTTPServer._mimeTypeMappings[".html"];
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            context.Response.OutputStream.Flush();
            context.Response.Close();

        }

        public static void SetCoockie(this HttpListenerContext context, Coockie coockie) =>
            context.Response.Headers.Add("Set-Cookie", coockie.key + "=" + coockie.val + ";max-age=" + coockie.maxAge.ToString());

    }

    public class Coockie
    {

        public const int MAX_AGE_MIN = 60;
        public const int MAX_AGE_HOUR = 3600;
        public const int MAX_AGE_DAY = 216000;

        public string key;
        public string val;
        public int maxAge;

    }

}