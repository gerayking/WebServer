using System.Collections.Generic;

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

        private StaticResCon()
        {
            _pattern = new List<string>();
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
            string replace = ".*." + pattern;

        _pattern.Add(replace);
        }
    }
}