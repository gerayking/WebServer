using System;
using System.Net;
using WebServer.Entry;
using WebServer.MiddleWare;

namespace WebServer.Error
{
    public class ResourceNotFoundExceptionHandler : IExceptionHandler
    {
        public void HandleException(HttpServerContext context, Exception exp)
        {
            var clientIp = context.Request.RemoteEndPoint.Address;
            var method = context.Request.HttpMethod;
            var path = context.Request.Url.LocalPath;
            var expMsg = "Excetion" + exp.Message;
            Console.WriteLine("[{0:yyyy-MM-dd HH:mm:ss}] {1} {2} {3} ",
                DateTime.Now, expMsg, method, path);
            Console.WriteLine(exp.StackTrace);
        }
    }
}