using System.Net;
using System.Text;

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
    }
}