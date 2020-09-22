namespace WebServer.MiddleWare
{
    public interface IWebServerBuilder
    {
        IWebServerBuilder Use(IMiddleware middleware);

        IWebServerBuilder UnhandledException(IExceptionHandler handler);
    }
}