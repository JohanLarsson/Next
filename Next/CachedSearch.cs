using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next
{
    public class CachedSearch<T> where T : class 
    {
        public DateTime CacheTime { get; set; }

        private T _cache;
        public T Cache
        {
            get { return _cache; }
            set
            {
                _cache = value;
                if (_cache != null)
                    CacheTime = DateTime.Now;
            }
        }

        public bool IsCached
        {
            get { return Cache != null && CacheTime.Day == DateTime.Now.Day; }
        }
    }

    public class CachedSearch<TKey,TValue> where TValue : class
    {
        public DateTime CacheTime { get; set; }

        private static readonly Dictionary<TKey, CachedSearch<TValue>> _cache = new Dictionary<TKey, CachedSearch<TValue>>();
        public TValue this[TKey key]
        {
            get
            {
                return _cache[key].Cache;
            }
            set
            {
                if (_cache.ContainsKey(key))
                    _cache[key].Cache = value;
                else
                    _cache[key] = new CachedSearch<TValue>() {Cache = value};
            }
        }

        public bool IsCached(TKey key)
        {
            if (!_cache.ContainsKey(key))
                return false;
            return _cache[key].IsCached;

        }
    }
}
