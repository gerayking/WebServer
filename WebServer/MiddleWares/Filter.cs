using System;
using System.IO;
using System.Text.RegularExpressions;
using RazorEngine.Compilation;
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
                    if (FindResource(absolutePath, httpServerContext, item) == false)
                    {
                        new ResourceNotFoundExceptionHandler().HandleException(httpServerContext,new Exception("url"));
                        httpServerContext.Response.NotFountResource(404);
                    }
                    return true;
                }
            }
            return false;
        }
        private bool FindResource(string url,HttpServerContext context,string MimeType)
        {
            if (MimeType == "svg")
            {
                MimeType = "svg";
            }
            var staticPathCon = StaticPathCon.GetInstance();
            var staticResCon = StaticResCon.GetInstance();
            foreach (var item in staticPathCon.GetFragment())
            {
                string basedir = System.IO.Directory.GetCurrentDirectory();
                string path = basedir + "\\" + item + "\\" + url;    
                if (!File.Exists(path)) continue;
                var result = File.ReadAllText(path, System.Text.Encoding.UTF8);
                if (MimeType.Equals("js") || MimeType.Equals("html") || MimeType.Equals("css"))
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