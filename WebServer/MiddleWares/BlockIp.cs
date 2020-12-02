using System.Linq;
using System.Net;
using WebServer.Entry;
using WebServer.Error;
using WebServer.MiddleWare;

namespace WebServer.HttpLog
{
    public class BlockIp : IMiddleware
    {
        public BlockIp(string[] forbiddens)
        {
            _forbiddens = forbiddens;
        }

        private string[] _forbiddens;
        public MiddlewareResult Execute(HttpServerContext context)
        {
            var clinetIp = context.Request.RemoteEndPoint.Address;
            if(_forbiddens.Contains(clinetIp.ToString()))
            {
                context.Response.Status(403, "Forbidden");
                return MiddlewareResult.Processed;
            }
            return MiddlewareResult.Continue;
        }
    }
}