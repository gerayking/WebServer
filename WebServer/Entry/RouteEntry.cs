using System.Linq;

namespace WebServer.Entry
{
    public class RouteEntry
    {
        private readonly string _name;
        private readonly RouteFragment[] _fragments;

        public RouteEntry(string name, string url, object defaults)
        {
            _name = name;
            _fragments = Parse(url,defaults);
        }
        // 将用户自定义的url转化成服务器可识别的
        /*
         *  将参数放入一个字典RouteFragment[]中
         */
        private RouteFragment[] Parse(string url, object defaults)
        {
            var defaultValues =  new RouteValueDictionary().Load(defaults);
            return url.Split('/').Select(x => new RouteFragment(x, defaultValues)).ToArray();
        }
        public RouteValueDictionary Match(HttpServerRequest request)
        {
            var urlPath = request.Url.LocalPath.TrimStart('/');
            var urlParts = urlPath.Split('/');
            var routeValues = new RouteValueDictionary();
            for (int i = 0; i < _fragments.Length; i++)
            {
                string urlPart = (i < urlParts.Length) ? urlParts[i] : "";
                if (!_fragments[i].Match(urlPart, routeValues))
                    return null;
            }

            /*if (routeValues.Values.Contains(UrlParameter.Missing))
                return null;*/

            return routeValues;
        }
    }
    
}