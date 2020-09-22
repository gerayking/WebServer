using System;
using WebServer.Error;
using WebServer.HttpLog;
using WebServer.MiddleWare;

namespace WebServer
{
    internal class Program
    {
        private const int currentCount = 20;
        private const string serverUrl = "http://localhost:9000/";
        public static void Main(string[] args)
        {
            var server = new WebServer(20);
            server.Use(new Httplog());
            server.Use(new ServerLet());
            server.Bind(serverUrl);
            server.start();
            Console.WriteLine($"Web server started at {serverUrl}. Press any key to exit...");
            Console.ReadKey();
        }
    }
    
}