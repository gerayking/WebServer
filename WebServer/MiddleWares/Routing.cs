using System.Collections.Generic;
using System.Net;
using WebServer.Entry;
using WebServer.MiddleWare;

namespace WebServer.MiddleWares
{
    public class Routing : IMiddleware
    {
        public Routing(List<RouteEntry> entries)
        {
            _entries = entries;
        }

        private List<RouteEntry> _entries;
        public MiddlewareResult Execute(HttpListenerContext context)
        {
            foreach (RouteEntry entry in _entries)
            {
                var routeValues = entry.Match(context.Request);
            }
            return MiddlewareResult.Continue;
        }
    }
}