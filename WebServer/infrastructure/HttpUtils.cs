using System;
using System.IO;
using System.Net;
using System.Text;
using WebServer.Entry;

namespace WebServer.infrastructure
{
    public static class HttpUtils
    {

        public static HttpListenerResponse Content(this HttpListenerResponse response,
            string content, string mimeType)
        {
            var contentByte = Encoding.UTF8.GetBytes(content);
            response.ContentType = mimeType;
            response.StatusCode = 200;
            response.ContentLength64 = contentByte.Length;
            response.OutputStream.Write(contentByte, 0, contentByte.Length);
            response.OutputStream.Close();
            return response;
        }

        public static HttpListenerResponse Image(this HttpListenerResponse response, string imgPath, string mimeType)
        {
            using (System.IO.FileStream fs = new FileStream(imgPath,FileMode.Open,FileAccess.Read))
            {
                byte[] picbBytes = new byte[fs.Length];
                using (BinaryReader br = new BinaryReader(fs))
                {
                    picbBytes = br.ReadBytes(Convert.ToInt32(fs.Length));
                    response.OutputStream.Write(picbBytes,0,Convert.ToInt32(fs.Length));
                }
            }

            return response;
        }
    }
}