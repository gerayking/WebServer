using System;
using System.Collections.Concurrent;
using System.Net;
using WebServer.Entry;
using WebServer.MiddleWare;

namespace WebServer.MiddleWares
{
    public class SessionManager : IMiddleware
    {
        public SessionManager()
        {
            _sessions = new ConcurrentDictionary<string, Session>();
        }

        private string GeneraterSessionId()
        {
            return Guid.NewGuid().ToString();
        }

        private const string _cookies = "__sessionid__";
        private ConcurrentDictionary<string, Session> _sessions;
        public MiddlewareResult Execute(HttpServerContext context)
        {
            var cookie =  context.Request.Cookies[_cookies];
            Session session = null;
            if (cookie != null)
            {
                _sessions.TryGetValue(cookie.Value, out session);
            }
            if (session == null)
            {    
                session = new Session();
                var sessionId = GeneraterSessionId();
                cookie = new Cookie(_cookies,sessionId);
                context.Response.SetCookie(cookie);
            }
            context.Session = session;
            return MiddlewareResult.Continue;
        }
    }
}