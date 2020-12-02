using System.Collections;
using System.Collections.Generic;

namespace WebServer.Entry
{ 
    /**
 *@date:  2020年12月2日 11点15分
 *@author: gerayking
 *负责添加静态资源的路径，如果遇到后缀则不需要跑路由，直接获取静态资源  
 * **/
    public class StaticPathCon
    {
        private static StaticPathCon SingleTon;
        private readonly List<string> _PathFragment;

        private StaticPathCon()
        {
            _PathFragment = new List<string>();
        }

        public List<string> GetFragment()
        {
            return _PathFragment;
        }

        public static StaticPathCon GetInstance()
        {
            if (SingleTon == null)
            {
                SingleTon = new StaticPathCon();
            }

            return SingleTon;
        }
        public void AddStaticPath(string path)
        {
            _PathFragment.Add(path);
        }
    }
}