using System;
using System.Collections.Generic;

namespace Isometric.Server
{
    public class NewsManager<T>
    {
        private readonly Dictionary<T, List<News>> _registeredNews 
            = new Dictionary<T, List<News>>();

        

        internal void BeginNewsRegistration(T t)
        {
            _registeredNews[t] = new List<News>();
        }

        internal void EndNewsRegistration(T t)
        {
            _registeredNews.Remove(t);
        }



        public void AddNews(T t, string type, Dictionary<string, dynamic> info)
        {
            if (_registeredNews.ContainsKey(t))
            {
                _registeredNews[t].Add(new News {Type = type, Info = info,});
            }
        }

        public News[] GetNews(T t)
        {
            if (_registeredNews.ContainsKey(t))
            {
                var result = _registeredNews[t].ToArray();
                _registeredNews[t].Clear();
                return result;
            }

            throw new InvalidOperationException();
        }
    }
}