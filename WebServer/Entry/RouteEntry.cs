using System.Linq;

namespace WebServer.Entry
{
    public class RouteEntry
    {
        private readonly string _name;
        private readonly RouteFragment[] _fragments;

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
            for (var i = 0; i < _fragments.Length; i++)
            {
                string urlPart = (i < urlParts.Length) ? urlParts[i] : "";
                if (!_fragments[i].Match(urlPart, routeValues))
                    return null;
            }

            if (routeValues.Values.Contains(UrlParameter.Missing))
                return null;

            return routeValues;
        }
    }
    
}