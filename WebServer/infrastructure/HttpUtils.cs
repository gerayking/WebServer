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

        public static HttpListenerResponse NotFountResource(this HttpListenerResponse response, int status)
        {
            response.StatusCode = status;
            return response;
        }
        public static HttpListenerResponse Image(this HttpListenerResponse response, string imgPath, string mimeType)
        {
                byte[] picbBytes = File.ReadAllBytes(imgPath);
                response.ContentType = mimeType;
                response.ContentLength64 = picbBytes.Length;
                response.StatusCode = 200;
            
                response.OutputStream.Write(picbBytes, 0, Convert.ToInt32(picbBytes.Length));
                response.OutputStream.Close();
            
                return response;
        }
    }
}