using WebServer.Entry;
using WebServer.interfaces;

namespace WebServer.infrastructure
{
    public class Controller : IController
    {
        public HttpServerContext HttpContext { get; internal set; }
        
    }
}