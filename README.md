# ＷebServer

## 1. 项目说明

利用C#语言开发一款web服务器，主要内容包括：

- 服务发布
- 服务监听
- HTTP协议解析
- HTTP压缩
- 静态资源扫描
- 路由检索与匹配
- 日志管理
- Session
- 异常处理
- 视图引擎渲染

## 2. 设计方案

基于中间件设计架构的解耦方案

![image-20201206194445221](https://i.bmp.ovh/imgs/2020/12/f68764280b89f9ec.png)

## 2. 技术细节

### 2.1 web基础容器搭建

基础的web服务器工作模式

- 定义了一个信号量，用来控制可接受的并发请求数量；
- 创建一个侦听器用来侦听请求；
- 定义服务器侦听的地址前缀，为了能够拼接出正确的url获取正确的资源
- 采用观察者模式，定义一个观察者不停的接受http请求，服务器接收到请求,程序根据请求的信息进行分析，并返回正确的资源文件
- 获取到上下文context后，可以从 context.Request 获取请求信息，向 context.Response 写入应答．

```c#
 public void Start()
        {
            _listener.Start();

            Task.Run(async () =>
            {
                while (true)
                {
                    _sem.WaitOne();
                    var context = await _listener.GetContextAsync();
                    _sem.Release();
                    HandleRequest(context);
                }
            });
        }
```

```c#
class Program
{
    private const int concurrentCount = 20;
    private const string serverUrl = "http://localhost:9000/";

    public static void Main(string[] args)
    {
        var server = new WebServer(concurrentCount);
        server.Bind(serverUrl);
        server.Start();

        Console.WriteLine($"Web server started at {serverUrl}. Press any key to exist...");
        Console.ReadKey();
    }
}

```



### 2.2 web服务器中间件设计架构

Web 框架是一个处理管线，而中间件设计架构将 Web 服务器本身、以及 Web 服务器所要支持的功能这两者从设计上解耦，让它们允许各自独立变化。这样的设计带来了众多优势：

- 每一个功能可以分解为小的、独立的组件，允许单独部署、测试和重用；
- Web 服务器可以在核心稳定不变的前提下，以类似插件的机制，灵活地启用或禁用各项功能，从而在功能和性能之间保持很好的平衡；
- 允许在单一Web 服务中承载多种应用（绝大多数现代 Web 服务器都有支持多种语言的插件）；
- 允许将多个 Web 服务器连接起来，每种浏览器承载各自最擅长的部分（Apache/Nginx 作前端反向代理，后端用动态服务器处理业务是最普遍的模式）；
- 在应用层面，可以通过配置或代码动态添加、修改或删除处理步骤，实现对处理过程的深度定制。

```c#
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
```

上述代码就是一个管线的处理模式，只需要定义中间件，就能够添加特定的功能，这样的设计模式达到了解耦合的目的，开发人员想要添加某个功能，不需要去修改原来的代码，只需要添加中间件即可达到目的.



### 2.3  路由模式

路由是动态请求的实现方式，对于绝大多数现代 Web 服务器来说，路由都是其中核心的部分。按照企业应用架构模式的分类，路由应该属于其中的“前端控制器”(controller)，主要目的是将接收到的 HTTP 请求分发到相应的后端业务模块去处理。而分发规则主要是基于请求的信息（路径、HTTP方法、头部信息、Cookie等）。目前的选择由两种，

+ **其中一种是ASP.NET MVC 的方式采用一个默认的 url 格式（controller/action/{id}）**

  ```c#
  name: "Default",
  url: "{controller}/{action}/{id}",
  defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional});
  ```

  

+ **另一种是Django 倾向于使用路径来规划请求，如下**

  ```c#
  urlpatterns = [
      path(url1, views.view1, name='view1'),
      path(url2, views.view2, name='view2'),
  ]
  ```

显示声明的方式能够清晰的表达网站的设计意图，但每种地址都要声明，略显繁琐。另一方面，早期的 ASP.NET 和 PHP 直接将请求映射到服务器文件（.aspx/.php），这种方式无需路由，但现代 Web 设计通常认为这种 URL 不够友好。ASP.NET MVC 采取了另外一种思路，用一个默认的 url 格式（controller/action/{id}）支持了大多数常规请求，当然也可以添加自定义的路由格式。所以我们参考了 ASP.NET MVC 的方式，为我们的 Web 服务器添加路由支持

业务流程如图：

![image-20201206201032884](https://i.bmp.ovh/imgs/2020/12/bae58ab49f5fa374.png)

匹配路由时的规则则大致为：

- 将请求的路径同样分解为片段；
- 由于我们已经知道每个参数是否可选，所以每个片段可以用正则表达式来匹配，只要在可选的部分加上？即可；
- 对于所有定义的参数处理如下：
- 如果请求路径中包含此变量，则使用请求路径中的值；
- 如果请求路径中不包含此变量，但是有默认值，则使用默认值；
- 如果请求路径中不包含此变量，但变量为可选（Optional），则跳过；
- 如果请求路径中不包含此变量，且没有默认值、也没有声明为可选，则出错。

1） 从接口开始，路由也是一个中间件（Middleware），把路由中间件添加到处理管线里面去

```c#
  static void RegisterRoutes(Routing routes)
        {
            routes.MapRoute(name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional});
        }
```

2)  为了保存路由中的参数变量，声明一个辅助对象 RouteValueDictionary，模拟 ASP.NET MVC 。

```c#
public class RouteValueDictionary : Dictionary<string, object>
{
    public RouteValueDictionary Load(object values)
    {
        if (values != null)
        {
            foreach (var prop in values.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                this[prop.Name] = prop.GetValue(values);
            }
        }

        return this;
    }
}
```

3) 创建 Routing 的配置部分：

```c#
    public class Routing : IMiddleware
    {
        public Routing()
        {
            _entries = new List<RouteEntry>();
        }

        private List<RouteEntry> _entries;

        public Routing MapRoute(string name, string url, object defaults = null)
        {
            _entries.Add(new RouteEntry(name, url, defaults));
            return this;
        }

        ...
    }
```

4) 当请求到来时，Routing 应该根据路由规则找到对应的控制器，并执行其中的方法：

```c#
public MiddlewareResult Execute(HttpListenerContext context)
{
    foreach (var entry in _entries)
    {
        var routeValues = entry.Match(context.Request);
        if (routeValues != null)
        {
            var controller = CreateController(routeValues);
            var actionMethod = GetActionMethod(controller, routeValues);
            var result = GetActionResult(controller, actionMethod, routeValues);
            context.Response.Status(200, result);

            return MiddlewareResult.Processed;
        }
    }

    return MiddlewareResult.Continue;
}
```

5) 查找程序集中所有 Controller 子类，按照控制器名称去匹配,然后执行相应controller方法

```c#
private IController CreateController(RouteValueDictionary routeValues)
{
    var controllerName = (string)routeValues["controller"];
    var className = char.ToUpper(controllerName[0]) + controllerName.Substring(1) + "Controller";
    foreach (var type in GetType().Assembly.GetExportedTypes())
    {
        if (type.Name == className && typeof(IController).IsAssignableFrom(type))
        {
            var instance = (IController) Activator.CreateInstance(type);
            return instance;
        }
    }
    throw new ArgumentException($"Controller {className} not found");
}

private MethodInfo GetActionMethod(IController controller, RouteValueDictionary routeValues)
{
    var controllerType = controller.GetType();
    string actionName = (string) routeValues["action"];
    actionName = char.ToUpper(actionName[0]) + actionName.Substring(1);
    var method = controller.GetType().GetMethod(actionName);
    if (method == null)
        throw new ArgumentException($"Controller {controllerType.Name} has no action method {actionName}");
    return method;
}
```

## 3.  Session

Session 是服务器与客户端之间的持久化对象，如果没有Session的话，每次请求都是独立的，都是一个新的请求．这意味着我们的服务器毫无记性，只能把每次请求都当作新的用户。Session 要求服务器有一定的机制去记住当前请求的用户。目前绝大多数的 Session 实现都是基于 Cookie 的．主流的实现会把绝大多数内容放在服务器端，客户端只记录一个用于鉴别的 key，这种实现在网络流量以及安全性方面都是极好的，缺点是会占据较多的服务器空间。也有一些实现为了减轻服务器压力以及方便客户端处理，会把部分数据放到客户端，但这样又需要考虑安全性和数据丢失的问题。我们采用第一种方案——即将所有内容保存在服务端。

1)  首先声明 Session 接口。对于大多数典型使用场景，Session 可以当作一个字典：

```c#
public interface ISession
{
    object this[string name] { get; set; }

    void Remove(string name);
}
```

2） 实现一个处理 Session 的中间件，session 的实现原理非常简单：用 Cookie 记录一个 key，对应服务器端中的数据，如果没有的话就新建一个。如果用于生产服务器的话，Cookie 是必须加密的，并且还要其他一些保护手段。由于实现加密需要引入很多代码，所以我们省略了这一部分。虽然实现一个 Session 从原理上来讲并不复杂，要实现真正安全、正确且健壮的 Session 并非易事，所以我们只实现了一个功能层面的Session，没有过多考虑其安全性的要求

```C#
public MiddlewareResult Execute(HttpServerContext context)
    {
        var cookie = context.Request.Cookies[_cookieName];
        Session session = null;
        if (cookie != null)
        {
            _sessions.TryGetValue(cookie.Value, out session);
        }
        if (session == null)
        {
            session = new Session();
            var sessionId = GenerateSessionId();
            _sessions[sessionId] = session;
            cookie = new Cookie(_cookieName, sessionId);
            context.Response.SetCookie(cookie);
        }
        context.Session = session;
        return MiddlewareResult.Continue;
    }
```

## 4.  静态资源扫描

基本业务流程：

![image-20201206201915380](https://i.bmp.ovh/imgs/2020/12/dd0315447eb666a3.png)

1） 静态资源类型扫描范围

```c#
 private void init()
        {
            _pattern.Add("js");
            _pattern.Add("css");
            _pattern.Add("html");
            _pattern.Add("gif");
            _pattern.Add("jpg");
            _pattern.Add("png");
            _pattern.Add("ico");
            _pattern.Add("svg");
            _pattern.Add("woff");
            _pattern.Add("woff2");
            _pattern.Add("eot");
            _pattern.Add("ttf");
            _pattern.Add("map");
            MimeTypeMap["svg"] = "image/svg+xml";
            MimeTypeMap["css"] = "text/css";
            MimeTypeMap["js"] = "text/javascript";
            MimeTypeMap["html"] = "text/html";
            MimeTypeMap["gif"] = "image/gif";
            MimeTypeMap["jpg"] = "image/jpeg";
            MimeTypeMap["png"] = "image/png";
            MimeTypeMap["ico"] = "image/vnd.microsoft.icon";
        }
```

2) 扫描路径

```c#
 private StaticPathCon()
        {
            _PathFragment = new List<string>();
            _PathFragment.Add("Views");
            _PathFragment.Add("web");
        }
```

3) 田间静态资源扫描中间件

```c#
public MiddlewareResult Execute(HttpServerContext context)
        {
            if (FilterResource(context))
            {
                return MiddlewareResult.Processed;
            }
            return MiddlewareResult.Continue;
        }
```

4）具体扫描细节

```c#
 private bool FilterResource(HttpServerContext httpServerContext)
        {
            var url = httpServerContext.Request.Url.ToString();
            string absolutePath = httpServerContext.Request.Url.AbsolutePath;
            var staticPathcon =  StaticPathCon.GetInstance();
            var staticResCon = StaticResCon.GetInstance();
            foreach (var item in staticResCon.GetPattern())
            {
                string resName = ".*." + item;
                var re =new Regex(item);
                var Match = re.Match(absolutePath);
                if (Match.Success)
                {
                    if (FindResource(absolutePath, httpServerContext, item) == false)
                    {
                        new ResourceNotFoundExceptionHandler().HandleException(httpServerContext,new Exception("url"));
                        httpServerContext.Response.NotFountResource(404);
                    }
                    return true;
                }
            }
            return false;
        }
```

## 5.  添加视图引擎支持

考虑到目前如果自己实现视图引擎学习成本过大，且与本课程关联度不大，所以决定决定集成市面上的RazorEngine到我们的服务器中．集成RazorEngine的时候需要整合路由模块的实现．具体方案流程如下：

![image-20201206203531149](https://i.bmp.ovh/imgs/2020/12/1520abc0e4a73cc3.png)

## 6. 日志及异常处理

定义相关中间件，记录相关信息，然后注册到 pipleLine

```c#
  public class Httplog : IMiddleware
    {
        public MiddlewareResult Execute(HttpServerContext context)
        {
            var request = context.Request;
            var path = request.Url.LocalPath;
            var clientIp = request.RemoteEndPoint.Address;
            var method = request.HttpMethod;

            Console.WriteLine("[{0:yyyy-MM-dd HH:mm:ss}] {1} {2} {3}",
                DateTime.Now, clientIp, method, path);
            return MiddlewareResult.Continue;
        }
    }
```

```c#
 public class ResourceNotFoundExceptionHandler : IExceptionHandler
    {
        public void HandleException(HttpServerContext context, Exception exp)
        {
            var clientIp = context.Request.RemoteEndPoint.Address;
            var method = context.Request.HttpMethod;
            var path = context.Request.Url.LocalPath;
            var expMsg = "Excetion" + exp.Message + "Not Found";
            Console.WriteLine("[{0:yyyy-MM-dd HH:mm:ss}] {1} {2} {3} ",
                DateTime.Now, expMsg, method, path);
            Console.WriteLine(exp.StackTrace);
        }
    }
```

日志及异常处理截图：

![image-20201206210918615](https://i.bmp.ovh/imgs/2020/12/96de8371db52e106.png)

## 7. 用户使用细则

修改 RegisterMiddlewares  ：增加或者删除一些中间件，来组合达到自己想要的功能

```c#
 // register Middlewares 注册中间件，路由，日志，异常处理等
         static void RegisterMiddlewares(IWebServerBuilder builder)
        {
            builder.UnhandledException(new ResourceNotFoundExceptionHandler());
            builder.Use(new SessionManager());
            builder.Use(new Httplog());
            builder.Use(new Filter());
            var route = new Routing();
            RegisterRoutes(route);
            builder.Use(route);
            builder.Use(new Http404());
        }
```

修改 RegisterRoutes  ：用户根据需求配置路由规则

```c#
 // 添加路由规则
        static void RegisterRoutes(Routing route)
        {
            route.AddRoute("Home","{controller}/{action}/{id}",
                new {controller = "Home",action = "details", id = UrlParameter.Missing});
            route.AddRoute("Index","{controller}/{action}",
                new {controller = "Home", action = "Index"});
            route.AddRoute("Indexhtml","{controller}/{action}",
                new {controller = "Home", action = "Index2"});
        }
```

默认web相关静态资源文件位于web或者View目录下，也可以修改配置文件，自定义web目录

```c#
private StaticPathCon()
        {
            _PathFragment = new List<string>();
            _PathFragment.Add("Views");
            _PathFragment.Add("web");
        }

```

默认服务器在9000端口打开，也可自定义端口。项目运行后，在浏览器输入相关网址即可查看。如遇异常可以根据日志输出进行相应纠错。

# *THANK YOU*

