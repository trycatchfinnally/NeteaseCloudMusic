using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Cache
{
    public class MemoryCacheManager : ICacheManager
    {
        private ObjectCache Cache => MemoryCache.Default;
        public T Get<T>(string key) => (T)Cache[key];
        public void Set(string key, object data, int cacheTime)
        {
            if (data == null) return;
            Cache.Add(new CacheItem(key, data), new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime)
            });
        }
        public bool IsSet(string key) => Cache.Contains(key);
        public void Remove(string key) => Cache.Remove(key);
        public void RemoveByPattern(string pattern) => this.RemoveByPattern(pattern, Cache.Select(x => x.Key));
        public void Clear() => Cache.Each(x => Remove(x.Key));

        void IDisposable.Dispose() { }
    }
}
