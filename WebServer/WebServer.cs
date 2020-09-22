using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebServer.MiddleWare;

namespace WebServer
{
/**
 * @description: web server
 * @params: params
 * @return: return
 * @date:  2020-9-22
 * @author: gerayking
 * **/
    public class WebServer : IWebServerBuilder
    {
        private readonly Semaphore _sem;
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
                    _sem.Release();
                    _pipeline.Execute(context);
                }
            });
        }
        /*private void HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var urlPath = request.Url.LocalPath.TrimStart('/');
            Console.WriteLine($"url path={urlPath}");
            try
            {
                string filePath = Path.Combine("file", urlPath);
                byte[] data = File.ReadAllBytes(filePath);
                response.ContentType = "text/html";
                response.ContentLength64 = data.Length;
                response.ContentEncoding = Encoding.UTF8;
                response.StatusCode = 200;
                response.OutputStream.Write(data, 0, data.Length);
                response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.StackTrace);
            }
        }*/

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