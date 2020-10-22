using WebServer.Entry;

namespace WebServer.infrastructure.Result
{
    public abstract class ActionResult
    {
        public abstract void Execute(HttpServerContext context);
    }
}