using System;
using System.Net;
using WebServer.MiddleWare;

namespace WebServer.HttpLog
{
    public class Httplog : IMiddleware
    {
        public MiddlewareResult Execute(HttpListenerContext context)
        {
            var request = context.Request;
            var path = request.Url.LocalPath;
            var clientIp = request.RemoteEndPoint.Address;
            var method = request.HttpMethod;

            Console.WriteLine("[{0:yyyy-MM-dd HH:mm:ss}] {1} {2} {3}",
                DateTime.Now, clientIp, method, path);
            return MiddlewareResult.Continue;
        }
    }
}