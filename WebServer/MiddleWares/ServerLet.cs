using System;
using System.IO;
using System.Net;
using System.Text;
using WebServer.Error;

namespace WebServer.MiddleWare
{
    public class ServerLet : IMiddleware
    {
        public MiddlewareResult Execute(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var urlPath = request.Url.LocalPath.TrimStart('/');
            Console.WriteLine($"url path={urlPath}");
            try
            {
                string filePath = Path.Combine("file", urlPath);
                byte[] data = File.ReadAllBytes(filePath);
                response.ContentType = "text/html";
                response.ContentLength64 = data.Length;
                response.ContentEncoding = Encoding.UTF8;
                response.StatusCode = 200;
                response.OutputStream.Write(data, 0, data.Length);
                response.OutputStream.Close();
            }
            catch (FileNotFoundException ex)
            {
                HttpUtil.Status(context.Response, 404, "File Not Found");
                return MiddlewareResult.Processed;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.StackTrace);
            }

            return MiddlewareResult.Continue;
        }
    }
}