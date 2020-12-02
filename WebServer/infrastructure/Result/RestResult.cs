using WebServer.Entry;

namespace WebServer.infrastructure.Result
{
    public class RestResult : ActionResult
    {
        private readonly string _content;
        private readonly string _mimeType;

        public RestResult(string content, string mimeType)
        {
            _content = content;
            _mimeType = mimeType;
        }
        
        public override void Execute(HttpServerContext context)
        {
            context.Response.Content(_content, _mimeType);
        }
    }
}