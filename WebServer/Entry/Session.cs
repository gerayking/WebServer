﻿using System.Collections.Concurrent;
using WebServer.interfaces;

namespace WebServer.Entry
{
    public class Session : ISession
    {
        public Session()
        {
            _items = new ConcurrentDictionary<string, object>();
        }
        
        private ConcurrentDictionary<string, object> _items;
        public object this[string name]
        {
            get { return _items.ContainsKey(name) ? _items[name] : null; }
            set { _items[name] = value; }
        }
        public void Remove(string name)
        {
            object value;
            _items.TryRemove(name, out value);
        }
    }
}