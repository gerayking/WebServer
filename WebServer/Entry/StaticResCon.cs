using System.Collections.Generic;
using System.IO;

namespace WebServer.Entry
{ 
    
/**
 *@date:  2020年12月2日 11点15分
 *@author: gerayking
 *负责添加静态资源的后缀，如果遇到后缀则不需要跑路由，直接获取静态资源  
 * **/
    public class StaticResCon
    {
        private static StaticResCon SingleTon;

        private readonly List<string> _pattern;

        private readonly Dictionary<string,string> MimeTypeMap; 

        private StaticResCon()
        {
            _pattern = new List<string>();
            MimeTypeMap = new Dictionary<string, string>();
            init();

        }

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
        
        public List<string> GetPattern()
        {
            return _pattern;
        }
        public static StaticResCon GetInstance()
        {
            if (SingleTon == null)
            {
                SingleTon = new StaticResCon();
            }

            return SingleTon;
        }

        public void AddPattern(string pattern)
        {
            string replace = pattern;
            _pattern.Add(replace);
        }
        public string ParseMimeType(string MimeName)
        {
            if (MimeTypeMap.ContainsKey(MimeName) == false) return "application/*";
            return MimeTypeMap[MimeName];
        }
    }
}