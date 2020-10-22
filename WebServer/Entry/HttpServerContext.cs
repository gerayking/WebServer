using System.Net;

namespace WebServer.Entry
{
    /*　自定义的上下文环境
     *　成员包括只读的HttpListenerContext以及自定义的Request以及返回的响应
     *  
     */
    public class HttpServerContext
    {
        public HttpServerContext(HttpListenerContext context)
        {
            _innerContext = context;
            Request = new HttpServerRequest(context.Request);
        }

        private readonly HttpListenerContext _innerContext;
        public HttpServerRequest Request;
        public HttpListenerResponse Response => _innerContext.Response;
    }
}