using System.IO;
using RazorEngine;
using WebServer.Entry;

namespace WebServer.infrastructure.Result
{
    public class ViewResult : ActionResult
    {
        private string _viewLocation;
        private object _model;

        public ViewResult(string controllerName, string viewName,
            object model)
        {
            _viewLocation = FindViewLocation(controllerName, viewName);
            _model = model;
        }
        private string FindViewLocation(string controllerName, string viewName)
        {
            var baseDir = System.IO.Directory.GetCurrentDirectory();//Path.GetDirectoryName();
            string filePath = Path.Combine(baseDir, "Views",viewName+".html");
            return filePath;
        }

        public override void Execute(HttpServerContext context)
        {
            var tpl = File.ReadAllText(_viewLocation, System.Text.Encoding.UTF8);
            var result = Razor.Parse(tpl, _model);
            context.Response.Content(result, "text/html");
        }
    }
}