using System;
using System.Net;

namespace WebServer.MiddleWare
{
    public interface IMiddleware
    {
        MiddlewareResult Execute(HttpListenerContext context);
    }
    public interface IExceptionHandler
    {
        void HandleException(HttpListenerContext context, Exception exp);
    }
}