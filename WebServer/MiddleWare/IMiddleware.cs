using System;
using System.Net;
using WebServer.Entry;

namespace WebServer.MiddleWare
{
    public interface IMiddleware
    {
        MiddlewareResult Execute(HttpServerContext context);
    }
    public interface IExceptionHandler
    {
        void HandleException(HttpServerContext context, Exception exp);
    }
}