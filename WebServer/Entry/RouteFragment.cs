using System.Text.RegularExpressions;

namespace WebServer.Entry
{
    public class RouteFragment
    {
        private readonly RouteValueDictionary _parameters;
        private readonly string _part;
        public RouteFragment(string part, RouteValueDictionary defaultValues)
        {
            _part = part;
            _parameters = new RouteValueDictionary();
            var re = new Regex("{([a-zA-Z0-9_]+)}");
            foreach (Match match in re.Matches(part))
            {
                string paramName = match.Groups[1].Value;
                if (defaultValues.ContainsKey(paramName))
                    _parameters[paramName] = defaultValues[paramName];
                else
                    _parameters[paramName] = UrlParameter.Missing;
            }
        }
        public RouteValueDictionary Parameters => _parameters;
        /*
         * 匹配当前_part是否与urlPart 符合，若符合则存入routeValues中
         */
        public bool Match(string urlPart, RouteValueDictionary routeValues)
        {
            string pattern = _part;
            foreach (var kv in _parameters)
            {
                string origin = "{" + kv.Key + "}";
                string replaceWith = string.Format("(?<{0}>[a-zA-Z0-9_]+)", kv.Key);
                if (kv.Value != UrlParameter.Missing)
                    replaceWith += "?";
                pattern = pattern.Replace(origin, replaceWith);
            }
            var re = new Regex(pattern);
            var match = re.Match(urlPart);
            foreach (var kv in _parameters)
            {
                if (!match.Success && kv.Value != UrlParameter.Missing) return false;
                string matchValue = match.Groups[kv.Key].Value;
                if (!(kv.Value == UrlParameter.Missing || kv.Value == UrlParameter.Optional))
                {
                    if (!matchValue.Equals(kv.Value)) return false;
                    routeValues[kv.Key] = kv.Value;
                    continue;
                }
                if (string.IsNullOrWhiteSpace(matchValue))
                    routeValues[kv.Key] = kv.Value;
                else
                    routeValues[kv.Key] = matchValue;
            }

            return true;
        }
    }
}