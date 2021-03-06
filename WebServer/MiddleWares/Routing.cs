using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using WebServer.Entry;
using WebServer.Error;
using WebServer.infrastructure;
using WebServer.infrastructure.Result;
using WebServer.interfaces;
using WebServer.MiddleWare;

namespace WebServer.MiddleWares
{
    public class Routing : IMiddleware
    {
        public Routing()
        {
            _entries = new List<RouteEntry>();
        }

        public Routing(List<RouteEntry> entries)
        {
            _entries = entries;
        }
        public void AddRoute(string name, string url, object defaults = null)
        {
            // 用户自定义的路由规则
            _entries.Add(new RouteEntry(name,url,defaults));
        }
        private List<RouteEntry> _entries;
        // 执行路由
        public MiddlewareResult Execute(HttpServerContext context)
        {
            var httpServerRequest = context.Request;
            var httpServerContext =  context;
            
            // 遍历每个用户的路由规则
            foreach (RouteEntry entry in _entries)
            {
                var routeValues = entry.Match(context.Request);
                if (routeValues != null)
                {
                    // 找到路由对象,获取Controller
                    try
                    {
                        var controller = createController(httpServerContext, routeValues);
                        // 获取对应的方法
                        if (controller == null) continue;
                        var actionMethod = getActionMethod(controller, routeValues);
                        if (actionMethod == null) continue;
                        // 对其执行对应的方法并且返回相应上下文以及模型
                        var result = getActionResult(controller, actionMethod, routeValues);
                        if (result == null) continue;
                        result.Execute(httpServerContext);
                        return MiddlewareResult.Processed;
                    }                    
                    catch (ArgumentException e)
                    {
                        return MiddlewareResult.Continue;
                    }
                }
            }
            return MiddlewareResult.Continue;
        }
        // 根据uri的路径来获取对应的Controller
        private IController createController(HttpServerContext context, RouteValueDictionary routeValue)
        {
            var controllerName = (string)routeValue["controller"];
            var className = char.ToUpper(controllerName[0]) + controllerName.Substring(1) + "Controller";
            foreach (var type in GetType().Assembly.GetExportedTypes())
            {
                if (type.Name == className && typeof(Controller).IsAssignableFrom(type))
                {
                    var instance = (Controller) Activator.CreateInstance(type);
                    instance.HttpContext = context;
                    return instance;
                }
            }
            return null;
            throw new ArgumentException($"Controller {className} not found");
        }

        // 获取Controller类中的方法,根据routeValue中的名称获取
        private MethodInfo getActionMethod(IController controller, RouteValueDictionary routeValue)
        {
            var controllerType = controller.GetType();
            string actionName = (string)routeValue["action"];
            actionName = char.ToUpper(actionName[0]) + actionName.Substring(1);
            var method = controller.GetType().GetMethod(actionName);
            if (method == null)
            {
                return null;
                throw new ArgumentException($"Controller {controllerType.Name} has no action method {actionName}");
            }
            return method;
        }

        // 初始化参数列表,并且调用方法返回结果
        private ActionResult getActionResult(IController controller, MethodInfo method, RouteValueDictionary routeValues)
        {
            var methodParams = method.GetParameters();
            var paramValues = new Object[methodParams.Length];
            for (int i = 0; i < methodParams.Length; i++)
            {
                var routeValue = routeValues[methodParams[i].Name];
                if (routeValue == UrlParameter.Missing)
                {
                    paramValues[i] = methodParams[i].DefaultValue;
                    continue;
                }
                var paramValue = Convert.ChangeType(routeValue, methodParams[i].ParameterType);
                paramValues[i] = paramValue;
            }

            var result = method.Invoke(controller,paramValues);
            var actionResult = result as ActionResult;
            if (actionResult != null)
            {
                return actionResult;
            }
            else
            {
                return new RestResult(Convert.ToString(result), "text/html");
            }
        }
        
    }
}