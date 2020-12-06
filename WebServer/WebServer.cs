using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Entry;
using WebServer.MiddleWare;

namespace WebServer
{
/**
 * @description: web server
 * @params: params
 * @return: return
 * @date:  2020-9-22
 * @author: gerayking
 *
 * The Server Builder
 * set the Semaphore and add middlewares
 * **/
    public class WebServer : IWebServerBuilder
    {
        private readonly Semaphore _sem ;
        private readonly HttpListener _listener;
        private readonly MiddlewarePipeline _pipeline;
        public WebServer(int currentCount)
        {
            _sem = new Semaphore(currentCount, currentCount);
            _listener = new HttpListener();
            _pipeline = new MiddlewarePipeline();
        }

        public void Bind(string url)
        {
            _listener.Prefixes.Add(url);
        }

        public void start()
        {
            _listener.Start();
            Task.Run(async () =>
            {
                while (true)
                {
                    _sem.WaitOne();
                    var context = await _listener.GetContextAsync();
                    Console.WriteLine(context.Request.Url);
                    _sem.Release();
                    _pipeline.Execute(new HttpServerContext(context));
                }
            });
        }
        public IWebServerBuilder Use(IMiddleware middleware)
        {
            _pipeline.Add(middleware);
            return this;
        }

        public IWebServerBuilder UnhandledException(IExceptionHandler handler)
        {
            _pipeline.UnhandledException(handler);
            return this;
        }
    }
}