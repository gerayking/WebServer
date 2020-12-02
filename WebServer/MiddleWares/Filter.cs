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
            var staticPathcon =  StaticPathCon.GetInstance();
            var staticResCon = StaticResCon.GetInstance();
            foreach (var item in staticResCon.GetPattern())
            {
                var re =new Regex(item);
                var Match = re.Match(url);
                if (Match.Success)
                {
                    string readdress = @"/(?<address>[A-Za-z]+\.[a-z]+)";
                    re = new Regex(readdress);
                    Match = re.Match(url);
                    string resource = Match.Groups["address"].Value;
                    FindResource(resource,httpServerContext);
                    return true;
                }
            }
            new ResourceNotFoundExceptionHandler().HandleException(httpServerContext,new Exception("url"));
            return false;
        }
        private bool FindResource(string url,HttpServerContext context)
        {
            var staticPathCon = StaticPathCon.GetInstance();
            foreach (var item in staticPathCon.GetFragment())
            {
                string basedir = System.IO.Directory.GetCurrentDirectory();
                string path = basedir + "\\" + item + "\\" + url;
                if (!File.Exists(path)) continue;
                var result = File.ReadAllText(path, System.Text.Encoding.UTF8);
                context.Response.Content(result, "text/html");
                return true;
            }
            return false;
        }
    }
}