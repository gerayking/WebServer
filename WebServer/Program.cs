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

         /**
         * @description: 启动入口
         * @params: params
         * @return: return
         * @date:  
         * @author: gerayking
         * 创建一个线程数20的服务，可以同时处理20个请求，然后注册中间件，注册url，启动并且打印信息
         * **/
        public static void Main(string[] args)
        {
            var server = new WebServer(20);
            RegisterMiddlewares(server);
            server.Bind(serverUrl);
            server.start();
            Console.WriteLine($"Web server started at {serverUrl}. Press any key to exit...");
            Console.ReadKey();
        }
         // register Middlewares 注册中间件，路由，日志，异常处理等
         static void RegisterMiddlewares(IWebServerBuilder builder)
        {
            builder.UnhandledException(new ResourceNotFoundExceptionHandler());
            builder.Use(new SessionManager());
            builder.Use(new Httplog());
            var route = new Routing();
            RegisterRoutes(route);
            builder.Use(route);
        }
         // 添加路由规则
        static void RegisterRoutes(Routing route)
        {
            route.AddRoute("Home","{controller}/{action}/{id}",
                new {controller = "Home",action = "details", id = UrlParameter.Optional});
            route.AddRoute("Index","{controller}/{action}",
                new {controller = "Home", action = "Index"});
        }
    }
    
}