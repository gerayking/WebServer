using System;
using System.Reflection;
using WebServer.Entry;
using WebServer.Error;
using WebServer.HttpLog;
using WebServer.MiddleWare;
using WebServer.MiddleWares;

namespace WebServer
{
    internal class Program
    {
        private const int currentCount = 20;
        private const string serverUrl = "http://localhost:9000/";
        public static void Main(string[] args)
        {
            var server = new WebServer(20);
            RegisterMiddlewares(server);
            server.Bind(serverUrl);
            server.start();
            Console.WriteLine($"Web server started at {serverUrl}. Press any key to exit...");
            Console.ReadKey();
        }

        static void RegisterMiddlewares(IWebServerBuilder builder)
        {
            builder.UnhandledException(new ResourceNotFoundExceptionHandler());
            builder.Use(new Httplog());
            var route = new Routing();
            RegisterRoutes(route);
            builder.Use(route);
        }
        static void RegisterRoutes(Routing route)
        {
            route.AddRoute("Index","{controller}/{action}/{id}",
                new {controller = "Home",action = "details", id = UrlParameter.Optional});
        }
    }
    
}