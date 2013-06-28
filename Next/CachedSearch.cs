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
}
