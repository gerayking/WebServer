using System;
using WebServer.Entry;
using WebServer.infrastructure;
using WebServer.MiddleWare;

namespace WebServer.MiddleWares
{
    public class Http404 : IMiddleware
    {
        public MiddlewareResult Execute(HttpServerContext context)
        {
            Console.WriteLine(context.Request.Url +  " Not Found");
            context.Response.StatusCode = 404;
            context.Response.StatusDescription = "File Not Found";
            return MiddlewareResult.Processed;
        }
    }
}