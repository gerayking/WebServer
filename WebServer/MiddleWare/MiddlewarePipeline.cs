using System;
using System.Collections.Generic;
using System.Net;

namespace WebServer.MiddleWare
{
    /**
 * @description: deal with MiddlewarePipeLine
 * @params: params
 * @return: return
 * @date: 2020-9-22
 * @author: geray
 * **/
    public class MiddlewarePipeline
    {
        public MiddlewarePipeline()
        {
            _middlewares = new List<IMiddleware>();
        }
        private readonly List<IMiddleware> _middlewares;
        private IExceptionHandler _exceptionHandler;

        internal void Add(IMiddleware middleware)
        {
            _middlewares.Add(middleware);
        }

        internal void UnhandledException(IExceptionHandler handler)
        {
            _exceptionHandler = handler;
        }

        internal void Execute(HttpListenerContext context)
        {
            try
            {
                foreach (var middleware in _middlewares)
                {
                    var result = middleware.Execute(context);
                    if (result == MiddlewareResult.Processed)
                    {
                        break;
                    }
                    else if (result == MiddlewareResult.Continue)
                    {
                        continue;
                    }
                }
            }
            catch (Exception e)
            {
                if(_exceptionHandler != null)
                    _exceptionHandler.HandleException(context,e);
                else throw;
            }
        }
    }
}