using System;
using System.IO;
using System.Text.RegularExpressions;
using WebServer.Entry;
using WebServer.Error;
using WebServer.infrastructure;
using WebServer.MiddleWare;

namespace WebServer.MiddleWares
{
    public class Filter : IMiddleware
    {
        public MiddlewareResult Execute(HttpServerContext context)
        {
            if (FilterResource(context))
            {
                return MiddlewareResult.Processed;
            }

            return MiddlewareResult.Continue;
        }
        private bool FilterResource(HttpServerContext httpServerContext)
        {
            var url = httpServerContext.Request.Url.ToString();
            string absolutePath = httpServerContext.Request.Url.AbsolutePath;
            var staticPathcon =  StaticPathCon.GetInstance();
            var staticResCon = StaticResCon.GetInstance();
            foreach (var item in staticResCon.GetPattern())
            {
                string resName = ".*." + item;
                var re =new Regex(item);
                var Match = re.Match(absolutePath);
                if (Match.Success)
                {
                    FindResource(absolutePath,httpServerContext,item);
                    return true;
                }
            }
            new ResourceNotFoundExceptionHandler().HandleException(httpServerContext,new Exception("url"));
            return false;
        }
        private bool FindResource(string url,HttpServerContext context,string MimeType)
        {
            var staticPathCon = StaticPathCon.GetInstance();
            var staticResCon = StaticResCon.GetInstance();
            foreach (var item in staticPathCon.GetFragment())
            {
                string basedir = System.IO.Directory.GetCurrentDirectory();
                string path = basedir + "\\" + item + "\\" + url;
                if (!File.Exists(path)) continue;
                var result = File.ReadAllText(path, System.Text.Encoding.UTF8);
                if (MimeType.Equals("js") || MimeType.Equals("html"))
                {
                    context.Response.Content(result, staticResCon.ParseMimeType(MimeType));
                }
                else
                {
                    context.Response.Image(path, staticResCon.ParseMimeType(MimeType));
                }

                return true;
            }
            return false;
        }
    }
}